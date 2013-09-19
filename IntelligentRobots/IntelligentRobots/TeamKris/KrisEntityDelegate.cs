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
        public List<SeekerDelegate> _seekers;

        public EntityReport Report { get; protected set; }
        public EntityTeam Team { get; protected set; }
        public FactSheet FactSheet { get; private set; }

        public KrisEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            _seekers = new List<SeekerDelegate>();
        }

        public Entity CreateEntity(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            var rand = (int)(possibleLocations.Length * Atlas.Rand);

            if(team.TeamMembers.Length % 4 == 1)
                return new HunterEntity(Atlas, team,
                    new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand,
                                possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand));


            return new SeekerEntity(Atlas, team,
                new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand,
                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand));
        }

        public void HasAdded(EntityTeam team, Entity entity)
        {
            _seekers.Add(new SeekerDelegate(this, entity));
            if (entity is SeekerEntity) {
            } 
        }

        public void Update(EntityTeam team, EntityReport report)
        {
            Team = team;
            Report = report;

            foreach (var s in _seekers)
            {
                if (s.WantsObjective)
                {
                    s.TrySetPath(new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), report.Trunk);
                }

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
            foreach (var s in _seekers)
            {
                s.DrawPath(Atlas);
            }
            Atlas.Graphics.Flush();
        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }


        public void Restart(EntityTeam team)
        {
            _seekers.Clear();
            FactSheet = new FactSheet(this);
        }

    }
}
