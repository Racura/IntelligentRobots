using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using AtlasEngine;
using AtlasEngine.BasicManagers;

namespace IntelligentRobots.Player
{
    public class PlayerManager
        : AtlasManager
    {
        PlayerEntity _entity;

        public Vector2 Position { get { return _entity.Position; } }

        public PlayerManager(AtlasGlobal atlas)
            : base(atlas)
        {
            _entity = new PlayerEntity(Atlas, 14);
        }

        public override void Update(string arg)
        {
            var cm = Atlas.GetManager<CameraManager>();
            var gm = Atlas.GetManager<Grid.GridManager>();

            _entity.Update();

            gm.MapCollide(_entity);
            cm.LookAt(10, _entity.Position, 450, 5, 1);
        }

        public override void Draw(int pass)
        {
            var gm = Atlas.GetManager<Grid.GridManager>();
            var cm = Atlas.GetManager<CameraManager>();
            _entity.Draw();

            Atlas.Graphics.Flush();
            Atlas.Graphics.SetPrimitiveType(PrimitiveType.LineStrip);

            List<Vector2> _list;

            foreach(var t in Atlas.Input.GetTouchCollection())
            {
                Vector2 p = cm.GetWorldPosition(t.Position, Vector2.One);

                if (gm.FindPath(_entity.Position, p, _entity.Radius, out _list) && _list.Count > 2)
                {
                    VertexPositionColorTexture[] vpct = new VertexPositionColorTexture[_list.Count];

                    for (int i = 0; i < vpct.Length; i++)
                    {
                        vpct[i].Position.X = _list[i].X;
                        vpct[i].Position.Y = _list[i].Y;

                        vpct[i].Color = Color.Red * 0.9f;
                    }
                    Atlas.Graphics.DrawVector(vpct);
                }
            }
            Atlas.Graphics.Flush();
        }
    }
}
