/*
 * Copyright 2013 ZXing authors
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
using System.Text;
using System.Text.RegularExpressions;

using NUnit.Framework;

using ZXing.Aztec.Internal;
using ZXing.Common;

namespace ZXing.Aztec.Test
{
   /**
    * Aztec 2D generator unit tests.
    *
    * @author Rustam Abdullaev
    */
   [TestFixture]
   public sealed class EncoderTest
   {
      private static readonly Encoding LATIN_1 = Encoding.GetEncoding("ISO-8859-1");
      private static readonly Regex DOTX = new Regex("[^.X]"
#if !SILVERLIGHT
, RegexOptions.Compiled
#endif
);
      public static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

      // real life tests

      [Test]
      public void testEncode1()
      {
         testEncode("This is an example Aztec symbol for Wikipedia.", true, 3,
             "X     X X       X     X X     X     X         \r\n" +
             "X         X     X X     X   X X   X X       X \r\n" +
             "X X   X X X X X   X X X                 X     \r\n" +
             "X X                 X X   X       X X X X X X \r\n" +
             "    X X X   X   X     X X X X         X X     \r\n" +
             "  X X X   X X X X   X     X   X     X X   X   \r\n" +
             "        X X X X X     X X X X   X   X     X   \r\n" +
             "X       X   X X X X X X X X X X X     X   X X \r\n" +
             "X   X     X X X               X X X X   X X   \r\n" +
             "X     X X   X X   X X X X X   X X   X   X X X \r\n" +
             "X   X         X   X       X   X X X X       X \r\n" +
             "X       X     X   X   X   X   X   X X   X     \r\n" +
             "      X   X X X   X       X   X     X X X     \r\n" +
             "    X X X X X X   X X X X X   X X X X X X   X \r\n" +
             "  X X   X   X X               X X X   X X X X \r\n" +
             "  X   X       X X X X X X X X X X X X   X X   \r\n" +
             "  X X   X       X X X   X X X       X X       \r\n" +
             "  X               X   X X     X     X X X     \r\n" +
             "  X   X X X   X X   X   X X X X   X   X X X X \r\n" +
             "    X   X   X X X   X   X   X X X X     X     \r\n" +
             "        X               X                 X   \r\n" +
             "        X X     X   X X   X   X   X       X X \r\n" +
             "  X   X   X X       X   X         X X X     X \r\n");
      }

      [Test]
      public void testEncode2()
      {
         testEncode("Aztec Code is a public domain 2D matrix barcode symbology" +
              " of nominally square symbols built on a square grid with a " +
              "distinctive square bullseye pattern at their center.", false, 6,
             "        X X     X X     X     X     X   X X X         X   X         X   X X       \r\n" +
             "  X       X X     X   X X   X X       X             X     X   X X   X           X \r\n" +
             "  X   X X X     X   X   X X     X X X   X   X X               X X       X X     X \r\n" +
             "X X X             X   X         X         X     X     X   X     X X       X   X   \r\n" +
             "X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X \r\n" +
             "    X X   X   X   X X X               X       X       X X     X X   X X       X   \r\n" +
             "X X     X       X       X X X X   X   X X       X   X X   X       X X   X X   X   \r\n" +
             "  X       X   X         X     X   X         X X       X         X     X   X   X X \r\n" +
             "X X   X X   X   X   X X       X X     X X     X X X   X X   X X   X X   X X X     \r\n" +
             "  X       X   X   X X     X X   X X         X X X   X     X     X X   X     X X X \r\n" +
             "  X   X X X   X X X   X   X X   X   X   X X   X X   X X X X X   X X X   X X     X \r\n" +
             "    X     X   X X   X   X X X X       X       X       X X X         X X     X   X \r\n" +
             "X X X   X           X X X X     X X X X X X X X   X       X X X     X   X   X   X \r\n" +
             "          X       X   X X X X     X   X           X   X X       X                 \r\n" +
             "  X     X X   X   X X   X X X X X X X X X X X X X X X X   X X       X   X X X     \r\n" +
             "    X X           X X       X                       X X X X X X             X X X \r\n" +
             "        X   X X   X X X   X X   X X X X X X X X X   X   X               X X X X   \r\n" +
             "          X X X       X     X   X               X   X X   X       X X X           \r\n" +
             "X X     X     X   X     X X X   X   X X X X X   X   X X       X         X   X X X \r\n" +
             "X X X X       X     X   X X X   X   X       X   X   X       X X X   X X       X X \r\n" +
             "X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X \r\n" +
             "    X     X       X     X   X   X   X       X   X   X       X                     \r\n" +
             "        X X     X X X X X   X   X   X X X X X   X   X X X     X     X   X         \r\n" +
             "X     X   X   X   X X X X   X   X               X   X X X   X X     X     X   X   \r\n" +
             "  X   X X X   X     X X X X X   X X X X X X X X X   X X X X X           X X X X   \r\n" +
             "    X X   X   X     X X     X                       X X X X       X   X     X     \r\n" +
             "    X X X X   X       X     X X X X X X X X X X X X X X       X     X   X X   X X \r\n" +
             "            X   X X     X     X X X X X     X X X       X X X X X   X         X   \r\n" +
             "X       X         X           X X   X X X X   X X   X X X     X X   X   X       X \r\n" +
             "X     X       X X     X     X X     X             X X   X       X     X   X X     \r\n" +
             "  X X X X X       X   X     X           X     X   X X X X   X X X X     X X   X X \r\n" +
             "X             X   X X X     X X       X       X X   X   X X     X X X         X X \r\n" +
             "    X   X X       X     X       X   X X X X X X   X X   X X X X X X X X X   X X   \r\n" +
             "    X         X X   X       X     X   X   X       X     X X X     X       X X     \r\n" +
             "X     X X     X X X X X X             X X X   X               X   X     X       X \r\n" +
             "X   X X     X               X X X X X     X X     X X X X X X X X     X   X   X X \r\n" +
             "X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X \r\n" +
             "X           X     X X X X     X     X         X         X   X       X X   X X X   \r\n" +
             "X   X   X X   X X X   X         X X     X X X X     X X   X   X     X   X       X \r\n" +
             "      X     X     X     X X     X   X X   X X   X         X X       X       X   X \r\n" +
             "X       X           X   X   X     X X   X               X     X     X X X         \r\n");
      }

      [Test]
      public void testAztecWriter()
      {
         testWriter("\u20AC 1 sample data.", "ISO-8859-1", 25, true, 2);
         testWriter("\u20AC 1 sample data.", "ISO-8859-15", 25, true, 2);
         testWriter("\u20AC 1 sample data.", "UTF-8", 25, true, 2);
         testWriter("\u20AC 1 sample data.", "UTF-8", 100, true, 3);
         testWriter("\u20AC 1 sample data.", "UTF-8", 300, true, 4);
         testWriter("\u20AC 1 sample data.", "UTF-8", 500, false, 5);
         // Test AztecWriter defaults
         var data = "In ut magna vel mauris malesuada";
         var writer = new AztecWriter();
         var matrix = writer.encode(data, BarcodeFormat.AZTEC, 0, 0);
         var aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), Internal.Encoder.DEFAULT_EC_PERCENT);
         var expectedMatrix = aztec.Matrix;
         Assert.AreEqual(matrix, expectedMatrix);
      }

      // synthetic tests (encode-decode round-trip)

      [Test]
      public void testEncodeDecode1()
      {
         testEncodeDecode("Abc123!", true, 1);
      }

      [Test]
      public void testEncodeDecode2()
      {
         testEncodeDecode("Lorem ipsum. http://test/", true, 2);
      }

      [Test]
      public void testEncodeDecode3()
      {
         testEncodeDecode("AAAANAAAANAAAANAAAANAAAANAAAANAAAANAAAANAAAANAAAAN", true, 3);
      }

      [Test]
      public void testEncodeDecode4()
      {
         testEncodeDecode("http://test/~!@#*^%&)__ ;:'\"[]{}\\|-+-=`1029384", true, 4);
      }

      [Test]
      public void testEncodeDecode5()
      {
         testEncodeDecode("http://test/~!@#*^%&)__ ;:'\"[]{}\\|-+-=`1029384756<>/?abc", false, 5);
      }

      [Test]
      public void testEncodeDecode10()
      {
         testEncodeDecode("In ut magna vel mauris malesuada dictum. Nulla ullamcorper metus quis diam" +
             " cursus facilisis. Sed mollis quam id justo rutrum sagittis. Donec laoreet rutrum" +
             " est, nec convallis mauris condimentum sit amet. Phasellus gravida, justo et congue" +
             " auctor, nisi ipsum viverra erat, eget hendrerit felis turpis nec lorem. Nulla" +
             " ultrices, elit pellentesque aliquet laoreet, justo erat pulvinar nisi, id" +
             " elementum sapien dolor et diam.", false, 10);
      }

      [Test]
      public void testEncodeDecode23()
      {
         testEncodeDecode("In ut magna vel mauris malesuada dictum. Nulla ullamcorper metus quis diam" +
             " cursus facilisis. Sed mollis quam id justo rutrum sagittis. Donec laoreet rutrum" +
             " est, nec convallis mauris condimentum sit amet. Phasellus gravida, justo et congue" +
             " auctor, nisi ipsum viverra erat, eget hendrerit felis turpis nec lorem. Nulla" +
             " ultrices, elit pellentesque aliquet laoreet, justo erat pulvinar nisi, id" +
             " elementum sapien dolor et diam. Donec ac nunc sodales elit placerat eleifend." +
             " Sed ornare luctus ornare. Vestibulum vehicula, massa at pharetra fringilla, risus" +
             " justo faucibus erat, nec porttitor nibh tellus sed est. Ut justo diam, lobortis eu" +
             " tristique ac, p.In ut magna vel mauris malesuada dictum. Nulla ullamcorper metus" +
             " quis diam cursus facilisis. Sed mollis quam id justo rutrum sagittis. Donec" +
             " laoreet rutrum est, nec convallis mauris condimentum sit amet. Phasellus gravida," +
             " justo et congue auctor, nisi ipsum viverra erat, eget hendrerit felis turpis nec" +
             " lorem. Nulla ultrices, elit pellentesque aliquet laoreet, justo erat pulvinar" +
             " nisi, id elementum sapien dolor et diam. Donec ac nunc sodales elit placerat" +
             " eleifend. Sed ornare luctus ornare. Vestibulum vehicula, massa at pharetra" +
             " fringilla, risus justo faucibus erat, nec porttitor nibh tellus sed est. Ut justo" +
             " diam, lobortis eu tristique ac, p. In ut magna vel mauris malesuada dictum. Nulla" +
             " ullamcorper metus quis diam cursus facilisis. Sed mollis quam id justo rutrum" +
             " sagittis. Donec laoreet rutrum est, nec convallis mauris condimentum sit amet." +
             " Phasellus gravida, justo et congue auctor, nisi ipsum viverra erat, eget hendrerit" +
             " felis turpis nec lorem. Nulla ultrices, elit pellentesque aliquet laoreet, justo" +
             " erat pulvinar nisi, id elementum sapien dolor et diam.", false, 23);
      }

      [Test]
      public void testEncodeDecode31()
      {
         testEncodeDecode("In ut magna vel mauris malesuada dictum. Nulla ullamcorper metus quis diam" +
           " cursus facilisis. Sed mollis quam id justo rutrum sagittis. Donec laoreet rutrum" +
           " est, nec convallis mauris condimentum sit amet. Phasellus gravida, justo et congue" +
           " auctor, nisi ipsum viverra erat, eget hendrerit felis turpis nec lorem. Nulla" +
           " ultrices, elit pellentesque aliquet laoreet, justo erat pulvinar nisi, id" +
           " elementum sapien dolor et diam. Donec ac nunc sodales elit placerat eleifend." +
           " Sed ornare luctus ornare. Vestibulum vehicula, massa at pharetra fringilla, risus" +
           " justo faucibus erat, nec porttitor nibh tellus sed est. Ut justo diam, lobortis eu" +
           " tristique ac, p.In ut magna vel mauris malesuada dictum. Nulla ullamcorper metus" +
           " quis diam cursus facilisis. Sed mollis quam id justo rutrum sagittis. Donec" +
           " laoreet rutrum est, nec convallis mauris condimentum sit amet. Phasellus gravida," +
           " justo et congue auctor, nisi ipsum viverra erat, eget hendrerit felis turpis nec" +
           " lorem. Nulla ultrices, elit pellentesque aliquet laoreet, justo erat pulvinar" +
           " nisi, id elementum sapien dolor et diam. Donec ac nunc sodales elit placerat" +
           " eleifend. Sed ornare luctus ornare. Vestibulum vehicula, massa at pharetra" +
           " fringilla, risus justo faucibus erat, nec porttitor nibh tellus sed est. Ut justo" +
           " diam, lobortis eu tristique ac, p. In ut magna vel mauris malesuada dictum. Nulla" +
           " ullamcorper metus quis diam cursus facilisis. Sed mollis quam id justo rutrum" +
           " sagittis. Donec laoreet rutrum est, nec convallis mauris condimentum sit amet." +
           " Phasellus gravida, justo et congue auctor, nisi ipsum viverra erat, eget hendrerit" +
           " felis turpis nec lorem. Nulla ultrices, elit pellentesque aliquet laoreet, justo" +
           " erat pulvinar nisi, id elementum sapien dolor et diam. Donec ac nunc sodales elit" +
           " placerat eleifend. Sed ornare luctus ornare. Vestibulum vehicula, massa at" +
           " pharetra fringilla, risus justo faucibus erat, nec porttitor nibh tellus sed est." +
           " Ut justo diam, lobortis eu tristique ac, p.In ut magna vel mauris malesuada" +
           " dictum. Nulla ullamcorper metus quis diam cursus facilisis. Sed mollis quam id" +
           " justo rutrum sagittis. Donec laoreet rutrum est, nec convallis mauris condimentum" +
           " sit amet. Phasellus gravida, justo et congue auctor, nisi ipsum viverra erat," +
           " eget hendrerit felis turpis nec lorem. Nulla ultrices, elit pellentesque aliquet" +
           " laoreet, justo erat pulvinar nisi, id elementum sapien dolor et diam. Donec ac" +
           " nunc sodales elit placerat eleifend. Sed ornare luctus ornare. Vestibulum vehicula," +
           " massa at pharetra fringilla, risus justo faucibus erat, nec porttitor nibh tellus" +
           " sed est. Ut justo diam, lobortis eu tris. In ut magna vel mauris malesuada dictum." +
           " Nulla ullamcorper metus quis diam cursus facilisis. Sed mollis quam id justo rutrum" +
           " sagittis. Donec laoreet rutrum est, nec convallis mauris condimentum sit amet." +
           " Phasellus gravida, justo et congue auctor, nisi ipsum viverra erat, eget" +
           " hendrerit felis turpis nec lorem.", false, 31);
      }

      [Test]
      public void testGenerateModeMessage()
      {
         testModeMessage(true, 2, 29, ".X .XXX.. ...X XX.. ..X .XX. .XX.X");
         testModeMessage(true, 4, 64, "XX XXXXXX .X.. ...X ..XX .X.. XX..");
         testModeMessage(false, 21, 660, "X.X.. .X.X..X..XX .XXX ..X.. .XXX. .X... ..XXX");
         testModeMessage(false, 32, 4096, "XXXXX XXXXXXXXXXX X.X. ..... XXX.X ..X.. X.XXX");
      }

      [Test]
      public void testStuffBits()
      {
         testStuffBits(5, ".X.X. X.X.X .X.X.",
             ".X.X. X.X.X .X.X.");
         testStuffBits(5, ".X.X. ..... .X.X",
             ".X.X. ....X ..X.X");
         testStuffBits(3, "XX. ... ... ..X XXX .X. ..",
             "XX. ..X ..X ..X ..X .XX XX. .X. ..X");
         testStuffBits(6, ".X.X.. ...... ..X.XX",
             ".X.X.. .....X. ..X.XX XXXX.");
         testStuffBits(6, ".X.X.. ...... ...... ..X.X.",
             ".X.X.. .....X .....X ....X. X.XXXX");
         testStuffBits(6, ".X.X.. XXXXXX ...... ..X.XX",
             ".X.X.. XXXXX. X..... ...X.X XXXXX.");
         testStuffBits(6,
             "...... ..XXXX X..XX. .X.... .X.X.X .....X .X.... ...X.X .....X ....XX ..X... ....X. X..XXX X.XX.X",
             ".....X ...XXX XX..XX ..X... ..X.X. X..... X.X... ....X. X..... X....X X..X.. .....X X.X..X XXX.XX .XXXXX");
      }

      [Test]
      public void testHighLevelEncode()
      {
         testHighLevelEncodeString("A. b.",
             "...X. ..... ...XX XXX.. ...XX XXXX. XX.X");
         testHighLevelEncodeString("Lorem ipsum.",
             ".XX.X XXX.. X.... X..XX ..XX. .XXX. ....X .X.X. X...X X.X.. X.XX. .XXX. XXXX. XX.X");
         testHighLevelEncodeString("Lo. Test 123.",
             ".XX.X XXX.. X.... ..... ...XX XXX.. X.X.X ..XX. X.X.. X.X.X ....X XXXX. ..XX .X.. .X.X XX.X");
         testHighLevelEncodeString("Lo...x",
             ".XX.X XXX.. X.... XXXX. XX.X XX.X XX.X XXX. XXX.. XX..X");
         testHighLevelEncodeString(". x://abc/.",
             "..... ...XX XXX.. XX..X ..... X.X.X ..... X.X.. ..... X.X.. ...X. ...XX ..X.. ..... X.X.. XXXX. XX.X");
      }

      [Test]
      public void testHighLevelEncodeBinary()
      {
         // binary short form single byte
         testHighLevelEncodeString("N\0N",
             ".XXXX XXXXX ...X. ........ .X..XXX.");
         // binary short form consecutive bytes
         testHighLevelEncodeString("N\0\u0080 A",
             ".XXXX XXXXX ...X. ........ X....... ....X ...X.");
         // binary skipping over single character
         testHighLevelEncodeString("\0a\u00FF\u0080 A",
             "XXXXX ..X.. ........ .XX....X XXXXXXXX X....... ....X ...X.");
         // binary long form optimization into 2 short forms (saves 1 bit)
         testHighLevelEncodeString(
             "\0\0\0\0 \0\0\0\0 \0\0\0\0 \0\0\0\0 \0\0\0\0 \0\0\0\0 \u0082\u0084\u0088\0 \0\0\0\0 \0\0\0\0 ",
             "XXXXX XXXXX ........ ........ ........ ........ ..X....." +
             " ........ ........ ........ ........ ..X....." +
             " ........ ........ ........ ........ ..X....." +
             " ........ ........ ........ ........ ..X....." +
             " ........ ........ ........ ........ ..X....." +
             " ........ ........ ........ ........ ..X....." +
             " X.....X. XXXXX .XXX. X....X.. X...X... ........ ..X....." +
             " ........ ........ ........ ........ ..X....." +
             " ........ ........ ........ ........ ..X.....");
         // binary long form
         testHighLevelEncodeString(
             "\0\0\0\0 \0\0\u0001\0 \0\0\u0002\0 \0\0\u0003\0 \0\0\u0004\0 \0\0\u0005\0 \0\0\u0006\0 \0\0\u0007\0 \0\0\u0008\0 \0\0\u0009\0 \0\0\u00F0\0 \0\0\u00F1\0 \0\0\u00F2\0A",
             "XXXXX ..... .....X...X. ........ ........ ........ ........ ..X....." +
             " ........ ........ .......X ........ ..X....." +
             " ........ ........ ......X. ........ ..X....." +
             " ........ ........ ......XX ........ ..X....." +
             " ........ ........ .....X.. ........ ..X....." +
             " ........ ........ .....X.X ........ ..X....." +
             " ........ ........ .....XX. ........ ..X....." +
             " ........ ........ .....XXX ........ ..X....." +
             " ........ ........ ....X... ........ ..X....." +
             " ........ ........ ....X..X ........ ..X....." +
             " ........ ........ XXXX.... ........ ..X....." +
             " ........ ........ XXXX...X ........ ..X....." +
             " ........ ........ XXXX..X. ........ .X.....X");
      }

      // Helper routines

      private static void testEncode(String data, bool compact, int layers, String expected)
      {
         AztecCode aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), 33);
         Assert.AreEqual(compact, aztec.isCompact, "Unexpected symbol format (compact)");
         Assert.AreEqual(layers, aztec.Layers, "Unexpected nr. of layers");
         BitMatrix matrix = aztec.Matrix;
         Assert.AreEqual(expected, matrix.ToString(), "encode() failed");
      }

      private static void testEncodeDecode(String data, bool compact, int layers)
      {
         AztecCode aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), 25);
         Assert.AreEqual(compact, aztec.isCompact, "Unexpected symbol format (compact)");
         Assert.AreEqual(layers, aztec.Layers, "Unexpected nr. of layers");
         BitMatrix matrix = aztec.Matrix;
         AztecDetectorResult r =
             new AztecDetectorResult(matrix, NO_POINTS, aztec.isCompact, aztec.CodeWords, aztec.Layers);
         DecoderResult res = new Internal.Decoder().decode(r);
         Assert.AreEqual(data, res.Text);
         // Check error correction by introducing a few minor errors
         Random random = getPseudoRandom();
         matrix.flip(random.Next(matrix.Width), random.Next(2));
         matrix.flip(random.Next(matrix.Width), matrix.Height - 2 + random.Next(2));
         matrix.flip(random.Next(2), random.Next(matrix.Height));
         matrix.flip(matrix.Width - 2 + random.Next(2), random.Next(matrix.Height));
         r = new AztecDetectorResult(matrix, NO_POINTS, aztec.isCompact, aztec.CodeWords, aztec.Layers);
         res = new Internal.Decoder().decode(r);
         Assert.AreEqual(data, res.Text);
      }


      private static void testWriter(String data,
                                     String charset,
                                     int eccPercent,
                                     bool compact,
                                     int layers)
      {
         // 1. Perform an encode-decode round-trip because it can be lossy.
         // 2. Aztec Decoder currently always decodes the data with a LATIN-1 charset:
         var expectedData = LATIN_1.GetString(Encoding.GetEncoding(charset).GetBytes(data));
         var hints = new Dictionary<EncodeHintType, Object>()
         ;
         hints[EncodeHintType.CHARACTER_SET] = charset;
         hints[EncodeHintType.ERROR_CORRECTION] = eccPercent;
         var writer = new AztecWriter();
         var matrix = writer.encode(data, BarcodeFormat.AZTEC, 0, 0, hints);
         var aztec = Internal.Encoder.encode(Encoding.GetEncoding(charset).GetBytes(data), eccPercent);
         Assert.AreEqual(compact, aztec.isCompact, "Unexpected symbol format (compact)");
         Assert.AreEqual(layers, aztec.Layers, "Unexpected nr. of layers");
         var matrix2 = aztec.Matrix;
         Assert.AreEqual(matrix, matrix2);
         var r = new AztecDetectorResult(matrix, NO_POINTS, aztec.isCompact, aztec.CodeWords, aztec.Layers);
         var res = new Internal.Decoder().decode(r);
         Assert.AreEqual(expectedData, res.Text);
         // Check error correction by introducing up to eccPercent errors
         int ecWords = aztec.CodeWords * eccPercent / 100;
         var random = getPseudoRandom();
         for (int i = 0; i < ecWords; i++)
         {
            // don't touch the core
            int x = random.Next(1) > 0
                       ? random.Next(aztec.Layers * 2)
                       : matrix.Width - 1 - random.Next(aztec.Layers * 2);
            int y = random.Next(1) > 0
                       ? random.Next(aztec.Layers * 2)
                       : matrix.Height - 1 - random.Next(aztec.Layers * 2);
            matrix.flip(x, y);
         }
         r = new AztecDetectorResult(matrix, NO_POINTS, aztec.isCompact, aztec.CodeWords, aztec.Layers);
         res = new Internal.Decoder().decode(r);
         Assert.AreEqual(expectedData, res.Text);
      }

      static Random getPseudoRandom()
      {
         //return new SecureRandom(new byte[] {(byte) 0xDE, (byte) 0xAD, (byte) 0xBE, (byte) 0xEF});
         return new Random((int)DateTime.Now.Ticks);
      }

      private static void testModeMessage(bool compact, int layers, int words, String expected)
      {
         BitArray @in = Internal.Encoder.generateModeMessage(compact, layers, words);
         Assert.AreEqual(expected.Replace(" ", ""), @in.ToString().Replace(" ", ""), "generateModeMessage() failed");
      }

      private static void testStuffBits(int wordSize, String bits, String expected)
      {
         BitArray @in = toBitArray(bits);
         BitArray stuffed = Internal.Encoder.stuffBits(@in, wordSize);
         Assert.AreEqual(expected.Replace(" ", ""),
                         stuffed.ToString().Replace(" ", ""), "stuffBits() failed for input string: " + bits);
      }

      private static BitArray toBitArray(string bits)
      {
         var @in = new BitArray();
         var str = DOTX.Replace(bits, "");
         foreach (char aStr in str)
         {
            @in.appendBit(aStr == 'X');
         }
         return @in;
      }

      private static void testHighLevelEncodeString(String s, String expectedBits)
      {
         BitArray bits = Internal.Encoder.highLevelEncode(LATIN_1.GetBytes(s));
         String receivedBits = bits.ToString().Replace(" ", "");
         Assert.AreEqual(expectedBits.Replace(" ", ""), receivedBits, "highLevelEncode() failed for input string: " + s);
      }
   }
}