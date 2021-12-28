/*
 * Copyright 2013 ZXing.Net authors, 2020 andriks
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
using System.Drawing;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.Rendering
{
    /// <summary>
    /// An alternative <see cref="BitmapRenderer" /> that gives a better looking result for
    /// EAN-8, EAN-13, UPC-A and UPC-E barcodes <b>with</b> text.
    /// </summary>
    public class AlternateBitmapRenderer : BitmapRenderer
    {
        /// <summary>
        /// This renderer uses smaller font versions for UPC labels. This enum stores info for use of that.
        /// </summary>
        private enum FontVersion
        {
            /// <summary>
            /// Use small version of the font
            /// </summary>
            Small,
            /// <summary>
            /// Use normal version
            /// </summary>
            Normal,      
        }

        /// <summary>
        /// For each type of barcode this struct contains all the info on how to split the text content
        /// into blocks and where to print those blocks with respect to the start of the barcode, which
        /// is dynamicly determined while printing the bars.
        ///
        /// A 'real' barcode consists of an start-section, a number of bars of constant width (7 units for
        /// EAN and UPC codes), optionally a middle-section and another number of bars, and and end-section.
        /// Start-section, middle-section and end-section may have different widths.
        /// E.g. an EAN-13 baecode has a 3 units wide start, 6 bars of 7 wide, a 5 wide middle-section
        /// anothers 6 bars of 7 wide and a 3 wide end-section for a total of 95 units.
        /// </summary>
        private struct PrintInfo
        {
            /// <summary>Required length of text</summary>
            public int TextLength;
            /// <summary>Total width of barcode in units</summary>
            public int Units;
            /// <summary>Start of a text block in contents</summary>
            public int[] TextIndex;
            /// <summary>Which font to use for block</summary>
            public FontVersion[] Version;
            /// <summary>Index in barcode units for printing. If -1, align before barcode</summary>
            public int[] PrintIndex;
            /// <summary>Width of block to clear in barcode units</summary>
            public int[] PrintWidth;
        }

        private static readonly Font DefaultTextFont;

        private PrintInfo EAN8_INFO = new PrintInfo
        {
            TextLength = 8,
            Units = 67,
            TextIndex = new int[] { 0, 4 },
            Version = new FontVersion[] { FontVersion.Normal, FontVersion.Normal },
            PrintIndex = new int[] {  3, 36 },
            PrintWidth = new int[] { 28, 28 }
        };
        private PrintInfo EAN13_INFO = new PrintInfo
        {
            TextLength = 13,
            Units = 95,
            TextIndex = new int[] { 0, 1, 7 },
            Version = new FontVersion[] { FontVersion.Normal, FontVersion.Normal, FontVersion.Normal },
            PrintIndex = new int[] { -1, 3, 50 },
            PrintWidth = new int[] { -1, 42, 42 }
        };
        private PrintInfo UPCA_INFO = new PrintInfo
        {
            TextLength = 12,
            Units = 95,
            TextIndex = new int[] { 0, 1, 6, 11 },
            Version = new FontVersion[] { FontVersion.Small, FontVersion.Normal, FontVersion.Normal, FontVersion.Small },
            PrintIndex = new int[] { -1,  3, 50, 95 },
            PrintWidth = new int[] { -1, 42, 42, -1 }
        };
        private PrintInfo UPCE_INFO = new PrintInfo
        {
            TextLength = 8,
            Units = 51,
            TextIndex = new int[] { 0, 1, 7 },
            Version = new FontVersion[] { FontVersion.Small, FontVersion.Normal, FontVersion.Small },
            PrintIndex = new int[] { -1,  3, 51 },
            PrintWidth = new int[] { -1, 42, -1 }
        };

        /// <summary>
        /// Static constructor, sets defaults
        /// </summary>
        static AlternateBitmapRenderer()
        {
            try
            {
                DefaultTextFont = new Font("Courier New", 14, FontStyle.Regular);
            }
            catch (Exception exc)
            {
                // have to ignore, no better idea
                System.Diagnostics.Trace.TraceError("default text font (Courier New, 14, regular) couldn't be loaded: {0}", exc.Message);
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="AlternateBitmapRenderer"/> class.
        /// </summary>
        public AlternateBitmapRenderer()
        {
            TextFont = DefaultTextFont;
        }

        /// <summary>
        /// Overrides the drawing methods for EAN-8, EAN-13, UPC-A and UPC-E barcodes.
        /// For other types it chains to the original renderer.
        /// </summary>
        /// <param name="matrix">The matrix with the pre-rendered data.</param>
        /// <param name="format">The <see cref="BarcodeFormat" />.</param>
        /// <param name="content">The textual description of the code.</param>
        /// <param name="options">The options for rendering.</param>
        /// <returns>A Windows Bitmap containing the barcode.</returns>
        public override Bitmap Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
        {
            if (!(format == BarcodeFormat.EAN_13 ||
                  format == BarcodeFormat.EAN_8 ||
                  format == BarcodeFormat.UPC_A ||
                  format == BarcodeFormat.UPC_E))
                return base.Render(matrix, format, content, options);
            else
            {
                Brush backgroundBrush = new SolidBrush(Background);
                Brush foregroundBrush = new SolidBrush(Foreground);

                int width = matrix.Width;
                int height = matrix.Height;
                int barH;                             // corrected height of each bar
                const int y = 0;                  // row to use for drawing matrix
                int start = Int32.MaxValue;     // first black stripe
                int end = Int32.MinValue;     // last black stripe

                Font font = TextFont ?? DefaultTextFont;
                bool outputContent = font != null &&
                                            (options == null || !options.PureBarcode) &&
                                            !String.IsNullOrEmpty(content);
                if (options != null)
                {
                    if (options.Width > width)
                    {
                        width = options.Width;
                    }
                    if (options.Height > height)
                    {
                        height = options.Height;
                    }
                }

                var bmp = new Bitmap(width, height);
                var gg = Graphics.FromImage(bmp);

                gg.Clear(Background);
                SizeF sizeF = gg.MeasureString("8", font, 100);
                int textHeight = (int)Math.Ceiling(sizeF.Height);
                int textTop = height - textHeight;

                if (outputContent)
                    barH = height - (int)(0.5f * textHeight);
                else
                    barH = height;

                for (int x = 0; x < matrix.Width - 1; x++)
                {

                    if (matrix[x, y])
                    {
                        if (x < start) start = x;
                        if (x > end) end = x;
                        gg.FillRectangle(foregroundBrush, x, y, 1, barH);
                    }
                    else
                    {
                        gg.FillRectangle(backgroundBrush, x, y, 1, barH);
                    }
                }
                if (outputContent)
                {
                    switch (format)
                    {
                        case BarcodeFormat.EAN_8:
                            printTexts(gg, content, EAN8_INFO, font, start, end, textTop, textHeight);
                            break;
                        case BarcodeFormat.EAN_13:
                            printTexts(gg, content, EAN13_INFO, font, start, end, textTop, textHeight);
                            break;
                        case BarcodeFormat.UPC_A:
                            printTexts(gg, content, UPCA_INFO, font, start, end, textTop, textHeight);
                            break;
                        case BarcodeFormat.UPC_E:
                            printTexts(gg, content, UPCE_INFO, font, start, end, textTop, textHeight);
                            break;
                    }
                }
                return bmp;
            }
        }


        /// <summary>
        /// Draws the text part of the barcode bitmap.
        /// </summary>
        /// <param name="canvas">The grapics context to draw on.</param>
        /// <param name="content">Barcode text</param>
        /// <param name="info">Print specifications</param>
        /// <param name="font">The selected font</param>
        /// <param name="start">Position of first a bar in pixels</param>
        /// <param name="end">Last position of a bar in pixels</param>
        /// <param name="top">Top of text in pixels</param>
        /// <param name="height">Height of text in pixels</param>
        /// <returns>Success</returns>
        /// <throws><see cref="System.ArgumentException" /> when no space for required text.</throws>
        private bool printTexts(Graphics canvas,
                                string content,
                                PrintInfo info,
                                Font font,
                                int start,
                                int end,
                                int top,
                                int height)
        {
            const string ERR_SMALL = "Bitmap dimension too small for text. Reduce fontsize or enlarge bitmap.";

            // positioning of text
            int         textLeft, textWidth;
            int         pixPerUnit;
            SizeF       sizeF;
            RectangleF  rectF;
            Font        fontS = new Font(font.FontFamily, 0.8f * font.Size);

            Brush       backgroundBrush = new SolidBrush(Background);
            Brush       foregroundBrush = new SolidBrush(Foreground);

            StringFormat sFmt = new StringFormat();

            sFmt.Alignment = StringAlignment.Center;
            pixPerUnit = (end - start + 1) / info.Units;
            
            string[] blocks = contentGroups(content, info);

            for (int index = 0; index < info.TextIndex.Length; index++)
            {
                Font useFont;
                if (info.Version[index] == FontVersion.Normal)
                {
                    useFont = font;
                    sFmt.LineAlignment = StringAlignment.Center;
                }
                else
                {
                    useFont = fontS;
                    sFmt.LineAlignment = StringAlignment.Near;
                }
                sizeF = canvas.MeasureString(blocks[index], useFont, 10000);
                if (info.PrintIndex[index] == -1)
                {
                    textWidth = (int)Math.Ceiling(sizeF.Width);
                    textLeft  = start - textWidth;
                    if (textLeft < 0) throw new ArgumentException(ERR_SMALL);
                }
                else
                {
                    if (info.PrintWidth[index] < 1)
                        textWidth = (int)Math.Ceiling(sizeF.Width);
                    else
                        textWidth = info.PrintWidth[index] * pixPerUnit;
                    textLeft  = start + info.PrintIndex[index] * pixPerUnit;
                    if (textWidth < sizeF.Width) throw new ArgumentException(ERR_SMALL);
                }
                rectF = new RectangleF(textLeft, top, textWidth, height);
                canvas.FillRectangle(backgroundBrush, rectF);
                canvas.DrawString(blocks[index], useFont, foregroundBrush, rectF, sFmt);
                canvas.DrawString(blocks[index], useFont, foregroundBrush, rectF, sFmt);
            }
            return true;
        }

        /// <summary>
        /// Split content in seperate groups for pretty printing
        /// </summary>
        /// <param name="content">The content string</param>
        /// <param name="info">The printing info</param>
        /// <returns>One or more content group strings</returns>
        private string[] contentGroups(string content, PrintInfo info)
        {
            const string ERR_LEN = "Requested contents should be {0} digits long, but got {1} !";

            if (content.Length != info.TextLength)
                content = OneDimensionalCodeWriter.CalculateChecksumDigitModulo10(content);
            if (content.Length != info.TextLength)
                throw new ArgumentException(
                    String.Format(ERR_LEN, info.TextLength, content.Length));

            string[] groups = new string[info.TextIndex.Length];

            for (int index = 0; index < info.TextIndex.Length; index++)
            {
                if (index == (info.TextIndex.Length - 1))
                    groups[index] = content.Substring(info.TextIndex[index]);
                else
                    groups[index] = content.Substring(info.TextIndex[index],
                                                      info.TextIndex[index+1] - info.TextIndex[index]);
            }
            return groups;
        }
    }
}