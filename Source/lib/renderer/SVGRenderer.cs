/*
 * Copyright 2012 ZXing.Net authors
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
using System.Collections.Generic;
using System.Text;
using System.IO;

#if NETFX_CORE
using Windows.UI;
#elif SILVERLIGHT
using System.Windows.Media;
#elif UNITY
using UnityEngine;
#elif !(PORTABLE || NETSTANDARD)
using System.Drawing;
#endif

using ZXing.Common;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a barcode into a Svg image
   /// </summary>
   public class SvgRenderer : IBarcodeRenderer<SvgRenderer.SvgImage>
   {
#if !UNITY
#if (PORTABLE || NETSTANDARD)
      /// <summary>
      /// represents a color value
      /// </summary>
      public struct Color
      {
         /// <summary>
         /// color black
         /// </summary>
         public static Color Black = new Color(255, 0, 0, 0);
         /// <summary>
         /// color white
         /// </summary>
         public static Color White = new Color(255, 255, 255, 255);

         /// <summary>
         /// alpha channel
         /// </summary>
         public byte A;
         /// <summary>
         /// red channel
         /// </summary>
         public byte R;
         /// <summary>
         /// green channel
         /// </summary>
         public byte G;
         /// <summary>
         /// blur channel
         /// </summary>
         public byte B;

         /// <summary>
         /// initializing constructor
         /// </summary>
         public Color(int color)
         {
            A = (byte)((color & 0xFF000000) >> 24);
            R = (byte)((color & 0x00FF0000) >> 16);
            G = (byte)((color & 0x0000FF00) >> 8);
            B = (byte)((color & 0x000000FF));
         }

         /// <summary>
         /// initializing constructor
         /// </summary>
         public Color(byte alpha, byte red, byte green, byte blue)
         {
            A = alpha;
            R = red;
            G = green;
            B = blue;
         }
      }
#endif
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      public Color Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      public Color Background { get; set; }
#else
      /// <summary>
      /// Gets or sets the foreground color.
      /// </summary>
      /// <value>The foreground color.</value>
      [System.CLSCompliant(false)]
      public Color32 Foreground { get; set; }

      /// <summary>
      /// Gets or sets the background color.
      /// </summary>
      /// <value>The background color.</value>
      [System.CLSCompliant(false)]
      public Color32 Background { get; set; }
#endif

      /// <summary>
      /// Initializes a new instance of the <see cref="SvgRenderer"/> class.
      /// </summary>
      public SvgRenderer()
      {
#if NETFX_CORE || SILVERLIGHT
         Foreground = Colors.Black;
         Background = Colors.White;
#elif UNITY
         Foreground = new Color32(0, 0, 0, 255);
         Background = new Color32(255, 255, 255, 255);
#else
         Foreground = Color.Black;
         Background = Color.White;
#endif
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content)
      {
         return Render(matrix, format, content, null);
      }

      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <param name="options">The options.</param>
      /// <returns></returns>
      public SvgImage Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var result = new SvgImage(matrix.Width, matrix.Height);

         Create(result, matrix, format, content, options);

         return result;
      }

      private void Create(SvgImage image, BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         const int quietZone = 5;

         if (matrix == null)
            return;

         int width = matrix.Width;
         int height = matrix.Height;
         image.AddHeader();
         image.AddTag(0, 0, 2 * quietZone + width, 2 * quietZone + height, Background, Foreground);
         AppendDarkCell(image, matrix, quietZone, quietZone);
         image.AddEnd();
      }

      private static void AppendDarkCell(SvgImage image, BitMatrix matrix, int offsetX, int offSetY)
      {
         if (matrix == null)
            return;

         int width = matrix.Width;
         int height = matrix.Height;
         var processed = new BitMatrix(width, height);
         bool currentIsBlack = false;
         int startPosX = 0;
         int startPosY = 0;
         for (int x = 0; x < width; x++)
         {
            int endPosX;
            for (int y = 0; y < height; y++)
            {
               if (processed[x, y])
                  continue;

               processed[x, y] = true;

               if (matrix[x, y])
               {
                  if (!currentIsBlack)
                  {
                     startPosX = x;
                     startPosY = y;
                     currentIsBlack = true;
                  }
               }
               else
               {
                  if (currentIsBlack)
                  {
                     FindMaximumRectangle(matrix, processed, startPosX, startPosY, y, out endPosX);
                     image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, y - startPosY);
                     currentIsBlack = false;
                  }
               }
            }
            if (currentIsBlack)
            {
               FindMaximumRectangle(matrix, processed, startPosX, startPosY, height, out endPosX);
               image.AddRec(startPosX + offsetX, startPosY + offSetY, endPosX - startPosX + 1, height - startPosY);
               currentIsBlack = false;
            }
         }
      }

      private static void FindMaximumRectangle(BitMatrix matrix, BitMatrix processed, int startPosX, int startPosY, int endPosY, out int endPosX)
      {
         endPosX = startPosX;

         for (int x = startPosX + 1; x < matrix.Width; x++)
         {
            for (int y = startPosY; y < endPosY; y++)
            {
               if (!matrix[x, y])
               {
                  return;
               }
            }
            endPosX = x;
            for (int y = startPosY; y < endPosY; y++)
            {
               processed[x, y] = true;
            }
         }
      }

      /// <summary>
      /// Represents a barcode as a Svg image
      /// </summary>
      public class SvgImage
      {
         private readonly StringBuilder content;

         /// <summary>
         /// Gets or sets the content.
         /// </summary>
         /// <value>
         /// The content.
         /// </value>
         public String Content
         {
            get { return content.ToString(); }
            set { content.Length = 0; if (value != null) content.Append(value); }
         }

         /// <summary>
         /// The original height of the bitmatrix for the barcode
         /// </summary>
         public int Height { get; set; }

         /// <summary>
         /// The original width of the bitmatrix for the barcode
         /// </summary>
         public int Width { get; set; }

         /// <summary>
         /// Initializes a new instance of the <see cref="SvgImage"/> class.
         /// </summary>
         public SvgImage()
         {
            content = new StringBuilder();
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="SvgImage"/> class.
         /// </summary>
         public SvgImage(int width, int height)
         {
            content = new StringBuilder();
            Width = width;
            Height = height;
         }

         /// <summary>
         /// Initializes a new instance of the <see cref="SvgImage"/> class.
         /// </summary>
         /// <param name="content">The content.</param>
         public SvgImage(string content)
         {
            this.content = new StringBuilder(content);
         }

         /// <summary>
         /// Gives the XML representation of the SVG image
         /// </summary>
         public override string ToString()
         {
            return content.ToString();
         }

         internal void AddHeader()
         {
            content.Append("<?xml version=\"1.0\" standalone=\"no\"?>");
            content.Append(@"<!-- Created with ZXing.Net (http://zxingnet.codeplex.com/) -->");
            content.Append("<!DOCTYPE svg PUBLIC \"-//W3C//DTD SVG 1.1//EN\" \"http://www.w3.org/Graphics/SVG/1.1/DTD/svg11.dtd\">");
         }

         internal void AddEnd()
         {
            content.Append("</svg>");
         }

         internal void AddTag(int displaysizeX, int displaysizeY, int viewboxSizeX, int viewboxSizeY, Color background, Color fill)
         {

            if (displaysizeX <= 0 || displaysizeY <= 0)
               content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" shape-rendering=\"crispEdges\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6}>",
                   viewboxSizeX,
                   viewboxSizeY,
                   GetColorRgb(background),
                   ConvertAlpha(background),
                   GetColorRgb(fill),
                   ConvertAlpha(fill),
                   GetBackgroundStyle(background)
                   ));
            else
               content.Append(string.Format("<svg xmlns=\"http://www.w3.org/2000/svg\" version=\"1.2\" baseProfile=\"tiny\" shape-rendering=\"crispEdges\" viewBox=\"0 0 {0} {1}\" viewport-fill=\"rgb({2})\" viewport-fill-opacity=\"{3}\" fill=\"rgb({4})\" fill-opacity=\"{5}\" {6} width=\"{7}\" height=\"{8}\">",
                   viewboxSizeX,
                   viewboxSizeY,
                   GetColorRgb(background),
                   ConvertAlpha(background),
                   GetColorRgb(fill),
                   ConvertAlpha(fill),
                   GetBackgroundStyle(background),
                   displaysizeX,
                   displaysizeY));
         }

         internal void AddRec(int posX, int posY, int width, int height)
         {
            content.AppendFormat(System.Globalization.CultureInfo.InvariantCulture, "<rect x=\"{0}\" y=\"{1}\" width=\"{2}\" height=\"{3}\"/>", posX, posY, width, height);
         }

#if !UNITY
         internal static double ConvertAlpha(Color alpha)
         {
            return Math.Round((((double)alpha.A) / (double)255), 2);
         }

         internal static string GetBackgroundStyle(Color color)
         {
            double alpha = ConvertAlpha(color);
            return string.Format("style=\"background-color:rgb({0},{1},{2});background-color:rgba({3});\"",
                color.R, color.G, color.B, alpha);
         }

         internal static string GetColorRgb(Color color)
         {
            return color.R + "," + color.G + "," + color.B;
         }

         internal static string GetColorRgba(Color color)
         {
            double alpha = ConvertAlpha(color);
            return color.R + "," + color.G + "," + color.B + "," + alpha;
         }
#else
         internal static double ConvertAlpha(Color32 alpha)
         {
            return Math.Round((((double)alpha.a) / (double)255), 2);
         }

         internal static string GetBackgroundStyle(Color32 color)
         {
            double alpha = ConvertAlpha(color);
            return string.Format("style=\"background-color:rgb({0},{1},{2});background-color:rgba({3});\"",
                color.r, color.g, color.b, alpha);
         }

         internal static string GetColorRgb(Color32 color)
         {
            return color.r + "," + color.g + "," + color.b;
         }

         internal static string GetColorRgba(Color32 color)
         {
            double alpha = ConvertAlpha(color);
            return color.r + "," + color.g + "," + color.b + "," + alpha;
         }
#endif
      }
   }
}