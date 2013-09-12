/*
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

namespace IntelligentRobots.TeamAlek
{
    
    public class AlekEntityDelegate : AtlasEntity, EntityDelegate
    {
        List<Vector2> _vectorList;
        int _version;
        Vector2 _destination;
        float timer;
        Vector2 _direction;
        

        public AlekEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            _version = -1;
        }

       

        public float getFloatAngle(Vector2 vector)
        {
            if (vector != null && (vector.X != 0 || vector.Y != 0))
                return (float)Math.Atan2(vector.Y, vector.X);
            else return 0;
        }


        public void Update(Entity entity, EntityReport report)
        {
            entity.TryCrouching((report.ListEntity.Length > 0));
           
        
            if (_vectorList == null || _vectorList.Count < 2)
            {
                if (report.Trunk.TryFindPath(entity.Position, new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = report.Trunk.Version;
                }
            }
            else if (_version != report.Trunk.Version)
            {
                if (!pathToSamePoint(entity, report))
                {
                    pathToRandomPoint(entity, report);
                }
            }
            
           


            if (_vectorList != null && _vectorList.Count > 1)
            {
                _direction = _vectorList[1] - entity.Position;
                entity.TryMove(_direction);
                
                if (hitPoint(entity.Position, _vectorList[1], entity.Radius)) // fixed the idle problem
                {
                    _vectorList.RemoveAt(1);
                }
            }
            else
            {
                entity.TryMove(Vector2.Zero);
                

            }

            timer += Atlas.Elapsed;
            if (timer > 2f)
            {
                float angle = entity.Angle + 2;
                entity.TryFace(angle);
            }
            else
            {
                entity.TryFace(getFloatAngle(_direction));
            }
            if (timer > 3.0f)
            {
                timer = 0;
                
            }
            
        }

        private bool pathToSamePoint(Entity entity, EntityReport report)
        {

            if (report.Trunk.TryFindPath(entity.Position, _vectorList[_vectorList.Count - 1], entity.Radius, out _vectorList))
            {
                _version = report.Trunk.Version;
                return true;
            }
            return false;
        }

        private void pathToRandomPoint(Entity entity, EntityReport report)
        {
            if (_vectorList == null || _vectorList.Count < 2)
            {
                if (report.Trunk.TryFindPath(entity.Position, new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = report.Trunk.Version;
                }
            }
        }

        public void Update2(Entity entity, EntityReport report)
        {
            if (_vectorList == null || _vectorList.Count == 0)
            {
                if (report.Trunk.TryFindPath(entity.Position, new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = report.Trunk.Version;
                }
            }
            else if (_version != report.Trunk.Version)
            {
                _vectorList = null;
            }


            if (_vectorList != null && _vectorList.Count != 0)
            {
                entity.TryMove(_vectorList[0] - entity.Position);
                if(hitPoint(entity.Position, _vectorList[0], entity.Radius))
                {
                    _vectorList.RemoveAt(0);
                }
            }
            else
            {
                entity.TryMove(Vector2.Zero);
            }
        }

      
        //check if we hit the point
        private bool hitPoint(Vector2 v1, Vector2 v2, float radius)
        {
            if (Vector2.Distance(v1, v2) < (radius))
            {
                return true;
            }

            return false;
        }
        public void DebugDraw(Entity entity)
        {
        }

        public bool Swappable(Entity entity, EntityDelegate entityDelegate)
        {
            return true;
        }
    }
     
}*/


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
using IntelligentRobots.EntityTypes;

namespace IntelligentRobots.TeamAlek
{
    //Controlls team
    public class AlekEntityDelegate : AtlasEntity, EntityDelegate
    {

        LinkedList<AlekEntitySubDelegate> team = new LinkedList<AlekEntitySubDelegate>();
        

        public AlekEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
        }

