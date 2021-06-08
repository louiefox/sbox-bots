﻿
using Sandbox;
using Sandbox.UI;
using BattleRoyale.UI;

[Library]
public partial class BRHud : HudEntity<RootPanel>
{
	public BRHud()
	{
		if ( !IsClient )
			return;

        RootPanel.AddChild<GameStatus>();

        RootPanel.StyleSheet.Load( "/ui/BRHud.scss" );

		RootPanel.AddChild<Vitals>();
		RootPanel.AddChild<Ammo>();

		RootPanel.AddChild<NameTags>();
		RootPanel.AddChild<DamageIndicator>();
		RootPanel.AddChild<HitIndicator>();
		
		RootPanel.AddChild<ChatBox>();
		RootPanel.AddChild<KillFeed>();
		RootPanel.AddChild<TabMenu>();
		RootPanel.AddChild<VoiceList>();

		RootPanel.AddChild<LootItemTags>();
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
