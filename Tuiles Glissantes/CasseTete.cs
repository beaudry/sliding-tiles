﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Tuiles_Glissantes
{
    public class CasseTete
    {
        private Tuile[,] tuiles;

        private bool isShuffling = false;
        private int wait;
        private int nbDeplacementsShuffle;
        private bool showUI;

        public CasseTete(int largeur, int hauteur, bool showUI = true, int wait = 0)
        {
            this.tuiles = new Tuile[hauteur, largeur];
            this.showUI = showUI;
            this.Init();
            this.wait = wait;
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

        private char ObtenirCaractereRangee(int rangee)
        {
            rangee += 48;

            if (rangee > 122)
            {
                rangee += 70;
            }

            return (char)rangee;
        }

        private void Init()
        {
            Console.WindowHeight = this.Hauteur;
            Console.WindowWidth = Math.Max(this.Largeur, 80);
            this.RemplirCasseTete();
        }

        private void RemplirCasseTete()
        {
            for (int j = 0; j < this.Hauteur; j++)
            {
                for (int i = 0; i < this.Largeur && j + i < this.Hauteur + this.Largeur - 2; i++)
                {
                    this[j, i] = new Tuile(i, j, this.ObtenirCaractereRangee(i), (ConsoleColor)15 - j % 8, (ConsoleColor)(j / 8));
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

            if (tuileTempo.DistanceFrom(posEchange) > 1)
            {
                throw new InvalidOperationException("On ne peut pas échanger avec la cellule vide une case qui ne se trouve pas à côté de la case vide");
            }

            if (!this.PositionVide.Equals(posEchange))
            {
                this[this.PositionVide] = this[posEchange];
                this[posEchange] = tuileTempo;
                this.PositionVide = posEchange;

                if (!this.isShuffling)
                {
                    this.NbDeplacementsResolution++;

                    if (this.wait > 0)
                    {
                        System.Threading.Thread.Sleep(wait);
                    }
                }
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

        public bool EstCompletementMelange()
        {
            bool estCompletementMelange = true;
            foreach (var tuile in this.tuiles)
            {
                if (tuile.EstBienPlacee())
                {
                    estCompletementMelange = false;
                    break;
                }
            }

            return estCompletementMelange;

        }
    }
}