using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuiles_Glissantes
{
    public class Tuile : IEquatable<Tuile>
    {
        public Tuile(int x, int y, char caractere, ConsoleColor couleur)
            : this(new Position(x, y), caractere, couleur)
        {
        }

        public Tuile(Position positionDepart, char caractere, ConsoleColor couleur)
        {
            this.Caractere = caractere;
            this.Couleur = couleur;
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
            return unchecked(this.Caractere.GetHashCode() * 13 + this.Couleur.GetHashCode());
        }

        public bool Equals(Tuile other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Caractere.Equals(other.Caractere) && this.Couleur == other.Couleur;
        }

        public readonly ConsoleColor Couleur;
        public readonly char Caractere;
    }
}
