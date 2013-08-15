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

namespace IntelligentRobots.TeamKris
{
    public class KrisEntityDelegate : AtlasEntity, EntityDelegate
    {
        public KrisEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
        }

        public void Update(Entity entity, EntityUtil util)
        {
            entity.TryMove(Vector2.One);
        }

        public void Report(Entity entity)
        {
        }

        public void DebugDraw(Entity entity)
        {
        }
    }
}
