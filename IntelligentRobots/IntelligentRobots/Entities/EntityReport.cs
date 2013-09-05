using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IntelligentRobots.Grid;

namespace IntelligentRobots.Entities
{
    public class EntityReport
    {
        private GridTrunk _trunk;
        private Dictionary<Entity, EntityStruct[]> _sightList;

        public bool Locked
        {
            get;
            private set;
        }

        public Dictionary<Entity, EntityStruct[]> SightList
        {
            get { return _sightList; }
            set
            {
                if (!Locked)
                    _sightList = value;
            }
        }
        public GridTrunk Trunk
        {
            get { return _trunk; }
            set
            {
                if (!Locked)
                    _trunk = value;
            }
        }
        public float TimeStamp
        {
            get { return 0; }
        }


        public EntityReport(GridTrunk trunk)
        {
            Locked = false;
            _trunk = trunk;
            _sightList = new Dictionary<Entity, EntityStruct[]>();
        }

        public void Lock()
        {
            Locked = true;
        }
    }
}
