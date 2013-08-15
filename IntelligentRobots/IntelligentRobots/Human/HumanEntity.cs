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

namespace IntelligentRobots.Human
{
    public class HumanEntity : Entities.Entity
    {
        public const float STANDING_SPEED = 192;
        public const float CROUCHING_SPEED = 64;

        private bool _crouching;
        private float _radius;
        private Vector2 _position;
        private Vector2 _velocity;
        private Vector2 _direction;

        private Vector2 _wantedVelocity;
        private float _crouchHeight;
        private bool _wantedCrouching;

        public override bool Crouching { get { return _crouching; } }
        public override float Radius { get { return _radius; } }
        public override Vector2 Position { get { return _position; } }
        public override Vector2 Velocity { get { return _velocity; } }

        public override Vector2 Direction { get { return _direction; } }

        public HumanEntity(AtlasGlobal atlas, EntityDelegate entityDelegate)
            : base(atlas, entityDelegate)
        {
            _radius = 14;
            _direction = Vector2.UnitY;

            _wantedVelocity = Vector2.Zero;
        }

        public override void Update()
        {
            _crouchHeight = MathHelper.Clamp(_crouchHeight + Atlas.Elapsed * 4f * (_wantedCrouching ? -1 : 1), 0, 1);

            float speed = (CROUCHING_SPEED + (STANDING_SPEED - CROUCHING_SPEED) * _crouchHeight) / STANDING_SPEED;

            if (_wantedVelocity.LengthSquared() > speed * speed)
            {
                _wantedVelocity.Normalize();
                _wantedVelocity = _wantedVelocity * speed;
            }

            _velocity += (_wantedVelocity - _velocity) * (8 * Atlas.Elapsed);

            if (Math.Abs(_velocity.X) < 0.001f && Math.Abs(_velocity.Y) < 0.001f)
            {
                _velocity = Vector2.Zero;
            }

            _position += _velocity * (STANDING_SPEED * Atlas.Elapsed);

            if (_velocity.X != 0 || _velocity.Y != 0)
            {
                _direction = Vector2.Normalize(_direction - (_direction - Vector2.Normalize(_velocity)) * Math.Max(Atlas.Elapsed * 16, 1));

            }

            _crouching = _crouchHeight <= 0.1f;
        }


        public override bool TryMove(Vector2 v) {
            _wantedVelocity = v;
            return true; 
        }

        public override bool TryCrouching(bool crouching)
        {
            _wantedCrouching = crouching;
            return true;
        }

        public override bool TryFaceDirection(Vector2 v) { return false; }

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
