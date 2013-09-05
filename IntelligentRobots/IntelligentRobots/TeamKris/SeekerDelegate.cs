using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using IntelligentRobots.Entities;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.TeamKris
{
    public class SeekerDelegate
    {
        Entity _entity;
        KrisEntityDelegate _teamDelegate;

        List<Vector2> _path;
        int _mapVerison;

        public Vector2 GoToPoint { get; protected set; }
        public bool WantsObjective { get; protected set; }

        public SeekerDelegate(KrisEntityDelegate teamDelegate, Entity entity)
            : base()
        {
            _teamDelegate = teamDelegate;
            _entity = entity;

            WantsObjective = true;
        }

        public void Update(EntityReport report)
        {
            _entity.TryFace(_entity.Angle + 1);

            if (report.Trunk.Version != _mapVerison && _path != null)
            {
                SetPath(GoToPoint, report.Trunk);
            }

            TryFollowPath();
        }


        public void SetPath(Vector2 point, Grid.GridTrunk trunk)
        {
            GoToPoint = point;
 
            List<Vector2> path;

            if (trunk.TryFindPath(_entity.Position, point, _entity.Radius, out path))
            {
                _path = path;
                _mapVerison = trunk.Version;

                WantsObjective = false;
            }
        }

        private void TryFollowPath()
        {
            if (_path == null || _path.Count == 0)
            {
                _entity.TryMove(Vector2.Zero);
                WantsObjective = true;
                return;
            }

            if (Vector2.DistanceSquared(_path[0], _entity.Position) < 16 * 16)
            {
                _path.RemoveAt(0);
            }

            if (_path.Count > 0)
            {
                Vector2 wantedDir = (_path[0] - _entity.Position) * 1;

                _entity.TryMove(wantedDir);
            }
        }

        public void DrawCurrentPath(AtlasGlobal atlas)
        {
            if (_path == null || _path.Count == 2)
            {
                return;
            }

            VertexPositionColorTexture[] vpct = new VertexPositionColorTexture[_path.Count];


            for (int i = 0; i < _path.Count; i++)
            {
                vpct[i].Position = new Vector3(_path[i], 0);
                vpct[i].Color = AtlasColorSystem.GetColorFromHue(i * 16);
            }

            atlas.Graphics.Flush();

            atlas.Graphics.SetPrimitiveType(PrimitiveType.LineStrip);
            atlas.Graphics.DrawVector(vpct);

            atlas.Graphics.Flush();
        }

    }
}
