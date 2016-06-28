using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorRownowazenia
{
    //Wedle obecnej implementacji, węzeł przydziela kwanty czasu zadaniom przy użyciu zmodyfikowanego algorytmu Round Robin - tj. w czasie
    //jednego kwantu czasu stara się rozłożyć kwanty czasu procesora równomiernie między taką ilość przydzielonych mu
    //zadań jaka jest podana w potencjale równomiernego przetwarzania.
    class Wezel
    {
        //Identyfikator węzła
        public int IDWezla { get; set; }
        //Lista podzadań aktualnie wykonujących się na danym węźle
        public List<Podzadanie> PrzypisaneZadania { get; set; }
        //Lista identyfikatorów usłyug obsługiwanych na danym węźle
        public List<int> ObslugiwaneUslugi { get; set; }
        //Sumaryczny czas w którym podczas symulacji na węźle nie wykonywało się żadne zadanie
        public int CzasNieaktywnosci { get; set; }
        //Sposób w jaki kwanty czasu procesora przypisywane są zadaniom na nim się wykonującym. Na razie niewykorzystywane. Kwanty aktualnie 
        //przydzielane są wariacją na temat algorytmu Round Robin, biorącemu pod uwagę potencjał równobieżnego przetwarzania węzła
        public int TrybObslugi { get; set; }
        //Ilość kwantów czasu procesora dostępnych w jednostce czasu
        public int MocObliczeniowa { get; set; }
        //Na razie nie jestem do końca pewny. :) Wedle obecnej implementacji, węzeł będzie starał się rozbić dostępne kwanty procesora na dana liczbę zadań.
        //PotencjalRownobieznegoPrzetwarzania musi być mniejszy lub równy MocObliczeniowa.
        public int PotencjalRownobieznegoPrzetwarzania { get; set; }

        //Funkcja przypisuje kwant czasu procesora danemu zadaniu, po czym zwraca 'true' jeżeli zadanie zostało zakończone, 'false' w przeciwnym razie
        public bool NadajKwant(Podzadanie zadanieDocelowe)
        {
            zadanieDocelowe.krok(true);

            if (zadanieDocelowe.Zakonczone == true) return true;
            else return false;
        }
        
        //Funkcja wywołująca się raz na kwant czasu, odpowiada za wykonywanie zadań
        public void WykonajKrok()
        {
            //Zadanie któremu przyporządkujemy kwant czasu
            int j = 0;
            //Maksymalne ID zadania do którego możemy się odwołać
            int makszadid = Math.Min(PrzypisaneZadania.Count(), PotencjalRownobieznegoPrzetwarzania)-1;

            for (int i = 0; i < MocObliczeniowa; i++)
            {
                if (PrzypisaneZadania.Count() > 0)
                {
                    Podzadanie zadanieWykonywane = PrzypisaneZadania.ElementAt(j);
                    bool zakonczone = NadajKwant(zadanieWykonywane);
                    if (zakonczone)
                    {
                        //Wyrzucamy podzadanie z węzła
                        PrzypisaneZadania.RemoveAt(j);
                        makszadid = Math.Min(PrzypisaneZadania.Count(), PotencjalRownobieznegoPrzetwarzania) - 1;
                    }

                    if (j >= makszadid) j = 0;
                    else j++;
                }
            }


            for (int i = makszadid + 1; i < PrzypisaneZadania.Count(); i++)
            {
                Podzadanie zadanieWykonywane = PrzypisaneZadania.ElementAt(i);
                zadanieWykonywane.krok(false);
            }

        }
    }
}
