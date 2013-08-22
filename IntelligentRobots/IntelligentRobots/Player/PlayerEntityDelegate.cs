using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.Player
{
    public class PlayerEntityDelegate : AtlasEntity, EntityDelegate
    {
        public PlayerEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
        }

        public void Update(Entity entity, EntityUtil util)
        {
            var cm = Atlas.GetManager<CameraManager>();
            Vector2 v = Vector2.Zero;
            var keys = Atlas.Input;

            v += (keys.IsKeyDown(Keys.Up) ? 1 : 0) * cm.Up
                + (keys.IsKeyDown(Keys.Down) ? -1 : 0) * cm.Up
                + (keys.IsKeyDown(Keys.Right) ? 1 : 0) * cm.Right
                + (keys.IsKeyDown(Keys.Left) ? -1 : 0) * cm.Right;

            entity.TryMove(v);
            entity.TryCrouching(keys.IsKeyDown(Keys.LeftControl));


            foreach(var t in keys.GetTouchCollection())
            {
                v = cm.GetWorldPosition(t.Position, Vector2.One) - entity.Position;
            }

            entity.TryFace((float)Math.Atan2(v.Y, v.X));
        }


        public void Report(Entity entity, EntityReport report)
        {
        }

        public void DebugDraw(Entity entity)
        {
            double d = entity.Angle;

            Vector2 n1 = new Vector2((float)Math.Cos(d + entity.FOV * 0.5), (float)Math.Sin(d + entity.FOV * 0.5));
            Vector2 n2 = new Vector2((float)Math.Cos(d - entity.FOV * 0.5), (float)Math.Sin(d - entity.FOV * 0.5));

            var vpct = new VertexPositionColorTexture[4];

            vpct[0].Position = vpct[2].Position = new Vector3(entity.Position, 0);

            vpct[1].Position = new Vector3(entity.Position + n1 * 256, 0);
            vpct[3].Position = new Vector3(entity.Position + n2 * 256, 0);

            vpct[0].Color = vpct[1].Color =
                vpct[2].Color = vpct[3].Color = Color.Green;
            Atlas.Graphics.SetPrimitiveType(PrimitiveType.LineList);
            Atlas.Graphics.DrawVector(vpct);
        }


        public bool Swappable(Entity entity, EntityDelegate entityDelegate)
        {
            return true;
        }

    }
}
