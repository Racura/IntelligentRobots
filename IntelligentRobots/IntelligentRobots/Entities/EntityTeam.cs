using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;


namespace IntelligentRobots.Entities
{
    public class EntityTeam : AtlasManager
    {
        private List<Entity> _team;
        private EntityUtil _util;



        public EntityTeam(AtlasGlobal atlas)
            : base(atlas)
        {
            _team = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            _team.Add(entity);
        }

        public override void Update(string arg)
        {
            var gm = Atlas.GetManager<Grid.GridManager>();
            var util = new EntityUtil(Atlas, gm.Trunk);

            base.Update(arg);
            foreach (var e in _team)
            {
                if (e.Delegate != null) e.Delegate.Update(e, util);
            }

            foreach (var e in _team)
            {
                e.Update();
            }
        }

        public override void Draw(int pass)
        {
            base.Draw(pass);

            foreach (var e in _team)
            {
                e.Draw();
            }
            foreach (var e in _team)
            {
                if (e.Delegate != null) e.Delegate.Draw(e);
            }
        }
    }
}
