using System;
using System.Collections.Generic;
using System.Linq;

namespace Tuiles_Glissantes
{
    public class CasseTete
    {
        private enum Direction
        {
            Haut = 1,
            Bas = -1,
            Gauche = 2,
            Droite = -2
        }

        private enum Axe
        {
            Y = 1,
            X = 2
        }

        public enum Coin
        {
            NO,
            NE,
            SE,
            SO
        }

        private char[] caracteres = { '░', '▒', '▓', '█' };

        private DictionnaireTuiles dictTuiles;
        private Tuile[,] tuiles;
        private Position positionVide;
        private Random rng;
        private HashSet<Direction> directionsPossibles = new HashSet<Direction>(
            new[] { Direction.Haut, Direction.Bas, Direction.Gauche, Direction.Droite }
        );

        private bool isShuffling = false;
        private const int wait = 0;
        private int nbDeplacementsShuffle = 0;
        private uint nbDeplacementsResolution = 0;

        private Direction dernierMouvement;

        public CasseTete(int largeur, int hauteur/*, int seed*/)
        {
            this.tuiles = new Tuile[largeur, hauteur];
            this.dictTuiles = new DictionnaireTuiles(this.Largeur * this.Hauteur);
            this.rng = new Random(/*seed*/);
            this.RemplirCasseTete();
        }

        public int Hauteur { get { return this.tuiles.GetLength(1); } }
        public int Largeur { get { return this.tuiles.GetLength(0); } }

        private Tuile this[Position position]
        {
            get { return this[position.X, position.Y]; }
            set { this[position.X, position.Y] = value; }
        }

        private Tuile this[int x, int y]
        {
            get { return this.tuiles[x, y]; }
            set
            {
                Console.SetCursorPosition(x, y);
                if (value != null)
                {
                    Console.ForegroundColor = value.Couleur;
                    value.PositionCourante = new Position(x, y);
                }

                Console.Write(value == null ? ' ' : value.Caractere);
                this.tuiles[x, y] = value;
            }
        }

        private void RemplirCasseTete()
        {
            for (int j = 0; j < this.Hauteur; j++)
            {
                for (int i = 0; i < this.Largeur && j + i < this.Hauteur + this.Largeur - 2; i++)
                {
                    this[i, j] = new Tuile(i, j, i.ToString("X").First(), (ConsoleColor)15 - j);
                    this.dictTuiles[i, j] = this[i, j];
                }
            }
            this.positionVide = new Position(this.Largeur - 1, this.Hauteur - 1);
            this[this.positionVide] = new Tuile(this.positionVide, '☻', ConsoleColor.Black);
            this.dictTuiles[this.positionVide] = this[this.positionVide];
        }

        public void MelangerCasseTete(int nbMouvements)
        {
            Direction mouvement;

            this.isShuffling = true;
            for (int i = 0; i < nbMouvements; i++)
            {
                mouvement = this.ObtenirDirectionAleatoireFromVide();
                this.EchangerAvecVide(mouvement);
                this.dernierMouvement = mouvement;
            }
            this.isShuffling = false;
            this.nbDeplacementsShuffle = nbMouvements;
        }

        private Direction ObtenirDirectionAleatoireFromVide()
        {
            Direction[] directionsPossibles = this.ObtenirDirectionsFromPos(this.positionVide).ToArray();
            return directionsPossibles[this.rng.Next(directionsPossibles.Length)];
        }

        private HashSet<Direction> ObtenirDirectionsFromPos(Position pos)
        {
            HashSet<Direction> directions = new HashSet<Direction>();
            if (pos.X + 1 == this.Largeur)
            {
                directions.Add(Direction.Droite);
            }
            else if (pos.X == 0)
            {
                directions.Add(Direction.Gauche);
            }

            if (pos.Y + 1 == this.Hauteur)
            {
                directions.Add(Direction.Bas);
            }
            else if (pos.Y == 0)
            {
                directions.Add(Direction.Haut);
            }
            if ((int)this.dernierMouvement != 0)
            {
                directions.Add((Direction)(-(int)(this.dernierMouvement)));
            }

            // Comme notre set est un sous-ensemble de directionsPossibles, on obtient seulement
            // les éléments qui ne font pas partis de notre ensemble.
            directions.SymmetricExceptWith(this.directionsPossibles);

            return directions;
        }

