using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AtlasEngine;

namespace IntelligentRobots.Entities
{
    public class Entity : AtlasEntity
    {
        private EntityDelegate _delegate;
        private bool _locked;

        public EntityDelegate Delegate  {   get { return _delegate; }   }
        public bool Locked              {   get { return _locked; } }

        public virtual Vector2 Position
        {
            get { return Vector2.Zero; }
            set { }
        }

        public virtual bool IsCrouching
        {
            get { return false; }
            set {  }
        }

        public virtual Vector2 Velocity
        {
            get { return Vector2.UnitX; }
            set {  }
        }

        public virtual Vector2 LookingDirection
        {
            get { return Vector2.UnitX; }
            set {  }
        }
        public virtual float Radius { get { return 12; } }

        public Entity(AtlasGlobal atlas, EntityDelegate entityDelegate)
            : base(atlas)
        {
            _delegate = entityDelegate;
        }
    }
}
