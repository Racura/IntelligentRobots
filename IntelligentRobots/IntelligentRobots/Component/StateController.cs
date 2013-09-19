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

        public StateController(AtlasGlobal atlas)
            : base(atlas)
        {
            this.State = GameState.Paused;
            this.Perpective = PerpectiveState.World;

            PerpectiveNumber = 0;
        }

        public override void Update(string arg)
        {
            base.Update(arg);

            TimePassed += Atlas.Elapsed;

            if (Atlas.Input.IsKeyJustReleased(Keys.Enter))
            {
                State = State == GameState.Combat ? GameState.Paused : GameState.Combat;
            }

            foreach (var key in new Keys[] { Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U })
            {

                if (Atlas.Input.IsKeyJustReleased(key))
                {
                    Restart(key);
                }
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

        private void Restart(Keys keys)
        {
            State = GameState.Paused;
            TimePassed = 0;

            var em = Atlas.GetManager<Entities.EntityManager>();
            var gm = Atlas.GetManager<Grid.GridManager>();
            em.NewGame();

            int count = 2;
            int size = 64;

            switch (keys)
            {
                case Keys.E:
                    count = 1;
                    size = 1;
                    goto default;
                case Keys.W:
                    count = 8;
                    size = 200;
                    goto default;
                default:
                    double tmp = Math.PI * 2 * Atlas.Rand;
                    var str = new string[] { "Kris", "Aleks", "Victory" };

                    for (int i = 0; i < str.Length; ++i)
                    {
                        var v = new Vector2((float)Math.Sin(i * Math.PI * 2 / str.Length + tmp) * 0.5f + 0.5f,
                                            (float)Math.Cos(i * Math.PI * 2 / str.Length + tmp) * 0.5f + 0.5f);

                        var rect = new RectangleF(v.X * (gm.Trunk.Width - size), v.Y * (gm.Trunk.Height - size), size, size);

                        for (int j = 0; j < count; ++j )
                            em.Spawn(str[i], new RectangleF[] { rect });
                    }
                    break;
            }
        }

        public override string ToString()
        {
            return "Game: " + State
                + ", Perpective: " + Perpective
                + ", PerpectiveNumber: " + PerpectiveNumber;
        }
    }
}
