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
        }

        public Position PositionCourante { get; set; }

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
            return unchecked((this.Caractere.GetHashCode() * 13 + this.CouleurPolice.GetHashCode()) * 13 + this.CouleurFond.GetHashCode());
        }

        public bool Equals(Tuile other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Caractere.Equals(other.Caractere) && this.CouleurPolice == other.CouleurPolice && this.CouleurFond == other.CouleurFond;
        }

        public readonly ConsoleColor CouleurPolice;
        public readonly ConsoleColor CouleurFond;
        public readonly char Caractere;
    }
}
