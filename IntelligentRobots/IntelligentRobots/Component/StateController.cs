using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using AtlasEngine;


namespace IntelligentRobots.Component
{
    public class StateController : AtlasEngine.AtlasManager
    {
        [Flags]
        public enum GameState
        {
            Paused,
            Combat
        }

        [Flags]
        public enum PerpectiveState
        {
            World,
            Entity,
            Team
        }

        //PerpectiveState _perpectiveState;
        public int PerpectiveNumber
        {
            get;
            protected set;
        }
        public GameState      State
        {
            get;
            protected set;
        }
        public PerpectiveState Perpective
        {
            get;
            protected set;
        }

        public float TimePassed
        {
            get;
            protected set;
        }

        private bool _started;

        public StateController(AtlasGlobal atlas)
            : base(atlas)
        {
            this.State = GameState.Paused;
            this.Perpective = PerpectiveState.World;

            PerpectiveNumber = 0;

            _started = false;
        }

        public override void Update(string arg)
        {
            if (!_started) {
                Restart();
                _started = true;
            }

            base.Update(arg);

            TimePassed += Atlas.Elapsed;

            if (Atlas.Input.IsKeyJustReleased(Keys.Enter))
            {
                State = State == GameState.Combat ? GameState.Paused : GameState.Combat;
            }

            if (Atlas.Input.IsKeyJustReleased(Keys.R))
            {
                Restart();
            }


            if (Atlas.Input.IsKeyJustReleased(Keys.D1))
                this.Perpective = PerpectiveState.World;
            if (Atlas.Input.IsKeyJustReleased(Keys.D2))
                this.Perpective = PerpectiveState.Team;
            if (Atlas.Input.IsKeyJustReleased(Keys.D3))
                this.Perpective = PerpectiveState.Entity;


            if (Atlas.Input.IsKeyJustReleased(Keys.Add))
                PerpectiveNumber++;
            if (Atlas.Input.IsKeyJustReleased(Keys.Subtract))
                PerpectiveNumber--;
        }

        private void Restart()
        {
            State = GameState.Paused;
            TimePassed = 0;

            var em = Atlas.GetManager<Entities.EntityManager>();
            var gm = Atlas.GetManager<Grid.GridManager>();
            em.NewGame();

            float w = gm.Trunk.Width * 0.33f;
            float h = gm.Trunk.Height * 0.33f;
                    
            for (int j = 0; j < 4; ++j )
                em.Spawn("Kris", new RectangleF[] { new RectangleF(w * 1, h * 2, w, h ) });
            for (int j = 0; j < 3; ++j )
                em.Spawn("Aleks", new RectangleF[] { new RectangleF(w * 0, h * 0, w, h ) });
            for (int j = 0; j < 1; ++j )
                em.Spawn("Victory", new RectangleF[] { new RectangleF(w * 2, h * 1, w, h ) });

        }

        public override string ToString()
        {
            return "Game: " + State
                + ", Perpective: " + Perpective
                + ", PerpectiveNumber: " + PerpectiveNumber;
        }
    }
}
