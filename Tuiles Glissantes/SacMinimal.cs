using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuiles_Glissantes
{
    public class SacMinimal<T>
    {
        private HashSet<T> itemsDisponibles = new HashSet<T>();

        public bool Add(T item)
        {
            return this.itemsDisponibles.Add(item);
        }

        public T RetirerItemMinimum(Func<T, int> methodeComparaison)
        {
            T meilleurItem = default(T);
            foreach (T item in itemsDisponibles)
            {
                if (meilleurItem == null || methodeComparaison(item) < methodeComparaison(meilleurItem))
                {
                    meilleurItem = item;
                }
            }

            itemsDisponibles.Remove(meilleurItem);
            return meilleurItem;
        }

        public bool EstVide()
        {
            return this.itemsDisponibles.Count == 0;
        }
    }
}
