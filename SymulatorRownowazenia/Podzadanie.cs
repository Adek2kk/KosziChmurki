using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SymulatorRownowazenia
{

    class Podzadanie
    {   
        //ID zadania nadrzędnego
        public int IDZadania { get; set; }
        //Identyfikator wymaganej usługi
        public int IDuslugi { get; set; }
        //Czy podzadanie zostało zakońćzone
        public bool Zakonczone { get; set; }
        //Wielkość zadania
        public int WymaganyCzasPrzetwarzania { get; set; }
        //Ilość kwantów czasu poświęconych na przetwarzanie zadania
        public int CzasPrzetwarzania { get; set; }
        //Ilość kwantów czasu które dane zadanie poświęciło na oczekiwanie
        public int CzasOczekiwania { get; set; }
        //Moment nadejścia danego (pod)zadania do systemu
        public int ChwilaNadejscia { get; set; }

        public void krok(bool wykonywane)
        {
          if (wykonywane) CzasPrzetwarzania++;
            else CzasOczekiwania++;

          if (CzasPrzetwarzania >= WymaganyCzasPrzetwarzania)
           { Zakonczone = true; }
        }
    }
}
