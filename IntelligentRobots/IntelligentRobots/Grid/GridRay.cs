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
    }
}
