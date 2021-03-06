﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorRownowazenia
{
    class Dyspozytor
    {
        //Zadania które są przetwarzane
        public List<Zadanie> ZadaniaPrzetwarzane;
        //Zadania które zostały przetworzone
        public List<Zadanie> ZadaniaZakonczone;
        //To tutaj wczytywane są zadania z listy
        public List<Podzadanie> ZadaniaDoWykonania;
        //Lista węzłów
        public List<Wezel> Wezly;


        //Tablica list potrzebna do parametru globalnego
        public List<double>[] ZadaniaDoUslugi;

        //Główny zegar logiczny
        public long Zegar;
        //Wymagana liczność kworum. Musi być mniejsza od powtórzeń usługi na różnych węzłach
        public int LicznoscKworum;
        

        public int IloscUslug = 0;

        //Funkcja obliczająca odchylenie standardowe z listy doubli
        private double getStandardDeviation(List<double> doubleList)
        {
            double average = 0;
            try
            {
                average = doubleList.Average();
            }
            catch { int a=1; }
            double sumOfDerivation = 0;
            foreach (double value in doubleList)
            {
                sumOfDerivation += (value) * (value);
            }
            //double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count - 1);
            double sumOfDerivationAverage = sumOfDerivation / (doubleList.Count);
            return Math.Sqrt(sumOfDerivationAverage - (average * average));
        }


        public void Init()
        {

            string stmp;
            int itmp, itmp2;
            int iloscuslug = 0;
            ZadaniaDoWykonania = new List<Podzadanie>();
            ZadaniaPrzetwarzane = new List<Zadanie>();
            ZadaniaZakonczone = new List<Zadanie>();
            Wezly = new List<Wezel>();

            try
            {   using (StreamReader sr = new StreamReader("Zadania_gen.txt"))
                {
                    Zegar = 0;
                    

                    //Wczytanie zadań
                    String line = sr.ReadLine();
                    //Pobieramy ilość grup
                    Int32.TryParse(line.Split(',')[1], out itmp);
                    //Pobieramy ilość usług w każdej grupie
                    Int32.TryParse(line.Split(',')[2], out itmp2);
                    iloscuslug = itmp * itmp2;
                    IloscUslug = iloscuslug;

                    //Stworzenie tablicy list potrzebnej do obliczenia globalnego paramteru
                    ZadaniaDoUslugi = new List<double>[IloscUslug];
                    for(int i=0;i<IloscUslug;i++)
                        ZadaniaDoUslugi[i] = new List<double>();

                    Console.WriteLine(line);
                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        Podzadanie nowezadanie = new Podzadanie();

                        int tmp;
                        Int32.TryParse(line.Split(',')[0], out tmp);
                        nowezadanie.IDZadania = tmp;
                        Int32.TryParse(line.Split(',')[1], out tmp);
                        nowezadanie.ChwilaNadejscia = tmp;
                        Int32.TryParse(line.Split(',')[2], out tmp);
                        nowezadanie.WymaganyCzasPrzetwarzania = tmp;
                        Int32.TryParse(line.Split(',')[3], out tmp);
                        nowezadanie.IDuslugi = tmp;

                        ZadaniaDoUslugi[nowezadanie.IDuslugi].Add(Convert.ToDouble(nowezadanie.WymaganyCzasPrzetwarzania));

                        ZadaniaDoWykonania.Add(nowezadanie);
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Coś poszło nie tak.");
                Console.WriteLine(e.Message);
            }

            Console.WriteLine("Wczytywanie zadań zakończyło się sukcesem. Rozpoczynanie wczytywania listy węzłów...");

            try
            {
                using (StreamReader sr = new StreamReader("Wezel_gen.txt"))
                {

                    //Kworum
                    Console.WriteLine("Podaj liczność kworum");
                    stmp = Console.ReadLine();

                    Int32.TryParse(stmp, out itmp);

                    LicznoscKworum = itmp;


                    String line;

                    //Sanity check
                    if (iloscuslug == 0)
                    {
                        Console.WriteLine("Coś nie zadziałało z ilością usług...");
                        Console.ReadKey();
                    }

                    //Tworzymy listę na której zaznaczać będziemy ilości wystąpień poszczególnych usług
                    int[] wystapienia = new int[iloscuslug];
                    for (int i = 0; i < iloscuslug; i++)
                    {
                        wystapienia[i] = 0;
                    }
                    

                    while ((line = sr.ReadLine()) != null)
                    {
                        //Console.WriteLine(line);
                        Wezel nowywezel = new Wezel();
                        nowywezel.CzasNieaktywnosci = 0;
                        nowywezel.PrzypisaneZadania = new List<Podzadanie>();
                        nowywezel.HistoriaZadania = new List<Podzadanie>();
                        nowywezel.ObslugiwaneUslugi = new List<int>();
                        nowywezel.DlugosciPrzypisanychZadan = new List<double>();
                        nowywezel.FragmentyPrzypisanychZadan = new List<int>();
                        nowywezel.WspolczynnikObciazenia = 0;

                        int tmp;
                        int ilosczadan;


                        //Wczytuję podstawowe informacje dotyczące węzła
                        Int32.TryParse(line.Split(' ')[0], out tmp);
                        nowywezel.IDWezla = tmp;
                        Int32.TryParse(line.Split(' ')[1], out tmp);
                        nowywezel.MocObliczeniowa = tmp;
                        Int32.TryParse(line.Split(' ')[2], out tmp);
                        nowywezel.PotencjalRownobieznegoPrzetwarzania = tmp;
                        Int32.TryParse(line.Split(' ')[3], out tmp);
                        ilosczadan = tmp;






                        //Wczytuję listę usług istniejących na danym węźle
                        for (int i = 4; i < ilosczadan + 4; i++)
                        {
                            Int32.TryParse(line.Split(' ')[i], out tmp);
                            wystapienia[tmp]++;
                            nowywezel.ObslugiwaneUslugi.Add(tmp);
                        }

                        Wezly.Add(nowywezel);
                    }

                    int min = wystapienia.Min();
                    if (min < LicznoscKworum)
                    {
                        Console.Write("Liczność kworum jest wyższa od minimalnej ilości wystąpień jednej z usług. Być może konieczna jest ponowna generacja węzłów/ zmniejszenie liczności kworum");
                        stmp = Console.ReadLine();
                    }



                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Coś poszło nie tak.");
                Console.WriteLine(e.Message);
            }




        }


        public int ZnajdzWezel(int usluga, List<int> wykorzystane)
        {
            //Wyszukuje wszystkie węzły obsługujące daną usługę
            List<Wezel> ZUsluga = Wezly.Where(e => e.ObslugiwaneUslugi.Contains(usluga)).ToList();
            //Usuwa z listy węzły które już zostały wykorzystane przy zapewnianiu kworum
            List<Wezel> ZUslugaClean = ZUsluga.Where(e => !(wykorzystane.Contains(e.IDWezla))).ToList();
            //Sortuje węzły na liście po ich współczynnikach obciążenia
            List<Wezel> ZUslugaCleanSort = ZUslugaClean.OrderBy(e => e.WspolczynnikObciazenia).ToList();
            //Zwraca ID węzła o najniższej wartości stosunku ilości przypisanych zadań do mocy obliczeniowej
            return ZUslugaCleanSort.First().IDWezla;   
        }


        public void Simulate()
        {
            Console.WriteLine("Trwa sortowanie zadań po chwili nadejścia.");
            ZadaniaDoWykonania = ZadaniaDoWykonania.OrderBy(e => e.ChwilaNadejscia).ToList();

            //DEBUG - wypisuje zadania do wykonania
            /*foreach (Podzadanie zad in ZadaniaDoWykonania)
            { Console.WriteLine(zad.IDZadania + "," + zad.ChwilaNadejscia + "," + zad.CzasPrzetwarzania + "," + zad.IDuslugi); }
            */

            //DEBUG - wypisuje węzły
            /*foreach (Wezel aktwez in Wezly)
            {
                Console.Write(aktwez.IDWezla.ToString() + ", moc obliczeniowa: " + aktwez.MocObliczeniowa.ToString() + ", równobieżność: " + aktwez.PotencjalRownobieznegoPrzetwarzania.ToString() + ", usługi: ");

                foreach (int usluga in aktwez.ObslugiwaneUslugi)
                { Console.Write(usluga.ToString() + " "); }

                Console.WriteLine();
            }*/

            Console.WriteLine("Aby rozpocząć przetwarzanie wciśnij klawisz Enter.");
            Console.ReadLine();

            Console.WriteLine("Trwa przetwarzanie.");

            while (ZadaniaDoWykonania.Count > 0 || ZadaniaPrzetwarzane.Count > 0)
            {
                //Nadeszło zadanie. Dodajemy je do odpowiedniej listy i rozmieszczamy na węzłach.
                while (ZadaniaDoWykonania.Count > 0 && ZadaniaDoWykonania[0].ChwilaNadejscia == Zegar)
                {
                    //Pobieram zadanie które nadeszło i usuwam je z listy zadań.
                    Podzadanie nadeszlo = ZadaniaDoWykonania[0];
                    ZadaniaDoWykonania.RemoveAt(0);
                    Zadanie nadchodzace = new Zadanie();
                    nadchodzace.IDZadania = nadeszlo.IDZadania;
                    nadchodzace.Zakonczone = false;
                    nadchodzace.Podzadania = new List<Podzadanie>();

                    List<int> wykorzystanewezly = new List<int>();
                    for (int i = 0; i < LicznoscKworum; i++)
                    {
                        //Generuję nowe podzadanie
                        Podzadanie exec = new Podzadanie();
                        exec.ChwilaNadejscia = (int)Zegar;
                        exec.CzasOczekiwania = 0;
                        exec.CzasPrzetwarzania = 0;
                        exec.IDuslugi = nadeszlo.IDuslugi;
                        exec.IDZadania = nadeszlo.IDZadania;
                        exec.WymaganyCzasPrzetwarzania = nadeszlo.WymaganyCzasPrzetwarzania;
                        exec.Zakonczone = false;

                        Podzadanie exec2 = new Podzadanie();
                        exec2.ChwilaNadejscia = (int)Zegar;
                        exec2.CzasOczekiwania = 0;
                        exec2.CzasPrzetwarzania = 0;
                        exec2.IDuslugi = nadeszlo.IDuslugi;
                        exec2.IDZadania = nadeszlo.IDZadania;
                        exec2.WymaganyCzasPrzetwarzania = nadeszlo.WymaganyCzasPrzetwarzania;
                        exec2.Zakonczone = false;

                        nadchodzace.Podzadania.Add(exec);

                        int idwezla = ZnajdzWezel(exec.IDuslugi, wykorzystanewezly);
                        wykorzystanewezly.Add(idwezla);

                        Wezel WybranyWezel = Wezly.Where(e => e.IDWezla == idwezla).FirstOrDefault();
                        WybranyWezel.PrzypisaneZadania.Add(exec);
                        WybranyWezel.HistoriaZadania.Add(exec2);
                        WybranyWezel.SumaCzasowPrzypisanychZadan = WybranyWezel.SumaCzasowPrzypisanychZadan + exec.WymaganyCzasPrzetwarzania;
                        WybranyWezel.DlugosciPrzypisanychZadan.Add(Convert.ToDouble(exec.WymaganyCzasPrzetwarzania));
                        WybranyWezel.FragmentyPrzypisanychZadan.Add(exec.IDuslugi);
                        WybranyWezel.ObliczWspolczynnikObciazenia();
                    }

                    ZadaniaPrzetwarzane.Add(nadchodzace);
                }

                //Każdy węzeł wykonuje swoje przetwarzanie
                foreach (Wezel wezelek in Wezly)
                {
                    if (wezelek.PrzypisaneZadania.Count() == 0) { wezelek.CzasNieaktywnosci++; }
                    else { wezelek.WykonajKrok(); }
                }

                //Sprawdzamy czy zakończyliśmy któreś zadanie
                foreach (Zadanie zadanko in ZadaniaPrzetwarzane.Reverse<Zadanie>())
                {
                    if (zadanko.czyukonczone())
                    {
                        ZadaniaPrzetwarzane.Remove(zadanko);
                        ZadaniaZakonczone.Add(zadanko);
                    }
                }


                Zegar++;
            }

            //DEBUG - komunikat o zakończeniu przetwarzania
            Console.WriteLine("Zadanie zakończono.");
            Console.WriteLine("Wykonano " + ZadaniaZakonczone.Count().ToString() + " zadań, czyli " + (ZadaniaZakonczone.Count() * LicznoscKworum).ToString() + " podzadań.");
            Console.WriteLine("Podczas zakończenia zegar logiczny miał wartość " + Zegar.ToString());

            //DEBUG - wypisuje statystyki poszczególnych zadań
            /*foreach (Zadanie ukonczone in ZadaniaZakonczone)
            {
                Console.WriteLine("Zadanie " + ukonczone.IDZadania.ToString());

                foreach (Podzadanie pukonczone in ukonczone.Podzadania)
                {
                    Console.WriteLine(pukonczone.ChwilaNadejscia.ToString() + " " + pukonczone.CzasOczekiwania.ToString());
                }

            }*/

            //Obliczenie parametru globalnego
            double odchsum = 0, sumaodchylen = 0;
            List<double> SumyUslug = new List<double>();

            for (int i = 0; i < IloscUslug; i++)
            {
                SumyUslug.Add(ZadaniaDoUslugi[i].Sum());
                sumaodchylen += getStandardDeviation(ZadaniaDoUslugi[i]);
            }
            odchsum = getStandardDeviation(SumyUslug);

            //DEBUG - oblicza i wyświetla drugi z parametrów dla każdego węzła
            using (var writer = new StreamWriter("paramOutput.csv"))
            {
                writer.WriteLine("global;" + odchsum / sumaodchylen);
                writer.WriteLine("node;local;");
                foreach (Wezel w in Wezly)
                {
                    double wartosc = 0;
                    wartosc = getStandardDeviation(w.DlugosciPrzypisanychZadan);
                    List<double>[] LocalZadaniaDoUslugi;
                    LocalZadaniaDoUslugi = new List<double>[IloscUslug];
                    for (int i = 0; i < IloscUslug; i++)
                        LocalZadaniaDoUslugi[i] = new List<double>();
                   
                    foreach(Podzadanie zadanko in w.HistoriaZadania)
                        LocalZadaniaDoUslugi[zadanko.IDuslugi].Add(Convert.ToDouble(zadanko.WymaganyCzasPrzetwarzania));
                    
                    double localsuma = 0;
                    for (int i =0;i< IloscUslug;i++)
                    {
                        if (LocalZadaniaDoUslugi[i].Count() != 0)
                            localsuma += getStandardDeviation(LocalZadaniaDoUslugi[i]);
                    }
                  
                    Console.WriteLine("Dla wezla " + w.IDWezla + " wartość lokalnego parametru wynosi: " + wartosc / localsuma);
                    writer.WriteLine(w.IDWezla + ";" + wartosc / localsuma);
                }
            }


            int max, min, sum, n;
            string ocz;

            using (var writer = new StreamWriter("tasksOutput.csv"))
            {
                writer.WriteLine("task;max;min;avg;kworum;");
                foreach (Zadanie ukonczone in ZadaniaZakonczone)
                {
                    max = -1;
                    min = -1;
                    sum = 0;
                    n = 0;
                    ocz = "";

                    foreach (Podzadanie pukonczone in ukonczone.Podzadania)
                    {
                        if (max < pukonczone.CzasOczekiwania)
                            max = pukonczone.CzasOczekiwania;
                        if (min == -1)
                            min = pukonczone.CzasOczekiwania;
                        else if (min > pukonczone.CzasOczekiwania)
                            min = pukonczone.CzasOczekiwania;
                        sum += pukonczone.CzasOczekiwania;
                        n++;
                        ocz += pukonczone.CzasOczekiwania + ";";
                    }

                    writer.WriteLine(ukonczone.IDZadania.ToString() + "; " + max + "; " + min + "; " + sum / n + "; " + ocz);

                }

            }
            string uslugi;
            using (var writer = new StreamWriter("nodesOutput.csv"))
            {
                writer.WriteLine("logicClock;" + Zegar);
                writer.WriteLine("node;noActive;workLoad;services");

                foreach (Wezel wez in Wezly)
                {
                    uslugi = "";
                    foreach (int usg in wez.ObslugiwaneUslugi)
                    {
                        uslugi += usg + ",";
                    }

                    writer.WriteLine(wez.IDWezla + ";" + wez.CzasNieaktywnosci + ";" + wez.WykonaneKwantyCzasu + ";" + uslugi + ";");

                }

            }

            using (var writer = new StreamWriter("serviceOutput.csv"))
            {
                int[,] serviceTable = new int[IloscUslug, 2];
                Array.Clear(serviceTable, 0, serviceTable.Length);
                writer.WriteLine("service;workToDo;count;");
                foreach (Zadanie ukonczone in ZadaniaZakonczone)
                {
                    foreach (Podzadanie pukonczone in ukonczone.Podzadania)
                    {
                        serviceTable[pukonczone.IDuslugi, 0] += pukonczone.WymaganyCzasPrzetwarzania;
                        serviceTable[pukonczone.IDuslugi, 1]++;
                    }
                }
                for (int i = 0; i < IloscUslug; i++)
                {
                    writer.WriteLine(i +";" + serviceTable[i, 0] + ";" + serviceTable[i, 1] + ";");
                }
            }

        }
    }
}
