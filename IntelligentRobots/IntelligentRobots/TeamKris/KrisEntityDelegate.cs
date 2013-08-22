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
            entity.TryMove(new Vector2(Atlas.Rand * 2 - 1, Atlas.Rand * 2 - 1));
        }

        public void Report(Entity entity, EntityReport report)
        {
        }

        public void DebugDraw(Entity entity)
        {
        }


        public bool Swappable(Entity entity, EntityDelegate entityDelegate)
        {
            return true;
        }
    }
}
