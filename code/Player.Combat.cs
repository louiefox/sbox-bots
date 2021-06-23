using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using BattleRoyale;
using System.Text.Json;

partial class BRPlayer
{
    private int RecentElimCount;
    private TimeSince LastElimination;

    private void OnPlayerEliminated( Player player )
    {
        if ( player.GetClientOwner() is not Client client ) return;

        List<EliminationTag.Types> tags = new();

        if ( (player as BRPlayer).LastDamage.HitboxIndex == player.GetBoneIndex( "head" ) ) tags.Add( EliminationTag.Types.Headshot );

        if ( Position.Distance( player.Position ) >= 1000f ) tags.Add( EliminationTag.Types.Longshot );

        if( (RecentElimCount > 0 && LastElimination > 5f) || RecentElimCount >= 4 )
        {
            RecentElimCount = 0;
        }

        RecentElimCount++;
        LastElimination = 0;

        switch(RecentElimCount)
        {
            case 2:
                tags.Add( EliminationTag.Types.DoubleKill );
                break;            
            case 3:
                tags.Add( EliminationTag.Types.TripleKill );
                break;            
            case 4:
                tags.Add( EliminationTag.Types.QuadKill );
                break;
        }

        EliminatedPlayer( To.Single( this ), client.Name, JsonSerializer.Serialize( tags ) );
    }

    [ClientRpc]
    public void EliminatedPlayer( string name, string tagsJson )
    {
        ElimIndicator.Current?.OnElimination( name, JsonSerializer.Deserialize<List<EliminationTag.Types>>( tagsJson ) );
    }
}

public class EliminationTag
{
    public static Dictionary<Types, EliminationTag> Tags = new()
    {
        { Types.Headshot, new( "Headshot" ) },
        { Types.DoubleKill, new( "Double Kill" ) },
        { Types.TripleKill, new( "Triple Kill" ) },
        { Types.QuadKill, new( "Quad Feed" ) },
        { Types.Longshot, new( "Longshot" ) },
    };

    public string Title;

    public EliminationTag( string title )
    {
        Title = title;
    }

    public enum Types
    {
        Headshot,
        DoubleKill,
        TripleKill,
        QuadKill,
        Longshot
    }
}
