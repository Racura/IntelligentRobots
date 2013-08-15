using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using IntelligentRobots.Entities;
using IntelligentRobots.Human;
using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.Player
{
    public class PlayerManager
        : EntityTeam
    {
        HumanEntity _entity;

        public Vector2 Position
        {
            get { return _entity.Position; }
        }

        public PlayerManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _entity = new HumanEntity(Atlas, new PlayerEntityDelegate(atlas));

            Color = Color.Gray;

            Add(_entity);
        }
    }
}
