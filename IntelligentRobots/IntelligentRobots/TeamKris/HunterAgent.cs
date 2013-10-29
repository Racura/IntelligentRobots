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

    public class HunterAgent : EntityAgent
    {
        public float CurrentJobCost { get; protected set; }

        public bool _dirty;

        public HunterAgent(KrisEntityDelegate teamDelegate, Entity entity)
            : base(teamDelegate, entity)
        {
        }

        public override void Update()
        {
            if (Order != null && Order.Type == OrderType.Attack)
            {
                FollowLogic();
            }

            var sightList = TeamDelegate.Report.SightList[Entity];
            foreach (var e in sightList)
            {
                switch (TeamDelegate.GetEntityStatus(e))
                {
                    case EntityStatus.Enemy:
                        TeamDelegate.AskTeam(new DelegateOrder(OrderType.Follow, e));
                        break;
                    case EntityStatus.Goal:
                        break;
                }
            }
        }

        private void FollowLogic()
        {
            Order.Target = TeamDelegate.FactSheet.GetLastSighting(Order.TargetAsEntity);

            float time = TeamDelegate.Report.TimeStamp - TeamDelegate.FactSheet.GetLastSightingTime(Order.TargetAsEntity);

            if (_dirty || Vector2.DistanceSquared(Order.TargetAsEntity.position + Order.TargetAsEntity.velocity * time, GoToPoint) > 16 * 16)
            {
                TrySetPath(Order.TargetAsEntity.position, TeamDelegate.Report.Trunk);

                CurrentJobCost = Order.DistanceToTargetSquared(Entity.Position) * 2;
            }

            if (Path != null)
            {
                FollowPath();
            }

        }



        public override void Draw(AtlasGlobal atlas)
        {
            EntityTypes.EntityDebugHelpers.DrawPath(atlas, Path);
        }


        public override void SetOrder(DelegateOrder order)
        {
            base.SetOrder(order);

            Path = null;
            _dirty = true;

            CurrentJobCost = float.MaxValue;
        }

        public override bool TryOrder(DelegateOrder order, out float cost)
        {
            //deny attack orders, as a seeker cannot attack
            if (order.Type == OrderType.Attack
                && order.IsValid())
            {
                cost = order.Weight * 1.0f * order.DistanceToTargetSquared(Entity.Position)
                     + CurrentJobCost;
                return Order == null
                    || (order.DistanceToTargetSquared(Entity.Position) < Order.DistanceToTargetSquared(Entity.Position)
                        && TeamDelegate.Report.TimeStamp - TeamDelegate.FactSheet.GetLastSightingTime(Order.TargetAsEntity) > 2);
            }

            cost = 0;
            return false;
        }
    }
}
