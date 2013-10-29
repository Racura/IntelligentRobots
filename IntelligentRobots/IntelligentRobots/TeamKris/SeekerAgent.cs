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
    public class SeekerAgent : EntityAgent
    {
        public float CurrentJobCost { get; protected set; }

        public bool _dirty;

        public SeekerAgent(KrisEntityDelegate teamDelegate, Entity entity)
            : base(teamDelegate, entity)
        {
        }

        public override void Update()
        {
            if (Order == null) // if no oder, move to a random point
            {
                MoveToRandom();
                return;
            }

            switch (Order.Type)
            {
                case OrderType.Move:

                    SearchLogic();
                    break;
                case OrderType.Follow:
                    FollowLogic();
                    break;
                default:
                    MoveToRandom();
                    break;
            }

            var sightList = TeamDelegate.Report.SightList[Entity];
            foreach (var e in sightList)
            {
                switch (TeamDelegate.GetEntityStatus(e))
                {
                    case EntityStatus.Enemy:
                        TeamDelegate.AskTeam(new DelegateOrder(OrderType.Follow, e));
                        TeamDelegate.AskTeam(new DelegateOrder(OrderType.Attack, e));
                        break;
                    case EntityStatus.Goal:
                        break;
                }
            }
        }

        private void MoveToRandom()
        {

            SetOrder(new DelegateOrder(OrderType.Move,
                new Vector2(TeamDelegate.Rand * TeamDelegate.Report.Trunk.Width, TeamDelegate.Rand * TeamDelegate.Report.Trunk.Height)));//LITTLE bit of random

            CurrentJobCost = 0;
        }

        private void SearchLogic()
        {
            //i want to get the robot inside an area
            if (_dirty)
            {
                if (Order.IsTargetVector2)
                {
                    TrySetPath(Order.TargetAsVector2, TeamDelegate.Report.Trunk);
                }

                _dirty = false;
            }

            if (Path != null)
            {
                FollowPath();
            }

            if (Path == null || Vector2.DistanceSquared(Order.TargetAsVector2, Entity.Position) < 32 * 32)
                MoveToRandom();
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

            //if path is invalid or at last seen point, and it's been more than 6 seconds since last seen
            if (Path == null
                || (Vector2.DistanceSquared(Order.TargetAsEntity.position, Entity.Position) < 32 * 32)
                && time > 6)
                MoveToRandom();
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
                || !order.IsValid())
            {
                cost = 0;
                return false;
            }

            cost = 0;

            //The cost is: What my opinion is of what it weights, plus how much i like what im doing
            switch (order.Type)
            {
                case OrderType.Move:
                    cost = order.Weight * 1.0f * order.DistanceToTargetSquared(Entity.Position)
                         + CurrentJobCost; //Likes to Trail Targets
                    return true;
                case OrderType.Defend:
                    cost = order.Weight * 1.2f * order.DistanceToTargetSquared(Entity.Position)
                        + CurrentJobCost; //Seekers great at watching stuff 
                    return true;
                case OrderType.Follow:
                    cost = order.Weight * 0.8f * order.DistanceToTargetSquared(Entity.Position)
                        + CurrentJobCost; //Seekers arent very good at defending by themselves.  So it will only defend if it's must 
                    return true;
            }

            return false;
        }
    }
}
