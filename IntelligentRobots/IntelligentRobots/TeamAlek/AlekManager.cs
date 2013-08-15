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

namespace IntelligentRobots.TeamAlek
{
    public class TeamAlekManager
        : EntityTeam
    {
        HumanEntity _entity;

        public Vector2 Position
        {
            get { return _entity.Position; }
        }

        public TeamAlekManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _entity = new HumanEntity(Atlas, new AlekEntityDelegate(atlas));


            Add(_entity);

            var e = new HumanEntity(Atlas, new AlekEntityDelegate(atlas));
            Add(e);
            e.TryMove(Vector2.One * 100000);
        }
    }
}
