using System;
namespace DungeonGenerator {
    public class Navigation {
        static public System.Random pseudoRandom;
        static public int getRandomNormalNumber() {
            /* Used for room sizes*/
            /*Special thanks to Superbest random:
             * https://bitbucket.org/Superbest/superbest-random/src
             *  */
            double u1 = pseudoRandom.NextDouble();
            double u2 = pseudoRandom.NextDouble();
            double mu = 12;
            double sigma = 8;
            double rand_std_normal = Math.Sqrt(-2 * Math.Log(u1)) *
                                Math.Sin(2 * Math.PI * u2);
            double rand_normal = mu + sigma * rand_std_normal;
            return (int)rand_normal;
        }
        static public int getDirection() {
            /* 0: UP  1: RIGHT 2: DOWN 3: LEFT */
            return pseudoRandom.Next(0, 4);
        }
    }
}