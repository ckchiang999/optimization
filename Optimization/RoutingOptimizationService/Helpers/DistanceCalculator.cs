namespace RoutingOptimizationService.Helpers
{
    public static class DistanceCalculator
    {
        /// <summary>
        /// Euclidean distance implemented as a callback. It uses an array of
        /// positions and computes the Euclidean distance between the two
        /// positions of two different indices.
        /// </summary>
        public static long[,] ComputeEuclideanDistanceMatrix(int[,] locations)
        {
            // Calculate the distance matrix using Euclidean distance.
            int locationNumber = locations.GetLength(0);
            long[,] distanceMatrix = new long[locationNumber, locationNumber];
            for (int fromNode = 0; fromNode < locationNumber; fromNode++)
            {
                for (int toNode = 0; toNode < locationNumber; toNode++)
                {
                    if (fromNode == toNode)
                    {
                        distanceMatrix[fromNode, toNode] = 0;
                    }
                    else
                    {
                        distanceMatrix[fromNode, toNode] =
                            (long)Math.Sqrt(
                                Math.Pow(locations[toNode, 0] - locations[fromNode, 0], 2) +
                                Math.Pow(locations[toNode, 1] - locations[fromNode, 1], 2));
                    }
                }
            }
            return distanceMatrix;
        }

        /// <summary>
        /// <see href="https://en.wikipedia.org/wiki/Taxicab_geometry">Manhattan distance</see> 
        /// implemented as a callback. It uses an array of positions and computes the distance between the two
        /// positions of two different indices using the formuat abs(x2 - x1) + abs(y2 - y1).
        /// </summary>
        public static long[,] ComputeMahattanDistanceMatrix(int[,] locations)
        {
            // Calculate the distance matrix using Euclidean distance.
            int locationNumber = locations.GetLength(0);
            long[,] distanceMatrix = new long[locationNumber, locationNumber];
            for (int fromNode = 0; fromNode < locationNumber; fromNode++)
            {
                for (int toNode = 0; toNode < locationNumber; toNode++)
                {
                    if (fromNode == toNode)
                    {
                        distanceMatrix[fromNode, toNode] = 0;
                    }
                    else
                    {
                        distanceMatrix[fromNode, toNode] =
                            Math.Abs(locations[toNode, 0] - locations[fromNode, 0]) +
                            Math.Abs(locations[toNode, 1] - locations[fromNode, 1]);
                    }
                }
            }
            return distanceMatrix;
        }

        /// <summary>
        /// Calculates the center of a set of locations.
        /// </summary>
        /// <param name="locations"></param>
        /// <returns></returns>
        public static (int x, int y) ComputeCentroid(int[,] locations)
        {
            var sumX = 0;
            var sumY = 0;
            var numLocations = locations.GetLength(0);
            for (int i = 0; i < numLocations; i++)
            {
                sumX += locations[i, 0];
                sumY += locations[i, 1];
            }
            return (sumX / numLocations, sumY / numLocations);
        }

        /// <summary>
        /// Convert a set of deliveries, with each delivery having a set of items, into
        /// a set of locations.
        /// </summary>
        /// <param name="deliveries"></param>
        /// <returns></returns>
        public static int[,] ComputeCentroidLocations(int[][,] deliveries)
        {
            var numDeliveries = deliveries.GetLength(0);
            var locations = new int[numDeliveries,2];
            for (int i = 0; i < numDeliveries; i++)
            {
                var centroid = ComputeCentroid(deliveries[i]);
                locations[i, 0] = centroid.x;
                locations[i, 1] = centroid.y;
            }
            return locations;
        }
    }
}
