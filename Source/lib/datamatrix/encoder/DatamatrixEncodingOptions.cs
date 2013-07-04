/*
 * Copyright 2013 ZXing.Net authors
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
using ZXing.Datamatrix.Encoder;

namespace ZXing.Datamatrix
{
   /// <summary>
   /// The class holds the available options for the DatamatrixWriter
   /// </summary>
   [Serializable]
   public class DatamatrixEncodingOptions : EncodingOptions
   {
      /// <summary>
      /// Specifies the matrix shape for Data Matrix
      /// </summary>
      public SymbolShapeHint? SymbolShape
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE))
            {
               return (SymbolShapeHint)Hints[EncodeHintType.DATA_MATRIX_SHAPE];
            }
            return null;
         }
         set
         {
            if (value == null)
            {
               if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_SHAPE))
                  Hints.Remove(EncodeHintType.DATA_MATRIX_SHAPE);
            }
            else
            {
               Hints[EncodeHintType.DATA_MATRIX_SHAPE] = value;
            }
         }
      }
            
      /// <summary>
      /// Specifies a minimum barcode size
      /// </summary>
      public Dimension MinSize
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.MIN_SIZE))
            {
               return (Dimension)Hints[EncodeHintType.MIN_SIZE];
            }
            return null;
         }
         set
         {
            if (value == null)
            {
               if (Hints.ContainsKey(EncodeHintType.MIN_SIZE))
                  Hints.Remove(EncodeHintType.MIN_SIZE);
            }
            else
            {
               Hints[EncodeHintType.MIN_SIZE] = value;
            }
         }
      }

      /// <summary>
      /// Specifies a maximum barcode size
      /// </summary>
      public Dimension MaxSize
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.MAX_SIZE))
            {
               return (Dimension)Hints[EncodeHintType.MAX_SIZE];
            }
            return null;
         }
         set
         {
            if (value == null)
            {
               if (Hints.ContainsKey(EncodeHintType.MAX_SIZE))
                  Hints.Remove(EncodeHintType.MAX_SIZE);
            }
            else
            {
               Hints[EncodeHintType.MAX_SIZE] = value;
            }
         }
      }

      /// <summary>
      /// Specifies the default encodation
      /// Make sure that the content fits into the encodation value, otherwise there will be an exception thrown.
      /// standard value: Encodation.ASCII
      /// </summary>
      public int? DefaultEncodation
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION))
            {
               return (int)Hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION];
            }
            return null;
         }
         set
         {
            if (value == null)
            {
               if (Hints.ContainsKey(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION))
                  Hints.Remove(EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION);
            }
            else
            {
               Hints[EncodeHintType.DATA_MATRIX_DEFAULT_ENCODATION] = value;
            }
         }
      }
   }
}
