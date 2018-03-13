using System;
using System.Collections.Generic;

using Extant;
using SharedComponents;
using SharedComponents.GameProperties;

using Extant.Networking;
using WorldServer.Control;

namespace WorldServer.World
{
    public abstract class Character : IDisposable , IInstanceTick , ILogging
    {
        public const int ID_NULL = -1;

        private String name;
        private int id = ID_NULL;
        private CharacterType type;
        private List<Character> charsInView;
        private List<Character> charsSeenBy;
        private bool isDisposed;
        private DebugLogger log;

        private Position2D position;

        /// <summary>
        /// Creates a character to be used in a game map.
        /// </summary>
        /// <param name="id">ID of the character.</param>
        public Character(String name, CharacterType type)
        {
            name = "Char:" + name;

            log = new DebugLogger(name);
            log.MessageLogged += Console.WriteLine;

            this.name = name;
            this.position = new Position2D();
            this.charsInView = new List<Character>();
            this.charsSeenBy = new List<Character>();
            this.isDisposed = false;

            this.type = type;
            this.SetID(ID_NULL);
        }

        ~Character()
        {
            this.Dispose(true);
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

            ClientToWorldPackets.Character_Position_c p = new ClientToWorldPackets.Character_Position_c(id, x, y);
            foreach (var c in charsSeenBy)
            {
                c.Inform_CharacterTeleport(this, this.Position);
            }
        }

        public void AddCharacterInView(Character c)
        {
            if (charsInView.Contains(c))
                return;

            charsInView.Add(c);

            this.Inform_AddCharacterInView(c);
        }

        public void AddCharacterSeenBy(Character c)
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

        public bool CanSeeCharacter(Character c)
        {
            //For now, everyone can see all characters in instance
            return true;
        }

        #region Overrides
        public abstract void Tick();

        public abstract void Inform_CharacterTeleport(Character charFrom, Position2D pos);
        public abstract void Inform_AddCharacterInView(Character newChar);
        public abstract void Inform_RemoveCharacterInView(Character newChar);

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

        public IEnumerable<Character> CharsInView
        {
            get
            {
                return charsInView;
            }
        }

        public IEnumerable<Character> CharsSeenBy
        {
            get
            {
                return charsSeenBy;
            }
        }

        public CharacterType CharacterType
        {
            get
            {
                return type;
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