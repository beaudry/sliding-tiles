using System;
using System.Collections.Generic;
using System.Linq;

namespace Tuiles_Glissantes
{
    public class CasseTete
    {
        private Tuile[,] tuiles;

        private bool isShuffling = false;
        private const int wait = 0;
        private int nbDeplacementsShuffle;
        private bool showUI;

        public CasseTete(int largeur, int hauteur, bool showUI = true)
        {
            this.tuiles = new Tuile[hauteur, largeur];
            this.showUI = showUI;
            this.RemplirCasseTete();
        }

        public int NbDeplacementsResolution { get; private set; }
        public int Hauteur { get { return this.tuiles.GetLength(0); } }
        public int Largeur { get { return this.tuiles.GetLength(1); } }
        public Position PositionVide { get; private set; }

        private Tuile this[Position position]
        {
            get { return this[position.Y, position.X]; }
            set { this[position.Y, position.X] = value; }
        }

        private Tuile this[int rangee, int colonne]
        {
            get { return this.tuiles[rangee, colonne]; }
            set
            {

                if (value != null)
                {
                    value.PositionCourante = new Position(colonne, rangee);
                }

                if (showUI)
                {
                    Console.SetCursorPosition(colonne, rangee);
                    Console.ForegroundColor = value.CouleurPolice;
                    Console.BackgroundColor = value.CouleurFond;
                    Console.Write(value == null ? ' ' : value.Caractere);
                }
                this.tuiles[rangee, colonne] = value;
            }
        }

        private void RemplirCasseTete()
        {
            for (int j = 0; j < this.Hauteur; j++)
            {
                for (int i = 0; i < this.Largeur && j + i < this.Hauteur + this.Largeur - 2; i++)
                {
                    this[j, i] = new Tuile(i, j, (char)(i + 48), (ConsoleColor)15 - j % 8, (ConsoleColor)(j / 8));
                }
            }
            this.PositionVide = new Position(this.Largeur - 1, this.Hauteur - 1);
            this[this.PositionVide] = new Tuile(this.PositionVide, '☻', ConsoleColor.Black, ConsoleColor.Black);
        }

        public void ShuffleStart()
        {
            this.isShuffling = true;
        }

        public void ShuffleStop(int nbDeplacementsShuffle)
        {
            this.isShuffling = false;
            this.nbDeplacementsShuffle = nbDeplacementsShuffle;
            this.NbDeplacementsResolution = 0;
        }

        public Dictionary<Position, Tuile> GetDictionary()
        {
            Dictionary<Position, Tuile> dict = new Dictionary<Position, Tuile>(this.tuiles.Length);
            for (int i = 0; i < this.Hauteur; i++)
            {
                for (int j = 0; j < this.Largeur; j++)
                {
                    dict.Add(this[i, j].PositionDepart, this[i, j]);
                }
            }

            return dict;
        }

        public void EchangerAvecVide(Position posEchange)
        {
            Tuile tuileTempo = this[this.PositionVide];
            this[this.PositionVide] = this[posEchange];
            this[posEchange] = tuileTempo;
            this.PositionVide = posEchange;

            if (!this.isShuffling)
            {
                this.NbDeplacementsResolution++;
                System.Threading.Thread.Sleep(wait);
            }
        }

        public bool EstTermine()
        {
            bool estTermine = true;
            foreach (var tuile in this.tuiles)
            {
                if (!tuile.EstBienPlacee())
                {
                    estTermine = false;
                    break;
                }
            }

            return estTermine;
        }
    }
}
