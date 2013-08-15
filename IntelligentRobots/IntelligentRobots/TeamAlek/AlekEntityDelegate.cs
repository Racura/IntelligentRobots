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

        public AlekEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            _version = -1;
        }

        public void Update(Entity entity, EntityUtil util)
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

        private void ohgod(Entity entity, EntityUtil util)
        {
            if (_vectorList == null || _vectorList.Count == 0 || _version != util.Trunk.Version)
            {
                if (_vectorList == null || _vectorList.Count == 0)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        _destination = new Vector2(util.Trunk.Width * Atlas.Rand, util.Trunk.Height * Atlas.Rand);

                        if (util.Trunk.TryFindPath(entity.Position, _destination, entity.Radius, out _vectorList))
                        {

                            break;
                        }
                    }
                }
                else
                {
                    if (util.Trunk.TryFindPath(entity.Position, _destination, entity.Radius, out _vectorList))
                    {

                        _version = util.Trunk.Version;
                    }

                }




                //check if path exists
                if (_vectorList != null && _vectorList.Count != 0)
                {
                    Vector2 direction = _vectorList[0] - entity.Position;
                    entity.TryMove(direction);
                    if (hitPoint(entity.Position, _vectorList[0], entity.Radius))
                    {
                        _vectorList.RemoveAt(0);
                    }

                }
                else
                {
                    entity.TryMove(Vector2.Zero);
                }
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
