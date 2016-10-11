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
            int nbTests = 300;
            Console.CursorVisible = false;
            CasseTete ct;
            int reussis = 0, echoues = 0, exceptions = 0;
            for (int i = 0; i < nbTests; i++)
            {
                ct = new CasseTete(16, 15);
                ct.MelangerCasseTete(9000);

                try
                {
                    if (ct.Resoudre())
                    {
                        reussis++;
                    }
                    else
                    {
                        echoues++;
                    }
                }
                catch (Exception)
                {
                    exceptions++;
                }

                Console.SetCursorPosition(0, ct.Hauteur + 1);
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("Nombre de tests: {0:n0}", reussis + echoues + exceptions);
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Réussis: {0:n}%", (double)reussis / (reussis + echoues + exceptions) * 100);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(" Échoués: {0:n}%", (double)echoues / (reussis + echoues + exceptions) * 100);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(" Erreurs: {0:n}%", (double)exceptions / (reussis + echoues + exceptions) * 100);
            }
            Console.ReadKey();
        }
    }
}
