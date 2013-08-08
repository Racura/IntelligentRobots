using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using AtlasEngine.Infrastructure;

using IntelligentRobots.Component;

namespace IntelligentRobots
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : AtlasEngine.Infrastructure.AtlasGame, IAtlasGamePage
    {
        GameStateComponent _component;

        public Game1()
            : base()
        {
        }

        protected override void Initialize()
        {
            _component = new GameStateComponent(this, graphics);
            Components.Add(_component);
            
            base.Initialize();
        }

        public void NavigateTo(string arg) { }

        public bool Active
        {
            get
            {
                return true;
            }
            set
            {
                
            }
        }

        public AtlasGame Game
        {
            get { return this; }
        }
    }
}
