using System;
using System.Diagnostics;

using SharedComponents.GameProperties;
using Extant;

namespace WorldServer.World
{
    public partial class Characters
    {
        public class RandomTeleportingWizard : Characters.Npc
        {
            int direction = 1;
            Stopwatch timer = new Stopwatch();

            public RandomTeleportingWizard(float x, float y)
                : base("RndTeleWizard", CharacterType.Npc)
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
                base.Tick();

                if (timer.ElapsedMilliseconds > 3000)
                {
                    this.TeleportTo(this.Position.x, this.Position.y + 10 * direction);
                    direction = direction * (-1);
                    timer.Restart();
                }
            }

            public override void Inform_AddCharacterInView(Character newChar)
            {
                Log.Log(this.GetHashCode() + ": Add- " + newChar.GetHashCode());
            }

            public override void Inform_RemoveCharacterInView(Character charFrom)
            {
                Log.Log(this.GetHashCode() + ": Removed- " + charFrom.GetHashCode());
            }

            public override void Inform_CharacterTeleport(Character charFrom, Position2D pos)
            {
                //Log.Log(this.GetHashCode() + ": Teleport- " + charFrom.GetHashCode() + "- " + charFrom.Position.ToString() + "->" + pos.ToString());
            }

            public override void Inform_CharacterMovePoint(Character charFrom, MovePoint mp)
            {
                Log.Log(this.GetHashCode() + ": Move- " + charFrom.GetHashCode() + "- " + mp.start.ToString() + "->" + mp.end.ToString());
            }
        }
    }
}