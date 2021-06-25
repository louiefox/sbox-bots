
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using BattleRoyale;
using BattleRoyale.UI;
using System.Collections.Generic;

[Library]
public partial class BattleRoyaleHUD : HudEntity<RootPanel>
{
    private Dictionary<Panel, bool> PanelList = new();
    public Crosshair Crosshair;

	public BattleRoyaleHUD()
	{
		if ( !IsClient ) return;

		PanelList.Add( RootPanel.AddChild<GameStatus>(), true );
        PanelList.Add( RootPanel.AddChild<EndScreen>(), true );
        PanelList.Add( RootPanel.AddChild<SpectatingInfo>(), true );
        PanelList.Add( RootPanel.AddChild<TabMenu>(), true );
        PanelList.Add( RootPanel.AddChild<MainMenu>(), true );

        PanelList.Add( RootPanel.AddChild<DamageIndicator>(), false );
        PanelList.Add( RootPanel.AddChild<HitIndicator>(), false );
        PanelList.Add( RootPanel.AddChild<ElimIndicator>(), false );

        PanelList.Add( RootPanel.AddChild<ArmourPlates>(), false );
        PanelList.Add( RootPanel.AddChild<ZoneWarnings>(), false );

        PanelList.Add( RootPanel.AddChild<ChatBox>(), true );
        PanelList.Add( RootPanel.AddChild<VoiceList>(), true );
        PanelList.Add( RootPanel.AddChild<KillFeed>(), true );
        PanelList.Add( RootPanel.AddChild<MiniMap>(), true );

        PanelList.Add( RootPanel.AddChild<NameTags>(), true );
        PanelList.Add( RootPanel.AddChild<LootItemTags>(), false );

        Crosshair = RootPanel.AddChild<Crosshair>();
        PanelList.Add( Crosshair, false );

        RootPanel.StyleSheet.Load( "/ui/BattleRoyaleHUD.scss" );

        PanelList.Add( RootPanel.AddChild<Vitals>(), false );
        PanelList.Add( RootPanel.AddChild<Ammo>(), false );
    }

    [Event( "client.tick" )]
    public void SpectatingUpdate()
    {
        if ( RootPanel == null ) return;

        foreach( var kv in PanelList )
        {
            if ( kv.Value == true ) continue;
            kv.Key.SetClass( "disableelement", BRGame.IsSpectating() && true );
        }
    }

	[ClientRpc]
	public void OnPlayerDied( string victim, string attacker = null )
	{
		Host.AssertClient();
	}

	[ClientRpc]
	public void ShowDeathScreen( string attackerName )
	{
		Host.AssertClient();
	}
}
