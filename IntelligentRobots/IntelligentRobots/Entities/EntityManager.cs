using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;

using IntelligentRobots.Component;

namespace IntelligentRobots.Entities
{
    public class EntityManager : AtlasManager
    {
        public List<EntityTeam> _list;

        public EntityManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _list = new List<EntityTeam>();
        }

        public void AddTeam(EntityTeam team)
        {
            _list.Add(team);
        }

        public override void Update(string arg)
        {
            base.Update(arg);

            var state = Atlas.GetStateController<StateController>();

            if (state.State == StateController.GameState.Paused)
                return;

            foreach (var e in _list)
            {
                e.Update();
            }

            bool first = true;

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();

            foreach (var team in _list)
            {
                foreach (var entity in team.Entities)
                {
                    if (first)
                    {
                        first = false;
                        min = max = entity.Position;
                    }
                    else
                    {
                        min.X = Math.Min(entity.Position.X, min.X);
                        min.Y = Math.Min(entity.Position.Y, min.Y);

                        max.X = Math.Max(entity.Position.X, max.X);
                        max.Y = Math.Max(entity.Position.Y, max.Y);
                    }
                }
            }

            var cm = Atlas.GetManager<AtlasEngine.BasicManagers.CameraManager>();
            cm.LookAt(10, 
                new Vector2((max.X + min.X) * 0.5f, (max.Y + min.Y) * 0.5f), 
                Math.Max(300, Vector2.Distance(max, min) * 0.7f), 2, 2);


        }

        public override void Draw(int pass)
        {
            foreach (var e in _list)
            {
                e.Draw();
            }
        }
    }
}
