using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maelstrom_Engine
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Initializing");
            using (Game game = new Game(1280, 720, "Maelstrom Engine"))
            {
                game.Run(60);
            }
        }
    }
}
