using System;
using System.Collections.Generic;
using System.Text.Json;
using Sandbox;

public static class PlayerData
{
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

        stats.Kills += kills;
        stats.Wins += wins;
        stats.Survived += survived;

        FileSystem.Data.WriteAllText( filePath, JsonSerializer.Serialize( stats ) );
    }

    public struct Stats
    {
        public int Kills { get; set; }
        public int Wins { get; set; }
        public int Survived { get; set; }
    }
}
