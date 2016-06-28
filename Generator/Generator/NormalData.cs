using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Generator
{
    class NormalData
    {
        private Random r;
        private double mean;
        private double stddev;

        public NormalData(double mean, double stddev, int seed)
        {
            r = new Random(seed);
            this.mean = mean;
            this.stddev = stddev;
        }
        public double NextData()
        {
            double u1 = r.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = r.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stddev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
        public double NextDataRand()
        {
            Thread.Sleep(1);
            Random r1 = new Random((int) DateTime.Now.Ticks & 0x0000FFFF);
            double u1 = r1.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = r1.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal = mean + stddev * randStdNormal; //random normal(mean,stdDev^2)
            return randNormal;
        }
        public List<double> NormalList(int ile)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < ile; i++)
            {
                result.Add(NextDataRand());
            }
            return result;
        }
    }
}
