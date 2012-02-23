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

namespace ZXing.Client.Result
{
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   public sealed class CalendarParsedResult : ParsedResult
   {
      private readonly String summary;
      private readonly String start;
      private readonly String end;
      private readonly String location;
      private readonly String attendee;
      private readonly String description;
      private readonly double latitude;
      private readonly double longitude;

      public CalendarParsedResult(String summary,
                                  String start,
                                  String end,
                                  String location,
                                  String attendee,
                                  String description)
         : this(summary, start, end, location, attendee, description, Double.NaN, Double.NaN)
      {
      }

      public CalendarParsedResult(String summary,
                                  String start,
                                  String end,
                                  String location,
                                  String attendee,
                                  String description,
                                  double latitude,
                                  double longitude)
         : base(ParsedResultType.CALENDAR)
      {
         validateDate(start);
         this.summary = summary;
         this.start = start;
         if (end != null)
         {
            validateDate(end);
            this.end = end;
         }
         else
         {
            this.end = null;
         }
         this.location = location;
         this.attendee = attendee;
         this.description = description;
         this.latitude = latitude;
         this.longitude = longitude;
      }

      public String Summary
      {
         get { return summary; }
      }

      /// <summary>
      /// <p>We would return the start and end date as a {@link java.util.Date} except that this code
      /// needs to work under JavaME / MIDP and there is no date parsing library available there, such
      /// as {@code java.text.SimpleDateFormat}.</p> See validateDate() for the return format.
      /// </summary>
      /// <return>start time formatted as a RFC 2445 DATE or DATE-TIME.</return>
      public String Start
      {
         get { return start; }
      }

      /// <summary>
      /// @see #getStart(). May return null if the event has no duration.
      /// </summary>
      public String End
      {
         get { return end; }
      }

      public String Location
      {
         get { return location; }
      }

      public String Attendee
      {
         get { return attendee; }
      }

      public String Description
      {
         get { return description; }
      }

      public double Latitude
      {
         get { return latitude; }
      }

      public double Longitude
      {
         get { return longitude; }
      }

      public override String DisplayResult
      {
         get
         {
            var result = new StringBuilder(100);
            maybeAppend(summary, result);
            maybeAppend(start, result);
            maybeAppend(end, result);
            maybeAppend(location, result);
            maybeAppend(attendee, result);
            maybeAppend(description, result);
            return result.ToString();
         }
      }

      /**
       * RFC 2445 allows the start and end fields to be of type DATE (e.g. 20081021) or DATE-TIME
       * (e.g. 20081021T123000 for local time, or 20081021T123000Z for UTC).
       *
       * @param date The string to validate
       */
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