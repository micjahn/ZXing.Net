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
using System.Text;

namespace com.google.zxing.client.result
{
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class CalendarParsedResult : ParsedResult
   {
      public String Summary { get; private set; }

      /// <summary> <p>We would return the start and end date as a {@link java.util.Date} except that this code
      /// needs to work under JavaME / MIDP and there is no date parsing library available there, such
      /// as <code>java.text.SimpleDateFormat</code>.</p> See validateDate() for the return format.
      /// 
      /// </summary>
      /// <returns> start time formatted as a RFC 2445 DATE or DATE-TIME.
      /// </returns>
      public String Start { get; private set; }
      public String End { get; private set; }
      public String Location { get; private set; }
      public String Attendee { get; private set; }
      public String Title { get; private set; }

      override public String DisplayResult
      {
         get
         {
            var result = new StringBuilder(100);
            maybeAppend(Summary, result);
            maybeAppend(Start, result);
            maybeAppend(End, result);
            maybeAppend(Location, result);
            maybeAppend(Attendee, result);
            maybeAppend(Title, result);
            return result.ToString();
         }

      }

      public CalendarParsedResult(String summary, String start, String end, String location, String attendee, String title)
         : base(ParsedResultType.CALENDAR)
      {
         // Start is required, end is not
         if (start == null)
         {
            throw new ArgumentException();
         }
         validateDate(start);
         validateDate(end);
         Summary = summary;
         Start = start;
         End = end;
         Location = location;
         Attendee = attendee;
         Title = title;
      }

      /// <summary> RFC 2445 allows the start and end fields to be of type DATE (e.g. 20081021) or DATE-TIME
      /// (e.g. 20081021T123000 for local time, or 20081021T123000Z for UTC).
      /// 
      /// </summary>
      /// <param name="date">The string to validate
      /// </param>
      private static void validateDate(String date)
      {
         if (date != null)
         {
            int length = date.Length;
            if (length != 8 && length != 15 && length != 16)
            {
               throw new ArgumentException();
            }
            for (int i = 0; i < 8; i++)
            {
               if (!Char.IsDigit(date[i]))
               {
                  throw new ArgumentException();
               }
            }
            if (length > 8)
            {
               if (date[8] != 'T')
               {
                  throw new ArgumentException();
               }
               for (int i = 9; i < 15; i++)
               {
                  if (!Char.IsDigit(date[i]))
                  {
                     throw new ArgumentException();
                  }
               }
               if (length == 16 && date[15] != 'Z')
               {
                  throw new ArgumentException();
               }
            }
         }
      }
   }
}