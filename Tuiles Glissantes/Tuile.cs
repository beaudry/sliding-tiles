using System;

namespace Tuiles_Glissantes
{
    public class Tuile : IEquatable<Tuile>
    {
        public Tuile(int x, int y, char caractere, ConsoleColor couleurPolice, ConsoleColor couleurFond)
            : this(new Position(x, y), caractere, couleurPolice, couleurFond)
        {
        }

        public Tuile(Position positionDepart, char caractere, ConsoleColor couleurPolice, ConsoleColor couleurFond)
        {
            this.Caractere = caractere;
            this.CouleurPolice = couleurPolice;
            this.CouleurFond = couleurFond;
            this.PositionCourante = positionDepart;
            this.PositionDepart = positionDepart;
        }

        public Position PositionCourante { get; set; }
        public readonly Position PositionDepart;

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

            return this.Equals((Tuile)obj);
        }

        public override int GetHashCode()
        {
            return this.PositionDepart.GetHashCode();
        }

        public bool Equals(Tuile other)
        {
            if (other == null)
            {
                return false;
            }

            return this.PositionDepart.Equals(other.PositionDepart);
        }

        public bool EstBienPlacee()
        {
            return this.PositionDepart.Equals(this.PositionCourante);
        }

        public int DistanceFromOrigin()
        {
            return this.DistanceFrom(this.PositionDepart);
        }

        public int DistanceFrom(Tuile other)
        {
            return this.DistanceFrom(other.PositionCourante);
        }

        public int DistanceFrom(Position pos)
        {
            return this.DistanceFrom(pos.X, pos.Y);
        }

        public int DistanceFrom(int x, int y)
        {
            return Math.Abs(this.PositionCourante.X - x) + Math.Abs(this.PositionCourante.Y - y);
        }

        public readonly ConsoleColor CouleurPolice;
        public readonly ConsoleColor CouleurFond;
        public readonly char Caractere;
    }
}
