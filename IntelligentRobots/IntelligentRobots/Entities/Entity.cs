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
        public virtual bool Alive   { get { return false; } }

        public virtual Vector2 Velocity{
            get { return Vector2.UnitX; }
        }

        public virtual Vector2 Direction{
            get { return Vector2.UnitX; }
        }
        public virtual float Radius { get { return 6; } }
        public virtual float FOV    { get { return (float)Math.PI; } }


        public Entity(AtlasGlobal atlas, EntityDelegate entityDelegate)
            : base(atlas)
        {
            _delegate = entityDelegate;
        }

        public EntityStruct GetStruct()
        {
            return new EntityStruct(this);
        }

        public virtual void Update(){}

        public virtual void Draw()
        {
            Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>("blop"),
                Position, null,
                Color.Lerp(Color.Blue, Color.Black, Crouching ? 0.5f : 0), Vector2.One * 16,
                (float)Math.Atan2(Direction.Y, Direction.X), Radius / 16);
        }
        public virtual void Collision(Vector2 v, Vector2 n) { }


        public virtual bool TryMove(Vector2 v) { return false; }
        public virtual bool TryCrouching(bool crouching) { return false; }
        public virtual bool TryFaceDirection(Vector2 v) { return false; }
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
