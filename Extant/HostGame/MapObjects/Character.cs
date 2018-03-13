using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.HostGame.MapObjects
{
    /// <summary>
    /// A character is anything that has movement properties or combat dynamics.
    /// </summary>
    public class Character : MapObject
    {
        private String name;
        private bool isDamageable;
        private bool isStationary;

        protected Character(Vec2 position, CharacterProperties properties)
            : base(position)
        {
            this.name = properties.name;
            this.isDamageable = properties.isDamageable;
            this.isStationary = properties.isStationary;
        }
    }

    public struct CharacterProperties
    {
        public String name;
        public bool isDamageable;
        public bool isStationary;
    }
}
