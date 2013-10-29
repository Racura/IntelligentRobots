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
                for (int i = 0; i < sight.Value.Length; ++i )
                    CompareEntities(sight.Value[i], report.TimeStamp);
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

            entities.Add(new SightEntity()
            {
                lastSeen = timestamp,
                entity = entity
            });
        }

        public EntityStruct GetLastSighting(EntityStruct entity)
        {
            foreach (var e in entities)
            {
                if (e.entity.id == entity.id)
                {
                    return e.entity;
                }
            }

            return entity;
        }

        public float GetLastSightingTime(EntityStruct entity)
        {
            foreach (var e in entities)
            {
                if (e.entity.id == entity.id)
                {

                    return e.lastSeen;
                }
            }

            return 0;
        }


        public class SightEntity
        {
            public EntityStruct entity;
            public float lastSeen;
        }


    }

    public enum EntityStatus
    {
        Enemy,
        Ally,
        Goal
    }
}
