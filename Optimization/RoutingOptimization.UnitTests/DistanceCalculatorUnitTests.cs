using RoutingOptimizationService.Helpers;

namespace RoutingOptimization.UnitTests
{
    public class DistanceCalculatorUnitTests
    {

        public DistanceCalculatorUnitTests()
        {

        }

        [Fact]
        public void ComputeCentroid()
        {
            var locations = new int[,] { { 30, 30 }, { 40, 40 }, { 50, 50 }, { 60, 60 }, { 60, 60 } };
            var centroid = DistanceCalculator.ComputeCentroid(locations);

            Assert.Equal((30 + 40 + 50 + 60 + 60) / 5, centroid.x);
            Assert.Equal((30 + 40 + 50 + 60 + 60) / 5, centroid.y);
        }

        [Fact]
        public void ComputeCentroidLocations()
        {
            int[][,] deliveries = new int[][,]
            {
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

            int[,] locations = DistanceCalculator.ComputeCentroidLocations(deliveries);

            Assert.Equal(10, locations.GetLength(0));
            Assert.Equal((10 + 10 + 20 + 20 + 30) / 5, locations[0, 0]);
            Assert.Equal((10 + 10 + 20 + 20 + 30) / 5, locations[0, 1]);
            Assert.Equal((60 + 70) / 2, locations[9, 0]);
            Assert.Equal((60 + 70) / 2, locations[9, 1]);
        }
    }
}