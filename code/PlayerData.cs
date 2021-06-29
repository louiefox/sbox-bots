using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using Sandbox;

public partial class PlayerData
{
    public static Dictionary<ulong, Stats> Data = new();
    public static List<ulong> RequiredData = new();
    public static TimeSince LastLoadData = 0;

    public static float RequestCooldown = 60f;
    public static Dictionary<Client, TimeSince> DataRequests = new();

    public static void AddStats( Client client, int kills, int wins, int survived )
    {
        if ( kills < 1 && wins < 1 && survived < 1 ) return;

        if ( !FileSystem.Data.DirectoryExists( "stats" ) )
        {
            FileSystem.Data.CreateDirectory( "stats" );
        }

        string filePath = $"stats/{client.SteamId}";

        Stats stats = new Stats();
        if ( FileSystem.Data.FileExists( filePath ) )
        {
            string data = FileSystem.Data.ReadAllText( filePath );
            stats = JsonSerializer.Deserialize<Stats>( data );
        }

        stats.Name = client.Name;
        stats.Kills += kills;
        stats.Wins += wins;
        stats.Survived += survived;

        FileSystem.Data.WriteAllText( filePath, JsonSerializer.Serialize( stats ) );
    }

	public delegate int GetSortValue( PlayerData.Stats stats );

	public static void LoadData()
    {
        if ( !FileSystem.Data.DirectoryExists( "stats" ) ) return;

        Data.Clear();

        foreach ( string steamID in FileSystem.Data.FindFile( "stats" ) )
        {
            string data = FileSystem.Data.ReadAllText( $"stats/{steamID}" );

            Data.Add( Convert.ToUInt64( steamID ), JsonSerializer.Deserialize<Stats>( data ) );
        }

		RequiredData.Clear();

		List<GetSortValue> sortValues = new() 
		{
			stats => stats.Wins,
			stats => stats.Kills,
			stats => stats.Survived
		};

		foreach( GetSortValue getSortValue in sortValues )
		{
			List<(ulong, int)> sortedStats = new();
			foreach ( var kv in Data )
			{
				sortedStats.Add( (kv.Key, getSortValue(kv.Value)) );
			}

			sortedStats = sortedStats.OrderByDescending( o => o.Item2 ).ToList();

			for ( int i = 0; i < Math.Min( 10, sortedStats.Count ); i++ )
			{
				if ( RequiredData.Contains( sortedStats[i].Item1 ) ) continue;
				RequiredData.Add( sortedStats[i].Item1 );
			}
		}
	}

	[ServerCmd( "br_request_data" )]
    public static void RequestData()
    {
        Client client = ConsoleSystem.Caller;
        if ( client == null ) return;

        if ( DataRequests.ContainsKey( client ) )
        {
            if ( DataRequests[client] < RequestCooldown ) return;
            DataRequests[client] = 0;
        } else
        {
            DataRequests.Add( client, 0 );
        }

        if( LastLoadData >= 300f )
        {
            LoadData();
            LastLoadData = 0;
        }

		Dictionary<ulong, Stats> dataToSend = new();
		
		foreach( ulong steamID in RequiredData )
		{
			dataToSend.Add( steamID, Data[steamID] );
		}

		// Will be replaced once dictionaries can be networked
        SendPlayerData( To.Single( client ), JsonSerializer.Serialize( Data ) );
    }

    [ClientRpc]
    public static void SendPlayerData( string json )
    {
        Data = JsonSerializer.Deserialize<Dictionary<ulong, Stats>>( json );
        Event.Run( "battleroyale.updateplayerdata" );
    }

    public struct Stats
    {
        public string Name { get; set; }
        public int Kills { get; set; }
        public int Wins { get; set; }
        public int Survived { get; set; }
    }    
}
