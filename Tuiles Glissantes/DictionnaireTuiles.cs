using System.Collections.Generic;

namespace Tuiles_Glissantes
{
    public class DictionnaireTuiles : Dictionary<Position, Tuile>
    {
        public DictionnaireTuiles()
            : base()
        {
        }

        public DictionnaireTuiles(int capacity)
            : base(capacity)
        {
        }

        public void Add(int x, int y, Tuile tuile)
        {
            this.Add(new Position(x, y), tuile);
        }

        public Tuile this[int x, int y]
        {
            get { return this[new Position(x, y)]; }
            set { this[new Position(x, y)] = value; }
        }

        public bool EstBienPlacee(int x, int y)
        {
            return this.EstBienPlacee(this[x, y]);
        }

        public bool EstBienPlacee(Tuile tuile)
        {
            return this[tuile.PositionCourante].Equals(tuile);
        }
    }
}
