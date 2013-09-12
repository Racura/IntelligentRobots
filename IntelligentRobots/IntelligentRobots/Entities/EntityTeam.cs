using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using AtlasEngine;


namespace IntelligentRobots.Entities
{
    public class EntityTeam : AtlasEntity
    {
        private List<Entity> _team;
        private EntityDelegate _delegate;
        public EntityDelegate Delegate { get { return _delegate; } }

        private Entity[] _teamArray;
        public Entity[] TeamMembers { get { return _teamArray; } }

        public Color Color { get;  set; }


        public bool Locked { get; private set; }
        private object _key;

        public void Lock(object key)    { if (!Locked) {        _key = key;     Locked = true; } }
        public void UnLock(object key)  { if (_key == key) {    _key = key;     Locked = false; } }


        public EntityTeam(AtlasGlobal atlas, EntityDelegate teamDelegate) 
            : base(atlas) 
        {
            _delegate = teamDelegate;
            Color = Color.White;
            _team = new List<Entity>();

            _teamArray = new Entity[0];
        }

        public void Add(Entity entity)
        {
            if (Locked) return;

            _team.Add(entity);
            _teamArray = _team.ToArray();

            _delegate.HasAdded(this, entity);
        }

        public void Spawn(RectangleF[] rectangleF)
        {
            if (Locked) return;

            var entity = _delegate.WillAdded(this, rectangleF, Atlas.GetManager<Grid.GridManager>().Trunk);

            if (entity == null)
                return;

            foreach (var r in rectangleF)
            {
                if(r.Overlap(entity.Position))
                {
                    Add(entity);
                    return;
                }
            }
        }

        public bool OnTeam(EntityStruct struc)
        {
            foreach (var e in _teamArray) {
                if (struc.Is(e))
                    return true;
            }
            return false;
        }

        public void Update()
        {
            if (Locked) return;

            foreach (var e in _teamArray) 
            {
                e.Update();
            }
        }


        public void Draw()
        {
            foreach (var e in _teamArray) {
                e.Draw(Color);
            }

            if (Atlas.Debug) 
            {
                if (_delegate != null) _delegate.DebugDraw(this);
            }
        }


        public virtual bool TrySetDelegate(EntityDelegate entityDelegate)
        {
            if (_delegate == null) {
                _delegate = entityDelegate;
                return true;
            }
            else if (_delegate.Swappable(this, entityDelegate)) {
                _delegate = entityDelegate;
                return true;
            }
            return false;
        }

        public void Clear()
        {
            if (Locked) return;

            _team.Clear();
            _teamArray = _team.ToArray();

            if (_delegate != null) _delegate.Restart(this);
        }
    }
}
