using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace IntelligentRobots.Grid
{
    public class GridRay
    {
        public delegate bool PaintGrid(int x, int y, byte lastValue, out byte newValue);

        public Vector2 point;
        public Vector2 normal;

        public float distance;

        public GridRay()
        {

        }

        public void Setup(Vector2 point, Vector2 normal)
        {
            this.point = point;
            this.normal = normal;
            this.distance = -1;
        }

        public void Grid(float tileSize, byte maxValue, GridObject<byte> paint, GridObject<byte> canvas)
        {
            if (paint == null || canvas == null)
                return;

            float dx = Math.Abs(normal.X);
            float dy = Math.Abs(normal.Y);

            int x = (int)(point.X / tileSize);
            int y = (int)(point.Y / tileSize);

            float error = dx - dy;

            int x_inc = (normal.X > 0) ? 1 : -1;
            int y_inc = (normal.Y > 0) ? 1 : -1;

            dx *= 2;
            dy *= 2;

            float distance = 0;
            byte value = 0;


            while(true)
            {
                value = Math.Max(value, paint.Get(x, y, maxValue));
                canvas.Set(x, y, value);

                if (value >= maxValue)
                {
                    return;
                }

                if (error > 0)
                {
                    x += x_inc;
                    error -= dy;
                    distance += dy * tileSize;
                }
                else
                {
                    y += y_inc;
                    error += dx;
                    distance += dx * tileSize;
                }
            }

        }
    }
}
