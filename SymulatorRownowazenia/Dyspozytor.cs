using System;
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
        //Mało intuicyjne, ale prawdziwe - to tutaj wczytywane są zadania z listy
        public List<Podzadanie> ZadaniaDoWykonania;
        //Lista węzłów
        public List<Wezel> Wezly;
        //Główny zegar logiczny
        public long Zegar;
        //Wymagana liczność kworum. Musi być mniejsza od powtórzeń usługi na różnych węzłach
        public int LicznoscKworum;
        //Na razie nieobsługiwane. Zadania przydzielane są modyfikacją JSFa - tj. na podstawie ilorazu zadań na danym węźle i mocy obliczeniowej danego węzła.
        public string SposobPrzydzialuZadan;







        public void Init()
        {

            try
            {   using (StreamReader sr = new StreamReader("Zadania.txt"))
                {
                    Zegar = 0;
                    string stmp;
                    int itmp;
                    int iloscuslug;
                    ZadaniaDoWykonania = new List<Podzadanie>();
                    ZadaniaPrzetwarzane = new List<Zadanie>();
                    ZadaniaZakonczone = new List<Zadanie>();
                    Wezly = new List<Wezel>();
                    int idwezlaiter = 1;  

                    //Wczytanie zadań
                    String line = sr.ReadLine();
                    Int32.TryParse(line.Split(',')[0], out itmp);
                    iloscuslug = itmp;
                    Console.WriteLine(line);
                    while ((line = sr.ReadLine()) != null)
                    {
                        Console.WriteLine(line);
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

                        ZadaniaDoWykonania.Add(nowezadanie);
                    }

                    //Kworum
                    Console.WriteLine("Podaj liczność kworum");
                    stmp = Console.ReadLine();

                    Int32.TryParse(stmp, out itmp);

                    LicznoscKworum = itmp;
                    //Tworzymy listę na której zaznaczać będziemy ilości wystąpień poszczególnych usług
                    int[] wystapienia = new int[iloscuslug+1];
                    for (int i = 0; i <= iloscuslug; i++)
                    {
                        wystapienia[i] = 0;
                    }


                    //Tworzenie węzłów;
                    do
                    {
                        Wezel nowywezel = new Wezel();
                        nowywezel.IDWezla = idwezlaiter;
                        idwezlaiter++;
                        nowywezel.CzasNieaktywnosci = 0;
                        nowywezel.PrzypisaneZadania = new List<Podzadanie>();
                        nowywezel.ObslugiwaneUslugi = new List<int>();

                        Console.Write("Proszę podać listę usług ulokowanych na tym węźle w formie u1,u2,u3,u4...");
                        Console.Write("Przypomnienie: Zadeklarowane jest " + (iloscuslug+1).ToString() + " usług. Podawaj więc numery od 0 do " + (iloscuslug).ToString());

                        stmp = Console.ReadLine();

                        foreach (string uslu in stmp.Split(','))
                        {
                            int numeruslugi;
                            Int32.TryParse(uslu, out numeruslugi);

                            wystapienia[numeruslugi]++;
                            nowywezel.ObslugiwaneUslugi.Add(numeruslugi);
                        }

                        Console.Write("Proszę podać moc obliczeniową węzła, tj. ilość kwantów czasu procesora w jednym kwancie czasu");
                        stmp = Console.ReadLine();
                        Int32.TryParse(stmp, out itmp);
                        nowywezel.MocObliczeniowa = itmp;

                        Console.Write("Proszę podać potencjał równomiernego przetwarzania węzła, tj. maksymalną ilość zadań które mogą otrzymać na danym procesorze kwant czasu procesora w kwancie czasu");
                        stmp = Console.ReadLine();
                        Int32.TryParse(stmp, out itmp);
                        nowywezel.PotencjalRownobieznegoPrzetwarzania = itmp;

                        Wezly.Add(nowywezel);

                        int min = wystapienia.Min();
                        if (min < LicznoscKworum)
                        {
                            Console.Write("Liczność kworum jest wyższa od minimalnej ilości wystąpień jednej z usług. Przechodzenie to tworzenia kolejnego węzła...");
                            stmp = Console.ReadLine();
                        }

                        else
                        {
                            Console.Write("Czy chcesz przejść do tworzenia kolejnego węzła? Odpowiedź 'N' spowoduje zakończenie tworzenia węzłów.");
                            stmp = Console.ReadLine();
                        }
                    }
                    while (!stmp.Equals("N"));
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
            //Zwraca ID węzła o najniższej wartości stosunku ilości przypisanych zadań do mocy obliczeniowej
            return ZUslugaClean.OrderBy(e => e.PrzypisaneZadania.Count() / e.MocObliczeniowa).First().IDWezla;   
        }


        public void Simulate()
        {
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

                        nadchodzace.Podzadania.Add(exec);

                        int idwezla = ZnajdzWezel(exec.IDuslugi, wykorzystanewezly);
                        wykorzystanewezly.Add(idwezla);

                        Wezel WybranyWezel = Wezly.Where(e => e.IDWezla == idwezla).FirstOrDefault();
                        WybranyWezel.PrzypisaneZadania.Add(exec);                        
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

            Console.WriteLine("Congratulations, the winner is you");
            Console.WriteLine("Wykonano " + ZadaniaZakonczone.Count().ToString() + " zadań, czyli " + (ZadaniaZakonczone.Count() * LicznoscKworum).ToString() + " podzadań.");
            Console.WriteLine("Podczas zakończenia zegar logiczny miał wartość " + Zegar.ToString());

            foreach (Zadanie ukonczone in ZadaniaZakonczone)
            {
                Console.WriteLine("Zadanie " + ukonczone.IDZadania.ToString());

                foreach (Podzadanie pukonczone in ukonczone.Podzadania)
                {
                    Console.WriteLine(pukonczone.ChwilaNadejscia.ToString() + " " + pukonczone.CzasOczekiwania.ToString());
                }

            }





        }


    }
}
