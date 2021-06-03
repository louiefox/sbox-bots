
using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

public class Vitals : Panel
{
	public Panel Armour1;
	public Panel Armour2;
	public Panel Armour3;
	public Panel Health;

	public Vitals()
	{
		Panel ArmourBarsBack = Add.Panel( "armourBarsBack" );

		Panel Armour1Bar = ArmourBarsBack.Add.Panel( "armourBar" );
		Armour1 = Armour1Bar.Add.Panel( "armour" );		
		
		Panel Armour2Bar = ArmourBarsBack.Add.Panel( "armourBar" );
		Armour2 = Armour2Bar.Add.Panel( "armour" );		
		
		Panel Armour3Bar = ArmourBarsBack.Add.Panel( "armourBar" );
		Armour3Bar.Style.MarginRight = 0;
		Armour3 = Armour3Bar.Add.Panel( "armour" );
		
		Panel HealthBar = Add.Panel( "healthBar" );
		Health = HealthBar.Add.Panel( "health" );
	}

	public override void Tick()
	{
		BRPlayer player = Local.Pawn as BRPlayer;
		if ( player == null ) return;

		Armour1.Style.Dirty();
		Armour1.Style.Width = Length.Percent( MathX.Clamp( ((float)player.Armour / 50f) * 100f, 0f, 100f ) );		
		
		Armour2.Style.Dirty();
		Armour2.Style.Width = Length.Percent( MathX.Clamp( ((float)(player.Armour-50) / 50f) * 100f, 0f, 100f ) );		
		
		Armour3.Style.Dirty();
		Armour3.Style.Width = Length.Percent( MathX.Clamp( ((float)(player.Armour-100) / 50f) * 100f, 0f, 100f ) );

		float extraHealth = 0f;
		if( player.RegenActive && Time.Now >= player.RegenStartTime+player.RegenStartDelay )
		{
			float regenProgress = (Time.Now - player.RegenStartTime - player.RegenStartDelay) / player.RegenTime;
			extraHealth = ((player.MaxHealth - player.Health) * regenProgress);
		}

		Health.Style.Dirty();
		Health.Style.Width = Length.Percent( MathX.Clamp( (((float)player.Health + extraHealth) / (float)player.MaxHealth) * 100f, 0f, 100f ) );
	}
}
