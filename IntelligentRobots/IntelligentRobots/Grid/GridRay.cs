using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace IntelligentRobots.Grid
{
    public class GridRay
    {
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

        public void Grid(float tileSize, GridObject<byte> grid, byte stopValue, GridObject<byte> paintGrid)
        {
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
                value = grid.Get(x, y, stopValue);

                if (value >= stopValue)
                {
                    return;
                }

                paintGrid.Set(x, y, value);

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
