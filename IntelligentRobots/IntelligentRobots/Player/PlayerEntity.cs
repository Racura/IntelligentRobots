using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.Player
{
    public class PlayerEntity : Entities.Entity
    {
        private bool _crouching;
        private float _radius;
        private Vector2 _position;
        private Vector2 _velocity;

        public override bool Crouching { get { return _crouching; } }
        public override float Radius { get { return _radius; } }
        public override Vector2 Position { get { return _position; } }
        public override Vector2 Velocity { get { return _velocity; } }

        


        public PlayerEntity(AtlasGlobal atlas, float radius)
            : base(atlas, null)
        {
            _radius = radius;
        }

        protected override void Update()
        {
            var cm = Atlas.GetManager<CameraManager>();
            Vector2 v = Vector2.Zero;
            var keys = Atlas.Input;

            v += (keys.IsKeyDown(Keys.Up) ? 1 : 0) * cm.Up
                + (keys.IsKeyDown(Keys.Down) ? -1 : 0) * cm.Up
                + (keys.IsKeyDown(Keys.Right) ? 1 : 0) * cm.Right
                + (keys.IsKeyDown(Keys.Left) ? -1 : 0) * cm.Right;

            if (keys.IsKeyJustPressed(Keys.Tab))
                _radius = (_radius + 8) % 48;

            _crouching = !keys.IsKeyDown(Keys.LeftControl);

            float speed = _crouching ? 1 : 0.4f;

            if (v.LengthSquared() > speed * speed)
            {
                v.Normalize();

                v = v * speed;
            }

            _velocity += (v - _velocity) * (8 * Atlas.Elapsed);
            _position += _velocity * (256 * Atlas.Elapsed);
        }
    }
}
