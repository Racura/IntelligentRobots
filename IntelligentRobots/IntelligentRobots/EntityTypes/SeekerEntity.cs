using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AtlasEngine;
using AtlasEngine.BasicManagers;

using IntelligentRobots.Entities;

namespace IntelligentRobots.EntityTypes
{
    public class SeekerEntity : Entities.Entity
    {
        public const float SPEED = 164;
        

        private float _radius;
        private Vector2 _position;
        private Vector2 _velocity;
        private float _angle;



        private Vector2 _wantedVelocity;
        private float _wantedAngle;

        public override bool Crouching { get { return true; } }
        public override float Radius { get { return _radius; } }
        public override Vector2 Position { get { return _position; } }
        public override Vector2 Velocity { get { return _velocity; } }

        public override float Angle { get { return _angle; } }

        public override float FOV { get { return MathHelper.Pi * 1.5f; } }

        public SeekerEntity(AtlasGlobal atlas, EntityTeam team, Vector2 position)
            : base(atlas, team)
        {
            _radius = 10;
            _angle = 0;

            this._position = position;

            _wantedVelocity = Vector2.Zero;
        }

        public override void Update()
        {
            //_crouchHeight = MathHelper.Clamp(_crouchHeight + Atlas.Elapsed * 4f * (_wantedCrouching ? -1 : 1), 0, 1);


            float speed = 1;

            if (_wantedVelocity.LengthSquared() > speed * speed)
            {
                _wantedVelocity.Normalize();
                _wantedVelocity = _wantedVelocity * speed;
            }


            while (Math.Abs(_wantedAngle - _angle) > Math.PI + 0.000001f)
                _wantedAngle -= (float)(Math.Sign(_wantedAngle - _angle) * 2 * Math.PI);

            _angle += Math.Min(Math.Abs(_wantedAngle - _angle), Atlas.Elapsed * 4) * Math.Sign(_wantedAngle - _angle);

            while (Math.Abs(_angle) > Math.PI + 0.000001f)
                _angle = _angle - (float)(Math.Sign(_angle) * 2 * Math.PI);
                        

            _velocity += (_wantedVelocity - _velocity) * (16 * Atlas.Elapsed);

            if (Math.Abs(_velocity.X) < 0.001f && Math.Abs(_velocity.Y) < 0.001f)
            {
                _velocity = Vector2.Zero;
            }
            else
            {
                float spec = (float)(1 - RadianDifference((float)Math.Atan2(_velocity.Y, _velocity.X), _angle) / Math.PI) * 0.5f;
                _position += _velocity * (SPEED * Atlas.Elapsed * (0.5f + spec));
            }
        }

        private float RadianDifference(float r1, float r2)
        {
            while (Math.Abs(r1 - r2) > Math.PI + 0.0001f)
                r1 -= (float)(Math.Sign(r1 - r2) * 2 * Math.PI);

            return Math.Abs(r1 - r2);
        }

        public override bool TryMove(Vector2 v) {
            _wantedVelocity = v;
            return true; 
        }

        public override bool TryCrouching(bool crouching)
        {
            //_wantedCrouching = crouching;
            return false;
        }

        public override bool TryFace(float angle)
        {
            _wantedAngle = angle;

            return true;
        }

        public override void Collision(Vector2 v, Vector2 n)
        {
            _position = v + n * Radius;

            _velocity = new Vector2(
                Math.Sign(Velocity.X) * Math.Min(Math.Abs(_velocity.X), 1 - Math.Abs(n.X)),
                Math.Sign(_velocity.Y) * Math.Min(Math.Abs(_velocity.Y), 1 - Math.Abs(n.Y))
            );
        }
    }
}
