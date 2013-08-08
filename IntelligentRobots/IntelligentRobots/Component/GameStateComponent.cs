﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;
using AtlasEngine.Infrastructure;
using AtlasEngine.BasicManagers;

using IntelligentRobots.Grid;
using IntelligentRobots.Player;

namespace IntelligentRobots.Component
{
    public class GameStateComponent : AtlasComponent
    {
        private CameraManager _camera;
        private GridManager _grid;
        private PlayerManager _player;

        public GameStateComponent(IAtlasGamePage gamePage, GraphicsDeviceManager graphicsDeviceManager)
            : base(gamePage, graphicsDeviceManager)
        {
             
        }

        public override void Initialize()
        {
            base.Initialize();

            Atlas.Content.LoadContent("image/mouse");
            
            _camera = new CameraManager(Atlas, Vector2.Zero, 300, new RectangleF(-10000, -10000, 20000, 20000));
            _grid = new GridManager(Atlas);
            _player = new PlayerManager(Atlas);

            AddManager(new AtlasManagerSorter(_camera, "camera",    1, 9999));
            AddManager(new AtlasManagerSorter(_grid, "camera",      120, 120));
            AddManager(new AtlasManagerSorter(_player, "camera",    100, 100));
        }

        public override void PreDraw()
        {
            Atlas.Graphics.Clear(Color.Black);
        }

        public override void PostDraw()
        {
            Atlas.Graphics.SetMatrixHandler(null);

            foreach (var t in Atlas.Input.GetTouchCollection())
            {
                Atlas.Graphics.DrawSprite(
                    Atlas.Content.GetContent<Texture2D>("image/mouse"),
                    t.Position, Color.Wheat);
            }

            Atlas.Graphics.Flush();
        }
    }
}
