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
    * @author Frank Yellin
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

      private static readonly ResultPoint[] NO_POINTS = new ResultPoint[0];

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
                    "  X       X   X     X X   X   X X   X X   X X X X X X   X X           X   X   X X \r\n" +
                    "X X   X X   X   X X X X   X X X X X X X X   X   X       X X   X X X X   X X X     \r\n" +
                    "  X       X   X     X       X X     X X   X   X   X     X X   X X X   X     X X X \r\n" +
                    "  X   X X X   X X       X X X         X X           X   X   X   X X X   X X     X \r\n" +
                    "    X     X   X X     X X X X     X   X     X X X X   X X   X X   X X X     X   X \r\n" +
                    "X X X   X             X         X X X X X   X   X X   X   X   X X   X   X   X   X \r\n" +
                    "          X       X X X   X X     X   X           X   X X X X   X X               \r\n" +
                    "  X     X X   X   X       X X X X X X X X X X X X X X X   X   X X   X   X X X     \r\n" +
                    "    X X                 X   X                       X X   X       X         X X X \r\n" +
                    "        X   X X   X X X X X X   X X X X X X X X X   X     X X           X X X X   \r\n" +
                    "          X X X   X     X   X   X               X   X X     X X X   X X           \r\n" +
                    "X X     X     X   X   X   X X   X   X X X X X   X   X X X X X X X       X   X X X \r\n" +
                    "X X X X       X       X   X X   X   X       X   X   X     X X X     X X       X X \r\n" +
                    "X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X   X \r\n" +
                    "    X     X       X         X   X   X       X   X   X     X   X X                 \r\n" +
                    "        X X     X X X X X   X   X   X X X X X   X   X X X     X X X X   X         \r\n" +
                    "X     X   X   X         X   X   X               X   X X   X X   X X X     X   X   \r\n" +
                    "  X   X X X   X   X X   X X X   X X X X X X X X X   X X         X X     X X X X   \r\n" +
                    "    X X   X   X   X X X     X                       X X X   X X   X   X     X     \r\n" +
                    "    X X X X   X         X   X X X X X X X X X X X X X X   X       X X   X X   X X \r\n" +
                    "            X   X   X X       X X X X X     X X X       X       X X X         X   \r\n" +
                    "X       X         X   X X X X   X     X X     X X     X X           X   X       X \r\n" +
                    "X     X       X X X X X     X   X X X X   X X X     X       X X X X   X   X X   X \r\n" +
                    "  X X X X X               X     X X X   X       X X   X X   X X X X     X X       \r\n" +
                    "X             X         X   X X   X X     X     X     X   X   X X X X             \r\n" +
                    "    X   X X       X     X       X   X X X X X X   X X   X X X X X X X X X   X   X \r\n" +
                    "    X         X X   X       X     X   X   X       X     X X X     X       X X X X \r\n" +
                    "X     X X     X X X X X X             X X X   X               X   X     X     X X \r\n" +
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
         for (int i = 0; i < 1000; i++)
         {
            testWriter("\u20AC 1 sample data.", "ISO-8859-1", 25, true, 2);
            testWriter("\u20AC 1 sample data.", "ISO-8859-15", 25, true, 2);
            testWriter("\u20AC 1 sample data.", "UTF-8", 25, true, 2);
            testWriter("\u20AC 1 sample data.", "UTF-8", 100, true, 3);
            testWriter("\u20AC 1 sample data.", "UTF-8", 300, true, 4);
            testWriter("\u20AC 1 sample data.", "UTF-8", 500, false, 5);
            // Test AztecWriter defaults
            const string data = "In ut magna vel mauris malesuada";
            var writer = new AztecWriter();
            var matrix = writer.encode(data, BarcodeFormat.AZTEC, 0, 0);
            var aztec = Internal.Encoder.encode(Encoding.GetEncoding("ISO8859-1").GetBytes(data),
                Internal.Encoder.DEFAULT_EC_PERCENT, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
            var expectedMatrix = aztec.Matrix;
            Assert.AreEqual(matrix, expectedMatrix);
         }
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
         testEncodeDecode("http://test/~!@#*^%&)__ ;:'\"[]{}\\|-+-=`1029384756<>/?abc"
                          + "Four score and seven our forefathers brought forth", false, 5);

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
                                   // 'A'  P/S   '. ' L/L    b    D/L    '.'
                                   "...X. ..... ...XX XXX.. ...XX XXXX. XX.X");
         testHighLevelEncodeString("Lorem ipsum.",
                                   // 'L'  L/L   'o'   'r'   'e'   'm'   ' '   'i'   'p'   's'   'u'   'm'   D/L   '.'
                                   ".XX.X XXX.. X.... X..XX ..XX. .XXX. ....X .X.X. X...X X.X.. X.XX. .XXX. XXXX. XX.X");
         testHighLevelEncodeString("Lo. Test 123.",
                                   // 'L'  L/L   'o'   P/S   '. '  U/S   'T'   'e'   's'   't'    D/L   ' '  '1'  '2'  '3'  '.'
                                   ".XX.X XXX.. X.... ..... ...XX XXX.. X.X.X ..XX. X.X.. X.X.X  XXXX. ...X ..XX .X.. .X.X XX.X");
         testHighLevelEncodeString("Lo...x",
                                   // 'L'  L/L   'o'   D/L   '.'  '.'  '.'  U/L  L/L   'x'
                                   ".XX.X XXX.. X.... XXXX. XX.X XX.X XX.X XXX. XXX.. XX..X");
         testHighLevelEncodeString(". x://abc/.",
                                   //P/S   '. '  L/L   'x'   P/S   ':'   P/S   '/'   P/S   '/'   'a'   'b'   'c'   P/S   '/'   D/L   '.'
                                   "..... ...XX XXX.. XX..X ..... X.X.X ..... X.X.. ..... X.X.. ...X. ...XX ..X.. ..... X.X.. XXXX. XX.X");
         // Uses Binary/Shift rather than Lower/Shift to save two bits.
         testHighLevelEncodeString("ABCdEFG",
                                   //'A'   'B'   'C'   B/S    =1    'd'     'E'   'F'   'G'
                                   "...X. ...XX ..X.. XXXXX ....X .XX..X.. ..XX. ..XXX .X...");

         testHighLevelEncodeString(
            // Found on an airline boarding pass.  Several stretches of Binary shift are
            // necessary to keep the bitcount so low.
            "09  UAG    ^160MEUCIQC0sYS/HpKxnBELR1uB85R20OoqqwFGa0q2uEi"
            + "Ygh6utAIgLl1aBVM4EOTQtMQQYH9M2Z3Dp4qnA/fwWuQ+M8L3V8U=",
            823);
      }

      [Test]
      public void testHighLevelEncodeBinary()
      {
         // binary short form single byte
         testHighLevelEncodeString("N\0N",
                                   // 'N'  B/S    =1   '\0'      N
                                   ".XXXX XXXXX ....X ........ .XXXX"); // Encode "N" in UPPER

         testHighLevelEncodeString("N\0n",
                                   // 'N'  B/S    =2   '\0'       'n'
                                   ".XXXX XXXXX ...X. ........ .XX.XXX."); // Encode "n" in BINARY

         // binary short form consecutive bytes
         testHighLevelEncodeString("N\0\u0080 A",
                                   // 'N'  B/S    =2    '\0'    \u0080   ' '  'A'
                                   ".XXXX XXXXX ...X. ........ X....... ....X ...X.");

         // binary skipping over single character
         testHighLevelEncodeString("\0a\u00FF\u0080 A",
                                   // B/S  =4    '\0'      'a'     '\3ff'   '\200'   ' '   'A'
                                   "XXXXX ..X.. ........ .XX....X XXXXXXXX X....... ....X ...X.");

         // getting into binary mode from digit mode
         testHighLevelEncodeString("1234\0",
                                   //D/L   '1'  '2'  '3'  '4'  U/L  B/S    =1    \0
                                   "XXXX. ..XX .X.. .X.X .XX. XXX. XXXXX ....X ........"
            );

         // Create a string in which every character requires binary
         StringBuilder sbBuild = new StringBuilder();
         for (int i = 0; i <= 3000; i++)
         {
            sbBuild.Append((char) (128 + (i%30)));
         }
         string sb = sbBuild.ToString();
         // Test the output generated by Binary/Switch, particularly near the
         // places where the encoding changes: 31, 62, and 2047+31=2078
         foreach (int i in new int[]
            {
               1, 2, 3, 10, 29, 30, 31, 32, 33,
               60, 61, 62, 63, 64, 2076, 2077, 2078, 2079, 2080, 2100
            })
         {
            // This is the expected length of a binary string of length "i"
            int expectedLength = (8*i) +
                                 ((i <= 31) ? 10 : (i <= 62) ? 20 : (i <= 2078) ? 21 : 31);
            // Verify that we are correct about the length.
            testHighLevelEncodeString(sb.Substring(0, i), expectedLength);
            if (i != 1 && i != 32 && i != 2079)
            {
               // The addition of an 'a' at the beginning or end gets merged into the binary code
               // in those cases where adding another binary character only adds 8 or 9 bits to the result.
               // So we exclude the border cases i=1,32,2079
               // A lower case letter at the beginning will be merged into binary mode
               testHighLevelEncodeString('a' + sb.Substring(0, i - 1), expectedLength);
               // A lower case letter at the end will also be merged into binary mode
               testHighLevelEncodeString(sb.Substring(0, i - 1) + 'a', expectedLength);
            }
            // A lower case letter at both ends will enough to latch us into LOWER.
            testHighLevelEncodeString('a' + sb.Substring(0, i) + 'b', expectedLength + 15);
         }
      }

      [Test]
      public void testHighLevelEncodePairs()
      {
         // Typical usage
         testHighLevelEncodeString("ABC. DEF\r\n",
                                   //  A     B    C    P/S   .<sp>   D    E     F    P/S   \r\n
                                   "...X. ...XX ..X.. ..... ...XX ..X.X ..XX. ..XXX ..... ...X.");

         // We should latch to PUNCT mode, rather than shift.  Also check all pairs
         testHighLevelEncodeString("A. : , \r\n",
                                   // 'A'    M/L   P/L   ". "  ": "   ", " "\r\n"
                                   "...X. XXX.X XXXX. ...XX ..X.X  ..X.. ...X.");

         // Latch to DIGIT rather than shift to PUNCT
         testHighLevelEncodeString("A. 1234",
                                   // 'A'  D/L   '.'  ' '  '1' '2'   '3'  '4'
                                   "...X. XXXX. XX.X ...X ..XX .X.. .X.X .X X."
            );
         // Don't bother leaving Binary Shift.
         //testHighLevelEncodeString("A\200. \200",
         //                          // 'A'  B/S    =2    \200      "."     " "     \200
         //                          "...X. XXXXX ..X.. X....... ..X.XXX. ..X..... X.......");
         //testHighLevelEncodeString("A" + 128 + ". " + 128,
         //   // 'A'  B/S    =2    \200      "."     " "     \200
         //                          "...X. XXXXX ..X.. X....... ..X.XXX. ..X..... X.......");
      }

      [Test]
      public void testUserSpecifiedLayers()
      {
         byte[] alphabet = LATIN_1.GetBytes("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
         AztecCode aztec = Internal.Encoder.encode(alphabet, 25, -2);
         Assert.AreEqual(2, aztec.Layers);
         Assert.IsTrue(aztec.isCompact);

         aztec = Internal.Encoder.encode(alphabet, 25, 32);
         Assert.AreEqual(32, aztec.Layers);
         Assert.IsFalse(aztec.isCompact);

         try
         {
            Internal.Encoder.encode(alphabet, 25, 33);
            Assert.Fail("Encode should have failed.  No such thing as 33 layers");
         }
         catch (ArgumentException expected)
         {
         }

         try
         {
            Internal.Encoder.encode(alphabet, 25, -1);
            Assert.Fail("Encode should have failed.  Text can't fit in 1-layer compact");
         }
         catch (ArgumentException expected)
         {
         }
      }

      [Test]
      public void testBorderCompact4Case()
      {
         // Compact(4) con hold 608 bits of information, but at most 504 can be data.  Rest must
         // be error correction
         const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
         // encodes as 26 * 5 * 4 = 520 bits of data
         const string alphabet4 = alphabet + alphabet + alphabet + alphabet;
         byte[] data = LATIN_1.GetBytes(alphabet4);
         try
         {
            Internal.Encoder.encode(data, 0, -4);
            Assert.Fail("Encode should have failed.  Text can't fit in 1-layer compact");
         }
         catch (ArgumentException expected)
         {
         }

         // If we just try to encode it normally, it will go to a non-compact 4 layer
         AztecCode aztecCode = Internal.Encoder.encode(data, 0, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
         Assert.IsFalse(aztecCode.isCompact);
         Assert.AreEqual(4, aztecCode.Layers);

         // But shortening the string to 100 bytes (500 bits of data), compact works fine, even if we
         // include more error checking.
         aztecCode = Internal.Encoder.encode(LATIN_1.GetBytes(alphabet4.Substring(0, 100)), 10, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
         Assert.IsTrue(aztecCode.isCompact);
         Assert.AreEqual(4, aztecCode.Layers);
      }

      // Helper routines

      private static void testEncode(String data, bool compact, int layers, String expected)
      {
         AztecCode aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), 33, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
         Assert.AreEqual(compact, aztec.isCompact, "Unexpected symbol format (compact)");
         Assert.AreEqual(layers, aztec.Layers, "Unexpected nr. of layers");
         BitMatrix matrix = aztec.Matrix;
         Assert.AreEqual(expected, matrix.ToString(), "encode() failed");
      }

      private static void testEncodeDecode(String data, bool compact, int layers)
      {
         AztecCode aztec = Internal.Encoder.encode(LATIN_1.GetBytes(data), 25, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
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
         var byteData = Encoding.GetEncoding(charset).GetBytes(data);
         var expectedData = LATIN_1.GetString(byteData, 0, byteData.Length);
         var hints = new Dictionary<EncodeHintType, Object>()
            ;
         hints[EncodeHintType.CHARACTER_SET] = charset;
         hints[EncodeHintType.ERROR_CORRECTION] = eccPercent;
         var writer = new AztecWriter();
         var matrix = writer.encode(data, BarcodeFormat.AZTEC, 0, 0, hints);
         var aztec = Internal.Encoder.encode(Encoding.GetEncoding(charset).GetBytes(data), eccPercent, Internal.Encoder.DEFAULT_AZTEC_LAYERS);
         Assert.AreEqual(compact, aztec.isCompact, "Unexpected symbol format (compact)");
         Assert.AreEqual(layers, aztec.Layers, "Unexpected nr. of layers");
         var matrix2 = aztec.Matrix;
         Assert.AreEqual(matrix, matrix2);
         var r = new AztecDetectorResult(matrix, NO_POINTS, aztec.isCompact, aztec.CodeWords, aztec.Layers);
         var res = new Internal.Decoder().decode(r);
         Assert.AreEqual(expectedData, res.Text);
         // Check error correction by introducing up to eccPercent/2 errors
         int ecWords = aztec.CodeWords * eccPercent / 100 / 2;
         var random = getPseudoRandom();
         for (int i = 0; i < ecWords; i++)
         {
            // don't touch the core
            int x = random.Next(1) > 0
                       ? random.Next(aztec.Layers*2)
                       : matrix.Width - 1 - random.Next(aztec.Layers*2);
            int y = random.Next(1) > 0
                       ? random.Next(aztec.Layers*2)
                       : matrix.Height - 1 - random.Next(aztec.Layers*2);
            matrix.flip(x, y);
         }
         r = new AztecDetectorResult(matrix, NO_POINTS, aztec.isCompact, aztec.CodeWords, aztec.Layers);
         res = new Internal.Decoder().decode(r);
         Assert.AreEqual(expectedData, res.Text);
      }

      private static Random getPseudoRandom()
      {
         return new Random(0x0EADBEEF);
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

      private static bool[] toBooleanArray(BitArray bitArray)
      {
         bool[] result = new bool[bitArray.Size];
         for (int i = 0; i < result.Length; i++)
         {
            result[i] = bitArray[i];
         }
         return result;
      }

      private static void testHighLevelEncodeString(String s, String expectedBits)
      {
         BitArray bits = new HighLevelEncoder(LATIN_1.GetBytes(s)).encode();
         String receivedBits = bits.ToString().Replace(" ", "");
         Assert.AreEqual(expectedBits.Replace(" ", ""), receivedBits, "highLevelEncode() failed for input string: " + s);
         Assert.AreEqual(s, Internal.Decoder.highLevelDecode(toBooleanArray(bits)));
      }

      private static void testHighLevelEncodeString(String s, int expectedReceivedBits)
      {
         BitArray bits = new HighLevelEncoder(LATIN_1.GetBytes(s)).encode();
         int receivedBitCount = bits.ToString().Replace(" ", "").Length;
         Assert.AreEqual(expectedReceivedBits, receivedBitCount, "highLevelEncode() failed for input string: " + s);
         Assert.AreEqual(s, Internal.Decoder.highLevelDecode(toBooleanArray(bits)));
      }
   }
}