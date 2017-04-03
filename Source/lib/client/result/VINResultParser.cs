/*
 * Copyright 2014 ZXing authors
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
using System.Text.RegularExpressions;

namespace ZXing.Client.Result
{
   /// <summary>
   /// Detects a result that is likely a vehicle identification number.
   /// @author Sean Owen
   /// </summary>
   public class VINResultParser : ResultParser
   {
#if SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2
      private static readonly Regex IOQ = new Regex("[IOQ]");
      private static readonly Regex AZ09 = new Regex(@"\A(?:" + "[A-Z0-9]{17}" + @")\z");
#else
      private static readonly Regex IOQ = new Regex("[IOQ]", RegexOptions.Compiled);
      private static readonly Regex AZ09 = new Regex(@"\A(?:" + "[A-Z0-9]{17}" + @")\z", RegexOptions.Compiled);
#endif

      public override ParsedResult parse(ZXing.Result result)
      {
         try
         {
            if (result.BarcodeFormat != BarcodeFormat.CODE_39)
            {
               return null;
            }
            var rawText = result.Text;
            rawText = IOQ.Replace(rawText, "").Trim();
            var az09Match = AZ09.Match(rawText);
            if (!az09Match.Success)
            {
               return null;
            }
            if (!checkChecksum(rawText))
            {
               return null;
            }

            var wmi = rawText.Substring(0, 3);
            return new VINParsedResult(rawText,
                                       wmi,
                                       rawText.Substring(3, 6),
                                       rawText.Substring(9, 8),
                                       countryCode(wmi),
                                       rawText.Substring(3, 5),
                                       modelYear(rawText[9]),
                                       rawText[10],
                                       rawText.Substring(11));
         }
         catch
         {
            return null;
         }
      }

      private static bool checkChecksum(String vin)
      {
         var sum = 0;
         for (var i = 0; i < vin.Length; i++)
         {
            sum += vinPositionWeight(i + 1) * vinCharValue(vin[i]);
         }
         var currentCheckChar = vin[8];
         var expectedCheckChar = checkChar(sum % 11);
         return currentCheckChar == expectedCheckChar;
      }

      private static int vinCharValue(char c)
      {
         if (c >= 'A' && c <= 'I')
         {
            return (c - 'A') + 1;
         }
         if (c >= 'J' && c <= 'R')
         {
            return (c - 'J') + 1;
         }
         if (c >= 'S' && c <= 'Z')
         {
            return (c - 'S') + 2;
         }
         if (c >= '0' && c <= '9')
         {
            return c - '0';
         }
         throw new ArgumentException(c.ToString());
      }

      private static int vinPositionWeight(int position)
      {
         if (position >= 1 && position <= 7)
         {
            return 9 - position;
         }
         if (position == 8)
         {
            return 10;
         }
         if (position == 9)
         {
            return 0;
         }
         if (position >= 10 && position <= 17)
         {
            return 19 - position;
         }
         throw new ArgumentException();
      }

      private static char checkChar(int remainder)
      {
         if (remainder < 10)
         {
            return (char) ('0' + remainder);
         }
         if (remainder == 10)
         {
            return 'X';
         }
         throw new ArgumentException();
      }

      private static int modelYear(char c)
      {
         if (c >= 'E' && c <= 'H')
         {
            return (c - 'E') + 1984;
         }
         if (c >= 'J' && c <= 'N')
         {
            return (c - 'J') + 1988;
         }
         if (c == 'P')
         {
            return 1993;
         }
         if (c >= 'R' && c <= 'T')
         {
            return (c - 'R') + 1994;
         }
         if (c >= 'V' && c <= 'Y')
         {
            return (c - 'V') + 1997;
         }
         if (c >= '1' && c <= '9')
         {
            return (c - '1') + 2001;
         }
         if (c >= 'A' && c <= 'D')
         {
            return (c - 'A') + 2010;
         }
         throw new ArgumentException(c.ToString());
      }

      private static String countryCode(String wmi)
      {
         char c1 = wmi[0];
         char c2 = wmi[1];
         switch (c1)
         {
            case '1':
            case '4':
            case '5':
               return "US";
            case '2':
               return "CA";
            case '3':
               if (c2 >= 'A' && c2 <= 'W')
               {
                  return "MX";
               }
               break;
            case '9':
               if ((c2 >= 'A' && c2 <= 'E') || (c2 >= '3' && c2 <= '9'))
               {
                  return "BR";
               }
               break;
            case 'J':
               if (c2 >= 'A' && c2 <= 'T')
               {
                  return "JP";
               }
               break;
            case 'K':
               if (c2 >= 'L' && c2 <= 'R')
               {
                  return "KO";
               }
               break;
            case 'L':
               return "CN";
            case 'M':
               if (c2 >= 'A' && c2 <= 'E')
               {
                  return "IN";
               }
               break;
            case 'S':
               if (c2 >= 'A' && c2 <= 'M')
               {
                  return "UK";
               }
               if (c2 >= 'N' && c2 <= 'T')
               {
                  return "DE";
               }
               break;
            case 'V':
               if (c2 >= 'F' && c2 <= 'R')
               {
                  return "FR";
               }
               if (c2 >= 'S' && c2 <= 'W')
               {
                  return "ES";
               }
               break;
            case 'W':
               return "DE";
            case 'X':
               if (c2 == '0' || (c2 >= '3' && c2 <= '9'))
               {
                  return "RU";
               }
               break;
            case 'Z':
               if (c2 >= 'A' && c2 <= 'R')
               {
                  return "IT";
               }
               break;
         }
         return null;
      }
   }
}