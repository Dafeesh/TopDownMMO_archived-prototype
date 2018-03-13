using System;
using System.Collections.Generic;

using Extant;
using SharedComponents.Global;
using SharedComponents.Global.GameProperties;

using Extant.Networking;
using InstanceServer.Control;
using System.Threading;

namespace InstanceServer.World
{
    public abstract class GameCharacter : IDisposable , IInstanceTick , ILogging
    {
        public const int ID_NULL = -1;

        private int id = ID_NULL;
        private String name;
        private CharacterLayout layout;

        private Position2D position;
        private Queue<MovePoint> movePoints = new Queue<MovePoint>();
        private MovePoint currentMovePoint = null;
        private List<GameCharacter> charsInView;
        private List<GameCharacter> charsSeenBy;

        private bool isDisposed;
        private DebugLogger log;

        private CharacterStats stats = new CharacterStats();

        /// <summary>
        /// Creates a character to be used in a game map.
        /// </summary>
        /// <param name="id">ID of the character.</param>
        public GameCharacter(String name, CharacterLayout layout)
        {
            name = "Char:" + name;

            log = new DebugLogger(name);
            log.MessageLogged += Console.WriteLine;

            this.name = name;
            this.position = new Position2D();
            this.charsInView = new List<GameCharacter>();
            this.charsSeenBy = new List<GameCharacter>();
            this.isDisposed = false;

            this.layout = layout;
            this.SetID(ID_NULL);
        }

        ~GameCharacter()
        {
            this.Dispose(true);
        }

        //Private
        private void HandleMovement(float frameDiff)
        {
            float unitsToMove = stats.MoveSpeed * frameDiff;
            while (!float.IsNaN(unitsToMove))
            {
                this.position = CalculateMove(position, currentMovePoint.end, ref unitsToMove);
                if (!float.IsNaN(unitsToMove))
                {
                    if (movePoints.Count != 0)
                    {
                        SetCurrentMovePoint(movePoints.Dequeue());
                    }
                    else
                    {
                        unitsToMove = float.NaN;
                        currentMovePoint = null;
                    }
                }
            }
        }

        private void SetCurrentMovePoint(MovePoint mp)
        {
            currentMovePoint = mp;
            position = currentMovePoint.start;
            this.Inform_CharacterMovePoint(this, currentMovePoint);
            foreach (GameCharacter c in this.CharsSeenBy)
            {
                c.Inform_CharacterMovePoint(this, currentMovePoint);
            }
        }

        private Position2D CalculateMove(Position2D old, Position2D target, ref float unitsToMove)
        {
            float x = (target.x - old.x);
            float y = (target.y - old.y);
            float angle = (float)Math.Atan2(y, x);
            float d = (float)Math.Sqrt(x * x + y * y);
            float deltaX = (float)(unitsToMove * Math.Cos(angle));
            float deltaY = (float)(unitsToMove * Math.Sin(angle));

            if (unitsToMove > d)
            {
                unitsToMove = unitsToMove - d;
                return target;
            }
            else
            {
                unitsToMove = float.NaN;
                return new Position2D(old.x + deltaX, old.y + deltaY);
            }
        }

        //Protected


        //Public
        public virtual void Tick(float frameDiff)
        {
            if (currentMovePoint != null)
            {
                HandleMovement(frameDiff);
            }
            else
            {
                if (movePoints.Count != 0)
                {
                    SetCurrentMovePoint(movePoints.Dequeue());
                }
            }
        }

        public void Dispose()
        {
            this.Dispose(false);

            this.isDisposed = true;
        }

        public void SetID(int id)
        {
            this.id = id;
        }

        public void TeleportTo(float x, float y)
        {
            position.x = x;
            position.y = y;

            this.Inform_CharacterTeleport(this, this.Position);
            foreach (var c in charsSeenBy)
            {
                c.Inform_CharacterTeleport(this, this.Position);
            }
        }

        public void AddCharacterInView(GameCharacter c)
        {
            if (charsInView.Contains(c))
                return;

            charsInView.Add(c);

            this.Inform_AddCharacterInView(c);
        }

        public void AddCharacterSeenBy(GameCharacter c)
        {
            if (charsSeenBy.Contains(c))
                return;

            charsSeenBy.Add(c);
        }

        public void RemoveSeenByAllOther()
        {
            foreach (var c in charsSeenBy)
            {
                c.Inform_RemoveCharacterInView(this);
            }

            charsSeenBy.Clear();
        }

        public bool CanSeeCharacter(GameCharacter c)
        {
            //For now, everyone can see all characters in instance
            return true;
        }

        public void SetMovePointsPath(MovePoint[] points)
        {
            currentMovePoint = null;
            movePoints.Clear();
            foreach (MovePoint p in points)
            {
                movePoints.Enqueue(p);
            }
        }

        #region Overrides

        public abstract void Inform_CharacterTeleport(GameCharacter charFrom, Position2D pos);
        public abstract void Inform_CharacterMovePoint(GameCharacter charFrom, MovePoint mp);
        public abstract void Inform_AddCharacterInView(GameCharacter newChar);
        public abstract void Inform_RemoveCharacterInView(GameCharacter newChar);

        protected abstract void Dispose(bool blocking);
        #endregion Overrides

        #region Accessors

        public int Id
        {
            get
            {
                return id;
            }
        }

        public Position2D Position
        {
            get
            {
                return position;
            }
        }

        public MovePoint CurrentMovePoint
        {
            get
            {
                return currentMovePoint;
            }
        }

        public IReadOnlyCollection<MovePoint> MovePoints
        {
            get
            {
                return movePoints.ToArray();
            }
        }

        public CharacterStats Stats
        {
            get
            {
                return stats;
            }
        }

        public IEnumerable<GameCharacter> CharsInView
        {
            get
            {
                return charsInView;
            }
        }

        public IEnumerable<GameCharacter> CharsSeenBy
        {
            get
            {
                return charsSeenBy;
            }
        }

        public CharacterLayout Layout
        {
            get
            {
                return layout;
            }
        }

        public bool IsDisposed
        {
            get
            {
                return isDisposed;
            }
        }

        public DebugLogger Log
        {
            get
            {
                return log;
            }
        }

        #endregion Accessors
    }
}