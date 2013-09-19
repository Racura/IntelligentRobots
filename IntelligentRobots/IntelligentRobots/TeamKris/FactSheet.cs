using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using IntelligentRobots.Entities;
using IntelligentRobots.EntityTypes;

namespace IntelligentRobots.TeamKris
{
    public class FactSheet
    {
        public List<SightEntity> entities;

        private KrisEntityDelegate _teamDelegate;

        public FactSheet(KrisEntityDelegate teamDelegate)
        {
            _teamDelegate = teamDelegate;
            entities = new List<SightEntity>();
        }

        public void UpdateFactSheet(EntityReport report)
        {
            List<EntityStruct> list = new List<EntityStruct>();

            foreach (var sight in report.SightList)
            {
                foreach (var entity in sight.Value)
                {
                    bool found = false;

                    for (int i = 0; i < list.Count; i++)
                    {
                        if (entity.Equals(list[i]))
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                        list.Add(entity);
                }
            }
        }

        private void CompareEntities(EntityStruct entity, float timestamp)
        {
            foreach (var e in entities)
            {
                if (e.entity.id == entity.id)
                {
                    e.entity = entity;
                    e.lastSeen = timestamp;

                    return;
                }
            }

            entities.Add( new SightEntity () {
                lastSeen = timestamp,
                entity = entity
            });
        }


        public class SightEntity
        {
            public EntityStruct entity;
            public float lastSeen;


        }



        public enum EntityType
        {
            Hostile,
            Ally,
            Goal
        }
    }
}
