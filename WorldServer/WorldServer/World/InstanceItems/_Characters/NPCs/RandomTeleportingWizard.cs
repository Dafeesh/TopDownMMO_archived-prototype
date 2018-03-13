﻿using System;
using System.Diagnostics;

using SharedComponents.GameProperties;
using Extant;

namespace WorldServer.World.InstanceItems
{
    public partial class Characters
    {
        public class RandomTeleportingWizard : Characters.Npc
        {
            int direction = 1;
            Stopwatch timer = new Stopwatch();

            public RandomTeleportingWizard(double x, double y)
                : base()
            {
                this.Position.x = x;
                this.Position.y = y;
                timer.Start();
            }

            protected override void Dispose(bool blocking)
            {
                
            }

            public override void Tick()
            {
                if (timer.ElapsedMilliseconds > 3000)
                {
                    this.TeleportTo(this.Position.x, this.Position.y + 10 * direction);
                    direction = direction * (-1);
                    timer.Restart();
                }
            }

            public override void Inform_AddCharacterInView(Character newChar)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Blank, this.GetHashCode() + ": Add- " + newChar.GetHashCode());
            }

            public override void Inform_RemoveCharacterInView(Character charFrom)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Blank, this.GetHashCode() + ": Removed- " + charFrom.GetHashCode());
            }

            public override void Inform_CharacterTeleport(Character charFrom, Position2D pos)
            {
                //DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Blank, this.GetHashCode() + ": Teleport!");
            }

            public override void Inform_MoveToMap(Map m)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Blank, this.GetHashCode() + ": Move to map- " + m.Id.ToString());
            }
        }
    }
}