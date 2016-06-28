using System;
using System.Collections.Generic;
using System.Linq;

namespace Node_generator
{
    class Services
    {
        public int Count { get; set; }
        public int GroupsCount { get; set; }
        public int ServicesWithinGroupCount { get; set; }
        public List<double> GroupsCorrelations { get; set; }
        public double[,] Correlations { get; set; }

        public Services()
        {
            GroupsCorrelations = new List<double>();
        }

        public void InitializeCorrelations()
        {
            Count = GroupsCount * ServicesWithinGroupCount;
            Correlations = new double[Count, Count];
        }

        public void AddCorrelation(int groupIdA, int serviceIdA, int groupIdB, int serviceIdB, double value)
        {
            var a = GetCoord(groupIdA, serviceIdA);
            var b = GetCoord(groupIdB, serviceIdB);
            Correlations[a, b] = value;
            Correlations[b, a] = value;
        }

        public void CalculateCorrelations()
        {
            // calculating correlations within group
            // i-j = avg(0-i, 0-j)
            for (int g = 0; g < GroupsCount; g++)
            {
                for (int i = 1; i < ServicesWithinGroupCount; i++)
                {
                    for (int j = i + 1; j < ServicesWithinGroupCount; j++)
                    {
                        var correlation = (Correlations[GetCoord(g, 0), GetCoord(g, i)] + Correlations[GetCoord(g, 0), GetCoord(g, j)]) / 2.0;
                        Correlations[GetCoord(g, i), GetCoord(g, j)] = correlation;
                        Correlations[GetCoord(g, j), GetCoord(g, i)] = correlation;
                    }
                }
            }
            // calculating correlations between first elements of the groups
            for (int i = 1; i < GroupsCount - 1; i++)
            {
                for (int j = i + 1; j < GroupsCount; j++)
                {
                    var idI = GetCoord(i, 0);
                    var idJ = GetCoord(j, 0);
                    var correlation = (Correlations[0, idI] + Correlations[0, idJ]) / 2.0;
                    Correlations[idI, idJ] = correlation;
                    Correlations[idJ, idI] = correlation;
                }
            }
            // calculating correlations between different groups elements
            for (int g1 = 0; g1 < GroupsCount - 1; g1++)
            {
                for (int g2 = g1 + 1; g2 < GroupsCount; g2++)
                {
                    for (int i = 0; i < ServicesWithinGroupCount; i++)
                    {
                        for (int j = 0; j < ServicesWithinGroupCount; j++)
                        {
                            // first between the groups * ((first -> i) + (first -> j)) / 2.0
                            // don't alter anything below - "it's a kind of magic"
                            if (i != 0 || j != 0) 
                            {
                                var correlation =
                                Math.Abs(Correlations[GetCoord(g1, 0), GetCoord(g2, 0)]) *
                                (Correlations[GetCoord(g1, 0), GetCoord(g1, i)] + Correlations[GetCoord(g2, 0), GetCoord(g2, j)]) / 2.0;
                                Correlations[GetCoord(g1, i), GetCoord(g2, j)] = correlation;
                                Correlations[GetCoord(g2, j), GetCoord(g1, i)] = correlation;
                            }
                        }
                    }
                }
            }
        }

        public List<int> GetServicesListByCorrelation(Generator.Distribution distribution) 
        {
            var correlations = new List<Tuple<int, int, double>>();
            for (int i = 0; i < Count; i++)
            {
                for (int j = i + 1; j < Count; j++) 
                {
                    correlations.Add(new Tuple<int, int, double>(i, j, Correlations[i, j]));
                }
            }
            if (distribution == Generator.Distribution.Negative) 
            {
                correlations = correlations.OrderBy(c => c.Item3).ToList();
            }
            else 
            {
                correlations = correlations.OrderByDescending(c => c.Item3).ToList();
            }
            var list = new List<int>();
            foreach (var c in correlations) 
            {
                list.Add(c.Item1);
                list.Add(c.Item2);
            }
            return list;
        }

        public int GetCoord(int groupId, int serviceId) 
        {
            return groupId * ServicesWithinGroupCount + serviceId;
        }

        public string GetCoord(int value) 
        {
            return String.Format("{0}|{1}", value / ServicesWithinGroupCount, value % ServicesWithinGroupCount);
        }
    }
}
