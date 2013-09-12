using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;

using IntelligentRobots.Entities;

namespace IntelligentRobots.EntityTypes
{
    public static class  EntityDebug
    {
        public static void DrawFov(AtlasGlobal atlas, Entity e)
        {
            VertexPositionColorTexture[] vpct = new VertexPositionColorTexture[4];

            vpct[0].Color = vpct[2].Color = Color.Blue;
            vpct[0].Position = vpct[2].Position = new Vector3(e.Position, 0);

            vpct[1].Position = new Vector3(
                e.Position.X + (float)Math.Cos(e.Angle + e.FOV * 0.5f) * 64,
                e.Position.Y + (float)Math.Sin(e.Angle + e.FOV * 0.5f) * 64, 0);
            vpct[3].Position = new Vector3(
                e.Position.X + (float)Math.Cos(e.Angle - e.FOV * 0.5f) * 64,
                e.Position.Y + (float)Math.Sin(e.Angle - e.FOV * 0.5f) * 64, 0);

            atlas.Graphics.SetPrimitiveType(PrimitiveType.LineList);
            atlas.Graphics.DrawVector(vpct);
        }
    }
}
