using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AtlasEngine;

namespace IntelligentRobots.Entities
{
    public interface EntityDelegate
    {
        Entity WillAdded(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk);
        void HasAdded(EntityTeam team, Entity enitity);

        void Update(EntityTeam team, EntityReport report);

        void DebugDraw(EntityTeam team);

        bool Swappable(EntityTeam team, EntityDelegate entityDelegate);
    }
}
