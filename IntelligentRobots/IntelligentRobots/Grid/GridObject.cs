using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentRobots.Grid
{
    public class GridObject<T> where T : struct
    {
        private T[,] _grid;
        
        private int _width;
        private int _height;

        public GridObject(int width, int height, T initValue)
            : base()
        {
            _width = width;
            _height = height;

            _grid = new T[_width, _height];

            for (int i = 0; i < _width; i++)
                for (int j = 0; j < _height; j++)
                    _grid[i, j] = initValue;
        }

        public GridObject(T[,] grid)
            : base()
        {
            _grid = (T[,])grid.Clone();

            _width = _grid.GetLength(0);
            _height = _grid.GetLength(1);
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
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return;

            _grid[x, y] = value;
        }

        public T Get(int x, int y, T defaultValue)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
                return defaultValue;

            return _grid[x, y];
        }
    }
}
