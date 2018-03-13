using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharedComponents.Global.Game.Character
{
    public enum CharacterType
    {
        Npc,
        Player
    }

    public enum CharacterThreat
    {
        Neutral,
        Enemy,
        Friendly
    }

    public enum CharacterVisibility
    {
        None,
        Normal
    }
}
