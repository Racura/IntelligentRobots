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

        public EntityDelegate Delegate  {   get { return _delegate; }   }

        public virtual Vector2 Position
        {
            get { return Vector2.Zero; }
        }

        public virtual bool Crouching{get { return false; }}
        public virtual bool Alive { get { return false; } }

        public virtual Vector2 Velocity{
            get { return Vector2.UnitX; }
        }

        public virtual Vector2 Direction{
            get { return Vector2.UnitX; }
        }
        public virtual float Radius { get { return 1; } }
        public virtual float FOV    { get { return 1; } }




        public Entity(AtlasGlobal atlas, EntityDelegate entityDelegate)
            : base(atlas)
        {
            _delegate = entityDelegate;
        }

        public EntityStruct GetStruct()
        {
            return new EntityStruct(this);
        }

        public void Logic(Grid.GridTrunk trunk)
        {
            //_delegate.Update(this, trunk);

            Update();
        }

        protected virtual void Update()
        {

        }

        public virtual void Draw()
        {
            Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>("blop"),
                Position, null,
                Color.Blue, Vector2.One * 16,
                (float)Math.Atan2(Direction.Y, Direction.X), Radius / 16);
        }

    }

    public struct EntityStruct
    {
        public Vector2 position, velocity, direction;

        public bool crouched, alive;

        public float radius, fov;

        public Type type;

        public EntityStruct(Entity e)
        {
            type = e.GetType();

            position    = e.Position;
            velocity    = e.Velocity;
            direction   = e.Direction;

            crouched    = e.Crouching;
            alive       = e.Alive;

            fov         = e.FOV;
            radius      = e.Radius;
        }
    }
}
