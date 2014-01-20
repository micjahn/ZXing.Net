/*
 * Copyright 2014 ZXing.Net authors
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
using System.Linq;
using System.Windows;
using System.Windows.Media;

using ZXing.Common;

namespace ZXing.Rendering
{
   /// <summary>
   /// Renders a barcode into a geometry
   /// Autor: Rob Fonseca-Ensor
   /// </summary>
   public class GeometryRenderer : IBarcodeRenderer<Geometry>
   {
      /// <summary>
      /// Renders the specified matrix.
      /// </summary>
      /// <param name="matrix">The matrix.</param>
      /// <param name="format">The format.</param>
      /// <param name="content">The content.</param>
      /// <returns></returns>
      public Geometry Render(BitMatrix matrix, BarcodeFormat format, string content)
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
      public Geometry Render(BitMatrix matrix, BarcodeFormat format, string content, EncodingOptions options)
      {
         var edges = new HashSet<Edge>();
         var edgeMap = new Dictionary<Coordinate, List<Edge>>();
         var cols = matrix.Width;
         var rows = matrix.Height;

         for (int c = 0; c <= cols; c++)
         {
            for (int r = 0; r <= rows; r++)
            {
               var cell = GetCell(c, r, matrix);
               var westCell = GetCell(c - 1, r, matrix);
               var northCell = GetCell(c, r - 1, matrix);
               if (northCell != cell)
               {
                  AddEdge(new Edge(c, r, c + 1, r), edges, edgeMap);
               }

               if (westCell != cell)
               {
                  AddEdge(new Edge(c, r, c, r + 1), edges, edgeMap);
               }
            }
         }

         var cycles = new List<List<Coordinate>>();

         while (edges.Count > 0)
         {
            var edge = edges.First();
            RemoveEdge(edge, edges, edgeMap);

            if (IsEdgeLeftHand(matrix, edge))
            {
               edge = edge.Reversed();
            }

            var currentCycle = new List<Coordinate> {edge.From, edge.To};
            while (edge.To != currentCycle[0])
            {
               var moves = from direction in Turns(edge.From - edge.To)
                           let nextCoordinate = direction + edge.To
                           from e in EdgesFrom(edge.To, edgeMap)
                           where e.To == nextCoordinate || e.From == nextCoordinate
                           select e;

               var nextEdge = moves.First();
               RemoveEdge(nextEdge, edges, edgeMap);
               edge = nextEdge.To != edge.To ? nextEdge : nextEdge.Reversed();
               currentCycle.Add(edge.To);
            }
            cycles.Add(currentCycle);
         }

         return new PathGeometry(cycles.Select(x => new PathFigure(x.First().ToPoint(1), x.Skip(1).Select(y => new LineSegment(y.ToPoint(1), true)), true)));

      }

      private static bool IsEdgeLeftHand(BitMatrix b, Edge edge)
      {
         var cell = GetCell(edge.From.Col, edge.From.Row, b);
         return (edge.From.Row < edge.To.Row && cell) || (!(edge.From.Col < edge.To.Col && cell));
      }

      private static IEnumerable<Edge> EdgesFrom(Coordinate c, Dictionary<Coordinate, List<Edge>> edgeMap)
      {
         return edgeMap.ContainsKey(c) ? edgeMap[c] : Enumerable.Empty<Edge>();
      }

      private static IEnumerable<Coordinate> Turns(Coordinate currentDirection)
      {
         int index = Array.IndexOf(Coordinate.Directions, currentDirection);
         return Coordinate.Directions.Skip(index + 1).Concat(Coordinate.Directions.Take(index));
      }

      private static bool GetCell(int c, int r, BitMatrix matrix)
      {
         if (r < 0 || r >= matrix.Height)
         {
            return false;
         }
         if (c < 0 || c >= matrix.Width)
         {
            return false;
         }
         return matrix[c, r];
      }

      private static void RemoveEdge(Edge e, HashSet<Edge> edges, Dictionary<Coordinate, List<Edge>> edgeMap)
      {
         edges.Remove(e);
         edgeMap[e.From].Remove(e);
         edgeMap[e.To].Remove(e);
      }

      private static void AddEdge(Edge e, HashSet<Edge> edges, Dictionary<Coordinate, List<Edge>> edgeMap)
      {
         edges.Add(e);
         AddCoordinate(e.From, e, edgeMap);
         AddCoordinate(e.To, e, edgeMap);
      }

      private static void AddCoordinate(Coordinate c, Edge e, Dictionary<Coordinate, List<Edge>> edgeMap)
      {
         List<Edge> list;
         if (!edgeMap.TryGetValue(c, out list))
         {
            edgeMap[c] = list = new List<Edge>();
         }
         list.Add(e);
      }


      private struct Coordinate
      {
         public readonly int Row, Col;

         public Coordinate(int col, int row)
         {
            Col = col;
            Row = row;
         }

         public bool Equals(Coordinate other)
         {
            return other.Row == Row && other.Col == Col;
         }

         public override bool Equals(object obj)
         {
            if (ReferenceEquals(null, obj))
               return false;
            if (obj.GetType() != typeof (Coordinate))
               return false;
            return Equals((Coordinate) obj);
         }

         public override int GetHashCode()
         {
            unchecked
            {
               return (Row*397) ^ Col;
            }
         }

         public override string ToString()
         {
            var s = "";
            if (this == North)
               s = " n";
            if (this == West)
               s = " w";
            if (this == South)
               s = " s";
            if (this == East)
               s = " e";
            return String.Format("({0}, {1}{2})", Col, Row, s);
         }

         public static bool operator ==(Coordinate left, Coordinate right)
         {
            return left.Equals(right);
         }

         public static bool operator !=(Coordinate left, Coordinate right)
         {
            return !left.Equals(right);
         }

         public static Coordinate operator +(Coordinate c1, Coordinate c2)
         {
            return new Coordinate(c1.Col + c2.Col, c1.Row + c2.Row);
         }

         public static Coordinate operator -(Coordinate c1, Coordinate c2)
         {
            return new Coordinate(c1.Col - c2.Col, c1.Row - c2.Row);
         }

         public Point ToPoint(double scale)
         {
            return new Point(Col*scale, Row*scale);
         }

         private static readonly Coordinate West = new Coordinate(-1, 0);
         private static readonly Coordinate South = new Coordinate(0, 1);
         private static readonly Coordinate East = new Coordinate(1, 0);
         private static readonly Coordinate North = new Coordinate(0, -1);

         public static readonly Coordinate[] Directions = new[]
            {
               West,
               South,
               East,
               North,
            };
      }

      private struct Edge
      {
         public readonly Coordinate From, To;

         public Edge(Coordinate from, Coordinate to)
         {
            From = from;
            To = to;
         }

         public Edge(int fromCol, int fromRow, int toCol, int toRow)
            : this(new Coordinate(fromCol, fromRow), new Coordinate(toCol, toRow))
         {

         }


         public bool Equals(Edge other)
         {
            return other.From.Equals(From) && other.To.Equals(To);
         }

         public override bool Equals(object obj)
         {
            if (ReferenceEquals(null, obj))
               return false;
            if (obj.GetType() != typeof (Edge))
               return false;
            return Equals((Edge) obj);
         }

         public override int GetHashCode()
         {
            unchecked
            {
               return (From.GetHashCode()*397) ^ To.GetHashCode();
            }
         }

         public override string ToString()
         {
            char angle = ' ';
            if (From.Col == To.Col)
            {
               angle = '|';
            }
            if (From.Row == To.Row)
            {
               angle = '-';
            }


            return string.Format("[{0} {2} {1}]", From, To, angle);
         }

         public static bool operator ==(Edge left, Edge right)
         {
            return left.Equals(right);
         }

         public static bool operator !=(Edge left, Edge right)
         {
            return !left.Equals(right);
         }

         public Edge Reversed()
         {
            return new Edge(To, From);
         }
      }
   }
}