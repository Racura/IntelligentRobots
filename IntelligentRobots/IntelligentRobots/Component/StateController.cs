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

            if (Atlas.Input.IsKeyJustReleased(Keys.Enter))
            {
                State = State == GameState.Combat ? GameState.Paused : GameState.Combat;
            }
            if (Atlas.Input.IsKeyJustReleased(Keys.R))
            {
                State = GameState.Paused;
                Atlas.GetManager<Entities.EntityManager>().NewGame();
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

        public override string ToString()
        {
            return "Game: " + State
                + ", Perpective: " + Perpective
                + ", PerpectiveNumber: " + PerpectiveNumber;
        }
    }
}
