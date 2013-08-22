using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AtlasEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using IntelligentRobots.Entities;

namespace IntelligentRobots.Grid
{
    public class GridManager
        : AtlasManager
    {
        private GridTrunk _trunk;
        private List<Entity> _registeredEntities;


        public GridTrunk Trunk { get { return _trunk; } }

        public GridManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _registeredEntities = new List<Entity>();
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


            foreach (var e in _registeredEntities)
            {
                MapCollide(e);
            }

            foreach (var e1 in _registeredEntities)
            {
                if (e1.Delegate == null)    continue;


                EntityReport report = new EntityReport();
                List<EntityStruct> _list = new List<EntityStruct>();

                foreach (var e2 in _registeredEntities)
                {

                    if (e1 == e2)   continue;

                    var n = Vector2.Normalize(e1.Position - e2.Position);

                    for (int i = 0; i < 5; i++)
                    {
                        Vector2 testPosition = new Vector2(
                            e2.Position.X - n.Y * e2.Radius * ((i - 2) / 2),
                            e2.Position.Y - n.X * e2.Radius * ((i - 2) / 2));

                        if (Trunk.Raytrace(e1.Position, testPosition) > ((e1.Crouching || e2.Crouching) ? 1 : 0))
                        {
                            _list.Add(e2.GetStruct());
                            break;
                        }
                    }
                }
                report.ListEntity = _list.ToArray();

                e1.Delegate.Report(e1, report);
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

        public void Register(Entity entity)
        {
            if (!_registeredEntities.Contains(entity))
            {
                _registeredEntities.Add(entity);
            }
        }
    }
}
