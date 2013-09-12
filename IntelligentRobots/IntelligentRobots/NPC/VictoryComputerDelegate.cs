using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.NPC
{
    public class VictoryComputerDelegate : AtlasEntity, EntityDelegate
    {

        public VictoryComputerDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
        }

        public Entity WillAdded(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            var rand = (int)(possibleLocations.Length * Atlas.Rand);

            return new EntityTypes.VictoryComputerEntity(Atlas, team, 
                new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand, 
                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand));
        }

        public void HasAdded(EntityTeam team, Entity enitity)
        {

        }

        public void Update(EntityTeam team, EntityReport report)
        {
            
        }

        public void DebugDraw(EntityTeam team)
        {

        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }


        public void Restart(EntityTeam team)
        {
        }
    }
}
