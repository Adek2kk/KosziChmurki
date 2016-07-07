using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Generator
{
    class Program
    {

        static void Main(string[] args)
        {
            int liczba_grup;
            int idzadania = 1;
            int liczb_podlist_w_grupie;
            int min_czasprzetw = 1;
            int max_czasprzetw = 5;
            List<double> korelacje_grup = new List<double>();
            List<List<double>> korelacje_w_grupie = new List<List<double>>();
            List<List<double>> startery = new List<List<double>>();
            List<List<List<double>>> przebiegi = new List<List<List<double>>>();
            double stddev;
            double mean;
            int czas; // ilosc elementow



            Console.WriteLine("Podaj okres nadchodzenia zadan ");
            string line = Console.ReadLine();
            Int32.TryParse(line, out czas);

            Console.WriteLine("Podaj ilość grup usług");
            line = Console.ReadLine();
            Int32.TryParse(line, out liczba_grup);

            Console.WriteLine("Podaj ilość usług w grupie");
            line = Console.ReadLine();
            Int32.TryParse(line, out liczb_podlist_w_grupie);

            Console.WriteLine("Podaj odchylenie standardowe generowanych obciążeń");
            line = Console.ReadLine();
            double.TryParse(line, out stddev);

            Console.WriteLine("Podaj srednią wartość generowanych obciążeń");
            line = Console.ReadLine();
            double.TryParse(line, out mean);

            for (int i = 1; i < liczba_grup; i++)
            {
                double temp;
                Console.WriteLine("Podaj korelację pierwszych przebiegów kolejnych grup z pierwszym przebiegiem pierwszej grupy");
                line = Console.ReadLine();
                double.TryParse(line, out temp);
                korelacje_grup.Add(temp);
            }

            for (int i = 0; i < liczba_grup; i++)
            {
                List<double> temp = new List<double>();
                Console.WriteLine("Podaj korelację kolejnych przebiegów z pierwszym przebiegiem grupy numer:" + i);
                for (int j = 1; j < liczb_podlist_w_grupie; j++)
                {

                    double temp1;
                    line = Console.ReadLine();
                    double.TryParse(line, out temp1);
                    temp.Add(temp1);

                }
                korelacje_w_grupie.Add(temp);
            }

            Console.WriteLine("Podaj minimalny a nastepnie maksymalny czas przetwarzania nadchodzącego zadania");
            line = Console.ReadLine();
            int.TryParse(line, out min_czasprzetw);
            line = Console.ReadLine();
            int.TryParse(line, out max_czasprzetw);

            Exponential wykladniczy_rand = new Exponential(min_czasprzetw, max_czasprzetw, 0);


            NormalData gen = new NormalData(mean, stddev, 1);


            List<double> starterSequence = gen.NormalList(czas);
            GrupaObciazen nowaGrupa = new GrupaObciazen(stddev, mean, czas);
            startery.Add(starterSequence);
            for (int i = 0; i < korelacje_grup.Count(); i++)
            {
                startery.Add(nowaGrupa.generujPrzebieg(starterSequence, korelacje_grup[i]));

            }

            for (int i = 0; i < korelacje_w_grupie.Count(); i++)
            {
                List<List<double>> tempo = new List<List<double>>();
                tempo.Add(startery[i]);
                for (int j = 0; j < korelacje_w_grupie[i].Count(); j++)
                {
                    //if(i==0)
                    Console.Write(korelacje_w_grupie[i][j]);

                    tempo.Add(nowaGrupa.generujPrzebieg(startery[i], korelacje_w_grupie[i][j]));

                }
                przebiegi.Add(tempo);
            }
            // do pliku obciazenia zapisywane są obciążenia generowane przez kolejne usługi w kolejnych jednostkach czasu, w kolejnych wierszach kolejne usługi
            string mydocpath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\Obciazenia.txt"))
            {
                for (int i = 0; i < liczba_grup; i++)
                {
                    Console.WriteLine();

                    for (int j = 0; j < liczb_podlist_w_grupie; j++)
                    {
                        Console.WriteLine();

                        for (int k = 0; k < czas; k++)
                        {
                            Console.Write(przebiegi[i][j][k] + ";");
                            double temphelp = przebiegi[i][j][k] + mean + 2 * stddev;
                            outputFile.Write(temphelp + ";");

                        }

                    }
                }

            }
            // do Zadania_gen zapisujemy, w pierwszym wierszu czas przez jaki generujemy zadania, liczbę grup, liczbę usług w grupie, w kolejnych id_zadania;czas w którym sie pojawia;długosc;grupa|numer_w_grupie
            using (StreamWriter outputFile = new StreamWriter(mydocpath + @"\Zadania_gen.txt"))
            {
                int bezw_numer_uslugi = 0;

                outputFile.WriteLine(czas.ToString()+","+ liczba_grup.ToString()+","+liczb_podlist_w_grupie.ToString());


                for (int i = 0; i < liczba_grup; i++)
                {
                    for (int j = 0; j < liczb_podlist_w_grupie; j++)
                    {
                        for (int k = 0; k < czas; k++)
                        {
                            Console.Write(przebiegi[i][j][k] + ";");
                            double temphelp = przebiegi[i][j][k] + mean + 2 * stddev;
                            while (temphelp > 0)
                            {
                                double helper = wykladniczy_rand.NextData();
                                outputFile.WriteLine(idzadania.ToString() + "," + k.ToString() + "," + Convert.ToInt32(helper).ToString("F0") + "," + bezw_numer_uslugi.ToString());
                                temphelp -= helper;
                                idzadania++;
                            }

                        }
                        bezw_numer_uslugi++;

                    }
                }

            }


#if DEBUG
            Console.WriteLine("Press enter to close...");
            Console.ReadLine();
#endif
        }
    }

}
