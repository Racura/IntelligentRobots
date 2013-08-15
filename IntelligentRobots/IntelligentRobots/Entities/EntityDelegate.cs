using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentRobots.Entities
{
    public interface EntityDelegate
    {
        void Update(Entity entity, EntityUtil util);
        void Report(Entity entity);
        void DebugDraw(Entity entity);
    }
}
