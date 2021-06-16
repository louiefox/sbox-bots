using Sandbox;
using System;
using System.Collections.Generic;

[Library( "ent_lootpickup_clientmodel" )]
public partial class LootPickupClientModel : ModelEntity
{
    [Event.Tick]
    public void DeleteCheck()
    {
        if( Parent == null || !Parent.IsValid() )
        {
            Delete();
        }
    }
}
