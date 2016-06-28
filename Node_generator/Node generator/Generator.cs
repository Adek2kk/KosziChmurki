using System;
using System.Collections.Generic;

namespace Node_generator
{
    class Generator
    {
        public enum Distribution { Random, Negative, Positive }
        public int NodesCount { get; set; }
        public int ServicesPerNodeCount { get; set; }
        public Services Services { get; set; }

        private Random random;
        private bool[] servicesAvailability;
        private int serviceRank;

        public Generator() 
        {
            random = new Random();
            Services = new Services();
        }

        public List<string> Generate(Distribution servicesDistribution) 
        {
            InitializeAvailableServicesList();
            // the below list is utilized only in case of non-random distribution
            var correlationList = new List<int>();
            if (servicesDistribution != Distribution.Random) 
            {
                correlationList = Services.GetServicesListByCorrelation(servicesDistribution);
            }
            var outputList = new List<string>();
            for (int nodeId = 0; nodeId < NodesCount; nodeId++)
            {
                var nodeProperties = new List<string>();
                // node id
                nodeProperties.Add(nodeId.ToString());
                /*int availableServicesCount = GetValueForDistribution(servicesDistribution, 1, Services.GroupsCount);
                // computing power
                nodeProperties.Add(GetValueForDistribution(computingPowerDistribution, 1, DefaultComputingPower));
                // parallel computing potential
                nodeProperties.Add(GetValueForDistribution(parallelComputingPotentialDistribution, 1, availableServicesCount));
                // liczba obsługiwanych węzłów
                nodeProperties.Add(availableServicesCount);*/
                var assignedServices = new List<int>();
                while (assignedServices.Count != ServicesPerNodeCount) 
                {
                    int serviceId;
                    if (servicesDistribution == Distribution.Random) 
                    {
                        serviceId = random.Next(Services.Count);
                    }
                    else 
                    {
                        serviceId = correlationList[serviceRank];
                        serviceRank++;
                    }

                    if (!assignedServices.Contains(serviceId) && IsServiceAvailable(serviceId))
                    {
                        assignedServices.Add(serviceId);
                        SetServiceUnavailability(serviceId);
                    }
                }
                foreach (var serviceId in assignedServices) 
                {
                    nodeProperties.Add(Services.GetCoord(serviceId));
                }
                outputList.Add(String.Join(" ", nodeProperties));
            }
            return outputList;
        }

        private void InitializeAvailableServicesList() 
        {
            servicesAvailability = new bool[Services.Count];
            serviceRank = 0;
            for (int i = 0; i < Services.Count; i++)
            {
                servicesAvailability[i] = true;
            }
        }

        private void SetServiceUnavailability(int serviceId)
        {
            servicesAvailability[serviceId] = false;
            var flag = false;
            for (int i = 0; i < Services.Count; i++) 
            {
                if (servicesAvailability[i]) 
                {
                    flag = true;
                    break;
                }
            }
            if (!flag) 
            {
                InitializeAvailableServicesList();
            }
        }

        private bool IsServiceAvailable(int serviceId) 
        {
            return servicesAvailability[serviceId];
        }
    }
}
