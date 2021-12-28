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
using System.Collections.Generic;
using System.Globalization;

namespace ZXing.Client.Result
{
    /// <summary>
    /// Partially implements the iCalendar format's "VEVENT" format for specifying a
    /// calendar event. See RFC 2445. This supports SUMMARY, DTSTART and DTEND fields.
    /// </summary>
    /// <author>  Sean Owen
    /// </author>
    /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
    /// </author>
    sealed class VEventResultParser : ResultParser
    {
        override public ParsedResult parse(ZXing.Result result)
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

            String summary = matchSingleVCardPrefixedField("SUMMARY", rawText);
            String start = matchSingleVCardPrefixedField("DTSTART", rawText);
            if (start == null)
            {
                return null;
            }
            String end = matchSingleVCardPrefixedField("DTEND", rawText);
            String duration = matchSingleVCardPrefixedField("DURATION", rawText);
            String location = matchSingleVCardPrefixedField("LOCATION", rawText);
            String organizer = stripMailto(matchSingleVCardPrefixedField("ORGANIZER", rawText));

            String[] attendees = matchVCardPrefixedField("ATTENDEE", rawText);
            if (attendees != null)
            {
                for (int i = 0; i < attendees.Length; i++)
                {
                    attendees[i] = stripMailto(attendees[i]);
                }
            }
            String description = matchSingleVCardPrefixedField("DESCRIPTION", rawText);

            String geoString = matchSingleVCardPrefixedField("GEO", rawText);
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
                if (semicolon < 0)
                {
                    return null;
                }
                if (!Double.TryParse(geoString.Substring(0, semicolon), NumberStyles.Float, CultureInfo.InvariantCulture, out latitude))
                    return null;
                if (!Double.TryParse(geoString.Substring(semicolon + 1), NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                    return null;
            }

            try
            {
                return new CalendarParsedResult(summary,
                                                start,
                                                end,
                                                duration,
                                                location,
                                                organizer,
                                                attendees,
                                                description,
                                                latitude,
                                                longitude);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        private static String matchSingleVCardPrefixedField(String prefix,
                                                            String rawText)
        {
            var values = VCardResultParser.matchSingleVCardPrefixedField(prefix, rawText, true, false);
            return values == null || values.Count == 0 ? null : values[0];
        }

        private static String[] matchVCardPrefixedField(String prefix, String rawText)
        {
            List<List<String>> values = VCardResultParser.matchVCardPrefixedField(prefix, rawText, true, false);
            if (values == null || values.Count == 0)
            {
                return null;
            }
            int size = values.Count;
            String[] result = new String[size];
            for (int i = 0; i < size; i++)
            {
                result[i] = values[i][0];
            }
            return result;
        }

        private static String stripMailto(String s)
        {
            if (s != null && (s.StartsWith("mailto:") || s.StartsWith("MAILTO:")))
            {
                s = s.Substring(7);
            }
            return s;
        }
    }
}
