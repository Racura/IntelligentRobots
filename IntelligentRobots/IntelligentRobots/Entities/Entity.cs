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
        private EntityTeam _team;

        public string Id { get; private set; }

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


        public Entity(AtlasGlobal atlas, EntityTeam team)
            : base(atlas)
        {
            _team = team;
            Id = Guid.NewGuid().ToString();
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
                Color.Lerp(color, Color.Black, Crouching ? 0.25f : 0), 
                Vector2.One * 16,
                Angle - MathHelper.PiOver2, Radius / 16);
        }

        public static void DrawEntity(AtlasGlobal atlas,  EntityStruct entity, Color color)
        {
            atlas.Graphics.DrawSprite(atlas.Content.GetContent<Texture2D>("image/simple"),
                entity.position, null,
                Color.Lerp(color, Color.Black, entity.crouching ? 0.25f : 0), 
                Vector2.One * 16,
                entity.angle - MathHelper.PiOver2, entity.radius / 16);
        }

        public virtual void Collision(Vector2 v, Vector2 n) { }


        public virtual bool TryMove(Vector2 v) { return false; }
        public virtual bool TryCrouching(bool crouching) { return false; }
        public virtual bool TryFace(float v) { return false; }

        public virtual bool IsHostile(EntityStruct struc)
        {
            return !_team.OnTeam(struc);
        }

    }

    public struct EntityStruct
    {
        public Vector2 position, velocity;

        public bool crouching, alive;

        public float radius, fov, angle;

        public Type type;

        private string _id;

        public EntityStruct(Entity e)
        {
            _id         = e.Id;

            type        = e.GetType();

            position    = e.Position;
            velocity    = e.Velocity;

            crouching   = e.Crouching;
            alive       = e.Alive;
            angle       = e.Angle;

            fov         = e.FOV;
            radius      = e.Radius;
        }

        public bool Is(Entity e)
        {
            return _id == e.Id;
        }
    }
}
