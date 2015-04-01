using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace RouletteStrategyTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double totalCapital = 0;
            for (int i = 0; i < 100; i++)
            {
                RouletteMachine machine = new RouletteMachine();
                StrategyPlayer player = new StrategyPlayer();

                machine.AddPlayer(player);
                machine.Run(10000);
                Thread.Sleep(100);
                totalCapital += player._capital;
                Console.WriteLine(string.Format("TotalCapital:{2}, Capital:{0}, Steps:{1}", player._capital, player._cashFlow.Count, totalCapital));
            }
            Console.Read();
        }
    }
}
