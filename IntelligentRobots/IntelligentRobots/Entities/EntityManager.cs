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
        private List<EntityTeam> _teams;
        public List<EntityTeam> Teams { get { return _teams; } }

        private object _key;

        public EntityManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _teams = new List<EntityTeam>();

            _key = new object();
        }

        public void AddTeam(EntityDelegate teamDelegate)
        {
            var team = new EntityTeam(Atlas, teamDelegate);
            _teams.Add(team);
            team.Lock(_key);
        }

        public void Clear()
        {
            foreach (var e in _teams)
            {
                e.UnLock(_key);
                e.Clear();
                e.Lock(_key);
            }
        }

        public void NewGame()
        {
            Clear();


            var gm = Atlas.GetManager<Grid.GridManager>();

            double tmp = Math.PI * 2 * Atlas.Rand;

            int size = 200;

            for (int i = 0; i < _teams.Count; i++)
            {
                var v = new Vector2((float)Math.Sin(i * Math.PI * 2 / _teams.Count + tmp) * 0.5f + 0.5f,
                                    (float)Math.Cos(i * Math.PI * 2 / _teams.Count + tmp) * 0.5f + 0.5f);

                var rect = new RectangleF(v.X * (gm.Trunk.Width - size), v.Y * (gm.Trunk.Height - size), size, size);

                _teams[i].UnLock(_key);
                _teams[i].Spawn(new RectangleF[] { rect });
                _teams[i].Spawn(new RectangleF[] { rect });
                _teams[i].Spawn(new RectangleF[] { rect });
                _teams[i].Lock(_key);
            }
        }

        public override void Update(string arg)
        {
            base.Update(arg);

            var state = Atlas.GetStateController<StateController>();

            if (state.State == StateController.GameState.Paused)
                return;

            foreach (var e in _teams)
            {
                e.UnLock(_key);
                e.Update();
                e.Lock(_key);
            }

            bool first = true;

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();

            foreach (var team in _teams)
            {
                foreach (var entity in team.TeamMembers)
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

            if (!first)
            {
                var cm = Atlas.GetManager<AtlasEngine.BasicManagers.CameraManager>();
                cm.LookAt(10,
                    new Vector2((max.X + min.X) * 0.5f, (max.Y + min.Y) * 0.5f),
                    Math.Max(250, Vector2.Distance(max, min) * 0.7f), 8, 8);
            }
        }

        public override void Draw(int pass)
        {
            foreach (var e in _teams)
            {
                e.Draw();
            }
        }
    }
}
