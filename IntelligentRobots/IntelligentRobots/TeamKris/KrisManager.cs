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

            _entity = new HumanEntity(Atlas, new KrisEntityDelegate(atlas));

            Add(_entity);
        }
    }
}
