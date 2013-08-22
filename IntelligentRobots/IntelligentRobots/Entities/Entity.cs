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

        public virtual float Angle{
            get { return 0; }
        }
        public virtual float Radius { get { return 6; } }
        public virtual float FOV    { get { return (float)Math.PI * 0.5f; } }


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

        public virtual void Draw(Color color)
        {
            Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>("image/simple"),
                Position, null,
                Color.Lerp(color, Color.Black, Crouching ? 0.25f : 0), Vector2.One * 16,
                Angle - MathHelper.PiOver2, Radius / 16);



            //var gm = Atlas.GetManager<Grid.GridManager>();
            //gm.Trunk.DrawHeightGrid(gm.Trunk.CanSee(Position, Angle, FOV, Crouching), Color.White, Color.Transparent);
        }
        public virtual void Collision(Vector2 v, Vector2 n) { }


        public virtual bool TryMove(Vector2 v) { return false; }
        public virtual bool TryCrouching(bool crouching) { return false; }
        public virtual bool TryFace(float v) { return false; }
        public virtual bool TrySetDelegate(EntityDelegate entityDelegate)
        {
            if (_delegate == null)
            {
                _delegate = entityDelegate;
                return true;
            }
            else if (_delegate.Swappable(this, entityDelegate))
            {
                _delegate = entityDelegate;
                return true;
            }
            return false; 
        }
    }

    public struct EntityStruct
    {
        public Vector2 position, velocity;

        public bool crouched, alive;

        public float radius, fov, angle;

        public Type type;

        public EntityStruct(Entity e)
        {
            type = e.GetType();

            position    = e.Position;
            velocity    = e.Velocity;

            crouched    = e.Crouching;
            alive       = e.Alive;
            angle       = e.Angle;

            fov         = e.FOV;
            radius      = e.Radius;
        }
    }
}
