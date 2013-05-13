using System.Text;

using NUnit.Framework;

using ZXing.PDF417.Internal;

namespace ZXing.PDF417.Test
{
   [TestFixture]
   public sealed class PDF417DecodedBitStreamParserTestCase
   {
      [Test]
      public void test_issue_599()
      {
         var codewords = new[]
                            {
                               48, 901, 56, 141, 627, 856, 330, 69, 244, 900, 852, 169, 843, 895, 852, 895, 913, 154, 845,
                               778, 387, 89, 869, 901, 219, 474, 543, 650, 169, 201, 9, 160, 35, 70, 900, 900, 900, 900,
                               900, 900, 900, 900, 900, 900, 900, 900, 900, 900, 769, 843, 591, 910, 605, 206, 706, 917,
                               371, 469, 79, 718, 47, 777, 249, 262, 193, 620, 597, 477, 450, 806, 908, 309, 153, 871, 686
                               , 838, 185, 674, 68, 679, 691, 794, 497, 479, 234, 250, 496, 43, 347, 582, 882, 536, 322,
                               317, 273, 194, 917, 237, 420, 859, 340, 115, 222, 808, 866, 836, 417, 121, 833, 459, 64,
                               159
                            };
         var expectedResultBytes = new byte[]
                                 {
                                    0x21, 0x82, 0x9f, 0x09, 0x21, 0x1a, 0x45, 0xf4, 0x09, 0x35, 0x2f, 0x44, 0x3f, 0x09, 0x3f, 0x9a,
                                    0x46, 0x5a, 0x09, 0x63, 0x27, 0x82, 0xff, 0x09, 0x6d, 0x15, 0x41, 0xc9, 0x09, 0xa0, 0x23, 0x46
                                 };
#if !SILVERLIGHT
         var expectedResult = Encoding.UTF7.GetString(expectedResultBytes);
#else
         var expectedResult = Encoding.GetEncoding("UTF-7").GetString(expectedResultBytes, 0, expectedResultBytes.Length);
#endif
         var result = DecodedBitStreamParser.decode(codewords, "L");
         Assert.AreEqual(expectedResult, result.Text);
      }
   }
}
