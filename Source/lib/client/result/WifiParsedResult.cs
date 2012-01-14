/*
 * Copyright 2010 ZXing authors
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
   /**
    * @author Vikram Aggarwal
    */
   public class WifiParsedResult : ParsedResult
   {
      private String ssid;
      private String networkEncryption;
      private String password;

      public WifiParsedResult(String networkEncryption, String ssid, String password)
         : base(ParsedResultType.WIFI)
      {
         this.ssid = ssid;
         this.networkEncryption = networkEncryption;
         this.password = password;
      }

      public String Ssid
      {
         get { return ssid; }
      }

      public String NetworkEncryption
      {
         get { return networkEncryption; }
      }

      public String Password
      {
         get { return password; }
      }

      override public String DisplayResult
      {
         get
         {
            var result = new StringBuilder(80);
            maybeAppend(ssid, result);
            maybeAppend(networkEncryption, result);
            maybeAppend(password, result);
            return result.ToString();
         }
      }
   }
}