using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Generator
{
    class GrupaObciazen
    {
        double stddev;
        double mean;
        int czas; // ilosc elementow

        public GrupaObciazen(double stdev, double mean, int czas){
            this.stddev = stdev;
            this.mean = mean;
            this.czas = czas;
        }

        double corelatedValue(double a, double b, double corel)
        {
            double result;
            result = corel * a + Math.Sqrt(1 - Math.Pow(corel, 2)) * b;

            return result;
        }


       public List<double> generujPrzebieg(List<double>start,double korelacja)
        {
            NormalData normaldejta = new NormalData(stddev, mean, 1);

            List<double> result = new List<double>();
            List<double> helper = normaldejta.NormalList(czas);
            for (int i = 0; i < start.Count(); i++)
            {
                result.Add(corelatedValue(start[i], helper[i], korelacja));
            }

            return result;
        }
    }
}
