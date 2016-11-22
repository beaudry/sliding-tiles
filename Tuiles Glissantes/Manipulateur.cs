using System;
using System.Collections.Generic;
using System.Linq;

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

        public int RotationCasseTete(int largeur, int hauteur, int nbMouvements, bool miroir)
        {
            if (miroir)
            {
                return this.RotationCasseTete(hauteur, largeur, -nbMouvements);
            }
            return this.RotationCasseTete(largeur, hauteur, nbMouvements);
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
                case Direction.Bas:
                    posEchange = posRelative.Offset(0, -(int)direction);
                    break;
                case Direction.Gauche:
                case Direction.Droite:
                    posEchange = posRelative.Offset(-(int)direction / 2, 0);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return posEchange;
        }

        public int Resoudre()
        {
            SacMinimal<Tuile> sac = new SacMinimal<Tuile>();
            Tuile tuileCourante;

            sac.Add(this.dictTuiles[new Position(0, 0)]);

            while (!sac.EstVide())
            {
                tuileCourante = sac.RetirerItemMinimum(t => t.DistanceFrom(ct.PositionVide) + 5 * t.DistanceFromOrigin());
                if (tuileCourante.PositionDepart.Equals(ct.Largeur - 2, ct.Hauteur - 2))
                {
                    if (!ct.PositionVide.Equals(ct.Largeur - 1, ct.Hauteur - 1))
                    {
                        this.RotationCasseTete(1, 1, 2);
                    }
                    for (int i = 0; i < 4 && !ct.EstTermine(); i++)
                    {
                        this.RotationCasseTete(-1, -1, 4);
                    }
                }
                else if (tuileCourante.PositionDepart.X == ct.Largeur - 2 || tuileCourante.PositionDepart.Y == ct.Hauteur - 2)
                {
                    sac.AjouterElementTraite(this.PlacerDernieresTuiles(tuileCourante));
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
            this.DeplacerTuileToPosition(tuileCourante, tuileCourante.PositionDepart, gererVide);
        }

        private void DeplacerTuileToPosition(Tuile tuileCourante, Position positionDestination, bool gererVide = true)
        {
            this.DeplacerTuileToPosition(tuileCourante, positionDestination.Y, positionDestination.X, gererVide);
        }

        private void DeplacerTuileToPosition(Tuile tuileCourante, int noRangee, int noColonne, bool gererVide = true)
        {
            if (tuileCourante.PositionCourante.Equals(noColonne, noRangee))
            {
                return;
            }

            this.RapprocherTuileVide(tuileCourante);

            if (noRangee >= tuileCourante.PositionCourante.Y && gererVide)
            {
                if (tuileCourante.PositionCourante.X > ct.PositionVide.X)
                {
                    this.EchangerAvecVide(tuileCourante.PositionCourante.Offset(-1, 0));
                    this.EchangerAvecVide(tuileCourante.PositionCourante);
                }
                else if (tuileCourante.PositionCourante.Y < ct.PositionVide.Y)
                {
                    this.EchangerAvecVide(Direction.Haut);
                }

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

        private int DeplacerSerieFromVide(Direction directionVide, int nbTuiles)
        {
            if (nbTuiles < 0)
            {
                directionVide = this.InverserDirection(directionVide);
                nbTuiles = -nbTuiles;
            }
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

        private Direction ObtenirDirectionPerpendic(Axe axe)
        {
            if (axe == Axe.X)
            {
                return Direction.Bas;
            }
            return Direction.Droite;
        }

        private Direction PlacerVidePourAxe(Axe axe, Position posTuile)
        {
            Direction directionPerpendic = this.ObtenirDirectionPerpendic(axe);
            if (axe == Axe.X)
            {
                BienPlacerVide(ct.PositionVide.X, posTuile.X, ct.PositionVide.Y, posTuile.Y, directionPerpendic);
            }
            else
            {
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

        private Tuile PlacerDernieresTuiles(Tuile tuileCourante)
        {
            Axe axe = (Axe)(Convert.ToInt32(tuileCourante.PositionDepart.X == ct.Largeur - 2) + 1);
            Direction perpendic = this.ObtenirDirectionPerpendic(axe);
            bool flip = axe != Axe.X;
            int tailleCote = (!flip ? ct.Largeur : ct.Hauteur) - 1;

            Func<Position, int> getCoord1, getCoord2;
            switch (axe)
            {
                case Axe.X:
                    getCoord1 = t => t.X;
                    getCoord2 = t => t.Y;
                    break;
                case Axe.Y:
                    getCoord1 = t => t.Y;
                    getCoord2 = t => t.X;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("Axe");
            }

            Tuile tuileCourante2 = this.dictTuiles[tuileCourante.PositionDepart.Offset(1, 0, flip)];

            if (tuileCourante2.EstBienPlacee() && !tuileCourante.EstBienPlacee())
            {
                this.DeplacerSerieFromVide(this.InverserDirection((Direction)axe), getCoord1(tuileCourante.PositionDepart) - getCoord1(ct.PositionVide));
                this.DeplacerSerieFromVide(perpendic, getCoord2(tuileCourante.PositionDepart.Offset(0, 1, flip)) - getCoord2(ct.PositionVide));
                this.RotationCasseTete(1, -1, -3, flip);
            }

            if (tuileCourante.PositionCourante.Equals(tuileCourante2.PositionDepart))
            {
                this.DeplacerSerieFromVide(this.InverserDirection((Direction)axe), getCoord1(tuileCourante.PositionDepart) - getCoord1(ct.PositionVide));
                this.DeplacerSerieFromVide(perpendic, getCoord2(tuileCourante.PositionDepart) - getCoord2(ct.PositionVide));
                this.RotationCasseTete(1, 2, 3, flip);
            }

            if (tuileCourante.PositionDepart.Equals(ct.PositionVide))
            {
                if (!tuileCourante2.EstBienPlacee() || !tuileCourante.PositionCourante.Equals(tuileCourante2.PositionDepart.Offset(1, 0, flip)))
                {
                    this.EchangerAvecVide(perpendic);
                }
                else
                {
                    this.RotationCasseTete(1, 3, -6, flip);
                }
            }

            if (!tuileCourante.EstBienPlacee() || !tuileCourante2.EstBienPlacee())
            {
                if (tuileCourante.EstBienPlacee())
                {
                    this.DeplacerSerieFromVide(this.InverserDirection((Direction)axe), getCoord1(tuileCourante.PositionDepart) - getCoord1(ct.PositionVide));
                    this.DeplacerSerieFromVide(perpendic, getCoord2(tuileCourante.PositionDepart) - getCoord2(ct.PositionVide));
                    this.RotationCasseTete(1, 2, 5, flip);
                }

                this.DeplacerTuileToPosition(tuileCourante2, tuileCourante.PositionDepart);
                this.DeplacerTuileToPosition(tuileCourante, tuileCourante.PositionDepart.Offset(0, 1, flip));

                if (getCoord1(ct.PositionVide) < tailleCote - 1)
                {
                    this.RotationCasseTete(tailleCote - getCoord1(ct.PositionVide) - 1, 1, getCoord1(ct.PositionVide) - tailleCote, flip);
                }

                if (getCoord1(ct.PositionVide) == tailleCote)
                {
                    this.DeplacerSerieFromVide(this.InverserDirection(perpendic), getCoord2(ct.PositionVide) - getCoord2(tuileCourante2.PositionDepart) - 1);
                    this.RotationCasseTete(-1, -1, -3, flip);
                }
                else
                {
                    this.DeplacerSerieFromVide(this.InverserDirection(perpendic), getCoord2(ct.PositionVide) - getCoord2(tuileCourante.PositionDepart) - 2);
                    this.RotationCasseTete(1, -2, 5, flip);
                }
            }
            return tuileCourante2;
        }
    }
}
