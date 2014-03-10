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

using ZXing.Common;
using ZXing.PDF417.Internal;

namespace ZXing.PDF417
{
   /// <summary>
   /// The class holds the available options for the <see cref="PDF417Writer" />
   /// </summary>
   [Serializable]
   public class PDF417EncodingOptions : EncodingOptions
   {
      /// <summary>
      /// Specifies whether to use compact mode for PDF417 (type <see cref="bool" />).
      /// </summary>
      public bool Compact
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PDF417_COMPACT))
            {
               return (bool) Hints[EncodeHintType.PDF417_COMPACT];
            }
            return false;
         }
         set { Hints[EncodeHintType.PDF417_COMPACT] = value; }
      }

      /// <summary>
      /// Specifies what compaction mode to use for PDF417 (type
      /// <see cref="Compaction" />).
      /// </summary>
      public Compaction Compaction
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PDF417_COMPACTION))
            {
               return (Compaction) Hints[EncodeHintType.PDF417_COMPACTION];
            }
            return Compaction.AUTO;
         }
         set { Hints[EncodeHintType.PDF417_COMPACTION] = value; }
      }

      /// <summary>
      /// Specifies the minimum and maximum number of rows and columns for PDF417 (type
      /// <see cref="Dimensions" />).
      /// </summary>
      public Dimensions Dimensions
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.PDF417_DIMENSIONS))
            {
               return (Dimensions) Hints[EncodeHintType.PDF417_DIMENSIONS];
            }
            return null;
         }
         set { Hints[EncodeHintType.PDF417_DIMENSIONS] = value; }
      }

      /// <summary>
      /// Specifies what degree of error correction to use
      /// </summary>
      public PDF417ErrorCorrectionLevel ErrorCorrection
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.ERROR_CORRECTION))
            {
               var value = Hints[EncodeHintType.ERROR_CORRECTION];
               if (value is PDF417ErrorCorrectionLevel)
               {
                  return (PDF417ErrorCorrectionLevel)value;
               }
               if (value is int)
               {
                  return (PDF417ErrorCorrectionLevel)Enum.Parse(typeof(PDF417ErrorCorrectionLevel), value.ToString(), true);
               }
            }
            return PDF417ErrorCorrectionLevel.L2;
         }
         set { Hints[EncodeHintType.ERROR_CORRECTION] = value; }
      }

      /// <summary>
      /// Specifies what character encoding to use where applicable (type {@link String})
      /// </summary>
      public string CharacterSet
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
            {
               return (string)Hints[EncodeHintType.CHARACTER_SET];
            }
            return null;
         }
         set
         {
            if (value == null)
            {
               if (Hints.ContainsKey(EncodeHintType.CHARACTER_SET))
                  Hints.Remove(EncodeHintType.CHARACTER_SET);
            }
            else
            {
               Hints[EncodeHintType.CHARACTER_SET] = value;
            }
         }
      }

      /// <summary>
      /// Explicitly disables ECI segment when generating PDF417 Code
      /// That is against the specification but some
      /// readers have problems if the charset is switched from
      /// CP437 (default) to UTF-8 with the necessary ECI segment.
      /// If you set the property to true you can use different encodings
      /// and the ECI segment is omitted.
      /// </summary>
      public bool DisableECI
      {
         get
         {
            if (Hints.ContainsKey(EncodeHintType.DISABLE_ECI))
            {
               return (bool)Hints[EncodeHintType.DISABLE_ECI];
            }
            return false;
         }
         set
         {
            Hints[EncodeHintType.DISABLE_ECI] = value;
         }
      }
   }
}