using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIREWALL
{
    class Vector2Int
    {
        public int X;
        public int Y;

        public Vector2Int(int value)
        {
            X = value;
            Y = value;
        }

        public Vector2Int(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int pixelWidth(double factor)
        {
            return Convert.ToInt32(Math.Round(X * factor));
        }

        public int pixelHeight(double factor)
        {
            return Convert.ToInt32(Math.Round(Y * factor));
        }

        public static Vector2Int Zero
        {
            get { return new Vector2Int(0); }
        }
    }
}
