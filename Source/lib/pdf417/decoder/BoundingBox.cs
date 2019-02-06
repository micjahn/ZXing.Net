/*
 * Copyright 2013 ZXing authors
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

using ZXing.Common;

namespace ZXing.PDF417.Internal
{
    /// <summary>
    /// A Bounding Box helper class
    /// </summary>
    /// <author>Guenther Grau</author>
    public sealed class BoundingBox
    {
        private readonly BitMatrix image;

        public ResultPoint TopLeft { get; private set; }
        public ResultPoint TopRight { get; private set; }
        public ResultPoint BottomLeft { get; private set; }
        public ResultPoint BottomRight { get; private set; }

        public int MinX { get; private set; }
        public int MaxX { get; private set; }
        public int MinY { get; private set; }
        public int MaxY { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZXing.PDF417.Internal.BoundingBox"/> class.
        /// returns null if the corner points don't match up correctly
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="topLeft">The top left.</param>
        /// <param name="bottomLeft">The bottom left.</param>
        /// <param name="topRight">The top right.</param>
        /// <param name="bottomRight">The bottom right.</param>
        /// <returns></returns>
        public static BoundingBox Create(BitMatrix image,
                                         ResultPoint topLeft,
                                         ResultPoint bottomLeft,
                                         ResultPoint topRight,
                                         ResultPoint bottomRight)
        {
            bool leftUnspecified = topLeft == null || bottomLeft == null;
            bool rightUnspecified = topRight == null || bottomRight == null;
            if (leftUnspecified && rightUnspecified)
            {
                return null;
            }

            return new BoundingBox(image, topLeft, bottomLeft, topRight, bottomRight);
        }

        /// <summary>
        /// Creates the specified box.
        /// </summary>
        /// <param name="box">The box.</param>
        /// <returns></returns>
        public static BoundingBox Create(BoundingBox box)
        {
            return new BoundingBox(box);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ZXing.PDF417.Internal.BoundingBox"/> class.
        /// Will throw an exception if the corner points don't match up correctly
        /// </summary>
        /// <param name="image">Image.</param>
        /// <param name="topLeft">Top left.</param>
        /// <param name="topRight">Top right.</param>
        /// <param name="bottomLeft">Bottom left.</param>
        /// <param name="bottomRight">Bottom right.</param>
        private BoundingBox(BitMatrix image,
                            ResultPoint topLeft,
                            ResultPoint bottomLeft,
                            ResultPoint topRight,
                            ResultPoint bottomRight)
        {
            if (topLeft == null || bottomLeft == null)
            {
                topLeft = new ResultPoint(0, topRight.Y);
                bottomLeft = new ResultPoint(0, bottomRight.Y);
            }
            else if (topRight == null || bottomRight == null)
            {
                topRight = new ResultPoint(image.Width - 1, topLeft.Y);
                bottomRight = new ResultPoint(image.Width - 1, topLeft.Y);
            }

            this.image = image;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;

            MinX = (int)Math.Min(TopLeft.X, BottomLeft.X);
            MaxX = (int)Math.Max(TopRight.X, BottomRight.X);
            MinY = (int)Math.Min(TopLeft.Y, TopRight.Y);
            MaxY = (int)Math.Max(BottomLeft.Y, BottomRight.Y);
        }

        private BoundingBox(BoundingBox boundingBox)
        {
            image = boundingBox.image;
            TopLeft = boundingBox.TopLeft;
            BottomLeft = boundingBox.BottomLeft;
            TopRight = boundingBox.TopRight;
            BottomRight = boundingBox.BottomRight;
            MinX = boundingBox.MinX;
            MaxX = boundingBox.MaxX;
            MinY = boundingBox.MinY;
            MaxY = boundingBox.MaxY;
        }

        /// <summary>
        /// Merge two Bounding Boxes, getting the left corners of left, and the right corners of right
        /// (Images should be the same)
        /// </summary>
        /// <param name="leftBox">Left.</param>
        /// <param name="rightBox">Right.</param>
        internal static BoundingBox merge(BoundingBox leftBox, BoundingBox rightBox)
        {
            if (leftBox == null)
                return rightBox;
            if (rightBox == null)
                return leftBox;
            return new BoundingBox(leftBox.image, leftBox.TopLeft, leftBox.BottomLeft, rightBox.TopRight, rightBox.BottomRight);
        }

        /// <summary>
        /// Adds the missing rows.
        /// </summary>
        /// <returns>The missing rows.</returns>
        /// <param name="missingStartRows">Missing start rows.</param>
        /// <param name="missingEndRows">Missing end rows.</param>
        /// <param name="isLeft">If set to <c>true</c> is left.</param>
        public BoundingBox addMissingRows(int missingStartRows, int missingEndRows, bool isLeft)
        {
            ResultPoint newTopLeft = TopLeft;
            ResultPoint newBottomLeft = BottomLeft;
            ResultPoint newTopRight = TopRight;
            ResultPoint newBottomRight = BottomRight;

            if (missingStartRows > 0)
            {
                ResultPoint top = isLeft ? TopLeft : TopRight;
                int newMinY = (int)top.Y - missingStartRows;
                if (newMinY < 0)
                {
                    newMinY = 0;
                }
                ResultPoint newTop = new ResultPoint(top.X, newMinY);
                if (isLeft)
                {
                    newTopLeft = newTop;
                }
                else
                {
                    newTopRight = newTop;
                }
            }

            if (missingEndRows > 0)
            {
                ResultPoint bottom = isLeft ? BottomLeft : BottomRight;
                int newMaxY = (int)bottom.Y + missingEndRows;
                if (newMaxY >= image.Height)
                {
                    newMaxY = image.Height - 1;
                }
                ResultPoint newBottom = new ResultPoint(bottom.X, newMaxY);
                if (isLeft)
                {
                    newBottomLeft = newBottom;
                }
                else
                {
                    newBottomRight = newBottom;
                }
            }

            return new BoundingBox(image, newTopLeft, newBottomLeft, newTopRight, newBottomRight);
        }
    }
}