using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorRownowazenia
{
    class Program
    {
        static void Main(string[] args)
        {

            Dyspozytor dysp = new Dyspozytor();
            dysp.Init();

            dysp.Simulate();
            Console.ReadKey();

        }

    }
}
