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

        public AlekEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            _version = -1;
        }

        public void spinbotUpdate(Entity entity, EntityUtil util)
        {
            
            Vector2 direction = Vector2.TransformNormal(entity.Direction, Matrix.CreateRotationZ(1f));
            entity.TryMove(direction);
            entity.TryFace(direction);
        }

        public void Update(Entity entity, EntityUtil util)
        {
           
            //spinbotUpdate(entity, util);
            //return;
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
                entity.TryMove(_vectorList[1] - entity.Position);
                entity.TryFace(_vectorList[1] - entity.Position);
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
                Vector2 direction = Vector2.TransformNormal(entity.Direction, Matrix.CreateRotationZ(1));
                entity.TryFace(direction);
            }
            if (timer > 2.75f)
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

        public void Report(Entity entity)
        {
        }

        public void DebugDraw(Entity entity)
        {
        }
    }
}
