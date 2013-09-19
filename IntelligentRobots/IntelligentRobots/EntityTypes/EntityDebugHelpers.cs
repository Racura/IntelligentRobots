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
    public static class  EntityDebugHelpers
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

        
        public static void DrawPath(AtlasGlobal atlas, List<Vector2> path)
        {
            if (path == null || path.Count == 2)
            {
                return;
            }

            VertexPositionColorTexture[] vpct = new VertexPositionColorTexture[path.Count];


            for (int i = 0; i < path.Count; i++)
            {
                vpct[i].Position = new Vector3(path[i], 0);
                vpct[i].Color = AtlasColorSystem.GetColorFromHue(i * 16) * 0.5f;
            }


            atlas.Graphics.SetPrimitiveType(PrimitiveType.LineStrip);
            atlas.Graphics.DrawVector(vpct);
        }
    }
}
