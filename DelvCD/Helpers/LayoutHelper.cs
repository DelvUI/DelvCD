using System.Collections.Generic;

namespace DelvCD.Helpers
{
    public static class LayoutHelper
    {
        private static List<GrowthDirections> DirectionOptionsValues = new List<GrowthDirections>
        {
            GrowthDirections.Right | GrowthDirections.Down,
            GrowthDirections.Right | GrowthDirections.Up,
            GrowthDirections.Left | GrowthDirections.Down,
            GrowthDirections.Left | GrowthDirections.Up,
            GrowthDirections.Centered | GrowthDirections.Up,
            GrowthDirections.Centered | GrowthDirections.Down,
            GrowthDirections.Centered | GrowthDirections.Left,
            GrowthDirections.Centered | GrowthDirections.Right
        };

        public static GrowthDirections GrowthDirectionsFromIndex(int index)
        {
            if (index > 0 && index < DirectionOptionsValues.Count)
            {
                return DirectionOptionsValues[index];
            }

            return DirectionOptionsValues[0];
        }
    }
}