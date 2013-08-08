using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Newtonsoft.Json;

namespace IntelligentRobots.Grid
{
    public class GridJson
    {
        public byte[,] heightMap;
        public short[,] tileMap;
        public int size;

        public GridJson()
        {
            heightMap = new byte[0, 0];
            tileMap = new short[0,0];
            size = 16;
        }

        public GridJson(int size, byte[,] heightMap, short[,] tileMap)
        {
            this.size = size;
            this.heightMap = heightMap;
            this.tileMap = tileMap;
        }

        public static GridJson FromFile(string file)
        {
            string str = File.ReadAllText(file);
            return JsonConvert.DeserializeObject<GridJson>(str);
        }

        public static void ToFile(string file, GridJson grid)
        {
            string str = JsonConvert.SerializeObject(grid);
            File.WriteAllText(file, str);
        }
    }
}
