/*
 * Copyright 2007 ZXing authors
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

namespace com.google.zxing
{
   /// <summary>
   /// Thrown when a barcode was successfully detected and decoded, but
   /// was not returned because its checksum feature failed.
   /// </summary>
   /// <author>Sean Owen</author>
   public class ChecksumException : ReaderException
   {
      private static readonly ChecksumException instance = new ChecksumException();

      private ChecksumException()
      {
         // do nothing
      }

      new public static ChecksumException Instance
      {
         get { return instance; }
      }
   }
}