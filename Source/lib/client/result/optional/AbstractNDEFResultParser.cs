/*
* Copyright 2008 ZXing authors
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
using System.IO;
using System.Text;

namespace com.google.zxing.client.result.optional
{

   /// <summary> <p>Superclass for classes encapsulating results in the NDEF format.
   /// See <a href="http://www.nfc-forum.org/specs/">http://www.nfc-forum.org/specs/</a>.</p>
   /// 
   /// <p>This code supports a limited subset of NDEF messages, ones that are plausibly
   /// useful in 2D barcode formats. This generally includes 1-record messages, no chunking,
   /// "short record" syntax, no ID field.</p>
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   abstract class AbstractNDEFResultParser : ResultParser
   {

      internal static String bytesToString(sbyte[] bytes, int offset, int length, String encoding)
      {
         try
         {
            return Encoding.GetEncoding(encoding).GetString(SupportClass.ToByteArray(bytes), offset, length);
         }
         catch (IOException uee)
         {
            // This should only be used when 'encoding' is an encoding that must necessarily
            // be supported by the JVM, like UTF-8
            //UPGRADE_TODO: The equivalent in .NET for method 'java.lang.Throwable.toString' may return a different value. "ms-help://MS.VSCC.v80/dv_commoner/local/redirect.htm?index='!DefaultContextWindowIndex'&keyword='jlca1043'"
            throw new SystemException("Platform does not support required encoding: " + uee);
         }
      }
   }
}