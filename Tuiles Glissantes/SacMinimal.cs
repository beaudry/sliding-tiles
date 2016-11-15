using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuiles_Glissantes
{
    public class SacMinimal<T>
    {
        private ISet<T> itemsDisponibles = new HashSet<T>();
        private ISet<T> itemsPasses = new HashSet<T>();

        public bool Add(T item)
        {
            if (itemsPasses.Contains(item))
            {
                return false;
            }

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
            itemsPasses.Add(meilleurItem);
            return meilleurItem;
        }

        public bool AjouterElementTraite(T item)
        {
            return this.itemsPasses.Add(item);
        }

        public bool AEteTraite(T item)
        {
            return this.itemsPasses.Contains(item);
        }

        public bool EstVide()
        {
            return this.itemsDisponibles.Count == 0;
        }
    }
}
