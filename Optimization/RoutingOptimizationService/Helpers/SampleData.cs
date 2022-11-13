namespace RoutingOptimizationService.Helpers
{
    public class SampleData
    {
        // Define the collection of deliveries and location of items
        public int[][,] GetRandomDeliveries(int minDeliveryCount = 10, int maxDeliveryCount = 100, int minItemCount = 1, int maxItemCount = 5)
        {
            var randomGenerator = new Random();
            int[,] itemLocations = new int[,]
            {
                { 10, 10 },
                { 20, 20 },
                { 30, 30 },
                { 40, 40 },
                { 50, 50 },
                { 60, 60 },
                { 70, 70 },
                { 80, 80 }
            };
            var deliveryCount = randomGenerator.Next(minDeliveryCount, maxDeliveryCount);
            int[][,] deliveries = new int[deliveryCount][,];
            for (int i = 0; i < deliveryCount; i++)
            {
                if (i == 0)
                {
                    // starting location
                    deliveries[i] = new int[1,2];
                    deliveries[i][0, 0] = 0;
                    deliveries[i][0, 1] = 1;
                    continue;
                }
                var itemCount = randomGenerator.Next(minItemCount, maxItemCount);
                deliveries[i] = new int[itemCount,2];
                // items
                for (int j = 0; j < itemCount; j++)
                {
                    int itemLocation = randomGenerator.Next(0, 7);
                    deliveries[i][j, 0] = itemLocations[itemLocation, 0];
                    deliveries[i][j, 1] = itemLocations[itemLocation, 1];
                }
            };

            return deliveries;
        }

        /// <summary>
        /// Data for simple case with 5 slots per cart
        /// </summary>
        /// <returns></returns>
        public int[][,] GetDemoDeliveriesFiveSlotsPerCart()
        {
            int[][,] deliveries = new int[][,]
            {
                new int[,] { { 0, 0 } },                                                   // start location
                new int[,] { { 10, 10 }, { 10, 10 }, { 20, 20 }, { 20, 20 }, { 30, 30 } }, // delivery 1
                new int[,] { { 10, 10 }, { 40, 40 }, { 50, 50 }, { 60, 60 }, { 60, 60 } }, // delivery 2
                new int[,] { { 20, 20 }, { 30, 30 }, { 40, 40 }, { 50, 50 } },             // delivery 3
                new int[,] { { 10, 10 }, { 20, 20 } },                                     // delivery 4
                new int[,] { { 30, 30 }, { 40, 40 }, { 50, 50 }, { 60, 60 }, { 60, 60 } }, // delivery 5
                new int[,] { { 10, 10 }, { 20, 20 }, { 20, 20 }, { 30, 30 } },             // delivery 6
                new int[,] { { 30, 30 }, { 50, 50 }, { 60, 60 }, { 70, 70 }, { 80, 80 } }, // delivery 7
                new int[,] { { 50, 50 }, { 50, 50 }, { 60, 60 }, { 70, 70 } },             // delivery 8
                new int[,] { { 10, 10 }, { 60, 60 }, { 70, 70 }, { 80, 80 } },             // delivery 9
                new int[,] { { 60, 60 }, { 70, 70 } },                                     // delivery 10
            };

            return deliveries;
        }

        /// <summary>
        /// Data for simple case with 2 slots per cart
        /// Expect pairing of delivery 1 and 2, 3 and 6, 4 and 10, 5 and 7, 8 and 9 because they should have identity routes.
        /// </summary>
        /// <returns></returns>
        public int[][,] GetDemoDeliveriesTwoSlotsPerCart()
        {
            int[][,] deliveries = new int[][,]
            {
                new int[,] { { 0, 0 } },                                                   // start location
                new int[,] { { 10, 10 }, { 10, 10 }, { 20, 20 }, { 20, 20 }, { 30, 30 } }, // delivery 1
                new int[,] { { 10, 10 }, { 10, 10 }, { 20, 20 }, { 20, 20 }, { 30, 30 } }, // delivery 2
                new int[,] { { 20, 20 }, { 30, 30 }, { 40, 40 }, { 50, 50 } },             // delivery 3
                new int[,] { { 60, 60 }, { 70, 70 } },                                     // delivery 4
                new int[,] { { 30, 30 }, { 40, 40 }, { 50, 50 }, { 60, 60 }, { 60, 60 } }, // delivery 5
                new int[,] { { 20, 20 }, { 30, 30 }, { 40, 40 }, { 50, 50 } },             // delivery 6
                new int[,] { { 30, 30 }, { 40, 40 }, { 50, 50 }, { 60, 60 }, { 60, 60 } }, // delivery 7
                new int[,] { { 50, 50 }, { 50, 50 }, { 60, 60 }, { 70, 70 } },             // delivery 8
                new int[,] { { 50, 50 }, { 50, 50 }, { 60, 60 }, { 70, 70 } },             // delivery 9
                new int[,] { { 60, 60 }, { 70, 70 } },                                     // delivery 10
            };

            return deliveries;
        }
    }
}
