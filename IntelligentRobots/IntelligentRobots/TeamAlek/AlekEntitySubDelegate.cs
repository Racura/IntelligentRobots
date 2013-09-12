using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using IntelligentRobots.Entities;

namespace IntelligentRobots.TeamAlek
{
    //Controlls individuals
    class AlekEntitySubDelegate
    {
        List<Vector2> _vectorList;
        int _version;
        Vector2 _destination;
        float timer;
        Vector2 _direction;
        bool panic = false;
        float panicTimer = 0f;

        public void Update(Entity entity, EntityReport report)
        {

        }
    }
}
