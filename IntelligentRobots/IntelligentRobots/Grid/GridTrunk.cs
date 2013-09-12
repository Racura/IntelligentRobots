using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;
using AtlasEngine.BasicManagers;

using IntelligentRobots.Component;

namespace IntelligentRobots.Grid
{
    public class GridTrunk : AtlasEntity
    {
        public static readonly float TwoSqrt = (float)Math.Sqrt(2);
        public static readonly float HalfSqrt = 1 / TwoSqrt;

        private GridObject<byte> _heightMap;
        private GridObject<short> _tileMap;

        private byte[,][][,] _sightMap;

        private int _height;
        private int _width;
        private int _tileSize;

        public List<Point>[] _nodeList;

        public int Width { get { return _width * _tileSize; } }
        public int Height { get { return _height * _tileSize; } }
        public int WidthInTiles { get { return _width; } }
        public int HeightInTiles { get { return _height; } }
        public int Size {   get { return _tileSize; } }

        public int Version { get; protected set; }

        private const byte MAX_HEIGHT = 2; 

        byte _value;


        VertexPositionColorTexture[] _debugVpct;
        int _debugCounter;

        public GridTrunk(AtlasGlobal atlas)
            : base(atlas)
        {
            _debugVpct = new VertexPositionColorTexture[256];
            _debugCounter = 0;

            _heightMap = new GridObject<byte>(64, 64, 0);
            _tileMap = new GridObject<short>(64, 64, 0);

            _visitedMap = new bool[64, 64];

            _width = _heightMap.Width;
            _height = _heightMap.Height;

            _tileSize = 16;

            Version = 0;
        }

        public void FromJson(GridJson json)
        {
            Version++;
            _tileSize = json.size;

            _width = json.heightMap.GetLength(0);
            _height = json.heightMap.GetLength(1);

            if (_width != json.tileMap.GetLength(0) || _height != json.tileMap.GetLength(1))
            {

            }

            _heightMap  = new GridObject<byte>(json.heightMap);
            _tileMap    = new GridObject<short>(json.tileMap);

            _visitedMap = new bool[_width, _height];
            _sightMap = null;
        }

        public GridJson ToJson()
        {
            return new GridJson(_tileSize, _heightMap.GetGrid(), _tileMap.GetGrid());
        }

        public void Update()
        {

            var cm = Atlas.GetManager<CameraManager>();

            foreach (var t in Atlas.Input.GetTouchCollection())
            {
                Vector2 tmp = cm.GetWorldPosition(t.Position, Vector2.One);

                tmp.Y = MathHelper.Clamp(tmp.Y, 0, _tileSize * _height - 1);
                tmp.X = MathHelper.Clamp(tmp.X, 0, _tileSize * _width - 1);

                if (t.State == TouchLocationState.Pressed)
                {
                    _value = (byte)((_heightMap.Get((int)(tmp.X / _tileSize), (int)(tmp.Y / _tileSize), 0) + 1) % (MAX_HEIGHT + 1));

                    Version++;
                }
                if (t.State == TouchLocationState.Moved)
                {
                    Version++;
                    _heightMap.Set((int)(tmp.X / _tileSize), (int)(tmp.Y / _tileSize), _value);
                    _sightMap = null;
                }
            }
        }

        public void Draw()
        {
            if (Atlas.GetManager<Component.StateController>().Perpective == Component.StateController.PerpectiveState.World)
            {
                DrawHeightGrid(_heightMap, Color.White, Color.Black);
            }


            if (Atlas.Debug && _debugCounter > 0)
            {
                Atlas.Graphics.Flush();

                Atlas.Graphics.SetPrimitiveType(PrimitiveType.LineList);
                Atlas.Graphics.DrawVector(_debugVpct, _debugCounter);

            }
            _debugCounter = 0;
        }

        public void DrawHeightGrid(GridObject<byte> grid, Color min, Color max)
        {
            var cm = Atlas.GetManager<CameraManager>();
            var t = Atlas.Content.GetContent<Texture2D>("blop");

            var rec = new Rectangle(0, 0, _tileSize, _tileSize);

            for (int i = Math.Max(0, (int)((cm.Position.X - cm.Width * 0.6f) / _tileSize));
                i < Math.Min(_width, (int)((cm.Position.X + cm.Width * 0.6f) / _tileSize));
                i++)
            {
                for (int j = Math.Max(0, (int)((cm.Position.Y - cm.Height * 0.6f) / _tileSize));
                    j < Math.Min(_height, (int)((cm.Position.Y + cm.Height * 0.6f) / _tileSize));
                    j++)
                {
                    Atlas.Graphics.DrawSprite(t,
                        new Vector2(i * _tileSize, j * _tileSize), rec,
                        Color.Lerp(min, max,
                        grid.Get(i, j, MAX_HEIGHT) / ((MAX_HEIGHT + 1) * 1f)));
                }
            }
        }

        #region PathFinding

        private bool[,] _visitedMap;

