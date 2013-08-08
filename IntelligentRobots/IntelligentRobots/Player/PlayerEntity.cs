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
        private float _radius;
        public override float Radius { get { return _radius; } }


        public PlayerEntity(AtlasGlobal atlas, float radius)
            : base(atlas)
        {
            _radius = radius;
        }

        public void Update()
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

            crouching = !keys.IsKeyDown(Keys.LeftControl);

            float speed = crouching ? 1 : 0.4f;

            if (v.LengthSquared() > speed * speed)
            {
                v.Normalize();

                v = v * speed;
            }

            velocity += (v - velocity) * (8 * Atlas.Elapsed);
            position += velocity * (256 * Atlas.Elapsed);
        }

        public void Draw()
        {
            Atlas.Graphics.DrawSprite(Atlas.Content.GetContent<Texture2D>("blop"),
                position, null,
                Color.Blue, Vector2.One * 16,
                0, Radius / 16);
        }
    }
}
