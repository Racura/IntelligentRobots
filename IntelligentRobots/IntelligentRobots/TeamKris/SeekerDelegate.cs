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
    public class SeekerDelegate : KrisSubDelegate
    {

        public SeekerDelegate(KrisEntityDelegate teamDelegate, Entity entity)
            : base(teamDelegate, entity)
        {
        }

        public override void Update()
        {
            Entity.TryFace(Entity.Angle + 1);

            if (TeamDelegate.Report.Trunk.Version != MapVerison && Path != null)
            {
                TrySetPath(GoToPoint, TeamDelegate.Report.Trunk);
            }

            TryFollowPath();
        }



        public void DrawPath(AtlasGlobal atlas)
        {
            EntityTypes.EntityDebugHelpers.DrawPath(atlas, Path);
        }
    }
}
