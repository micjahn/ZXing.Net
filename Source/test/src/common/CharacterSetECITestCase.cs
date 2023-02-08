/*
 * Copyright 2012 ZXing.Net authors
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

namespace ZXing.Common.Test
{
    using System;
    using System.Text;

    using NUnit.Framework;

    [TestFixture]
   public sealed class CharacterSetECITestCase
   {
      [Test]
      public void CharacterSetECI_Should_Return_Usable_Charactersets()
      {
         var errors = String.Empty;
         foreach (var charSetEntry in CharacterSetECI.VALUE_TO_ECI)
         {
            try
            {
               Encoding.GetEncoding(charSetEntry.Value.EncodingName);
            }
            catch (Exception exc)
            {
               errors += exc.Message + Environment.NewLine;
            }
         }
         Assert.IsEmpty(errors);
      }

        [TestCase(0, "CP437", null)]
        [TestCase(1, "ISO-8859-1", null)]
        [TestCase(4, "ISO-8859-2", null)]
        [TestCase(5, "ISO-8859-3", null)]
        [TestCase(6, "ISO-8859-4", null)]
        [TestCase(7, "ISO-8859-5", null)]
        [TestCase(8, "ISO-8859-6", null)]
        [TestCase(9, "ISO-8859-7", null)]
        [TestCase(10, "ISO-8859-8", null)]
        [TestCase(11, "ISO-8859-9", null)]
        [TestCase(12, "ISO-8859-10", "ISO-8859-4", ExpectedException = typeof(ArgumentException))] // ISO-8859-10 isn't supported by .Net
        [TestCase(13, "ISO-8859-11", null)]
        // [TestCase(14, "", null)]
        [TestCase(15, "ISO-8859-13", null)]
        [TestCase(16, "ISO-8859-14", "ISO-8859-1", ExpectedException = typeof(ArgumentException))] // ISO-8859-14 isn't supported by .Net
        [TestCase(17, "ISO-8859-15", null)]
        [TestCase(18, "ISO-8859-16", "ISO-8859-3", ExpectedException = typeof(ArgumentException))] // ISO-8859-16 isn't supported by .Net
        // [TestCase(19, "", null)]
        [TestCase(20, "SJIS", null)]
        [TestCase(21, "WINDOWS-1250", null)]
        [TestCase(22, "WINDOWS-1251", null)]
        [TestCase(23, "WINDOWS-1252", null)]
        [TestCase(24, "WINDOWS-1256", null)]
        [TestCase(25, "UTF-16BE", null)]
        [TestCase(26, "UTF-8", null)]
        [TestCase(27, "US-ASCII", null)]
        [TestCase(28, "BIG5", null)]
        [TestCase(29, "GB18030", null)]
        [TestCase(30, "EUC-KR", null)]
        public void Should_Resolve_Correct_ECI(int eciValue, string encodingName, string alternativeNameIfNotSupportedByPlatform)
        {
            // original Java values
            // // Enum name is a Java encoding valid for java.lang and java.io
            // Cp437(new int[] { 0, 2 }),
            // ISO8859_1(new int[] { 1, 3 }, "ISO-8859-1"),
            // ISO8859_2(4, "ISO-8859-2"),
            // ISO8859_3(5, "ISO-8859-3"),
            // ISO8859_4(6, "ISO-8859-4"),
            // ISO8859_5(7, "ISO-8859-5"),
            // // ISO8859_6(8, "ISO-8859-6"),
            // ISO8859_7(9, "ISO-8859-7"),
            // // ISO8859_8(10, "ISO-8859-8"),
            // ISO8859_9(11, "ISO-8859-9"),
            // // ISO8859_10(12, "ISO-8859-10"),
            // // ISO8859_11(13, "ISO-8859-11"),
            // ISO8859_13(15, "ISO-8859-13"),
            // // ISO8859_14(16, "ISO-8859-14"),
            // ISO8859_15(17, "ISO-8859-15"),
            // ISO8859_16(18, "ISO-8859-16"),
            // SJIS(20, "Shift_JIS"),
            // Cp1250(21, "windows-1250"),
            // Cp1251(22, "windows-1251"),
            // Cp1252(23, "windows-1252"),
            // Cp1256(24, "windows-1256"),
            // UnicodeBigUnmarked(25, "UTF-16BE", "UnicodeBig"),
            // UTF8(26, "UTF-8"),
            // ASCII(new int[] { 27, 170 }, "US-ASCII"),
            // Big5(28),
            // GB18030(29, "GB2312", "EUC_CN", "GBK"),
            // EUC_KR(30, "EUC-KR");

            // Table from Wikipedia
            // ECI indicator        Code page or encoding   Notes
            // \000000, \000002     Code page 437
            // \000001, \000003     ISO / IEC 8859-1        Latin - 1
            // \000004              ISO / IEC 8859-2        Latin - 2
            // \000005              ISO / IEC 8859-3        Latin - 3
            // \000006              ISO / IEC 8859-4        Latin - 4
            // \000007              ISO / IEC 8859-5        Latin / Cyrillic
            // \000008              ISO / IEC 8859-6        Latin / Arabic
            // \000009              ISO / IEC 8859-7        Latin / Greek
            // \000010              ISO / IEC 8859-8        Latin / Hebrew
            // \000011              ISO / IEC 8859-9        Latin - 5
            // \000012              ISO / IEC 8859-10       Latin - 6
            // \000013              ISO / IEC 8859-11       Latin / Thai
            // \000015              ISO / IEC 8859-13       Latin - 7
            // \000016              ISO / IEC 8859-14       Latin - 8(Celtic)
            // \000017              ISO / IEC 8859-15       Latin - 9
            // \000018              ISO / IEC 8859-16       Latin - 10
            // \000020              Shift JIS
            // \000021              Windows-1250            Superset of Latin - 2
            // \000022              Windows-1251            Latin / Cyrillic
            // \000023              Windows-1252            Superset of Latin - 1
            // \000024              Windows-1256            Arabic
            // \000025              UTF-16                  Big endian
            // \000026              UTF-8
            // \000027              US-ASCII
            // \000028              Big5
            // \000029              GB 18030
            // \000030              EUC-KR

            var eciValueFrom = CharacterSetECI.getCharacterSetECIByName(encodingName);
            Assert.That(eciValueFrom.Value, Is.EqualTo(eciValue));

            var encodingNameFrom = CharacterSetECI.getCharacterSetECIByValue(eciValue);
            Assert.That(encodingNameFrom.EncodingName, Is.EqualTo(encodingName).Or.EqualTo(alternativeNameIfNotSupportedByPlatform));

            // Check name against available encodings
            var encoding = System.Text.Encoding.GetEncoding(encodingName);

            var eciFromEncodingName = CharacterSetECI.getCharacterSetECIByName(encoding.WebName);
            Assert.That(eciFromEncodingName, Is.Not.Null);
        }
    }
}
