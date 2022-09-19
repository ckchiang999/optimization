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
    }
}
