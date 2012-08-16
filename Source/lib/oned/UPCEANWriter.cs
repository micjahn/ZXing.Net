/*
 * Copyright 2009 ZXing authors
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

namespace ZXing.OneD
{
   /// <summary>
   ///   <p>Encapsulates functionality and implementation that is common to UPC and EAN families
   /// of one-dimensional barcodes.</p>
   ///   <author>aripollak@gmail.com (Ari Pollak)</author>
   ///   <author>dsbnatut@gmail.com (Kazuki Nishiura)</author>
   /// </summary>
   public abstract class UPCEANWriter : OneDimensionalCodeWriter
   {
      /// <summary>
      /// Gets the default margin.
      /// </summary>
      public override int DefaultMargin
      {
         get
         {
            // Use a different default more appropriate for UPC/EAN
            return UPCEANReader.START_END_PATTERN.Length;
         }
      }
   }
}