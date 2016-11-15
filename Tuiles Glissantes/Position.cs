using System;

namespace Tuiles_Glissantes
{
    public struct Position
    {
        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public readonly int X;
        public readonly int Y;

        public bool Equals(int x, int y)
        {
            return this.Equals(new Position(x, y));
        }

        public Position Offset(int xOffset, int yOffset)
        {
            return new Position(this.X + xOffset, this.Y + yOffset);
        }
    }
}
