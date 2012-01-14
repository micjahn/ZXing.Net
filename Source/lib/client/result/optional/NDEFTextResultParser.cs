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

namespace com.google.zxing.client.result.optional
{

   /// <summary> Recognizes an NDEF message that encodes text according to the
   /// "Text Record Type Definition" specification.
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class NDEFTextResultParser : AbstractNDEFResultParser
   {
      public static TextParsedResult parse(Result result)
      {
         var bytes = result.RawBytes;
         if (bytes == null)
         {
            return null;
         }
         var ndefRecord = NDEFRecord.readRecord(bytes, 0);
         if (ndefRecord == null || !ndefRecord.MessageBegin || !ndefRecord.MessageEnd)
         {
            return null;
         }
         if (!ndefRecord.Type.Equals(NDEFRecord.TEXT_WELL_KNOWN_TYPE))
         {
            return null;
         }
         var languageText = decodeTextPayload(ndefRecord.Payload);
         return new TextParsedResult(languageText[0], languageText[1]);
      }

      internal static String[] decodeTextPayload(sbyte[] payload)
      {
         var statusByte = payload[0];
         var isUTF16 = (statusByte & 0x80) != 0;
         var languageLength = statusByte & 0x1F;
         // language is always ASCII-encoded:
         var language = bytesToString(payload, 1, languageLength, "US-ASCII");
         var encoding = isUTF16 ? "UTF-16" : "UTF-8";
         var text = bytesToString(payload, 1 + languageLength, payload.Length - languageLength - 1, encoding);
         return new[] { language, text };
      }
   }
}
