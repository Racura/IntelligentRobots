using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;

using AtlasEngine;

namespace IntelligentRobots.TeamKris
{
    public class KrisSubDelegate
    {
        Entity _entity;
        KrisEntityDelegate _teamDelegate;

        List<Vector2> _path;
        int _mapVerison;

        public Vector2 GoToPoint { get; protected set; }
        public bool WantsObjective { get; protected set; }



        protected virtual void TryFollowPath()
        {
            if (_path == null || _path.Count == 0)
            {
                _entity.TryMove(Vector2.Zero);
                WantsObjective = true;
                return;
            }

            if (Vector2.DistanceSquared(_path[0], _entity.Position) < 16 * 16)
            {
                _path.RemoveAt(0);
            }

            if (_path.Count > 0)
            {
                Vector2 wantedDir = (_path[0] - _entity.Position) * 1;

                _entity.TryMove(wantedDir);
                _entity.TryFace((float)(Math.Atan2(wantedDir.Y, wantedDir.X) + Math.Sin(_teamDelegate.Report.TimeStamp)));
            }
        }
    }
}
