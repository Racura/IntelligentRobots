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

    /// <summary>
    /// This class is the base class for all of the entity agents
    /// </summary>
    public class EntityAgent
    {

        public DelegateOrder Order { get; private set; }
        public Entity Entity { get; private set; }
        public KrisEntityDelegate TeamDelegate { get; private set; }

        public List<Vector2> Path { get; protected set; }
        public int MapVerison { get; protected set; }

        public Vector2 GoToPoint { get; protected set; }
        public bool WantsObjective { get; protected set; }


        public EntityAgent(KrisEntityDelegate teamDelegate, Entity entity)
            : base()
        {
            TeamDelegate = teamDelegate;
            Entity = entity;

            WantsObjective = true;
        }



        protected virtual void FollowPath()
        {
            if (Path != null && MapVerison != TeamDelegate.Report.Trunk.Version)
            {
                Path = null;
                TrySetPath(GoToPoint, TeamDelegate.Report.Trunk);
            }

            if (Path == null || Path.Count == 0)
            {
                Entity.TryMove(Vector2.Zero);
                WantsObjective = true;
                return;
            }

            if (Vector2.DistanceSquared(Path[0], Entity.Position) < Entity.Radius * Entity.Radius)
            {
                Path.RemoveAt(0);
            }

            if (Path.Count > 0)
            {
                Vector2 wantedDir = (Path[0] - Entity.Position) * 160;
                Entity.TryMove(wantedDir);
                Entity.TryFace((float)(Math.Atan2(wantedDir.Y, wantedDir.X) + Math.Sin(TeamDelegate.Report.TimeStamp * 8) * 1f));
            }
        }

        public bool TrySetPath(Vector2 point, Grid.GridTrunk trunk)
        {
            GoToPoint = point;

            List<Vector2> path;

            if (trunk.TryFindPath(Entity.Position, point, Entity.Radius, out path)) {

                path.RemoveAt(0);

                Path = path;
                MapVerison = trunk.Version;

                WantsObjective = false;

                return true;
            }

            return false;
        }

        public virtual bool TryOrder(DelegateOrder order, out float cost)
        {
            cost = 0;
            return false;
        }

        public virtual void SetOrder(DelegateOrder order)
        {
            Order = order;
        }


        public virtual void Update()
        {

        }

        public virtual void Draw(AtlasGlobal atlas)
        {

        }
    }
}
