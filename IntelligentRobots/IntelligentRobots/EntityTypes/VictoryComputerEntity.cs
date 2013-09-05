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
    public class VictoryComputerEntity : Entities.Entity
    {
        public const float SPEED = 164;
        

        private Vector2 _position;



        private Vector2 _wantedVelocity;
        private float _wantedAngle;

        public override bool Crouching { get { return false; } }
        public override float Radius { get { return 24; } }
        public override Vector2 Position { get { return _position; } }

        public VictoryComputerEntity(AtlasGlobal atlas, EntityTeam team, Vector2 position)
            : base(atlas, team)
        {

            this._position = position;

            _wantedVelocity = Vector2.Zero;
        }

        public override void Update()
        {
            //I do nothing :(
        }

        public override void Collision(Vector2 v, Vector2 n)
        {
            _position = v + n * Radius;

            
        }
    }
}