        public Entity WillAdded(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            var rand = (int)(possibleLocations.Length * Atlas.Rand);

            return new EntityTypes.HumanEntity(Atlas, team, 
                new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand, 
                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand));
        }

        public void HasAdded(EntityTeam team, Entity enitity)
        {

        }

        private bool hitPoint(Vector2 v1, Vector2 v2, float radius)
        {
            if (Vector2.Distance(v1, v2) < (radius))
            {
                return true;
            }

            return false;
        }

        public float getFloatAngle(Vector2 vector)
        {
            if (vector != null && (vector.X != 0 || vector.Y != 0))
                return (float)Math.Atan2(vector.Y, vector.X);
            else return 0;
        }


        public void Update(EntityTeam entityTeam, EntityReport report)
        {
            entityTeam.Color = AtlasColorSystem.GetColorFromHue(Atlas.TotalTime * 360); ;
            if (entityTeam.TeamMembers.Length == 0) return;
            var entity = entityTeam.TeamMembers[0];

            foreach (Entity e in entityTeam.TeamMembers)
            {

            }
            
            //entity.TryCrouching((report.SightList.Count > 0));
            Vector2? destination = getWinPosition(entity, report);

            if (destination.HasValue)
            {
                pathToSpecificPoint(entity, report, destination.Value);
                panic = true;
                panicTimer = 5f;
            }

            if (_vectorList == null || _vectorList.Count < 2)
            {
                if (report.Trunk.TryFindPath(entity.Position, new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = report.Trunk.Version;
                }
            }
            else if (_version != report.Trunk.Version)
            {
                if (!pathToSamePoint(entity, report))
                {
                    pathToRandomPoint(entity, report);
                }
            }




            if (_vectorList != null && _vectorList.Count > 1)
            {
                _direction = _vectorList[1] - entity.Position;
                entity.TryMove(_direction);

                if (hitPoint(entity.Position, _vectorList[1], entity.Radius)) // fixed the idle problem
                {
                    _vectorList.RemoveAt(1);
                }
            }
            else
            {
                entity.TryMove(Vector2.Zero);


            }

            if (!panic)
            {
                timer += Atlas.Elapsed;
                if (timer > 2f)
                {
                    float angle = entity.Angle + 2;
                    entity.TryFace(angle);
                }
                else
                {
                    entity.TryFace(getFloatAngle(_direction));
                }
                if (timer > 3.0f)
                {
                    timer = 0;

                }
            }
            else
            {
                entity.TryFace(getFloatAngle(_direction));
                panicTimer -= Atlas.Elapsed;
                if (panicTimer <= 0)
                {
                    panic = false;
                    
                }
            }

        }

        private Vector2? getWinPosition(Entity entity, EntityReport report)
        {
            //var e = report.SightList[entity][0].type == typeof(VictoryComputerEntity);
            foreach (var s in report.SightList[entity])
            {
                if (s.type ==  typeof(VictoryComputerEntity))
                {
                    
                    return s.position;
                }
            }
            return null;
        }

        private bool pathToSamePoint(Entity entity, EntityReport report)
        {

            if (report.Trunk.TryFindPath(entity.Position, _vectorList[_vectorList.Count - 1], entity.Radius, out _vectorList))
            {
                _version = report.Trunk.Version;
                return true;
            }
            return false;
        }

        private void pathToSpecificPoint(Entity entity, EntityReport report, Vector2 pos)
        {
            if (report.Trunk.TryFindPath(entity.Position, pos, entity.Radius, out _vectorList))
            {
                _version = report.Trunk.Version;
            }

        }

        private void pathToRandomPoint(Entity entity, EntityReport report)
        {
            if (_vectorList == null || _vectorList.Count < 2)
            {
                if (report.Trunk.TryFindPath(entity.Position, new Vector2(report.Trunk.Width * Atlas.Rand, report.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = report.Trunk.Version;
                }
            }
        }

        public void DebugDraw(EntityTeam team)
        {

        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }

    }
}