        private Position GetPositionRelative(Position posRelative, Direction direction)
        {
            Position posEchange;

            switch (direction)
            {
                case Direction.Haut:
                    posEchange = new Position(posRelative.X, posRelative.Y - 1);
                    break;
                case Direction.Bas:
                    posEchange = new Position(posRelative.X, posRelative.Y + 1);
                    break;
                case Direction.Gauche:
                    posEchange = new Position(posRelative.X - 1, posRelative.Y);
                    break;
                case Direction.Droite:
                    posEchange = new Position(posRelative.X + 1, posRelative.Y);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return posEchange;
        }

        private void EchangerAvecVide(Direction directionEchange)
        {
            this.EchangerAvecVide(this.GetPositionRelative(this.positionVide, directionEchange));
        }

        private void EchangerAvecVide(Position posEchange)
        {
            Tuile tuileTempo = this[this.positionVide];
            this[this.positionVide] = this[posEchange];
            this[posEchange] = tuileTempo;
            this.positionVide = posEchange;

            if (!this.isShuffling)
            {
                this.nbDeplacementsResolution++;
                System.Threading.Thread.Sleep(wait);
            }
        }

        private Direction PeutEtreInverserDirection(Direction directionOriginale, bool inverser)
        {
            if (inverser)
            {
                return this.InverserDirection(directionOriginale);
            }
            return directionOriginale;
        }

        private Direction InverserDirection(Direction direction)
        {
            return (Direction)(-(int)direction);
        }

        private void RapprocherTuileVide(int courante, int vide, Direction plusPetit)
        {
            Direction direction;
            int nbTuiles;

            if (courante != vide)
            {
                if (courante < vide)
                {
                    direction = plusPetit;
                    nbTuiles = vide - courante;
                }
                else
                {
                    direction = this.InverserDirection(plusPetit);
                    nbTuiles = courante - vide;
                }
                this.DeplacerSerieFromVide(direction, nbTuiles - 1);
            }
        }

        public bool Resoudre()
        {
            Tuile tuileCourante;
            int noRangee, noColonne;
            for (noRangee = 0; noRangee < this.Hauteur - 2; noRangee++)
            {
                for (noColonne = 0; noColonne < this.Largeur - 1; noColonne++)
                {
                    tuileCourante = this.dictTuiles[noColonne, noRangee];


                    if (!this.dictTuiles.EstBienPlacee(tuileCourante))
                    {
                        this.DeplacerTuileToPosition(tuileCourante, noRangee, noColonne);
                    }
                }

                if (this.positionVide.Equals(noColonne, noRangee))
                {
                    this.EchangerAvecVide(Direction.Bas);
                }

                if (!this.dictTuiles.EstBienPlacee(this.dictTuiles[noColonne, noRangee]))
                {
                    this.DeplacerTuileToPosition(this.dictTuiles[noColonne, noRangee], noRangee + 1, noColonne - 1);
                    this.Debloquer(noRangee, noColonne - 2);
                    this.WhileDeplacement(Axe.X, this.dictTuiles[noColonne, noRangee], noColonne);
                    this.Embloquer(noRangee, noColonne - 2);
                }
            }

            for (noColonne = 0; noColonne < this.Largeur - 1; noColonne++)
            {
                if (!this.dictTuiles.EstBienPlacee(noColonne, noRangee))
                {
                    this.DeplacerTuileToPosition(this.dictTuiles[noColonne, noRangee], noRangee, noColonne);
                }

                if (!this.dictTuiles.EstBienPlacee(noColonne, noRangee + 1))
                {
                    if (noColonne < this.Largeur - 2)
                    {
                        if (this.dictTuiles[noColonne, noRangee + 1].PositionCourante.Equals(noColonne + 1, noRangee + 1) && this.positionVide.Equals(noColonne, noRangee + 1))
                        {
                            this.EchangerAvecVide(Direction.Droite);
                        }
                        else
                        {
                            this.DeplacerTuileToPosition(this.dictTuiles[noColonne, noRangee + 1], noRangee, noColonne + 1);
                        }
                    }
                    this.Debloquer(noRangee - 1, noColonne);
                    this.DeplacerTuileToPosition(this.dictTuiles[noColonne, noRangee + 1], noRangee, noColonne);
                    if (this.positionVide.Y < this.Hauteur - 1)
                    {
                        this.EchangerAvecVide(Direction.Bas);
                    }
                    if (this.positionVide.X > noColonne)
                    {
                        this.EchangerAvecVide(Direction.Gauche);
                    }
                    this.Embloquer(noRangee - 1, noColonne);
                }
            }

            if (this.positionVide.Y < this.Hauteur - 1)
            {
                this.EchangerAvecVide(Direction.Bas);
            }

            bool estTermine = this.EstTermine();

            //Console.SetCursorPosition(0, this.Hauteur + 1);
            //if (estTermine)
            //{
            //    Console.ForegroundColor = ConsoleColor.Green;
            //    Console.WriteLine("Terminé en {0:n0} mouvements pour {1:n0} de mélange!", this.nbDeplacementsResolution, this.nbDeplacementsShuffle);
            //}
            //else
            //{
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    Console.WriteLine("Erreur! :'(");
            //    //this.Resoudre();
            //}

            return estTermine;
        }

        private void Debloquer(int noRangee, int noColonne)
        {
            if (!this.positionVide.Equals(noColonne, noRangee))
            {
                this.DeplacerSerieFromVide(Direction.Droite, this.Largeur - 1 - this.positionVide.X);
                this.DeplacerSerieFromVide(Direction.Haut, this.positionVide.Y - noRangee);
            }

            this.DeplacerSerieFromVide(Direction.Gauche, this.Largeur - 1 - noColonne);
            this.EchangerAvecVide(Direction.Bas);
        }

        private void Embloquer(int noRangee, int noColonne)
        {
            this.DeplacerSerieFromVide(Direction.Gauche, this.positionVide.X - noColonne);
            this.DeplacerSerieFromVide(Direction.Haut, this.positionVide.Y - noRangee);
            this.DeplacerSerieFromVide(Direction.Droite, this.Largeur - 1 - noColonne);
            this.EchangerAvecVide(Direction.Bas);
        }

        private void DeplacerSerieFromVide(Direction directionVide, int nbTuiles)
        {
            for (int i = 0; i < nbTuiles; i++)
            {
                this.EchangerAvecVide(this.GetPositionRelative(this.positionVide, directionVide));
            }
        }

        //public void EffectuerQuartDeTour(Coin coin /* je suis un canard! */, bool estRotationHoraire)
        //{
        //    Direction[] serieInstruction = { Direction.Haut, Direction.Gauche, Direction.Bas, Direction.Droite };
        //    int instructPosition;
        //    switch (coin)
        //    {
        //        case Coin.NE:
        //            instructPosition = 3;
        //            break;
        //        case Coin.SE:
        //            instructPosition = 2;
        //            break;
        //        case Coin.SO:
        //            instructPosition = 1;
        //            break;
        //        case Coin.NO:
        //            instructPosition = 0;
        //            break;
        //        default:
        //            throw new ArgumentOutOfRangeException();
        //    }

        //    if (estRotationHoraire)
        //    {
        //        for (int i = instructPosition; i < instructPosition + 3; i++)
        //        {
        //            this.EchangerAvecVide(serieInstruction[i % serieInstruction.Length]);
        //        }
        //    }
        //    else
        //    {
        //        for (int i = instructPosition + 5; i > instructPosition + 2; i--)
        //        {
        //            this.EchangerAvecVide(serieInstruction[i % serieInstruction.Length]);
        //        }
        //    }


        //}

        private void BienPlacerVide(int axeVide, int axeTuile, int nonAxeVide, int nonAxeTuile, Direction direction, Coin coin2, Coin coin1, bool rotation)
        {
            if (nonAxeVide < nonAxeTuile && axeVide == axeTuile)
            {
                this.EchangerAvecVide(direction);
            }
        }

        private Direction PlacerVidePourAxe(Axe axe, Position posTuile)
        {
            Direction directionPerpendic;
            if (axe == Axe.X)
            {
                directionPerpendic = Direction.Bas;
                BienPlacerVide(positionVide.X, posTuile.X, positionVide.Y, posTuile.Y, directionPerpendic, Coin.SO, Coin.SE, false);
            }
            else
            {
                directionPerpendic = Direction.Droite;
                BienPlacerVide(positionVide.Y, posTuile.Y, positionVide.X, posTuile.X, directionPerpendic, Coin.NO, Coin.SO, true);
            }
            return directionPerpendic;
        }

        private bool VideEstEgalPourAxe(Position position, Axe axe)
        {
            if (axe == Axe.X)
            {
                return this.positionVide.X == position.X;
            }
            return this.positionVide.Y == position.Y;
        }

        private void DeplacerTuileVersDirection(Tuile tuile, Direction direction, bool tuileImmobile = false)
        {
            Axe axe = (Axe)Math.Abs((int)direction);

            Direction directionPerpendic = PlacerVidePourAxe(axe, tuile.PositionCourante);

            if (direction == Direction.Haut && tuile.PositionCourante.X == this.Largeur - 1 ||
                axe == Axe.X && tuile.PositionCourante.Y == this.Hauteur - 1)
            {
                directionPerpendic = this.InverserDirection(directionPerpendic);
            }

            Position destination = this.GetPositionRelative(tuile.PositionCourante, direction);
            if (destination.Equals(this.positionVide))
            {
                this.EchangerAvecVide(tuile.PositionCourante);
                return;
            }

            if (this.positionVide.Equals(this.GetPositionRelative(tuile.PositionCourante, this.InverserDirection(direction))))
            {
                this.EchangerAvecVide(directionPerpendic);
            }

            while (!this.VideEstEgalPourAxe(destination, axe))
            {
                this.EchangerAvecVide(direction);
            }

            if (!this.positionVide.Equals(this.GetPositionRelative(tuile.PositionCourante, direction)))
            {
                this.EchangerAvecVide(this.InverserDirection(directionPerpendic));
            }


            if (!tuileImmobile)
            {
                this.EchangerAvecVide(this.InverserDirection(direction));
            }
        }

        private void DeplacerTuileToPosition(Tuile tuileCourante, int noRangee, int noColonne)
        {
            this.RapprocherTuileVide(tuileCourante.PositionCourante.Y, this.positionVide.Y, Direction.Haut);

            if (tuileCourante.PositionCourante.Y > this.positionVide.Y)
            {
                this.EchangerAvecVide(Direction.Bas);
            }

            this.RapprocherTuileVide(tuileCourante.PositionCourante.X, this.positionVide.X, Direction.Gauche);

            this.WhileDeplacement(Axe.X, tuileCourante, noColonne);

            if (this.positionVide.X == tuileCourante.PositionCourante.X - 1 && tuileCourante.PositionCourante.X == noColonne)
            {
                this.DeplacerTuileVersDirection(
                    tuileCourante,
                    (Direction)(Math.Sign(tuileCourante.PositionCourante.X - noColonne - 2) * 2),
                    true
                );
            }

            this.WhileDeplacement(Axe.Y, tuileCourante, noRangee);
        }

        private void WhileDeplacement(Axe axe, Tuile tuileCourante, int noSerie)
        {
            Func<Tuile, int> getCoordFunction;
            switch (axe)
            {
                case Axe.Y:
                    getCoordFunction = t => t.PositionCourante.Y;
                    break;
                case Axe.X:
                    getCoordFunction = t => t.PositionCourante.X;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Axe");
            }


            while (getCoordFunction(tuileCourante) != noSerie)
            {
                this.DeplacerTuileVersDirection(
                    tuileCourante,
                    (Direction)(Math.Sign(getCoordFunction(tuileCourante) - noSerie - Convert.ToInt32(getCoordFunction(tuileCourante) < noSerie)) * (int)axe)
                );
            }
        }

        private bool EstTermine()
        {
            bool estTermine = true;
            foreach (var tuile in this.dictTuiles.Values)
            {
                if (!this.dictTuiles.EstBienPlacee(tuile))
                {
                    estTermine = false;
                    break;
                }
            }

            return estTermine;
        }
    }
}
