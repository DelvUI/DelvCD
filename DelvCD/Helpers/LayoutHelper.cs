using System.Collections.Generic;
using System.Numerics;
using System;

namespace DelvCD.Helpers
{
    public static class LayoutHelper
    {
        private static List<GrowthDirections> DirectionOptionsValues = new List<GrowthDirections>()
        {
            GrowthDirections.Right | GrowthDirections.Down, // grow x axis right, y axis down
            GrowthDirections.Right | GrowthDirections.Up, // grow x axis right, y axis up
            GrowthDirections.Left | GrowthDirections.Down, // grow x axis left, y axis down
            GrowthDirections.Left | GrowthDirections.Up, // grow x axis left, y axis up
            GrowthDirections.Centered | GrowthDirections.Up, // center x axis, y axis -> grow up
            GrowthDirections.Centered | GrowthDirections.Down, // center x axis, y axis -> grow down
            GrowthDirections.Centered | GrowthDirections.Left, // center y axis, x axis -> grow to left
            GrowthDirections.Centered | GrowthDirections.Right // center y axis, x axis -> grow to right
        };
        public static GrowthDirections GrowthDirectionsFromIndex(int index)
        {
            if (index > 0 && index < DirectionOptionsValues.Count)
            {
                return DirectionOptionsValues[index];
            }

            return DirectionOptionsValues[0];
        }

        public static Vector2 CalculateAxisDirections(
            GrowthDirections growthDirections,
            int row,
            int col)
        {
            Vector2 direction;
            if ((growthDirections & GrowthDirections.Centered) != 0)
            {
                // if axis is centered -> even elements right/down | odd elements left/up
                if ((growthDirections & GrowthDirections.Up) != 0 || (growthDirections & GrowthDirections.Down) != 0)
                {
                    direction.X = col % 2 == 0 ? 1 : -1;
                    direction.Y = (growthDirections & GrowthDirections.Down) != 0 ? 1 : -1;
                }
                else
                {
                    direction.X = (growthDirections & GrowthDirections.Right) != 0 ? 1 : -1;
                    direction.Y = row % 2 == 0 ? 1 : -1;
                }
            }
            else
            {
                direction.X = (growthDirections & GrowthDirections.Right) != 0 ? 1 : -1;
                direction.Y = (growthDirections & GrowthDirections.Down) != 0 ? 1 : -1;
            }
            return direction;
        }

        public static Vector2 GetOffsetMultiplier(GrowthDirections directions, int row, int col)
        {
            // determine if y or x axis is getting centered and scale by .5 for that axis
            if ((directions & GrowthDirections.Centered) != 0)
            {
                if ((directions & GrowthDirections.Up) != 0 || (directions & GrowthDirections.Down) != 0)
                {
                    return new Vector2((float)Math.Ceiling(col / 2f), row);
                }
                else
                {
                    return new Vector2(col, (float)Math.Ceiling(row / 2f));
                }
            }
            return new Vector2(col, row);
        }

        public static Vector2 CalculateElementPosition(
            GrowthDirections directions,
            int maxPerRow,
            int index,
            Vector2 startingPos,
            Vector2 offset)
        {
            List<Vector2> list = new List<Vector2>();
            int actualMaxPerRow = maxPerRow > 0 ? maxPerRow : 1;
            int row = index / actualMaxPerRow;
            int col = index % actualMaxPerRow;

            Vector2 direction = CalculateAxisDirections(
                directions,
                row,
                col
            );

            Vector2 offsetMultiplier = GetOffsetMultiplier(directions, row, col);

            return new Vector2(
                startingPos.X + offsetMultiplier.X * direction.X * offset.X,
                startingPos.Y + offsetMultiplier.Y * direction.Y * offset.Y
            );
        }
    }
}
