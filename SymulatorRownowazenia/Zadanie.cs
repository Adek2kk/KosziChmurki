using SymulatorRownowazenia;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorRownowazenia
{
    class Zadanie
    {
        //Identyfikator obsługiwanego zadania
        public int IDZadania { get; set; }
        //Podzadania na które rozbite zostało dane zadanie
        public List<Podzadanie> Podzadania { get; set; }
        //Czy zadanie zostało w całości zakończone
        public bool Zakonczone { get; set; }

        //Funkcja sprawdzająca czy wszystkie podzadania zostały ukończone.
        public bool czyukonczone()
        {
            foreach (Podzadanie j in Podzadania)
            { if (j.Zakonczone == false) return false; }
            Zakonczone = true;
            return true;
        }
    }
}