        public bool TryFindPath(Vector2 v1, Vector2 v2, float raduis, out List<Vector2> path)
        {
            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                    _visitedMap[i, j] = false;

            int startX = (int)((v1.X - raduis * 0.99f) / _tileSize);
            int startY = (int)((v1.Y - raduis * 0.99f) / _tileSize);
            int goalX = (int)((v2.X - raduis * 0.99f) / _tileSize);
            int goalY = (int)((v2.Y - raduis * 0.99f) / _tileSize);

            //int tileSize = (int)((raduis * 2 - 1) / _tileSize) + 1;
            float r = raduis / _tileSize;
            
            int d = (int)((raduis * 2 * TwoSqrt - 1) / _tileSize) + 1;

            bool success = false;

            path = null;

            if (startX < 0 || startY < 0
                || startX >= _width || startY >= _height
                || goalX < 0 || goalY < 0
                || goalX >= _width || goalY >= _height
                || _heightMap.Get(startX, startY, 0) != 0 || _heightMap.Get(goalX, goalY, 0) != 0)
            {
                return false;
            }

            List<GridNode> inList = new List<GridNode>();
            inList.Add(new GridNode(-1, startX, startY, 0, GridNode.EstimateDistanceTo(startX, startY, goalX, goalY)));

            List<GridNode> outList = new List<GridNode>();

            _visitedMap[startX, startY] = true;

            float offset = 0.0f;

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
                                if (CanFit(i + n.x + offset, j + n.y + offset, r))
                                {
                                    bool diagonal = Math.Abs(i - j) != 1;

                                    if (!diagonal ||
                                        CanFit(i + n.x + offset + i * 0.5f, j + n.y + offset + j * 0.5f, r))
                                    {
                                        var tmpNode = new GridNode(outList.Count, i + n.x, j + n.y,
                                                n.steps + (diagonal ? TwoSqrt : 1),
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

            path.Add(new Vector2((node.x + offset) * _tileSize, (node.y + offset) * _tileSize));

            while (node.parent != -1)
            {
                node = outList[node.parent];
                path.Add(new Vector2((node.x + offset) * _tileSize, (node.y + offset) * _tileSize));
            }

            path.Reverse();

            return true;
        }

        /*public bool CanFit(int x, int y, int tileSize)
        {
            if (x + tileSize >= _width || y + tileSize >= _height || x < 0 || y < 0)
                return false;

            for (int i = 0; i < tileSize; i++)
                for (int j = 0; j < tileSize; j++)
                    if (_heightMap.Get(x + i, y + j, 0) != 0)
                        return false;


            return true;
        }*/

        public bool CanFit(float centerX, float centerY, float radius)
        {

            for (int y =         -(int)(radius + 1); y <= (radius + 1); y++)
                for (int x =     -(int)(radius + 1); x <= (radius + 1); x++)
                {
                    float tmpX = x + (x < 0 ? 1 : 0);
                    float tmpY = y + (y < 0 ? 1 : 0);

                    if (tmpX * tmpX + tmpY * tmpY <= (radius * radius)
                        && _heightMap.Get((int)(x + centerX), (int)(y + centerY), 0) != 0)
                        return false;
                }

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
                        if (_heightMap.Get(i, j, 1) != 0) //does it collide
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

        public bool Raytrace(Vector2 v1, Vector2 v2, int maxHeight)
        {
            var i = Raytrace(_tileSize, v1, v2, (byte)maxHeight) < maxHeight; ;

            return i;
        }


        private byte Raytrace(float size, Vector2 v1, Vector2 v2, byte maxHeight = MAX_HEIGHT)
        {
            var debug = Atlas.Debug && false;

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
            if (debug)
            {
                _debugVpct[_debugCounter * 2 + 0].Position = new Vector3(v1, 0);
                _debugVpct[_debugCounter * 2 + 0].Color
                    = _debugVpct[_debugCounter * 2 + 1].Color = Color.Red;
            }

            for (; n > 0; --n)
            {
                value = Math.Max(_heightMap.Get(x, y, maxHeight), value);

                if (x == x2 && y == y2)
                {
                    break;
                }

                if (value >= maxHeight)
                {
                    if (debug)
                    {
                        _debugVpct[_debugCounter * 2 + 1].Position
                            = new Vector3(new Vector2(x + ((v2.X > v1.X) ? 0 : 1), y + ((v2.Y > v1.Y) ? 0 : 1)) * size, 0);
                        _debugCounter++;
                    }

                    return (byte)maxHeight;
                }
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
            if (debug)
            {
                _debugVpct[_debugCounter * 2 + 1].Position = new Vector3(v2, 0);
                _debugVpct[_debugCounter * 2 + 0].Color
                    = _debugVpct[_debugCounter * 2 + 1].Color = Color.Green;
                _debugCounter++;
            }

            return value;
        }

        public byte[,] CanSee(int x, int y, byte height)
        {

            if (_sightMap == null || _width != _sightMap.GetLength(0) || _height != _sightMap.GetLength(1))
                _sightMap = new byte[_width, _height][][,];

            Vector2 v = new Vector2(x + 0.5f, y + 0.5f);
            
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

                return Math.Abs(tmpX - tmpY) + Math.Min(tmpX, tmpY) * TwoSqrt;
            }

            public float GetDistance()
            {
                return d + steps;
            }
        }
    }
}
