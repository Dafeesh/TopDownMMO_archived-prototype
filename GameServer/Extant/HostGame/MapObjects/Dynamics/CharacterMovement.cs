using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.HostGame.MapObjects.Dynamics
{
    public class CharacterMovement
    {
        private bool isMoving;
        private Vec2 lastPosition;
        private Vec2 lastSpeed; // Last speed recorded -Left/Right+. -1 <= s <= 1
        private Vec2 speedMult;
        private Int32 timeStamp;

        public CharacterMovement(Vec2 position, Vec2 speedMult)
        {
            this.lastPosition = position;
            this.speedMult.x = speedMult.x;
            this.speedMult.y = speedMult.y;
            this.lastSpeed = new Vec2();
            this.isMoving = false;
        }

        public void Update(Vec2 newSpeed, Int32 newTimeStamp)
        {
            if (!(newTimeStamp <= timeStamp))
            {
                if (isMoving)
                {
                    double diffSec = (newTimeStamp - timeStamp) / 1000.0f;

                    lastPosition.x = diffSec * lastSpeed.x * speedMult.x;
                    lastPosition.y = diffSec * lastSpeed.y * speedMult.y;
                }

                lastSpeed = newSpeed;
                timeStamp = newTimeStamp;

                isMoving = !lastSpeed.Equals(Vec2.Zero);
            }
        }
    }
}
