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
using System.Globalization;
using System.Text.RegularExpressions;

namespace ZXing.Client.Result
{
   /// <summary> Parses a "geo:" URI result, which specifies a location on the surface of
   /// the Earth as well as an optional altitude above the surface. See
   /// <a href="http://tools.ietf.org/html/draft-mayrhofer-geo-uri-00">
   /// http://tools.ietf.org/html/draft-mayrhofer-geo-uri-00</a>.
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class GeoResultParser : ResultParser
   {
      private static readonly Regex GEO_URL_PATTERN = new Regex(@"\A(?:" + "geo:([\\-0-9.]+),([\\-0-9.]+)(?:,([\\-0-9.]+))?(?:\\?(.*))?" + @")\z"
#if !(SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2)
         , RegexOptions.Compiled | RegexOptions.IgnoreCase);
#else
         , RegexOptions.IgnoreCase);
#endif

      override public ParsedResult parse(ZXing.Result result)
      {
         String rawText = result.Text;
         if (rawText == null)
         {
            return null;
         }

         var matcher = GEO_URL_PATTERN.Match(rawText);
         if (!matcher.Success)
         {
            return null;
         }

         String query = matcher.Groups[4].Value;
         if (String.IsNullOrEmpty(query))
            query = null;

         double latitude;
         double longitude;
         double altitude = 0.0;
#if WindowsCE
         try { latitude = Double.Parse(matcher.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture); }
         catch { return null; }
#else
         if (!Double.TryParse(matcher.Groups[1].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out latitude))
            return null;
#endif
         if (latitude > 90.0 || latitude < -90.0)
         {
            return null;
         }
#if WindowsCE
         try { longitude = Double.Parse(matcher.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture); }
         catch { return null; }
#else
         if (!Double.TryParse(matcher.Groups[2].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
            return null;
#endif
         if (longitude > 180.0 || longitude < -180.0)
         {
            return null;
         }
         if (!String.IsNullOrEmpty(matcher.Groups[3].Value))
         {
#if WindowsCE
            try { altitude = Double.Parse(matcher.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture); }
            catch { return null; }
#else
            if (!Double.TryParse(matcher.Groups[3].Value, NumberStyles.Float, CultureInfo.InvariantCulture, out altitude))
               return null;
#endif
            if (altitude < 0.0)
            {
               return null;
            }
         }
         return new GeoParsedResult(latitude, longitude, altitude, query);
      }
   }
}
