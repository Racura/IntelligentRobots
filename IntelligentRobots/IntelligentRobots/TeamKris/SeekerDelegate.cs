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

namespace IntelligentRobots.TeamKris
{
    public class SeekerDelegate
    {

        public SeekerDelegate(KrisEntityDelegate teamDelegate, Entity entity)
            : base()
        {
            _teamDelegate = teamDelegate;
            _entity = entity;

            WantsObjective = true;
        }

        public void Update(EntityReport report)
        {
            _entity.TryFace(_entity.Angle + 1);

            if (report.Trunk.Version != _mapVerison && _path != null)
            {
                SetPath(GoToPoint, report.Trunk);
            }

            TryFollowPath();
        }


        public void SetPath(Vector2 point, Grid.GridTrunk trunk)
        {
            GoToPoint = point;

            List<Vector2> path;

            if (trunk.TryFindPath(_entity.Position, point, _entity.Radius, out path))
            {
                _path = path;
                _mapVerison = trunk.Version;

                WantsObjective = false;
            }
        }


        public void DrawPath(AtlasGlobal atlas)
        {
            EntityTypes.EntityDebugHelpers.DrawPath(atlas, _path);
        }
    }
}
