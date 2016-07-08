using System;
using System.IO;

namespace Node_generator
{
    class Program
    {
        static private Generator generator;
        static void Main(string[] args)
        {
            generator = new Generator();
            Console.WriteLine("Podaj liczbę węzłów:");
            generator.NodesCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj domyślną moc węzłów:");
            generator.ComputingPower = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj domyślną liczbę jednocześnie przetwarzanych procesów:");
            generator.ParallelComputingPotential = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj liczbę usług na węźle:");
            generator.ServicesPerNodeCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj liczbę grup usług:");
            generator.Services.GroupsCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Podaj liczbę usług w grupie:");
            generator.Services.ServicesWithinGroupCount = Convert.ToInt32(Console.ReadLine());
            generator.Services.InitializeCorrelations();
            Console.WriteLine("Podaj korelacje między grupą pierwszą a kolejnymi:");
            for (int i = 1; i < generator.Services.GroupsCount; i++) 
            {
                generator.Services.AddCorrelation(0, 0, i, 0, Convert.ToDouble(Console.ReadLine()));
            }
            Console.WriteLine("Podaj korelacje między pierwszą usługą a kolejnymi, dla kolejnych grup usług:");
            for (int i = 0; i < generator.Services.GroupsCount; i++) 
            {
                for (int j = 1; j <= generator.Services.ServicesWithinGroupCount - 1; j++)
                {
                    generator.Services.AddCorrelation(i, 0, i, j, Convert.ToDouble(Console.ReadLine()));
                }
            }
            // below function calculates correlations between all the services
            generator.Services.CalculateCorrelations();

            Console.WriteLine("Podaj typ rozkładu usług (0 - Random, 1 - Negative Correlation, 2 -  Postive Correlation):");
            using (var writer = new StreamWriter("output.txt"))
            {
                foreach (var line in generator.Generate((Generator.Distribution)Convert.ToInt32(Console.ReadLine())))
                {
                    writer.WriteLine(line);
                }
            }
        }

        static private void DisplayCorrelations() 
        {
            for (int i = 0; i < generator.Services.Count; i++)
            {
                for (int j = 0; j < generator.Services.Count; j++)
                {
                    Console.WriteLine(i + " " + j + " " + generator.Services.Correlations[i, j]);
                }
            }
        }
    }
}
