using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_floorusable" )]
public partial class FloorUsable : Prop
{
	public static Dictionary<int, FloorUsable> IndexEnts = new Dictionary<int, FloorUsable>();

	[Net]
	public int Index { get; set; }

	public override void Spawn()
	{
		base.Spawn();

        if ( !IsServer ) return;

		int newIndex = -1;
		for ( int i = 0; i < IndexEnts.Count; i++ )
		{
			if ( IndexEnts.ContainsKey( i ) ) continue;

			newIndex = i;
			break;
		}

		Index = newIndex >= 0 ? newIndex : IndexEnts.Count;
		IndexEnts.Add( Index, this );
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		IndexEnts.Remove( Index );
	}

	public virtual void EnableGlow()
	{
		GlowColor = new Color( 0.1f, 1.0f, 1.0f, 1.0f );
	}

	public virtual void DisableGlow()
	{
		GlowColor = new Color( 1.0f, 1.0f, 1.0f, 1.0f );
	}

	public virtual void Use(Player ply)
	{

	}

	[ServerCmd( "request_forusable_use" )]
	public static void RequestPickup( int index )
	{
		var ply = ConsoleSystem.Caller.Pawn as BRPlayer;

		if ( ply == null || ply.LifeState != LifeState.Alive || !IndexEnts.ContainsKey( index ) ) return;

		FloorUsable ent = IndexEnts[index];
		if ( ent == null || !ent.IsValid() ) return;

		ent.Use( ply );
	}
}
