
using System;
using System.Collections.Generic;
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class ArmourPlates : Panel
{
    private Panel ArmourInsert;
    private List<Panel> InsertTimeBars = new();
    private Label ArmourCount;

	public ArmourPlates()
	{
        StyleSheet.Load( "/ui/ArmourPlates.scss" );

        Panel armour = Add.Panel( "armour" );
        armour.Add.Panel( "armourhint" ).Add.Label( "4", "armourhinttxt" );
        ArmourCount = armour.Add.Label( "0", "armourcount" );

        ArmourInsert = Add.Panel( "armourinsert" );
        ArmourInsert.Add.Label( "INSERTING ARMOUR", "startingtitle" );

        Panel timerColumn = ArmourInsert.Add.Panel( "timerrow" );
        int barCount = 6;
        for ( int i = 0; i < barCount; i++ )
        {
            Panel timerBar = timerColumn.Add.Panel( "timerbarslot" );
            timerBar.Style.MarginRight = i < barCount-1 ? 5 : 0;

            InsertTimeBars.Add( timerBar.Add.Panel( "timerbar" ) );
        }
    }

    public override void Tick()
	{
        if ( Local.Pawn is not BRPlayer player ) return;

        ArmourCount.Text = $"x{player.GetInvItemCount( "armour_plate" )}";

        ArmourInsert.SetClass( "active", player.ArmourInserting );

        if ( player.ArmourInserting )
        {
            double time = Math.Min( player.ArmourInsertDuration, player.ArmourInsertTime );

            float slotDuration = player.ArmourInsertDuration / InsertTimeBars.Count;
            int currentCount = 0;
            foreach( Panel panel in InsertTimeBars )
            {
                currentCount++;
                int count = currentCount;

                float percent = ((float)time - slotDuration * (count - 1)) / slotDuration;
                if ( (panel.Style.Opacity <= 0f && percent <= 0f) || (panel.Style.Opacity >= 1f && percent >= 1f) ) continue;

                panel.Style.Dirty();
                panel.Style.Opacity = Math.Clamp( percent, 0f, 1f );
            }
        }
    }
}
