using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IntelligentRobots.Grid;

using AtlasEngine;

namespace IntelligentRobots.Entities
{
    public class EntityUtil : AtlasEntity
    {
        private GridTrunk _trunk;


        public GridTrunk Trunk{
            get{ return _trunk; }
        }
        public float TimeStamp{
            get{ return Atlas.TotalTime; }
        }

        public EntityUtil(AtlasGlobal atlas, GridTrunk trunk)
            : base(atlas)
        {

        }
    }
}
