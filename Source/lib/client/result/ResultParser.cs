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

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ZXing.Client.Result
{
   /// <summary> <p>Abstract class representing the result of decoding a barcode, as more than
   /// a String -- as some type of structured data. This might be a subclass which represents
   /// a URL, or an e-mail address. {@link #parseResult(com.google.zxing.Result)} will turn a raw
   /// decoded string into the most appropriate type of structured representation.</p>
   /// 
   /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
   /// on exception-based mechanisms during parsing.</p>
   /// </summary>
   /// <author>Sean Owen</author>
   public abstract class ResultParser
   {
      private static readonly ResultParser[] PARSERS =
         {
            new BookmarkDoCoMoResultParser(),
            new AddressBookDoCoMoResultParser(),
            new EmailDoCoMoResultParser(),
            new AddressBookAUResultParser(),
            new VCardResultParser(),
            new BizcardResultParser(),
            new VEventResultParser(),
            new EmailAddressResultParser(),
            new SMTPResultParser(),
            new TelResultParser(),
            new SMSMMSResultParser(),
            new SMSTOMMSTOResultParser(),
            new GeoResultParser(),
            new WifiResultParser(),
            new URLTOResultParser(),
            new URIResultParser(),
            new ISBNResultParser(),
            new ProductResultParser(),
            new ExpandedProductResultParser(),
            new VINResultParser(),
         };

#if SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE || NETSTANDARD1_0 || NETSTANDARD1_1 || NETSTANDARD1_2
      private static readonly Regex DIGITS = new Regex(@"\A(?:" + "\\d+" + @")\z");
      private static readonly Regex AMPERSAND = new Regex("&");
      private static readonly Regex EQUALS = new Regex("=");
#else
      private static readonly Regex DIGITS = new Regex(@"\A(?:" + "\\d+" + @")\z", RegexOptions.Compiled);
      private static readonly Regex AMPERSAND = new Regex("&", RegexOptions.Compiled);
      private static readonly Regex EQUALS = new Regex("=", RegexOptions.Compiled);
#endif

      /// <summary>
      /// Attempts to parse the raw {@link Result}'s contents as a particular type
      /// of information (email, URL, etc.) and return a {@link ParsedResult} encapsulating
      /// the result of parsing.
      /// </summary>
      /// <param name="theResult">the raw <see cref="Result"/> to parse</param>
      /// <returns><see cref="ParsedResult" /> encapsulating the parsing result</returns>
      public abstract ParsedResult parse(ZXing.Result theResult);

      /// <summary>
      /// Parses the result.
      /// </summary>
      /// <param name="theResult">The result.</param>
      /// <returns></returns>
      public static ParsedResult parseResult(ZXing.Result theResult)
      {
         foreach (var parser in PARSERS)
         {
            var result = parser.parse(theResult);
            if (result != null)
            {
               return result;
            }
         }
         return new TextParsedResult(theResult.Text, null);
      }

      protected static void maybeAppend(String value, System.Text.StringBuilder result)
      {
         if (value != null)
         {
            result.Append('\n');
            result.Append(value);
         }
      }

      protected static void maybeAppend(String[] value, System.Text.StringBuilder result)
      {
         if (value != null)
         {
            for (int i = 0; i < value.Length; i++)
            {
               result.Append('\n');
               result.Append(value[i]);
            }
         }
      }

      protected static String[] maybeWrap(String value)
      {
         return value == null ? null : new[] { value };
      }

      protected static String unescapeBackslash(String escaped)
      {
         if (escaped != null)
         {
            int backslash = escaped.IndexOf('\\');
            if (backslash >= 0)
            {
               int max = escaped.Length;
               var unescaped = new System.Text.StringBuilder(max - 1);
               unescaped.Append(escaped.ToCharArray(), 0, backslash);
               bool nextIsEscaped = false;
               for (int i = backslash; i < max; i++)
               {
                  char c = escaped[i];
                  if (nextIsEscaped || c != '\\')
                  {
                     unescaped.Append(c);
                     nextIsEscaped = false;
                  }
                  else
                  {
                     nextIsEscaped = true;
                  }
               }
               return unescaped.ToString();
            }
         }
         return escaped;
      }

      protected static int parseHexDigit(char c)
      {
         if (c >= 'a')
         {
            if (c <= 'f')
            {
               return 10 + (c - 'a');
            }
         }
         else if (c >= 'A')
         {
            if (c <= 'F')
            {
               return 10 + (c - 'A');
            }
         }
         else if (c >= '0')
         {
            if (c <= '9')
            {
               return c - '0';
            }
         }
         return -1;
      }

      internal static bool isStringOfDigits(String value, int length)
      {
         return value != null && length > 0 && length == value.Length && DIGITS.Match(value).Success;
      }

      internal static bool isSubstringOfDigits(String value, int offset, int length)
      {
         if (value == null || length <= 0)
         {
            return false;
         }
         int max = offset + length;
         return value.Length >= max && DIGITS.Match(value, offset, length).Success;
      }

      internal static IDictionary<string, string> parseNameValuePairs(String uri)
      {
         int paramStart = uri.IndexOf('?');
         if (paramStart < 0)
         {
            return null;
         }
         var result = new Dictionary<String, String>(3);
         foreach (var keyValue in AMPERSAND.Split(uri.Substring(paramStart + 1)))
         {
            appendKeyValue(keyValue, result);
         }
         return result;
      }

      private static void appendKeyValue(String keyValue, IDictionary<String, String> result)
      {
         String[] keyValueTokens = EQUALS.Split(keyValue, 2);
         if (keyValueTokens.Length == 2)
         {
            String key = keyValueTokens[0];
            String value = keyValueTokens[1];
            try
            {
               //value = URLDecoder.decode(value, "UTF-8");
               value = urlDecode(value);
               result[key] = value;
            }
            catch (Exception uee)
            {
               throw new InvalidOperationException("url decoding failed", uee); // can't happen
            }
            result[key] = value;
         }
      }

      internal static String[] matchPrefixedField(String prefix, String rawText, char endChar, bool trim)
      {
         IList<string> matches = null;
         int i = 0;
         int max = rawText.Length;
         while (i < max)
         {
            i = rawText.IndexOf(prefix, i);
            if (i < 0)
            {
               break;
            }
            i += prefix.Length; // Skip past this prefix we found to start
            int start = i; // Found the start of a match here
            bool done = false;
            while (!done)
            {
               i = rawText.IndexOf(endChar, i);
               if (i < 0)
               {
                  // No terminating end character? uh, done. Set i such that loop terminates and break
                  i = rawText.Length;
                  done = true;
               }
               else if (countPrecedingBackslashes(rawText, i)%2 != 0)
               {
                  // semicolon was escaped (odd count of preceding backslashes) so continue
                  i++;
               }
               else
               {
                  // found a match
                  if (matches == null)
                  {
                     matches = new List<string>();
                  }
                  String element = unescapeBackslash(rawText.Substring(start, (i) - (start)));
                  if (trim)
                  {
                     element = element.Trim();
                  }
                  if (!String.IsNullOrEmpty(element))
                  {
                     matches.Add(element);
                  }
                  i++;
                  done = true;
               }
            }
         }
         if (matches == null || (matches.Count == 0))
         {
            return null;
         }
         return SupportClass.toStringArray(matches);
      }

      private static int countPrecedingBackslashes(String s, int pos)
      {
         int count = 0;
         for (int i = pos - 1; i >= 0; i--)
         {
            if (s[i] == '\\')
            {
               count++;
            }
            else
            {
               break;
            }
         }
         return count;
      }

      internal static String matchSinglePrefixedField(String prefix, String rawText, char endChar, bool trim)
      {
         String[] matches = matchPrefixedField(prefix, rawText, endChar, trim);
         return matches == null ? null : matches[0];
      }

      protected static String urlDecode(String escaped)
      {
         // Should we better use HttpUtility.UrlDecode?
         // Is HttpUtility.UrlDecode available for all platforms?
         // What about encoding like UTF8?

         if (escaped == null)
         {
            return null;
         }
         char[] escapedArray = escaped.ToCharArray();

         int first = findFirstEscape(escapedArray);
         if (first < 0)
         {
            return escaped;
         }

         int max = escapedArray.Length;
         // final length is at most 2 less than original due to at least 1 unescaping
         var unescaped = new System.Text.StringBuilder(max - 2);
         // Can append everything up to first escape character
         unescaped.Append(escapedArray, 0, first);

         for (int i = first; i < max; i++)
         {
            char c = escapedArray[i];
            if (c == '+')
            {
               // + is translated directly into a space
               unescaped.Append(' ');
            }
            else if (c == '%')
            {
               // Are there even two more chars? if not we will just copy the escaped sequence and be done
               if (i >= max - 2)
               {
                  unescaped.Append('%'); // append that % and move on
               }
               else
               {
                  int firstDigitValue = parseHexDigit(escapedArray[++i]);
                  int secondDigitValue = parseHexDigit(escapedArray[++i]);
                  if (firstDigitValue < 0 || secondDigitValue < 0)
                  {
                     // bad digit, just move on
                     unescaped.Append('%');
                     unescaped.Append(escapedArray[i - 1]);
                     unescaped.Append(escapedArray[i]);
                  }
                  unescaped.Append((char) ((firstDigitValue << 4) + secondDigitValue));
               }
            }
            else
            {
               unescaped.Append(c);
            }
         }
         return unescaped.ToString();
      }

      private static int findFirstEscape(char[] escapedArray)
      {
         int max = escapedArray.Length;
         for (int i = 0; i < max; i++)
         {
            char c = escapedArray[i];
            if (c == '+' || c == '%')
            {
               return i;
            }
         }
         return -1;
      }
   }
}