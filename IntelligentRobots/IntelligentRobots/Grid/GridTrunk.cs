using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.Grid
{
    public class GridTrunk : AtlasEntity
    {
        public static readonly float SQRT_2 = (float)Math.Sqrt(2);
        public static readonly float SQRT_HALF = 1 / SQRT_2;

        private byte[,] _heightMap;
        private short[,] _tileMap;

        private byte[,][][,] _sightMap;

        private int _height;
        private int _width;
        private int _tileSize;

        public List<Point>[] _nodeList;

        public int Width {  get { return _width; } }
        public int Height { get { return _height; } }
        public int Size {   get { return _tileSize; } }

        private const byte MAX_HEIGHT = 2; 

        byte _value;

        public GridTrunk(AtlasGlobal atlas)
            : base(atlas)
        {
            _heightMap = new byte[64, 64];
            _tileMap = new short[64, 64];

            _visitedMap = new bool[64, 64];

            _width = _heightMap.GetLength(0);
            _height = _heightMap.GetLength(1);

            _tileSize = 16;
        }

        public void FromJson(GridJson json)
        {
            _tileSize = json.size;

            _width = json.heightMap.GetLength(0);
            _height = json.heightMap.GetLength(1);

            if (_width != json.tileMap.GetLength(0) || _height != json.tileMap.GetLength(1))
            {

            }

            _heightMap = json.heightMap;
            _tileMap = json.tileMap;

            _visitedMap = new bool[_width, _height];
            _sightMap = null;
        }

        public GridJson ToJson()
        {
            return new GridJson(_tileSize, _heightMap, _tileMap);
        }

        public void Update()
        {
            var cm = Atlas.GetManager<CameraManager>();

            foreach (var t in Atlas.Input.GetTouchCollection())
            {
                Vector2 tmp = cm.GetWorldPosition(t.Position, Vector2.One);

                tmp.Y = MathHelper.Clamp(tmp.Y, 0, _tileSize * _height - 1);
                tmp.X = MathHelper.Clamp(tmp.X, 0, _tileSize * _width - 1);

                if (t.State == TouchLocationState.Released)
                {
                    _value = (byte)(((_heightMap[(int)(tmp.X / _tileSize), (int)(tmp.Y / _tileSize)] + 1) % (MAX_HEIGHT + 1)));
                }
                if (t.State == TouchLocationState.Moved)
                {
                    _heightMap[(int)(tmp.X / _tileSize), (int)(tmp.Y / _tileSize)] = _value;
                    _sightMap = null;
                }
            }
        }

        public void Draw()
        {
            var cm = Atlas.GetManager<CameraManager>();
            var t = Atlas.Content.GetContent<Texture2D>("blop");


            var pm = Atlas.GetManager<Player.PlayerManager>();

            var tmp = CanSee((int)(pm.Position.X / _tileSize), (int)(pm.Position.Y / _tileSize), 2);

            var rec = new Rectangle(0, 0, _tileSize, _tileSize);

            for (int i = Math.Max(0, (int)((cm.Position.X - cm.Width * 0.6f) / _tileSize));
                i < Math.Min(_width, (int)((cm.Position.X + cm.Width * 0.6f) / _tileSize)); 
                i++) {
                for (int j = Math.Max(0, (int)((cm.Position.Y - cm.Height * 0.6f) / _tileSize));
                    j < Math.Min(_height, (int)((cm.Position.Y + cm.Height * 0.6f) / _tileSize));
                    j++)
                {
                    Atlas.Graphics.DrawSprite(t,
                        new Vector2(i * _tileSize, j * _tileSize), rec,
                        Color.Lerp(Color.White, Color.Black,
                        (tmp[i, j]) / ((MAX_HEIGHT + 1) * 1f)));
                        //(_heightMap[i, j]) / (MAX_HEIGHT * 1f)));
                }   
            }
        }

        #region PathFinding

        private bool[,] _visitedMap;

        public bool TryFindPath(Vector2 v1, Vector2 v2, float raduis, out List<Vector2> path)
        {
            for(int i = 0; i < _width; i++)
                for(int j = 0; j < _height; j++) 
                    _visitedMap[i,j] = false;
            
            int startX = (int)((v1.X - raduis * 0.99f) / _tileSize);
            int startY = (int)((v1.Y - raduis * 0.99f) / _tileSize);
            int goalX =  (int)((v2.X - raduis * 0.99f) / _tileSize);
            int goalY =  (int)((v2.Y - raduis * 0.99f) / _tileSize);

            int tileSize = (int)((raduis * 2 - 1) / _tileSize) + 1;
            int d = (int)((raduis * 2 * SQRT_2 - 1) / _tileSize) + 1;

            bool success = false;

            path = null;

            if (startX < 0 || startY < 0
                || startX >= _width || startY >= _height
                || goalX < 0 || goalY < 0
                || goalX >= _width || goalY >= _height
                || _heightMap[startX, startY] != 0 || _heightMap[goalX, goalY] != 0)
            {
                return false;
            }

            List<GridNode> inList = new List<GridNode>();
            inList.Add(new GridNode(-1, startX, startY, 0, GridNode.EstimateDistanceTo(startX, startY, goalX, goalY)));

            List<GridNode> outList = new List<GridNode>();

            _visitedMap[startX, startY] = true;

            while (inList.Count > 0 && !success)
            {
                float distance = float.MaxValue;
                int current = -1;
                
                for (int i = 0; i < inList.Count; i++)
                {
                    if (distance > inList[i].GetDistance())
                    {
                        current = i;
                        distance = inList[i].GetDistance();
                    }
                }

                GridNode n = inList[current];

                if (n.x == goalX && n.y == goalY)
                    success = true;

                if (!success)
                {
                    for (int i = -1; i < 2; i++)
                    {
                        for (int j = -1; j < 2; j++)
                        {
                            if (i + n.x >= 0 && i + n.x < _width && j + n.y >= 0 && j + n.y < _height)
                            {
                                if (CanFit(i + n.x, j + n.y, tileSize))
                                {
                                    bool diagonal = Math.Abs(i - j) != 1;

                                    if (!diagonal || CanFit(
                                        i + n.x - (i + 1) / 2, 
                                        j + n.y - (j + 1) / 2, tileSize + 1))
                                    {
                                        var tmpNode = new GridNode(outList.Count, i + n.x, j + n.y,
                                                n.steps + (diagonal ? SQRT_2 : 1),
                                                GridNode.EstimateDistanceTo(i + n.x, j + n.y, goalX, goalY));

                                        if (!_visitedMap[i + n.x, j + n.y])
                                        {
                                            inList.Add(tmpNode);

                                            _visitedMap[i + n.x, j + n.y] = true;
                                        }
                                        else
                                        {
                                            for (int k = 0; k < inList.Count; k++)
                                            {
                                                if (inList[k].x == i + n.x && inList[k].y == j + n.y)
                                                {
                                                    if (inList[k].steps > tmpNode.steps)
                                                        inList[k] = tmpNode;

                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                inList.RemoveAt(current);
                outList.Add(n);
            }

            if (!success)
                return false;


            GridNode node = outList.Last();

            path = new List<Vector2>();

            path.Add(new Vector2((node.x) * _tileSize + raduis, (node.y) * _tileSize + raduis));

            while (node.parent != -1)
            {
                node = outList[node.parent];
                path.Add(new Vector2((node.x) * _tileSize + raduis, (node.y) * _tileSize + raduis));
                
            }
            path.Reverse();

            return true;
        }

        private bool CanFit(int x, int y, int tileSize)
        {
            if(x + tileSize >= _width || y + tileSize >= _height || x < 0 || y < 0)
                return false;

            for (int i = 0; i < tileSize; i++ )
                for (int j = 0; j < tileSize; j++)
                    if (_heightMap[x + i, y + j] != 0)
                        return false;


            return true;
        }

        #endregion

        #region Collide
        GridCollision[] _tempCollisions;

        public void Collide(Entities.Entity entity)
        {
            if(_tempCollisions == null)
                _tempCollisions = new GridCollision[16];

            bool runAgain = true;

            for (int check = 0; check < 16 && runAgain; check++)
            {
                runAgain = false;

                int collisionCounter = 0;

                for (int i = Math.Max(0, (int)(entity.Position.X - entity.Radius) / _tileSize);
                    i < Math.Min(_width, (entity.Position.X + entity.Radius) / _tileSize)
                        && collisionCounter < _tempCollisions.Length;
                    i++)
                {
                    for (int j = Math.Max(0, (int)(entity.Position.Y - entity.Radius) / _tileSize);
                        j < Math.Min(_height, (entity.Position.Y + entity.Radius) / _tileSize)
                            && collisionCounter < _tempCollisions.Length;
                        j++)
                    {
                        if (_heightMap[i, j] != 0)
                        {
                            _tempCollisions[collisionCounter].Set(i, j);
                            collisionCounter++;
                        }
                    }
                }


                for (int i = 0; i < collisionCounter; i++)
                {
                    float d1 = Vector2.DistanceSquared(
                        new Vector2((_tempCollisions[i].x + 0.5f) * _tileSize, (_tempCollisions[i].y + 0.5f) * _tileSize),
                        entity.Position);

                    float d2 = 0;

                    for (int j = i + 1; j < collisionCounter; j++)
                    {
                        d2 = Vector2.DistanceSquared(
                            new Vector2((_tempCollisions[j].x + 0.5f) * _tileSize, (_tempCollisions[j].y + 0.5f) * _tileSize),
                            entity.Position);

                        if (d1 > d2)
                        {
                            d1 = d2;

                            GridCollision tmp = _tempCollisions[j];
                            _tempCollisions[j] = _tempCollisions[i];
                            _tempCollisions[i] = tmp;
                        }
                    }

                    Vector2 v = new Vector2(
                        MathHelper.Clamp(entity.Position.X, (_tempCollisions[i].x) * _tileSize, (_tempCollisions[i].x + 1) * _tileSize),
                        MathHelper.Clamp(entity.Position.Y, (_tempCollisions[i].y) * _tileSize, (_tempCollisions[i].y + 1) * _tileSize)
                    );

                    if (Vector2.DistanceSquared(v, entity.Position) < entity.Radius * entity.Radius)
                    {
                        Vector2 n = Vector2.Normalize(entity.Position - v);
                        if (!(float.IsNaN(n.X) && float.IsNaN(n.Y)))
                        {
                            entity.Collision(v, n);

                            runAgain = true;
                        }
                    }
                    else
                        break;
                }
            }
        }
        #endregion

        public byte Raytrace(Vector2 v1, Vector2 v2)
        {
            byte i = Raytrace(_tileSize, v1, v2);

            return i;
        }

        private byte Raytrace(float size, Vector2 v1, Vector2 v2)
        {
            float dx = Math.Abs(v2.X - v1.X);
            float dy = Math.Abs(v2.Y - v1.Y);
            
            int x = (int)(v1.X / size);
            int y = (int)(v1.Y / size);
            int x2 = (int)(v2.X / size);
            int y2 = (int)(v2.Y / size);

            float error = dx - dy;

            int n = (int)(2 + dx + dy);
            int x_inc = (v2.X > v1.X) ? 1 : -1;
            int y_inc = (v2.Y > v1.Y) ? 1 : -1;

            dx *= 2;
            dy *= 2;

            byte value = 0;

            for (; n > 0; --n)
            {
                value = Math.Max(_heightMap[x, y], value);

                if (x == x2 && y == y2)
                {
                    break;
                }

                if (value >= MAX_HEIGHT)
                    return MAX_HEIGHT;

                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                }
            }

            return value;
        }

        public byte[,] CanSee(int x, int y, byte height)
        {

            if (_sightMap == null || _width != _sightMap.GetLength(0) || _height != _sightMap.GetLength(1))
                _sightMap = new byte[_width, _height][][,];

            Vector2 v = new Vector2(x + 0.5f, y + 0.5f);
            //Vector2 v = new Vector2(x + 0.5f, y + 0.5f) * _tileSize;
            
            if (_sightMap[x, y] == null)
            {
                _sightMap[x, y] = new byte[MAX_HEIGHT + 1][,];
            }

            if (_sightMap[x, y][height] == null)
            {
                var tmpSight = new GridObject<byte>(_width, _height, height);

                for (int i = 0; i < _width; i++)
                {
                    for (int j = 0; j < _height; j++)
                    {
                        var tmp = Math.Min(Math.Min(
                            Raytrace(1, v, new Vector2(i + 0.3f, j + 0.3f)),
                            Raytrace(1, v, new Vector2(i + 0.7f, j + 0.3f))),
                        Math.Min(
                            Raytrace(1, v, new Vector2(i + 0.3f, j + 0.7f)),
                            Raytrace(1, v, new Vector2(i + 0.7f, j + 0.7f))));

                        tmpSight.Set(i, j, tmp);
                    }
                }
                _sightMap[x, y][height] = tmpSight.GetGrid();
            }

            return _sightMap[x, y][height];
        }

        private struct GridNode
        {
            public int parent;
            public int x;
            public int y;
            public float steps;
            public float d;

            public GridNode(int parent, int x, int y, float steps, float d)
            {
                this.parent = parent;
                this.x = x;
                this.y = y;
                this.steps = steps;
                this.d = d;
            }

            public static float EstimateDistanceTo(int x, int y, int goalX, int goalY)
            {
                int tmpX = Math.Abs(x - goalX);
                int tmpY = Math.Abs(y - goalY);

                return Math.Abs(tmpX - tmpY) + Math.Min(tmpX, tmpY) * SQRT_2;
            }

            public float GetDistance()
            {
                return d + steps;
            }
        }
    }
}
