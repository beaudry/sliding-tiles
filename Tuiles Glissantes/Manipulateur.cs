using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuiles_Glissantes
{
    public class Manipulateur
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

        private CasseTete ct;
        private Dictionary<Position, Tuile> dictTuiles;
        private Direction dernierMouvement;
        private int? seed = null;
        private Random rng;
        private HashSet<Direction> directionsPossibles = new HashSet<Direction>(
            new[] { Direction.Haut, Direction.Bas, Direction.Gauche, Direction.Droite }
        );

        public Manipulateur(CasseTete ct)
            : this(ct, null)
        {
        }

        public Manipulateur(CasseTete ct, int? seed)
        {
            this.ct = ct;
            this.seed = seed;

            if ((this.seed = seed).HasValue)
            {
                this.rng = new Random(this.seed.Value);
            }
            else
            {
                this.rng = new Random();
            }

            this.dictTuiles = ct.GetDictionary();
        }

        private void EchangerAvecVide(Direction directionEchange)
        {
            this.EchangerAvecVide(this.GetPositionRelative(ct.PositionVide, directionEchange));
        }

        private void EchangerAvecVide(int x, int y)
        {
            this.EchangerAvecVide(new Position(x, y));
        }

        private void EchangerAvecVide(Position pos)
        {
            ct.EchangerAvecVide(pos);
        }

        public int InverserCasseTete()
        {
            ct.ShuffleStart();
            int mouvements = 0;
            for (int i = 1; i <= Math.Min(ct.Largeur, ct.Hauteur) / 2; i++)
            {
                //mouvements += this.RotationCasseTete(-this.Largeur + 1 + 2 * (i - 1), -this.Hauteur + 1 + 2 * (i - 1), (int)Math.Pow(this.Largeur + this.Hauteur - 2 - 4 * (i - 1), 2) * 2 - 1);
                mouvements += this.RotationCasseTete(-ct.Largeur + 2 * i - 1, -ct.Hauteur + 2 * i - 1, (int)Math.Pow(ct.Largeur + ct.Hauteur + 2 - 4 * i, 2) * 2 - 1);
                this.EchangerAvecVide(Direction.Gauche);
            }
            ct.ShuffleStop(mouvements);
            return mouvements;
        }

        public void MelangerCasseTeteAleatoire(int nbMouvements)
        {
            Direction mouvement;
            ct.ShuffleStart();

            for (int i = 0; i < nbMouvements; i++)
            {
                mouvement = this.ObtenirDirectionAleatoireFromVide();
                this.EchangerAvecVide(mouvement);
                this.dernierMouvement = mouvement;
            }
            ct.ShuffleStop(nbMouvements);
        }

        private int tailleCoteGrandeRotationAleatoire(int position, int tailleCote)
        {
            return position < (tailleCote - 1) / 2 ?
                rng.Next(tailleCote / 5 * 4, tailleCote) - position :
                rng.Next(0, tailleCote / 5) - position;
        }

        public void MelangerCasseTeteRotation(int nbMouvements)
        {
            int nbMouvementsFait = 0;
            ct.ShuffleStart();
            while (nbMouvementsFait < nbMouvements)
            {
                nbMouvementsFait += this.RotationCasseTeteAleatoire(
                    this.tailleCoteGrandeRotationAleatoire(ct.PositionVide.X, ct.Largeur),
                    this.tailleCoteGrandeRotationAleatoire(ct.PositionVide.Y, ct.Hauteur),
                    nbMouvements - nbMouvementsFait + 1
                );
            }
            ct.ShuffleStop(nbMouvements);
        }

        public int RotationCasseTeteAleatoire(int largeur, int hauteur, int maxMouvements)
        {
            int valeur = (int)Math.Pow(Math.Abs(largeur) + Math.Abs(hauteur), 2) / 2;
            int min = Math.Min(valeur, maxMouvements);
            return this.RotationCasseTete(largeur, hauteur, rng.Next(min, min * 3));
        }

        public int RotationCasseTete(int largeur, int hauteur, int nbMouvements)
        {
            Direction horizontal = Direction.Droite, vertical = Direction.Bas;

            if (largeur < 0)
            {
                horizontal = this.InverserDirection(horizontal);
                largeur = -largeur;
            }

            if (hauteur < 0)
            {
                vertical = this.InverserDirection(vertical);
                hauteur = -hauteur;
            }

            largeur = Math.Max(largeur, 1);
            hauteur = Math.Max(hauteur, 1);

            int nbMouvementsRestants = Math.Abs(nbMouvements);
            bool reverse = nbMouvements < 0;

            while (nbMouvementsRestants > 0)
            {
                if (reverse)
                {
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(vertical, Math.Min(hauteur, nbMouvementsRestants));
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(horizontal, Math.Min(largeur, nbMouvementsRestants));
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(this.InverserDirection(vertical), Math.Min(hauteur, nbMouvementsRestants));
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(this.InverserDirection(horizontal), Math.Min(largeur, nbMouvementsRestants));
                }
                else
                {

                    nbMouvementsRestants -= this.DeplacerSerieFromVide(horizontal, Math.Min(largeur, nbMouvementsRestants));
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(vertical, Math.Min(hauteur, nbMouvementsRestants));
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(this.InverserDirection(horizontal), Math.Min(largeur, nbMouvementsRestants));
                    nbMouvementsRestants -= this.DeplacerSerieFromVide(this.InverserDirection(vertical), Math.Min(hauteur, nbMouvementsRestants));
                }
            }
            return nbMouvements;
        }

        private Direction ObtenirDirectionAleatoireFromVide()
        {
            Direction[] directionsPossibles = this.ObtenirDirectionsFromPos(ct.PositionVide).ToArray();
            return directionsPossibles[this.rng.Next(directionsPossibles.Length)];
        }

        private HashSet<Direction> ObtenirDirectionsFromPos(Position pos)
        {
            HashSet<Direction> directions = new HashSet<Direction>();
            if (pos.X + 1 == ct.Largeur)
            {
                directions.Add(Direction.Droite);
            }
            else if (pos.X == 0)
            {
                directions.Add(Direction.Gauche);
            }

            if (pos.Y + 1 == ct.Hauteur)
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

        public int Resoudre()
        {
            SacMinimal<Tuile> sac = new SacMinimal<Tuile>();
            Tuile tuileCourante, tuileCourante2;

            sac.Add(this.dictTuiles[new Position(0, 0)]);
            //try
            //{


            while (!sac.EstVide())
            {
                tuileCourante = sac.RetirerItemMinimum(t => t.DistanceFrom(ct.PositionVide) + 5 * t.DistanceFromOrigin());
                if (tuileCourante.PositionDepart.Equals(ct.Largeur - 2, ct.Hauteur - 2))
                {
                    this.RotationCasseTete(1, 1, 2);
                    for (int i = 0; i < 4 && !ct.EstTermine(); i++)
                    {
                        this.RotationCasseTete(-1, -1, 4);
                    }
                }
                else if (tuileCourante.PositionDepart.X == ct.Largeur - 2)
                {
                    tuileCourante2 = this.dictTuiles[tuileCourante.PositionDepart.Offset(1, 0)];
                    if (tuileCourante.PositionDepart.Equals(ct.PositionVide))
                    {
                        if (tuileCourante.PositionCourante.Equals(tuileCourante2.PositionDepart))
                        {
                            this.RotationCasseTete(1, 2, 5);
                        }
                        else if (!tuileCourante2.EstBienPlacee() || !tuileCourante.PositionCourante.Equals(tuileCourante2.PositionDepart.Offset(0, 1)))
                        {
                            this.EchangerAvecVide(Direction.Bas);
                        }
                        else
                        {
                            this.RotationCasseTete(1, 3, -6);
                        }
                    }
                    if (!tuileCourante.EstBienPlacee() || !tuileCourante2.EstBienPlacee())
                    {
                        if (tuileCourante.EstBienPlacee())
                        {
                            this.DeplacerSerieFromVide(Direction.Droite, tuileCourante.PositionDepart.X - ct.PositionVide.X);
                            this.DeplacerSerieFromVide(Direction.Bas, tuileCourante.PositionDepart.Y - ct.PositionVide.Y - 1);
                            this.EchangerAvecVide(Direction.Haut);
                            this.RotationCasseTete(1, 2, 4);
                        }
                        this.DeplacerTuileToPosition(tuileCourante2, tuileCourante.PositionDepart.Y, tuileCourante.PositionDepart.X);
                        this.DeplacerTuileToPosition(tuileCourante, tuileCourante.PositionDepart.Y + 1, tuileCourante.PositionDepart.X);

                        if (ct.PositionVide.X == ct.Largeur - 1)
                        {
                            this.RotationCasseTete(-1, -1, -3);
                        }
                        else
                        {
                            this.RotationCasseTete(1, -2, 5);
                        }
                    }
                    sac.AjouterElementTraite(tuileCourante2);
                }
                else if (tuileCourante.PositionDepart.Y == ct.Hauteur - 2)
                {
                    tuileCourante2 = this.dictTuiles[tuileCourante.PositionDepart.Offset(0, 1)];
                    //if (tuileCourante.DistanceFromOrigin() < tuileCourante2.DistanceFromOrigin())
                    //{
                    //    this.PlacerColonneSurDeuxLignes(tuileCourante.PositionDepart.Y, tuileCourante2.PositionDepart.Y, tuileCourante.PositionDepart.X);
                    //}
                    //else
                    //{
                    //    this.PlacerColonneSurDeuxLignes(tuileCourante2.PositionDepart.Y, tuileCourante.PositionDepart.Y, tuileCourante.PositionDepart.X);
                    //}
                    if (tuileCourante.PositionDepart.Equals(ct.PositionVide))
                    {
                        if (tuileCourante.PositionCourante.Equals(tuileCourante2.PositionDepart))
                        {
                            this.RotationCasseTete(2, 1, -5);
                        }
                        else
                        {
                            this.EchangerAvecVide(Direction.Droite);
                        }
                    }

                    if (!tuileCourante.EstBienPlacee() || !tuileCourante2.EstBienPlacee())
                    {
                        if (tuileCourante.EstBienPlacee())
                        {
                            this.DeplacerSerieFromVide(Direction.Bas, tuileCourante.PositionDepart.Y - ct.PositionVide.Y);
                            this.DeplacerSerieFromVide(Direction.Gauche, tuileCourante.PositionDepart.X - ct.PositionVide.X - 1);
                            this.EchangerAvecVide(Direction.Gauche);
                            this.RotationCasseTete(2, 1, -4);
                        }
                        this.DeplacerTuileToPosition(tuileCourante2, tuileCourante.PositionDepart.Y, tuileCourante.PositionDepart.X);
                        this.DeplacerTuileToPosition(tuileCourante, tuileCourante.PositionDepart.Y, tuileCourante.PositionDepart.X + 1);

                        if (ct.PositionVide.Y == ct.Hauteur - 1)
                        {
                            this.RotationCasseTete(-1, -1, 3);
                        }
                        else
                        {
                            this.RotationCasseTete(-2, 1, -5);
                        }
                    }
                    sac.AjouterElementTraite(tuileCourante2);
                }
                else
                {
                    this.DeplacerTuileToPosition(tuileCourante);
                }

                if (tuileCourante.PositionDepart.Y == 0 || tuileCourante.PositionDepart.X < ct.Largeur - 2 && sac.AEteTraite(this.dictTuiles[tuileCourante.PositionDepart.Offset(1, -1)]))
                {
                    sac.Add(this.dictTuiles[tuileCourante.PositionDepart.Offset(1, 0)]);
                }

                if (tuileCourante.PositionDepart.X == 0 || tuileCourante.PositionDepart.Y < ct.Hauteur - 2 && sac.AEteTraite(this.dictTuiles[tuileCourante.PositionDepart.Offset(-1, 1)]))
                {
                    sac.Add(this.dictTuiles[tuileCourante.PositionDepart.Offset(0, 1)]);
                }
            }
            //}
            //catch (Exception e)
            //{
            //    if (ct.EstTermine())
            //    {
            //        return ct.NbDeplacementsResolution;
            //    }
            //    throw e;
            //}


            //int noRangee, noColonne;
            //Position pos;
            //// Theta(H)
            //for (noRangee = 0; noRangee < ct.Hauteur - 2; noRangee++)
            //{
            //    // Theta(L)
            //    for (noColonne = 0; noColonne < ct.Largeur - 1; noColonne++)
            //    {
            //        //if (ct.PositionVide.Equals(pos))
            //        //{
            //        //    this.EchangerAvecVide(Direction.Bas);
            //        //}

            //        tuileCourante = this.dictTuiles[new Position(noColonne, noRangee)];

            //        if (!tuileCourante.EstBienPlacee())
            //        {
            //            this.DeplacerTuileToPosition(tuileCourante);
            //        }
            //    }

            //    pos = new Position(noColonne, noRangee);
            //    //if (ct.PositionVide.Equals(pos))
            //    //{
            //    //    this.EchangerAvecVide(Direction.Bas);
            //    //}

            //    if (!(tuileCourante = this.dictTuiles[pos]).EstBienPlacee())
            //    {
            //        //modificateur = tuileCourante.PositionCourante.Y == noRangee + 1 && tuileCourante.PositionCourante.X < noColonne;
            //        this.DeplacerTuileToPosition(tuileCourante, noRangee + 1, noColonne - 1);

            //        // On met l'espace à gauche
            //        if (ct.PositionVide.X == noColonne)
            //        {
            //            this.DeplacerTuileVersDirection(tuileCourante, Direction.Gauche, true);
            //        }

            //        this.Embloquer(noRangee, noColonne - 2, noColonne - 1);
            //        this.Debloquer(noRangee, noColonne - 2, noColonne);
            //    }
            //}

            //Position posRangee, posRangeePlusUn;
            //for (noColonne = 0; noColonne < ct.Largeur - 1; noColonne++)
            //{
            //    posRangee = new Position(noColonne, noRangee);
            //    posRangeePlusUn = new Position(noColonne, noRangee + 1);

            //    if (this.ct.PositionVide.X == noColonne)
            //    {
            //        this.EchangerAvecVide(Direction.Droite);
            //    }

            //    if (!this.dictTuiles[posRangee].EstBienPlacee() || !this.dictTuiles[new Position(noColonne, noRangee + 1)].EstBienPlacee())
            //    {
            //        if (this.dictTuiles[posRangee].PositionCourante.X < this.dictTuiles[posRangeePlusUn].PositionCourante.X)
            //        {
            //            this.PlacerColonneSurDeuxLignes(noRangee, noRangee + 1, noColonne);
            //        }
            //        else
            //        {
            //            this.PlacerColonneSurDeuxLignes(noRangee + 1, noRangee, noColonne);
            //        }
            //    }
            //}

            //if (!this.dictTuiles[ct.PositionVide].EstBienPlacee())
            //{
            //    ct.EchangerAvecVide(new Position(ct.Largeur - 1, ct.Hauteur - 1));
            //}

            if (ct.EstTermine())
            {
                return this.ct.NbDeplacementsResolution;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private void DeplacerTuileToPosition(Tuile tuileCourante, bool gererVide = true)
        {
            this.DeplacerTuileToPosition(tuileCourante, tuileCourante.PositionDepart.Y, tuileCourante.PositionDepart.X, gererVide);
        }

        private void DeplacerTuileToPosition(Tuile tuileCourante, int noRangee, int noColonne, bool gererVide = true)
        {
            if (tuileCourante.PositionCourante.Equals(noColonne, noRangee))
            {
                return;
            }

            //if (gererVide && ct.PositionVide.Equals(noColonne, noRangee))
            //{
            //    this.EchangerAvecVide(Direction.Bas);
            //}

            //if (tuileCourante.PositionCourante.Y < noRangee)
            //{
            //    this.RapprocherTuileVide(tuileCourante.PositionCourante.X, ct.PositionVide.X, Direction.Gauche);
            //    this.RapprocherTuileVide(tuileCourante.PositionCourante.Y, ct.PositionVide.Y, Direction.Haut);

            //    //if (tuileCourante.PositionCourante.X > ct.PositionVide.X)
            //    //{
            //    //    this.EchangerAvecVide(Direction.Droite);
            //    //}

            //    this.WhileDeplacement(Axe.Y, tuileCourante, noRangee);

            //    if (ct.PositionVide.Y == tuileCourante.PositionCourante.Y - 1 && tuileCourante.PositionCourante.Y == noRangee)
            //    {
            //        this.DeplacerTuileVersDirection(
            //            tuileCourante,
            //            (Direction)(Math.Sign(tuileCourante.PositionCourante.Y - noRangee - 2)),
            //            true
            //        );
            //    }

            //    this.WhileDeplacement(Axe.X, tuileCourante, noColonne);

            //}
            //else
            //{

            //if (ct.PositionVide.X < ct.Largeur - 2)
            //{
            //    this.EchangerAvecVide(Direction.Droite);
            //}

            this.RapprocherTuileVide(tuileCourante);

            if (noRangee >= tuileCourante.PositionCourante.Y && gererVide)
            {
                if (tuileCourante.PositionCourante.X > ct.PositionVide.X)
                {
                    this.EchangerAvecVide(tuileCourante.PositionCourante.Offset(-1, 0));
                    this.EchangerAvecVide(tuileCourante.PositionCourante);
                }
                //if (tuileCourante.PositionCourante.X == ct.PositionVide.X)
                //{
                //    this.RotationCasseTete(-1, -1, 3);
                //}

                this.WhileDeplacement(Axe.Y, tuileCourante, noRangee);

                this.DeplacerTuileVersDirection(
                    tuileCourante,
                    (Direction)(Math.Sign(tuileCourante.PositionCourante.Y - noRangee - 2)),
                    true
                );
            }
            else if (tuileCourante.PositionCourante.Y > ct.PositionVide.Y)
            {
                this.EchangerAvecVide(Direction.Bas);
            }

            this.WhileDeplacement(Axe.X, tuileCourante, noColonne);

            if (ct.PositionVide.X == tuileCourante.PositionCourante.X - 1 && tuileCourante.PositionCourante.X == noColonne)
            {
                this.DeplacerTuileVersDirection(
                    tuileCourante,
                    (Direction)(Math.Sign(tuileCourante.PositionCourante.X - noColonne - 2) * 2),
                    true
                );
            }

            this.WhileDeplacement(Axe.Y, tuileCourante, noRangee);
            //}
        }

        private Direction InverserDirection(Direction direction)
        {
            return (Direction)(-(int)direction);
        }

        private void RapprocherTuileVide(Tuile tuile)
        {
            int[] coordonnees = new int[2];
            int[] coordonneesVide = new int[2];
            Direction[] directions = new Direction[2];

            if (tuile.PositionCourante.X < ct.PositionVide.X)
            {
                coordonnees[0] = Math.Min(tuile.PositionCourante.Y + 1, ct.Hauteur - 1);
                coordonneesVide[0] = ct.PositionVide.Y;
                directions[0] = Direction.Haut;
                coordonnees[1] = Math.Min(tuile.PositionCourante.X, ct.Largeur - 1);
                coordonneesVide[1] = ct.PositionVide.X;
                directions[1] = Direction.Gauche;
            }
            else
            {
                coordonnees[0] = Math.Min(tuile.PositionCourante.X + 1, ct.Largeur - 1);
                coordonneesVide[0] = ct.PositionVide.X;
                directions[0] = Direction.Gauche;
                coordonnees[1] = Math.Min(tuile.PositionCourante.Y, ct.Hauteur - 1);
                coordonneesVide[1] = ct.PositionVide.Y;
                directions[1] = Direction.Haut;
            }

            for (int i = 0; i < coordonnees.Length; i++)
            {
                this.RapprocherTuileVideSingleDirection(coordonnees[i], coordonneesVide[i], directions[i], i);
            }
        }

        private void RapprocherTuileVideSingleDirection(int courante, int vide, Direction plusPetit, int distanceAcceptable)
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

        private void PlacerColonneSurDeuxLignes(int noRangee1, int noRangee2, int noColonne)
        {
            Position position1 = new Position(noColonne, noRangee1);
            Position position2 = new Position(noColonne, noRangee2);

            if (this.dictTuiles[position2].PositionCourante.X == noColonne)
            {
                if (ct.PositionVide.Y != noRangee1)
                {
                    this.EchangerAvecVide(ct.PositionVide.X, noRangee1);
                }
                this.RapprocherTuileVideSingleDirection(noColonne, ct.PositionVide.X, Direction.Gauche, 1);
                this.RotationCasseTete(-1, noRangee2 - noRangee1, 3);
                this.RotationCasseTete(1, noRangee1 - noRangee2, 3);
                this.RotationCasseTete(-1, noRangee2 - noRangee1, -4);
            }
            else if (this.dictTuiles[position1].EstBienPlacee() && this.dictTuiles[new Position(noColonne, noRangee2)].PositionCourante.X == noColonne + 1)
            {
                if (ct.PositionVide.Y != noRangee2)
                {
                    this.EchangerAvecVide(ct.PositionVide.X, noRangee2);
                }
                this.RapprocherTuileVideSingleDirection(noColonne, ct.PositionVide.X, Direction.Gauche, 1);

                if (this.dictTuiles[position2].PositionCourante.Y == noRangee2)
                {
                    this.RotationCasseTete(-1, noRangee1 - noRangee2, 4);
                }
                else
                {
                    this.RotationCasseTete(1, noRangee1 - noRangee2, 3);
                    this.RotationCasseTete(-1, noRangee2 - noRangee1, -3);
                }
            }

            this.DeplacerTuileToPosition(this.dictTuiles[position1], noRangee2, noColonne, false);

            if (ct.PositionVide.Y == noRangee1 && ct.PositionVide.X < ct.Largeur - 1)
            {
                this.EchangerAvecVide(Direction.Droite);
            }

            this.DeplacerTuileToPosition(this.dictTuiles[position2], noRangee2, noColonne + 1, false);

            if (ct.PositionVide.Y == noRangee2)
            {
                this.EchangerAvecVide(ct.PositionVide.X, ct.PositionVide.Y - noRangee2 + noRangee1);
            }

            this.DeplacerSerieFromVide(Direction.Gauche, ct.PositionVide.X - noColonne);
            this.EchangerAvecVide(ct.PositionVide.X, ct.PositionVide.Y - noRangee1 + noRangee2);
            this.EchangerAvecVide(Direction.Droite);
        }

        private void Debloquer(int noRangee, int noColonneEntree, int noColonneSortie)
        {
            if (!ct.PositionVide.Equals(noColonneEntree, noRangee))
            {
                this.DeplacerSerieFromVide(Direction.Droite, noColonneSortie - ct.PositionVide.X);
                this.DeplacerSerieFromVide(Direction.Haut, ct.PositionVide.Y - noRangee);
            }

            this.DeplacerSerieFromVide(Direction.Gauche, noColonneSortie - noColonneEntree);
            this.EchangerAvecVide(Direction.Bas);
        }

        private void Embloquer(int noRangee, int noColonneEntree, int noColonneSortie)
        {
            // Theta(nbTuiles)
            this.DeplacerSerieFromVide(Direction.Gauche, ct.PositionVide.X - noColonneEntree);
            this.DeplacerSerieFromVide(Direction.Haut, ct.PositionVide.Y - noRangee);
            this.DeplacerSerieFromVide(Direction.Droite, noColonneSortie - noColonneEntree);
            this.EchangerAvecVide(Direction.Bas);
        }

        private int DeplacerSerieFromVide(Direction directionVide, int nbTuiles)
        {
            // Theta(nbTuiles)
            for (int i = 0; i < nbTuiles; i++)
            {
                this.EchangerAvecVide(this.GetPositionRelative(ct.PositionVide, directionVide));
            }
            return nbTuiles;
        }

        private void BienPlacerVide(int axeVide, int axeTuile, int nonAxeVide, int nonAxeTuile, Direction direction)
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
                BienPlacerVide(ct.PositionVide.X, posTuile.X, ct.PositionVide.Y, posTuile.Y, directionPerpendic);
            }
            else
            {
                directionPerpendic = Direction.Droite;
                BienPlacerVide(ct.PositionVide.Y, posTuile.Y, ct.PositionVide.X, posTuile.X, directionPerpendic);
            }
            return directionPerpendic;
        }

        private bool VideEstEgalPourAxe(Position position, Axe axe)
        {
            if (axe == Axe.X)
            {
                return ct.PositionVide.X == position.X;
            }
            return ct.PositionVide.Y == position.Y;
        }

        private void DeplacerTuileVersDirection(Tuile tuile, Direction direction, bool tuileImmobile = false)
        {
            Axe axe = (Axe)Math.Abs((int)direction);

            Direction directionPerpendic = PlacerVidePourAxe(axe, tuile.PositionCourante);

            if (direction == Direction.Haut && tuile.PositionCourante.X == ct.Largeur - 1 ||
                axe == Axe.X && tuile.PositionCourante.Y == ct.Hauteur - 1)
            {
                directionPerpendic = this.InverserDirection(directionPerpendic);
            }

            Position destination = this.GetPositionRelative(tuile.PositionCourante, direction);
            if (destination.Equals(ct.PositionVide))
            {
                this.EchangerAvecVide(tuile.PositionCourante);
                return;
            }

            if (ct.PositionVide.Equals(this.GetPositionRelative(tuile.PositionCourante, this.InverserDirection(direction))))
            {
                this.EchangerAvecVide(directionPerpendic);
            }

            while (!this.VideEstEgalPourAxe(destination, axe))
            {
                this.EchangerAvecVide(direction);
            }

            if (!ct.PositionVide.Equals(this.GetPositionRelative(tuile.PositionCourante, direction)))
            {
                this.EchangerAvecVide(this.InverserDirection(directionPerpendic));
            }

            if (!tuileImmobile)
            {
                this.EchangerAvecVide(this.InverserDirection(direction));
            }
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
    }
}
