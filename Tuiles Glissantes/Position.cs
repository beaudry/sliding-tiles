
namespace Tuiles_Glissantes
{
    public struct Position : System.IEquatable<Position>
    {
        public Position(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public readonly int X;
        public readonly int Y;

        // override object.Equals
        public override bool Equals(object obj)
        {
            //       
            // See the full list of guidelines at
            //   http://go.microsoft.com/fwlink/?LinkID=85237  
            // and also the guidance for operator== at
            //   http://go.microsoft.com/fwlink/?LinkId=85238
            //

            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            return this.Equals((Position)obj);
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + this.X.GetHashCode();
                hash = hash * 23 + this.X.GetHashCode();
                return hash;
            }
        }

        public bool Equals(Position other)
        {
            return this.Equals(other.X, other.Y);
        }

        public bool Equals(int x, int y)
        {
            return this.X == x && this.Y == y;
        }

        public Position Offset(int xOffset, int yOffset, bool flip)
        {
            if (flip)
            {
                return this.Offset(yOffset, xOffset);
            }
            return this.Offset(xOffset, yOffset);
        }

        public Position Offset(int xOffset, int yOffset)
        {
            return new Position(this.X + xOffset, this.Y + yOffset);
        }
    }
}
