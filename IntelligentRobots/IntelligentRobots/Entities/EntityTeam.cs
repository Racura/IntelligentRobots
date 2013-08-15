using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;


namespace IntelligentRobots.Entities
{
    public class EntityTeam : AtlasEntity
    {
        private List<Entity> _team;
        private EntityUtil _util;

        public Color Color { get; protected set; }


        public EntityTeam(AtlasGlobal atlas)
            : base(atlas)
        {
            Color = Color.White;
            _team = new List<Entity>();
        }

        public void Add(Entity entity)
        {
            _team.Add(entity);
        }

        public IEnumerable<Entity> Entities {
            get
            {
                return _team;
            }
        }

        public void Update()
        {
            var gm = Atlas.GetManager<Grid.GridManager>();
            var util = new EntityUtil(Atlas, gm.Trunk);

            foreach (var e in _team)
            {
                if (e.Delegate != null) e.Delegate.Update(e, util);
            }

            foreach (var e in _team)
            {
                e.Update();
                gm.MapCollide(e);
            }
        }

        public void Draw()
        {
            foreach (var e in _team)
            {
                e.Draw(Color);
            }
            foreach (var e in _team)
            {
                if (e.Delegate != null) e.Delegate.DebugDraw(e);
            }
        }
    }
}
