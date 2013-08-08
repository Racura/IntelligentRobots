using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AtlasEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace IntelligentRobots.Grid
{
    public class GridManager
        : AtlasManager
    {
        private GridTrunk _trunk;


        public GridTrunk Trunk { get { return _trunk; } }

        public GridManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _trunk = new GridTrunk(atlas);

            try {
                var json = GridJson.FromFile("map.json");
                _trunk.FromJson(json);
            } catch { }
        }

        public override void Update(string arg)
        {
            base.Update(arg);

            if (Atlas.Input.IsKeyJustReleased(Keys.Enter))
            {
                try {
                    var json = _trunk.ToJson();
                    GridJson.ToFile("map.json", json);
                } catch { }
            }

            _trunk.Update();
        }

        public override void Draw(int pass)
        {
            _trunk.Draw();
        }

        public void MapCollide(Entities.Entity entity)
        {
            _trunk.Collide(entity);
        }

        public bool FindPath(Vector2 start, Vector2 goal, float raduis, out List<Vector2> output)
        {
            return _trunk.TryFindPath(start, goal, raduis, out output);
        }
    }
}
