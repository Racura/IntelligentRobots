using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentRobots.Grid
{
    public enum GridObjectSetType
    {
        None = 0,
        Set = 1,
        Min = 2,
        Max = 3,
    }

    public class GridObject<T> where T : struct, IComparable<T>
    {

        public bool Locked { get { return _key != null; } }

        private IEquatable<object> _key;

        private T[,] _grid;

        public int Width
        {
            get;
            protected set;
        }

        public int Height
        {
            get;
            protected set;
        }

        private GridObjectSetType _setType;
        public GridObjectSetType SetType
        {
            get
            {
                return _setType;
            }
            set
            {
                if (!Locked)
                    _setType = value;
            }
        }

        public GridObject(int width, int height, T initValue)
            : base()
        {
            _setType = GridObjectSetType.Set;

            Width = width;
            Height = height;

            _grid = new T[Width, Height];

            for (int i = 0; i < Width; i++)
                for (int j = 0; j < Height; j++)
                    _grid[i, j] = initValue;
        }

        public GridObject(T[,] grid)
            : base()
        {
            _setType = GridObjectSetType.Set;
            _grid = (T[,])grid.Clone();

            Width = _grid.GetLength(0);
            Height = _grid.GetLength(1);
        }

        public T[,] GetGrid()
        {
            return (T[,])_grid.Clone();
        }

        public GridObject<T> Clone()
        {
            var r = new GridObject<T>(_grid);
            return r;
        }

        public void Set(int x, int y, T value)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return;

            switch (_setType)
            {
                case GridObjectSetType.Max:
                    _grid[x, y] = value.CompareTo(_grid[x, y]) > 0 ? value : _grid[x, y];
                    break;
                case GridObjectSetType.Min:
                    _grid[x, y] = value.CompareTo(_grid[x, y]) < 0 ? value : _grid[x, y];
                    break;
                case GridObjectSetType.Set:
                    _grid[x, y] = value;
                    break;
                default:
                    break;
            }
        }

        public T Get(int x, int y, T defaultValue)
        {
            if (x < 0 || x >= Width || y < 0 || y >= Height)
                return defaultValue;

            return _grid[x, y];
        }


        public void Lock(IEquatable<object> key)
        {
            if (_key != null)
                _key = key;
        }

        public void Unlock(IEquatable<object> key)
        {
            if (_key == key)
                _key = null;
        }
    }
}
