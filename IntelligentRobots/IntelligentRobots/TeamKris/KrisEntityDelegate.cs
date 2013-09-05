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
        public List<SeekerDelegate> seekers;

        public KrisEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            seekers = new List<SeekerDelegate>();
        }

        public Entity WillAdded(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            if (team.TeamMembers.Length == 0)
                seekers.Clear();

            var rand = (int)(possibleLocations.Length * Atlas.Rand);

            return new SeekerEntity(Atlas, team,
                new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand,
                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand));
        }

        public void HasAdded(EntityTeam team, Entity entity)
        {
            if (entity is SeekerEntity)
            {
                seekers.Add(new SeekerDelegate(this, entity));
            }
        }

        public void Update(EntityTeam team, EntityReport report)
        {
            foreach (var s in seekers)
            {
                if (s.WantsObjective)
                {
                    s.SetPath(new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), report.Trunk);
                }

                s.Update(report);
            }
        }

        public void DebugDraw(EntityTeam team)
        {

            foreach (var s in seekers)
            {
                s.DrawCurrentPath(Atlas);
            }
        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }
    }
}
