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

        public AlekEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
        }

        public void Update(Entity entity, EntityUtil util)
        {
            
            if (_vectorList == null || _vectorList.Count == 0)
            {
                //get random point
                Vector2 randomPoint = new Vector2(util.Trunk.Width * Atlas.Rand, util.Trunk.Height * Atlas.Rand);

                //get path to random point
                util.Trunk.TryFindPath(entity.Position, randomPoint, entity.Radius, out _vectorList);
            }

            
            if (_vectorList != null && _vectorList.Count != 0)
            {
                Vector2 direction = _vectorList[0] - entity.Position;
                entity.TryMove(direction);
                if (hitPoint(entity.Position, _vectorList[0], entity.Radius))
                {
                    _vectorList.RemoveAt(0);
                }
                

            }
        }

        //check if we hit the point
        private bool hitPoint(Vector2 v1, Vector2 v2, float radius)
        {
            if (Vector2.DistanceSquared(v1, v2) < (radius*radius))
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
