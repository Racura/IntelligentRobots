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
using IntelligentRobots.EntityTypes;

namespace IntelligentRobots.TeamAlek
{
    //Controlls team
    public class AlekEntityDelegate : AtlasEntity, EntityDelegate
    {

        const int WORLD_SPLITS = 16;
        LinkedList<AlekEntitySubDelegate> subDelegates = new LinkedList<AlekEntitySubDelegate>();
        List<List<byte>> exploredInfo;//exploredInfo : 0 not explored, 1 explored
        int[] yCount = new int[WORLD_SPLITS];
        bool exploredEverywhere = false;

        public Vector2? winPoint = null;

        float ratioX;// = report.Trunk.Width / WORLD_SPLITS;
        float ratioY;// = report.Trunk.Height / WORLD_SPLITS;


        public AlekEntityDelegate(AtlasGlobal atlas)
            : base(atlas)
        {
            initExplored();
        }

        private void initExplored()
        {
            
            exploredInfo = new List<List<byte>>();
            for (int x = 0; x < WORLD_SPLITS; x++)
            {
                exploredInfo.Add(new List<byte>());
                yCount[x] = WORLD_SPLITS;
                for (int y = 0; y < WORLD_SPLITS; y++)
                {
                    exploredInfo[x].Add(0);

                }
            }
        }

        public void exploreTile (Entity entity, EntityReport report)
        {
            float ratioX = report.Trunk.Width / WORLD_SPLITS;
            float ratioY = report.Trunk.Height / WORLD_SPLITS;

            int exploredX = (int)(entity.Position.X / ratioX);
            int exploredY = (int)(entity.Position.Y / ratioY);

            if (exploredInfo[exploredX][exploredY] == 0)
            {
                exploredInfo[exploredX][exploredY] = 1;
                yCount[exploredX]--;
            }
            
        }

        private Vector2 getTilePosition(int x, int y)
        {
            if (x > 0 && x < WORLD_SPLITS && y > 0 && y < WORLD_SPLITS)
            {
                return new Vector2(x * ratioX + ratioX/2, y * ratioY + ratioY/2);
            }
            return new Vector2(-1);
        }

        public Entity CreateEntity(EntityTeam team, RectangleF[] possibleLocations, Grid.GridTrunk trunk)
        {
            var rand = (int)(possibleLocations.Length * Atlas.Rand);
            for (int i = 0; i < 16;i++)
            {
                Vector2 point = new Vector2(possibleLocations[rand].X + possibleLocations[rand].Width * Atlas.Rand, 
                            possibleLocations[rand].Y + possibleLocations[rand].Height * Atlas.Rand);
                if (trunk.CanFit(point,14))
                {
                    return new EntityTypes.HumanEntity(Atlas, team, point);
                }
            }
            //Entity e = new EntityTypes.HumanEntity(Atlas, team, 
            
            return null;
        }

        public Vector2 whereShouldIGo(Entity entity)
        {
            if (exploredEverywhere)
            {
                return new Vector2(-1);
            }
            int x = (int)(Atlas.Rand * WORLD_SPLITS);
            x = findTileOnXAxis(x);

            if (x == -1)
            {
                exploredEverywhere = true;
            }
            int y = (int)(Atlas.Rand * WORLD_SPLITS);
            return findTileOnYAxis(x, y);

            //return new Vector2(-1);

        }

        private int findTileOnXAxis(int x)
        {
            bool found = false;
            int start = x;
            while (!found)
            {
                if (yCount[x] > 0)
                {
                    return x;
                }
                else
                {
                    x++;
                    if (x > WORLD_SPLITS)
                    {
                        x = 0;
                    }

                    if (x == start)
                    {

                        found = false;
                    }

                }
            }
            return -1;

        }

        private Vector2 findTileOnYAxis(int x, int y)
        {
            bool found = false;
            int start = y;
            while(!found)
            {
                if (exploredInfo[x][y] == 0)
                {
                    return getTilePosition(x, y);
                }
                else
                {
                    y++;
                    if (y > WORLD_SPLITS)
                    {
                        y = 0;
                    }

                    if (y == start)
                    {

                        found = false;
                    }
                        
                }
            }
            return new Vector2(-1);
            
        }

        public void HasAdded(EntityTeam team, Entity entity)
        {
            if (entity is HumanEntity)
            {
                subDelegates.AddLast(new AlekEntitySubDelegate(Atlas, entity, this));
                
            }
        }

       


        public void Update(EntityTeam entityTeam, EntityReport report)
        {
            ratioX = report.Trunk.Width / WORLD_SPLITS;
            ratioY = report.Trunk.Height / WORLD_SPLITS;

            entityTeam.Color = Color.PeachPuff;
            if (entityTeam.TeamMembers.Length == 0) return;


            foreach (AlekEntitySubDelegate a in subDelegates)
            {
                a.Update(report);
            }

        }

        

        public void DebugDraw(EntityTeam team)
        {
            foreach (var v in subDelegates)
            {
                v.debugDraw();
            }
        }

        public bool Swappable(EntityTeam team, EntityDelegate entityDelegate)
        {
            return true;
        }



        public void Restart(EntityTeam team)
        {

            subDelegates.Clear();
        }

        internal void sendWinPoint(Vector2 vector2)
        {
            winPoint = vector2;
        }

        internal void sendSeekerThreat(Vector2 vector2)
        {
            //throw new NotImplementedException();
        }

        internal void sendHunterThreat(Vector2 vector2)
        {
            //throw new NotImplementedException();
        }
    }
}
