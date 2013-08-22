using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IntelligentRobots.Entities;
using IntelligentRobots.Human;
using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.TeamKris
{
    public class KrisManager
        : EntityTeam
    {
        HumanEntity _entity;

        public Vector2 Position
        {
            get { return _entity.Position; }
        }

        public KrisManager(AtlasGlobal atlas)
            : base(atlas)
        {
            Color = Color.Red;

            _entity = AddAt(new RectangleF(100, 100, 100, 100)) as HumanEntity;
        }

        public override Entity AddAt(RectangleF spawn)
        {
            var h = HumanEntity.CreateAt(Atlas, spawn);

            h.TrySetDelegate(new KrisEntityDelegate(Atlas));
            Add(h);

            return h;
        }
    }
}
