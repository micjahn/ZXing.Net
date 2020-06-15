using System;

namespace ZXing
{
    /// <summary>
    /// Representation of restricted scanning area in PERCENTAGE. 
    /// Allowed values: 0 <= value <= 1 && startY != endY
    /// Values of startY and endY are ABSOLUTE to image that means if use values of
    /// startY:0.49 and endY:0.51 we will scan only 2% of the whole image
    /// starting at 49% and finishing at 51% of the image height.
    /// </summary>
    public class RestrictedScanningArea
    {
        public readonly float StartY;
        public readonly float EndY;

        public RestrictedScanningArea(float startY, float endY)
        {
            //if difference between parameters is less than 1% we assume those are equal
            if (Math.Abs(startY - endY) < 0.01f)
            {
                throw new ArgumentException($"Values of {nameof(startY)} and {nameof(endY)} cannot be the same");
            }

            //Reverse values instead of throwing argument exception
            if (startY > endY)
            {
                var temp = endY;
                endY = startY;
                startY = temp;
            }

            if (startY < 0)
            {
                startY = 0;
            }

            if (endY > 1)
            {
                endY = 1;
            }

            StartY = startY;
            EndY = endY;
        }
    }
}
