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
        private Vector2 _position;

        private float _angle;

        public override float Angle { get { return _angle; } }
        public override bool Crouching { get { return false; } }
        public override float Radius { get { return 24; } }
        public override Vector2 Position { get { return _position; } }

        public VictoryComputerEntity(AtlasGlobal atlas, EntityTeam team, Vector2 position)
            : base(atlas, team)
        {
            this._position = position;
        }

        public override void Update()
        {
            //I do nothing :(
            _angle += Atlas.Elapsed * (Math.Sign(Math.Sin(Atlas.TotalTime)));
            
        }

        public override void Collision(Vector2 v, Vector2 n)
        {
            _position = v + n * Radius;

            
        }
    }
}
