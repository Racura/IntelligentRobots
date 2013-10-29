using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;
using IntelligentRobots.EntityTypes;

using AtlasEngine;
using AtlasEngine.BasicManagers;


namespace IntelligentRobots.TeamKris
{
    public class KrisEntityDelegate : AtlasEntity, EntityDelegate
    {
        public List<SeekerAgent> _seekers;
        public List<EntityAgent> _subs;
        public float Rand
        {
            get
            {
                return Atlas.Rand;
            }
        }

        public EntityReport Report { get; protected set; }
        public EntityTeam Team { get; protected set; }
        public FactSheet FactSheet { get; private set; }

        public KrisEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            _subs = new List<EntityAgent>();
            _seekers = new List<SeekerAgent>();
        }

        public Entity CreateEntity(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            var rand = (int)(possibleLocations.Length * Atlas.Rand);

            for (int i = 0; i < 16; i++ )
            {
                Vector2 point = new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand,
                                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand);

                if(trunk.CanFit(point, 16)) {
                    if (team.TeamMembers.Length % 4 == 1)
                        return new HunterEntity(Atlas, team, point); //spawn a hunter instead, roughly 3 seekers for every hunter
                    return new SeekerEntity(Atlas, team, point);
                }
            }

            return null;
        }

        public void HasAdded(EntityTeam team, Entity entity)
        {
            EntityAgent sub = null;

            if (entity is SeekerEntity)
            {
                sub = new SeekerAgent(this, entity);
                _seekers.Add(sub as SeekerAgent);
            }
            else if (entity is HunterEntity)
            {

                sub = new HunterAgent(this, entity);
                //_seekers.Add(sub as HunterAgent);
            }

            if (sub != null)
                _subs.Add(sub);
        }

        public void Update(EntityTeam team, EntityReport report)
        {
            Team = team;
            Report = report;

            FactSheet.UpdateFactSheet(report);

            foreach (var s in _subs)
            {
                s.Update();
            }
        }

        public void DebugDraw(EntityTeam team)
        {
            foreach (var e in team.TeamMembers)
            {
                EntityDebugHelpers.DrawFov(Atlas, e);
            }

            Atlas.Graphics.Flush();
            foreach (var s in _subs)
            {
                s.Draw(Atlas);
            }
            Atlas.Graphics.Flush();
        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }

        public bool AskTeam(DelegateOrder order)
        {
            float bid = 0;
            EntityAgent d = null;

            foreach (var e in _subs)
            {
                float tmp = 0;

                if (e.Order != null && e.Order.IsDuplicate(order))
                {
                    return false;
                }

                if (e.TryOrder(order, out tmp))
                {
                    if (d == null || tmp < bid)
                    {
                        d = e;
                        bid = tmp;
                    }
                }
            }

            if (d != null)
            {
                d.SetOrder(order);
                return true;
            }

            return false;
        }




        public void Restart(EntityTeam team)
        {
            _subs.Clear();
            _seekers.Clear();
            FactSheet = new FactSheet(this);
        }


        public EntityStatus GetEntityStatus(EntityStruct entity)
        {
            if (Team.OnTeam(entity))
                return EntityStatus.Ally;

            if (entity.type == typeof(VictoryComputerEntity))
                return EntityStatus.Goal;

            return EntityStatus.Enemy;
        }

    }
}
