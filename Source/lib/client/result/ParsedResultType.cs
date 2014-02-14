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

namespace ZXing.Client.Result
{
   /// <summary>
   /// Represents the type of data encoded by a barcode -- from plain text, to a
   /// URI, to an e-mail address, etc.
   /// </summary>
   /// <author>Sean Owen</author>
   public enum ParsedResultType
   {
      ADDRESSBOOK,
      EMAIL_ADDRESS,
      PRODUCT,
      URI,
      TEXT,
      GEO,
      TEL,
      SMS,
      CALENDAR,
      WIFI,
      ISBN,
      VIN
   }
}