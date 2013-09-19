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
        private Dictionary<string, EntityTeam> _teams;
        public Dictionary<string, EntityTeam> Teams { get { return _teams; } }

        private object _key;

        public EntityManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _teams = new Dictionary<string, EntityTeam>();

            _key = new object();
        }

        public void AddTeam(string key, EntityDelegate teamDelegate)
        {
            var team = new EntityTeam(Atlas, teamDelegate);
            _teams.Add(key, team);
            team.Lock(_key);
        }

        public void Clear()
        {
            foreach (var e in _teams)
            {
                e.Value.UnLock(_key);
                e.Value.Clear();
                e.Value.Lock(_key);
            }
        }

        public void NewGame()
        {
            Clear();
            
        }

        public override void Update(string arg)
        {
            base.Update(arg);

            var state = Atlas.GetStateController<StateController>();

            if (state.State != StateController.GameState.Combat)
                return;

            foreach (var e in _teams)
            {
                e.Value.UnLock(_key);
                e.Value.Update();
                e.Value.Lock(_key);
            }

            bool first = true;

            Vector2 min = new Vector2();
            Vector2 max = new Vector2();

            foreach (var team in _teams)
            {
                foreach (var entity in team.Value.TeamMembers)
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
                e.Value.Draw();
            }
        }

        public void Spawn(string key, RectangleF[] rectangleF)
        {
            if (!_teams.ContainsKey(key))
                return;

            _teams[key].UnLock(_key);
            _teams[key].Spawn(rectangleF);
            _teams[key].Lock(_key);
        }
    }
}
