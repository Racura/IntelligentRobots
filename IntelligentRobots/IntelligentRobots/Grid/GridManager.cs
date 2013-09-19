using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using AtlasEngine;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;

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
            _trunk.Update();

            if (Atlas.Input.IsKeyJustReleased(Keys.Enter))
            {
                try {
                    var json = _trunk.ToJson();
                    GridJson.ToFile("map.json", json);
                } catch { }
            }

            var teams = Atlas.GetManager<EntityManager>().Teams;

            List<Entity> entityList = new List<Entity>();

            foreach (var t in teams)
            {
                foreach (var e in t.Value.TeamMembers)
                {
                    MapCollide(e);

                    entityList.Add(e);
                }
            }


            var state = Atlas.GetStateController<Component.StateController>();

            if (state.State == Component.StateController.GameState.Combat)
            {
                foreach (var t in teams)
                {
                    EntityReport report = new EntityReport(_trunk, state.TimePassed);

                    if (t.Value.Delegate == null) continue;

                    foreach (var e1 in t.Value.TeamMembers)
                    {
                        List<EntityStruct> _list = new List<EntityStruct>();

                        Vector2 facingPos = new Vector2((float)Math.Cos(e1.Angle + e1.FOV * 0.5), (float)Math.Sin(e1.Angle + e1.FOV * 0.5));
                        Vector2 facingNeg = new Vector2((float)Math.Cos(e1.Angle - e1.FOV * 0.5), (float)Math.Sin(e1.Angle - e1.FOV * 0.5));

                        foreach (var e2 in entityList)
                        {
                            if (e1 == e2) continue;

                            var n = Vector2.Normalize(e2.Position - e1.Position);


                            for (int i = 0; i < 5; i++)
                            {
                                Vector2 testNormal = new Vector2(
                                    e2.Position.X + n.Y * e2.Radius * ((i - 2f) / 2),
                                    e2.Position.Y - n.X * e2.Radius * ((i - 2f) / 2));


                                if (e1.FOV < Math.PI * 2 
                                    && ((e1.FOV < Math.PI 
                                        && (facingPos.X * (testNormal.Y - e1.Position.Y) - facingPos.Y * (testNormal.X - e1.Position.X) > 0
                                            || facingNeg.X * (testNormal.Y - e1.Position.Y) - facingNeg.Y * (testNormal.X - e1.Position.X) < 0))
                                    || (facingPos.X * (testNormal.Y - e1.Position.Y) - facingPos.Y * (testNormal.X - e1.Position.X) > 0
                                            && facingNeg.X * (testNormal.Y - e1.Position.Y) - facingNeg.Y * (testNormal.X - e1.Position.X) < 0)))
                                    continue;

                                if (Trunk.Raytrace(e1.Position, testNormal, (e1.Crouching || e2.Crouching) ? 1 : 2))
                                {
                                    _list.Add(e2.GetStruct());
                                    break;
                                }
                            }
                        }
                        report.SightList.Add(e1, _list.ToArray());
                    }

                    t.Value.Delegate.Update(t.Value, report);
                }
            }

            Atlas.GetManager<AtlasEngine.BasicManagers.CameraManager>().LookAt(1,
                new Vector2(Trunk.Width * 0.5f, Trunk.Height * 0.5f),
                Math.Max(Trunk.Width, Trunk.Height) * 0.55f, 1, 1);
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
