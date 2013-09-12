using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.Player
{
    public class SeekerEntityDelegate : AtlasEntity, EntityDelegate
    {

        public SeekerEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            
        }

        

        public void HasAdded(EntityTeam team, Entity enitity)
        {

        }

        public void Update(EntityTeam team, EntityReport report)
        {
            //_report = report;
            team.Color = Color.Red;
            if (team.TeamMembers.Length == 0) return;

            var entity = team.TeamMembers[0];

            var cm = Atlas.GetManager<CameraManager>();
            Vector2 v = Vector2.Zero;
            var keys = Atlas.Input;

            v += (keys.IsKeyDown(Keys.Up) ? 1 : 0) * cm.Up
                + (keys.IsKeyDown(Keys.Down) ? -1 : 0) * cm.Up
                + (keys.IsKeyDown(Keys.Right) ? 1 : 0) * cm.Right
                + (keys.IsKeyDown(Keys.Left) ? -1 : 0) * cm.Right;

            entity.TryMove(v);
            entity.TryCrouching(keys.IsKeyDown(Keys.LeftControl));


            foreach (var t in keys.GetTouchCollection())
            {
                v = cm.GetWorldPosition(t.Position, Vector2.One) - entity.Position;
            }

            entity.TryFace((float)Math.Atan2(v.Y, v.X));
        }

        public void DebugDraw(EntityTeam team)
        {

        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }

        public Entity WillAdded(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            var rand = (int)(possibleLocations.Length * Atlas.Rand);

            return new EntityTypes.SeekerEntity(Atlas, team,
                new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand,
                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand));
        }
    }
}
