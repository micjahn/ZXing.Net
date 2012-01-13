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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace com.google.zxing.client.result
{
   /// <summary> Parses contact information formatted according to the VCard (2.1) format. This is not a complete
   /// implementation but should parse information as commonly encoded in 2D barcodes.
   /// 
   /// </summary>
   /// <author>  Sean Owen
   /// </author>
   /// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
   /// </author>
   sealed class VCardResultParser : ResultParser
   {
      private static Regex BEGIN_VCARD = new Regex("BEGIN:VCARD", RegexOptions.Compiled | RegexOptions.IgnoreCase);
      private static Regex VCARD_LIKE_DATE = new Regex("\\d{4}-?\\d{2}-?\\d{2}", RegexOptions.Compiled);
      private static Regex CR_LF_SPACE_TAB = new Regex("\r\n[ \t]", RegexOptions.Compiled);
      private static Regex NEWLINE_ESCAPE = new Regex("\\\\[nN]", RegexOptions.Compiled);
      private static Regex VCARD_ESCAPES = new Regex("\\\\([,;\\\\])", RegexOptions.Compiled);
      private static Regex EQUALS = new Regex("=", RegexOptions.Compiled);
      private static Regex SEMICOLON = new Regex(";", RegexOptions.Compiled);

      override public ParsedResult parse(Result result)
      {
         // Although we should insist on the raw text ending with "END:VCARD", there's no reason
         // to throw out everything else we parsed just because this was omitted. In fact, Eclair
         // is doing just that, and we can't parse its contacts without this leniency.
         String rawText = result.Text;
         var m = BEGIN_VCARD.Match(rawText);
         if (!m.Success || m.Index != 0)
         {
            return null;
         }
         List<List<String>> names = matchVCardPrefixedField("FN", rawText, true);
         if (names == null)
         {

            // If no display names found, look for regular name fields and format them
            names = matchVCardPrefixedField("N", rawText, true);
            formatNames(names);
         }
         List<List<String>> phoneNumbers = matchVCardPrefixedField("TEL", rawText, true);
         List<List<String>> emails = matchVCardPrefixedField("EMAIL", rawText, true);
         List<String> note = matchSingleVCardPrefixedField("NOTE", rawText, false);
         List<List<String>> addresses = matchVCardPrefixedField("ADR", rawText, true);
         if (addresses != null)
         {
            foreach (var list in addresses)
            {
               list.Insert(0, list[0]);
            }
         }
         List<String> org = matchSingleVCardPrefixedField("ORG", rawText, true);
         List<String> birthday = matchSingleVCardPrefixedField("BDAY", rawText, true);
         if (birthday != null && !isLikeVCardDate(birthday[0]))
         {



            birthday = null;
         }
         List<String> title = matchSingleVCardPrefixedField("TITLE", rawText, true);
         List<String> url = matchSingleVCardPrefixedField("URL", rawText, true);
         List<String> instantMessenger = matchSingleVCardPrefixedField("IMPP", rawText, true);
         return new AddressBookParsedResult(toPrimaryValues(names),
                                            null,
                                            toPrimaryValues(phoneNumbers),
                                            toTypes(phoneNumbers),
                                            toPrimaryValues(emails),
                                            toTypes(emails),
                                            toPrimaryValue(instantMessenger),
                                            toPrimaryValue(note),
                                            toPrimaryValues(addresses),
                                            toTypes(addresses),
                                            toPrimaryValue(org),
                                            toPrimaryValue(birthday),
                                            toPrimaryValue(title),
                                            toPrimaryValue(url));
      }

      private static List<List<String>> matchVCardPrefixedField(String prefix,
                                                                String rawText,
                                                                bool trim)
      {
         List<List<String>> matches = null;
         int i = 0;
         int max = rawText.Length;

         while (i < max)
         {

            // At start or after newline, match prefix, followed by optional metadata 
            // (led by ;) ultimately ending in colon
            var matcher = new Regex("(?:^|\n)" + prefix + "(?:;([^:]*))?:", RegexOptions.Compiled).Match(rawText);
            if (i > 0)
            {
               i--; // Find from i-1 not i since looking at the preceding character
            }
            if (!matcher.find(i))
            {
               break;
            }
            i = matcher.end(0); // group 0 = whole pattern; end(0) is past final colon

            String metadataString = matcher.Groups[1].Value; // group 1 = metadata substring
            List<String> metadata = null;
            bool quotedPrintable = false;
            String quotedPrintableCharset = null;
            if (metadataString != null)
            {
               foreach (String metadatum in SEMICOLON.Split(metadataString))
               {
                  if (metadata == null)
                  {
                     metadata = new List<String>(1);
                  }
                  metadata.Add(metadatum);
                  String[] metadatumTokens = EQUALS.Split(metadatum, 2);
                  if (metadatumTokens.Length > 1)
                  {
                     String key = metadatumTokens[0];
                     String value = metadatumTokens[1];
                     if (String.Compare("ENCODING", key, StringComparison.OrdinalIgnoreCase) == 0 &&
                        String.Compare("QUOTED-PRINTABLE", value, StringComparison.OrdinalIgnoreCase) == 0)
                     {
                        quotedPrintable = true;
                     }
                     else if (String.Compare("CHARSET", key, StringComparison.OrdinalIgnoreCase) == 0)
                     {
                        quotedPrintableCharset = value;
                     }
                  }
               }
            }

            int matchStart = i; // Found the start of a match here

            while ((i = rawText.IndexOf('\n', i)) >= 0)
            { // Really, end in \r\n
               if (i < rawText.Length - 1 &&           // But if followed by tab or space,
                   (rawText[i + 1] == ' ' ||        // this is only a continuation
                    rawText[i + 1] == '\t'))
               {
                  i += 2; // Skip \n and continutation whitespace
               }
               else if (quotedPrintable &&             // If preceded by = in quoted printable
                        (rawText[i - 1] == '=' || // this is a continuation
                         rawText[i - 2] == '='))
               {
                  i++; // Skip \n
               }
               else
               {
                  break;
               }
            }

            if (i < 0)
            {
               // No terminating end character? uh, done. Set i such that loop terminates and break
               i = max;
            }
            else if (i > matchStart)
            {


               // found a match
               if (matches == null)
               {
                  matches = new List<List<String>>(1); // lazy init
               }
               if (rawText[i - 1] == '\r')
               {
                  i--; // Back up over \r, which really should be there
               }
               String element = rawText.Substring(matchStart, i);
               if (trim)
               {
                  element = element.Trim();
               }
               if (quotedPrintable)
               {
                  element = decodeQuotedPrintable(element, quotedPrintableCharset);
               }
               else
               {
                  element = CR_LF_SPACE_TAB.Match(element).Result("");
                  element = NEWLINE_ESCAPE.Match(element).Result("\n");
                  element = VCARD_ESCAPES.Match(element).Result("$1");
               }
               if (metadata == null)
               {
                  var match = new List<String>(1);
                  match.Add(element);
                  matches.Add(match);
               }
               else
               {
                  metadata.Insert(0, element);
                  matches.Add(metadata);
               }
               i++;
            }
            else
            {
               i++;
            }

         }

         return matches;
      }

      private static String decodeQuotedPrintable(String value, String charset)
      {
         int length = value.Length;
         var result = new StringBuilder(length);
         var fragmentBuffer = new MemoryStream();
         for (int i = 0; i < length; i++)
         {
            char c = value[i];
            switch (c)
            {
               case '\r':
               case '\n':
                  break;
               case '=':
                  if (i < length - 2)
                  {
                     char nextChar = value[i + 1];
                     if (nextChar == '\r' || nextChar == '\n')
                     {
                        // Ignore, it's just a continuation symbol
                     }
                     else
                     {
                        char nextNextChar = value[i + 2];
                        int firstDigit = parseHexDigit(nextChar);
                        int secondDigit = parseHexDigit(nextNextChar);
                        if (firstDigit >= 0 && secondDigit >= 0)
                        {
                           // TODO: Correct order?
                           fragmentBuffer.WriteByte((byte)firstDigit);
                           fragmentBuffer.WriteByte((byte)secondDigit);
                        } // else ignore it, assume it was incorrectly encoded
                        i += 2;
                     }
                  }
                  break;
               default:
                  maybeAppendFragment(fragmentBuffer, charset, result);
                  result.Append(c);
                  break;
            }
         }
         maybeAppendFragment(fragmentBuffer, charset, result);
         return result.ToString();
      }

      private static void maybeAppendFragment(MemoryStream fragmentBuffer,
                                              String charset,
                                              StringBuilder result)
      {
         if (fragmentBuffer.Length > 0)
         {
            byte[] fragmentBytes = fragmentBuffer.ToArray();
            String fragment;
            if (charset == null)
            {
               fragment = Encoding.Default.GetString(fragmentBytes);
            }
            else
            {
               try
               {
                  fragment = Encoding.GetEncoding(charset).GetString(fragmentBytes);
               }
               catch (Exception )
               {
                  // Yikes, well try anyway:
                  fragment = Encoding.Default.GetString(fragmentBytes);
               }
            }
            fragmentBuffer.Seek(0, SeekOrigin.Begin);
            result.Append(fragment);
         }
      }

      internal static List<String> matchSingleVCardPrefixedField(String prefix,
                                                    String rawText,
                                                    bool trim)
      {
         List<List<String>> values = matchVCardPrefixedField(prefix, rawText, trim);
         return values == null || values.Count == 0 ? null : values[0];
      }

      private static String toPrimaryValue(List<String> list)
      {
         return list == null || list.Count == 0 ? null : list[0];
      }

      private static String[] toPrimaryValues(ICollection<List<String>> lists)
      {
         if (lists == null || lists.Count == 0)
         {
            return null;
         }
         List<String> result = new List<String>(lists.Count);
         foreach (var list in lists)
         {
            result.Add(list[0]);
         }
         return SupportClass.toStringArray(result);
      }

      private static String[] toTypes(ICollection<List<String>> lists)
      {
         if (lists == null || lists.Count == 0)
         {
            return null;
         }
         List<String> result = new List<String>(lists.Count);
         foreach (var list in lists)
         {
            String type = null;
            for (int i = 1; i < list.Count; i++)
            {
               String metadatum = list[i];
               int equals = metadatum.IndexOf('=');
               if (equals < 0)
               {
                  // take the whole thing as a usable label
                  type = metadatum;
                  break;
               }
               if (String.Compare("TYPE", metadatum.Substring(0, equals), StringComparison.OrdinalIgnoreCase) == 0)
               {
                  type = metadatum.Substring(equals + 1);
                  break;
               }
            }
            result.Add(type);
         }
         return SupportClass.toStringArray(result);
      }

      private static bool isLikeVCardDate(String value)
      {
         return value == null || VCARD_LIKE_DATE.Match(value).Success;
      }

      /**
       * Formats name fields of the form "Public;John;Q.;Reverend;III" into a form like
       * "Reverend John Q. Public III".
       *
       * @param names name values to format, in place
       */
      private static void formatNames(IEnumerable<List<String>> names)
      {
         if (names != null)
         {
            foreach (var list in names)
            {
               String name = list[0];
               String[] components = new String[5];
               int start = 0;
               int end;
               int componentIndex = 0;
               while ((end = name.IndexOf(';', start)) > 0)
               {
                  components[componentIndex] = name.Substring(start, end);


                  componentIndex++;
                  start = end + 1;
               }
               components[componentIndex] = name.Substring(start);
               StringBuilder newName = new StringBuilder(100);
               maybeAppendComponent(components, 3, newName);
               maybeAppendComponent(components, 1, newName);
               maybeAppendComponent(components, 2, newName);
               maybeAppendComponent(components, 0, newName);
               maybeAppendComponent(components, 4, newName);
               list.Insert(0, newName.ToString().Trim());
            }
         }
      }

      private static void maybeAppendComponent(String[] components, int i, StringBuilder newName)
      {
         if (components[i] != null)
         {
            newName.Append(' ');
            newName.Append(components[i]);
         }
      }
   }
}