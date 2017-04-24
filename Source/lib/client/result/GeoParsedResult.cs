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
using System.Text;

namespace ZXing.Client.Result
{
   /// <summary>
   /// Represents a parsed result that encodes a geographic coordinate, with latitude, longitude and altitude.
   /// </summary>
   /// <author>Sean Owen</author>
   public sealed class GeoParsedResult : ParsedResult
   {
      internal GeoParsedResult(double latitude, double longitude, double altitude, String query)
         : base(ParsedResultType.GEO)
      {
         Latitude = latitude;
         Longitude = longitude;
         Altitude = altitude;
         Query = query;
         GeoURI = getGeoURI();
         GoogleMapsURI = getGoogleMapsURI();
         displayResultValue = getDisplayResult();
      }

      /// <returns> latitude in degrees
      /// </returns>
      public double Latitude { get; private set; }

      /// <returns> longitude in degrees
      /// </returns>
      public double Longitude { get; private set; }

      /// <returns> altitude in meters. If not specified, in the geo URI, returns 0.0
      /// </returns>
      public double Altitude { get; private set; }

      /// <return> query string associated with geo URI or null if none exists</return>
      public String Query { get; private set; }

      /// <summary>
      /// the geo URI
      /// </summary>
      public String GeoURI { get; private set; }
      
      /// <returns> a URI link to Google Maps which display the point on the Earth described
      /// by this instance, and sets the zoom level in a way that roughly reflects the
      /// altitude, if specified
      /// </returns>
      public String GoogleMapsURI { get; private set; }

      private String getDisplayResult()
      {
         var result = new StringBuilder(20);
         result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Latitude);
         result.Append(", ");
         result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Longitude);
         if (Altitude > 0.0)
         {
            result.Append(", ");
            result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Altitude);
            result.Append('m');
         }
         if (Query != null)
         {
            result.Append(" (");
            result.Append(Query);
            result.Append(')');
         }
         return result.ToString();
      }

      private String getGeoURI()
      {
         var result = new StringBuilder();
         result.Append("geo:");
         result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Latitude);
         result.Append(',');
         result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Longitude);
         if (Altitude > 0)
         {
            result.Append(',');
            result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Altitude);
         }
         if (Query != null)
         {
            result.Append('?');
            result.Append(Query);
         }
         return result.ToString();
      }

      private String getGoogleMapsURI()
      {
         var result = new StringBuilder(50);
         result.Append("http://maps.google.com/?ll=");
         result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Latitude);
         result.Append(',');
         result.AppendFormat(CultureInfo.InvariantCulture, "{0:0.0###########}", Longitude);
         if (Altitude > 0.0f)
         {
            // Map altitude to zoom level, cleverly. Roughly, zoom level 19 is like a
            // view from 1000ft, 18 is like 2000ft, 17 like 4000ft, and so on.
            double altitudeInFeet = Altitude * 3.28;
            int altitudeInKFeet = (int)(altitudeInFeet / 1000.0);
            // No Math.log() available here, so compute log base 2 the old fashioned way
            // Here logBaseTwo will take on a value between 0 and 18 actually
            int logBaseTwo = 0;
            while (altitudeInKFeet > 1 && logBaseTwo < 18)
            {
               altitudeInKFeet >>= 1;
               logBaseTwo++;
            }
            int zoom = 19 - logBaseTwo;
            result.Append("&z=");
            result.Append(zoom);
         }
         return result.ToString();
      }
   }
}