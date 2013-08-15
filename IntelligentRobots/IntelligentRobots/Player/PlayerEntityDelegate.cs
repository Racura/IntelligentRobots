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
        }

        public void Report(Entity entity)
        {
        }

        public void DebugDraw(Entity entity)
        {
        }
    }
}
