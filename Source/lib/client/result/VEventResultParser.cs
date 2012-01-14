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

namespace com.google.zxing.client.result
{

   /// <summary> Partially implements the iCalendar format's "VEVENT" format for specifying a
   /// calendar event. See RFC 2445. This supports SUMMARY, DTSTART and DTEND fields.
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class VEventResultParser : ResultParser
   {
      override public ParsedResult parse(Result result)
      {
         String rawText = result.Text;
         if (rawText == null)
         {
            return null;
         }
         int vEventStart = rawText.IndexOf("BEGIN:VEVENT");
         if (vEventStart < 0)
         {
            return null;
         }

         String summary = matchSingleVCardPrefixedField("SUMMARY", rawText, true);
         String start = matchSingleVCardPrefixedField("DTSTART", rawText, true);
         if (start == null)
         {
            return null;
         }
         String end = matchSingleVCardPrefixedField("DTEND", rawText, true);
         String location = matchSingleVCardPrefixedField("LOCATION", rawText, true);
         String description = matchSingleVCardPrefixedField("DESCRIPTION", rawText, true);

         String geoString = matchSingleVCardPrefixedField("GEO", rawText, true);
         double latitude;
         double longitude;
         if (geoString == null)
         {
            latitude = Double.NaN;
            longitude = Double.NaN;
         }
         else
         {
            int semicolon = geoString.IndexOf(';');
            if (!Double.TryParse(geoString.Substring(0, semicolon), out latitude))
               return null;
            if (!Double.TryParse(geoString.Substring(semicolon + 1), out longitude))
               return null;
         }

         try
         {
            return new CalendarParsedResult(summary, start, end, location, null, description, latitude, longitude);
         }
         catch (ArgumentException )
         {
            return null;
         }
      }

      private static String matchSingleVCardPrefixedField(String prefix,
                                                          String rawText,
                                                          bool trim)
      {
         var values = VCardResultParser.matchSingleVCardPrefixedField(prefix, rawText, trim);
         return values == null || values.Count == 0 ? null : values[0];
      }
   }
}
