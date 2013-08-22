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

        public void Update(Entity entity, EntityUtil util)
        {
           
        
            if (_vectorList == null || _vectorList.Count < 2)
            {
                if (util.Trunk.TryFindPath(entity.Position, new Vector2(util.Trunk.Width * Atlas.Rand, util.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = util.Trunk.Version;
                }
            }
            else 
            if (_version != util.Trunk.Version)
            {
                if (!pathToSamePoint(entity, util))
                {
                    pathToRandomPoint(entity, util);
                }
            }
            
           


            if (_vectorList != null && _vectorList.Count > 1)
            {
                _direction = _vectorList[1] - entity.Position;
                entity.TryMove(_direction);
                
                if (hitPoint(entity.Position, _vectorList[0], entity.Radius))
                {
                    _vectorList.RemoveAt(0);
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

        private bool pathToSamePoint(Entity entity, EntityUtil util)
        {
            
            if (util.Trunk.TryFindPath(entity.Position, _vectorList[_vectorList.Count-1], entity.Radius, out _vectorList))
            {
                _version = util.Trunk.Version;
                return true;
            }
            return false;
        }

        private void pathToRandomPoint(Entity entity, EntityUtil util)
        {
            if (_vectorList == null || _vectorList.Count < 2)
            {
                if (util.Trunk.TryFindPath(entity.Position, new Vector2(util.Trunk.Width * Atlas.Rand, util.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = util.Trunk.Version;
                }
            }
        }

        public void Update2(Entity entity, EntityUtil util)
        {
            if (_vectorList == null || _vectorList.Count == 0)
            {
                if (util.Trunk.TryFindPath(entity.Position, new Vector2(util.Trunk.Width * Atlas.Rand, util.Trunk.Height * Atlas.Rand), entity.Radius, out _vectorList))
                {
                    _version = util.Trunk.Version;
                }
            }
            else if (_version != util.Trunk.Version)
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

        public void Report(Entity entity, EntityReport report)
        {
                entity.TryCrouching((report.ListEntity.Length > 0));
            
        }

        public void DebugDraw(Entity entity)
        {
        }

        public bool Swappable(Entity entity, EntityDelegate entityDelegate)
        {
            return true;
        }
    }
}
