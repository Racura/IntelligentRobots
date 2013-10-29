using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using IntelligentRobots.Entities;

namespace IntelligentRobots.TeamKris
{
    public class DelegateOrder
    {
        public float Weight     { get; private set; }
        public OrderType Type   { get; private set; }
        public object Target    { get; set; }

        public bool IsTargetEntity { get { return typeof(EntityStruct) == Target.GetType(); } }
        public bool IsTargetVector2     { get { return typeof(Vector2) == Target.GetType(); } }
        public bool IsTargetRectange    { get { return typeof(Rectangle) == Target.GetType(); } }

        public EntityStruct TargetAsEntity
        {
            get
            {
                if (typeof(EntityStruct) == Target.GetType())
                    return (EntityStruct)Target;
                return new EntityStruct();
            }
        }
        public Vector2 TargetAsVector2
        {
            get
            {
                if (typeof(Vector2) == Target.GetType())
                    return (Vector2)Target;
                return Vector2.Zero;
            }
        }
        public Rectangle TargetAsRectangle
        {
            get
            {
                if (typeof(Rectangle) == Target.GetType())
                    return (Rectangle)Target;
                return Rectangle.Empty;
            }
        }

        public DelegateOrder(OrderType type, object target)
        {
            Type = type;
            Target = target;

            Weight = BaseOrderWeight(Type);

            if (!IsValid())
                Weight = 1000000;
        }

        public static float BaseOrderWeight(OrderType type)
        {
            switch (type)
            {
                case OrderType.Move:
                    return 0.8f; //
                case OrderType.Follow:
                    return 0.8f; //
                case OrderType.Defend:
                    return 1f;
                case OrderType.Attack:
                    return 0.5f;
            }

            return 100;
        }

        public float DistanceToTargetSquared(Vector2 point)
        {
            if (IsTargetVector2)
            {
                return Vector2.DistanceSquared(TargetAsVector2, point);
            }
            if (IsTargetRectange)
            {
                var rect = TargetAsRectangle;
                return Vector2.DistanceSquared(
                    new Vector2(
                            MathHelper.Clamp(point.X, rect.X, rect.X + rect.Width), 
                            MathHelper.Clamp(point.Y, rect.Y, rect.Y + rect.Height)), 
                        point);
            }
            if (IsTargetEntity)
            {
                return Vector2.DistanceSquared(TargetAsEntity.position, point);
            }
            return float.MaxValue;
        }


        public bool IsValid()
        {
            switch(Type)
            {
                case OrderType.Move:
                    return typeof(Vector2) == Target.GetType();
                case OrderType.Defend:
                    return typeof(Rectangle)    == Target.GetType()
                        || typeof(Vector2)      == Target.GetType()
                        || typeof(EntityStruct) == Target.GetType();
                case OrderType.Follow:
                    return typeof(EntityStruct) == Target.GetType();
                case OrderType.Attack:
                    return typeof(EntityStruct) == Target.GetType();
            }

            return false;
        }

        public bool IsDuplicate(DelegateOrder order)
        {
            if (order.Type == this.Type)
            {
                if (IsTargetEntity && order.IsTargetEntity)
                    return TargetAsEntity.id == order.TargetAsEntity.id;

                if (IsTargetRectange && order.IsTargetRectange)
                    return TargetAsRectangle == order.TargetAsRectangle;

                if (IsTargetVector2 && order.IsTargetVector2)
                    return TargetAsVector2 == order.TargetAsVector2;

            }
            return false;
        }
    }

    public enum OrderType
    {
        Move,
        Defend,
        Follow,
        Attack
    }
}
