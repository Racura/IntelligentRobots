using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace IntelligentRobots.Grid
{
    public struct GridCollision
    {
        public bool valid;
        public int x;
        public int y;

        public void Invalid()
        {
            valid = false;
        }
        public void Set(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}
