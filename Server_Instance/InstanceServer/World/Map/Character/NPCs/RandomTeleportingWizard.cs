using System;
using System.Diagnostics;

using SharedComponents.Global.GameProperties;

using Extant;

namespace InstanceServer.World.Map.Character
{
    public partial class Characters
    {
        public class RandomTeleportingWizard : Characters.Npc
        {
            Stopwatch timer = new Stopwatch();

            public RandomTeleportingWizard(float x, float y)
                : base("RndTeleWizard", CharacterType.Npc)
            {
                this.Position.x = x;
                this.Position.y = y;
            }

            protected override void Dispose(bool blocking)
            {

            }

            public override void Tick(float frameDiff)
            {
                base.Tick(frameDiff);

                if (!timer.IsRunning && this.MovePoints.Count == 0 && this.CurrentMovePoint == null)
                {
                    timer.Start();
                }
                else if (timer.ElapsedMilliseconds > 3000)
                {
                    Position2D moveTo = new Position2D(this.Position.x + 5.0f, this.Position.y + 5.0f);
                    this.SetMovePointsPath(new MovePoint[] { new MovePoint(this.Position, moveTo), new MovePoint(moveTo, this.Position) });
                    timer.Reset();
                }
            }

            public override void Inform_AddCharacterInView(GameCharacter newChar)
            {
                Log.Log(this.GetHashCode() + ": Add- " + newChar.GetHashCode());
            }

            public override void Inform_RemoveCharacterInView(GameCharacter charFrom)
            {
                Log.Log(this.GetHashCode() + ": Removed- " + charFrom.GetHashCode());
            }

            public override void Inform_CharacterTeleport(GameCharacter charFrom, Position2D pos)
            {
                //Log.Log(this.GetHashCode() + ": Teleport- " + charFrom.GetHashCode() + "- " + charFrom.Position.ToString() + "->" + pos.ToString());
            }

            public override void Inform_CharacterMovePoint(GameCharacter charFrom, MovePoint mp)
            {
                //Log.Log(this.GetHashCode() + ": Move- " + charFrom.GetHashCode() + "- " + mp.start.ToString() + "->" + mp.end.ToString());
            }
        }
    }
}