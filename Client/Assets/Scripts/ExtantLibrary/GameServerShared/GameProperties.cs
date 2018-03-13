using System;

namespace Extant.GameServerShared
{
    public enum DayPhase
    {
        Day,
        Night
    }

    public enum TeamColor
    {
        Spectator,
        Neutral,
        Blue,
        Red
    }

    public class PlayerInfo
    {
        public string Name;
        public TeamColor TeamColor;
        public string Clan;
        public int Level;
        public int ModelNumber;
        public int Kills;
        public int Deaths;
        public int Assists;
    }

    public enum VisibilityMode
    {
        None,
        Full
    }
}