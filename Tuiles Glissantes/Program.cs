using System;

namespace Tuiles_Glissantes
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            int nbTests = 3000;
            Console.CursorVisible = false;
            Console.Clear();
            //int reussis = 0, echoues = 0, exceptions = 0;
            uint nbMouvementsResolution = 0;
            //int nbMouvementsShuffle = (int)Math.Pow(2, 12);
            int nbMouvementsShuffle = Int16.MaxValue * 8;
            int curMouvements = 0, meilleurMouvement = int.MaxValue/*, meilleurShuffle = 0*/;
            int start = 1;
            Manipulateur ctManip;
            CasseTete ct = new CasseTete(80, 20, true, 0);
            //ctManip = new Manipulateur(ct);
            for (int i = start; i <= nbTests; i++)
            {
                ctManip = new Manipulateur(ct, i);

                //ctManip.MelangerCasseTeteAleatoire(nbMouvementsShuffle);
                ctManip.MelangerCasseTeteRotation(nbMouvementsShuffle);
                //nbMouvementsShuffle = ctManip.InverserCasseTete();
                curMouvements = ctManip.Resoudre();
                nbMouvementsResolution += (uint)curMouvements;

                if ((double)nbMouvementsShuffle / meilleurMouvement < (double)nbMouvementsShuffle / curMouvements)
                {
                    meilleurMouvement = curMouvements;
                }

                Console.SetCursorPosition(0, ct.Hauteur + 1);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Nombre de tests: {0:n0}", i);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Dernier: {0:n0}/{1:0 000}", nbMouvementsShuffle, curMouvements);
                Console.ForegroundColor = ConsoleColor.Yellow;

                //Console.ForegroundColor = ConsoleColor.Red;
                Console.Write(" Rapport: {0:n}%", (double)nbMouvementsShuffle * (i - start + 1) / nbMouvementsResolution * 100);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(" Déplacements avg: {0:n0} ", nbMouvementsResolution / (i - start + 1));
            }
            Console.ReadKey();
        }
    }
}
