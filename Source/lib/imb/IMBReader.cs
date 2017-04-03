/*
 * Copyright 2014 ZXing.Net authors
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
#if (SILVERLIGHT4 || SILVERLIGHT5 || NET40 || NET45 || NETFX_CORE || NETSTANDARD) && !NETSTANDARD1_0
using System.Numerics;
#else
using BigIntegerLibrary;
#endif
using System.Text;

using ZXing.Common;
using ZXing.OneD;

namespace ZXing.IMB
{
   /// <summary>
   /// implements an Intelligent Mail barcode
   /// <author>Rishabh Hatgadkar</author>
   /// </summary>
   public sealed class IMBReader : OneDReader
   {
      private const int NUM_BARS_IMB = 65;

      private static readonly int[] barPosA = new[] {2, 6, 13, 16, 21, 30, 34, 40, 45, 48, 52, 56, 62};
      private static readonly int[] barPosB = new[] {22, 18, 39, 41, 11, 57, 54, 50, 7, 32, 2, 62, 26};
      private static readonly int[] barPosC = new[] {40, 35, 57, 52, 49, 7, 24, 17, 3, 63, 29, 44, 12};
      private static readonly int[] barPosD = new[] {47, 5, 35, 39, 30, 42, 15, 60, 20, 10, 65, 54, 23};
      private static readonly int[] barPosE = new[] {20, 41, 46, 1, 8, 51, 29, 61, 34, 15, 25, 37, 58};
      private static readonly int[] barPosF = new[] {51, 25, 19, 64, 56, 4, 44, 31, 28, 36, 47, 11, 6};
      private static readonly int[] barPosG = new[] {33, 37, 21, 9, 17, 49, 59, 14, 64, 26, 42, 4, 53};
      private static readonly int[] barPosH = new[] {60, 14, 1, 27, 38, 61, 10, 24, 50, 55, 19, 32, 45};
      private static readonly int[] barPosI = new[] {27, 46, 65, 59, 31, 12, 16, 43, 55, 5, 9, 22, 36};
      private static readonly int[] barPosJ = new[] {63, 58, 53, 48, 43, 38, 33, 28, 23, 18, 13, 8, 3};
      private static readonly int[][] barPos = new[] {barPosA, barPosB, barPosC, barPosD, barPosE, barPosF, barPosG, barPosH, barPosI, barPosJ};

      private static readonly char[] barTypeA = new[] {'A', 'D', 'A', 'D', 'A', 'A', 'D', 'D', 'D', 'A', 'A', 'A', 'D'};
      private static readonly char[] barTypeB = new[] {'A', 'D', 'A', 'D', 'A', 'D', 'A', 'A', 'A', 'A', 'D', 'A', 'D'};
      private static readonly char[] barTypeC = new[] {'A', 'D', 'A', 'D', 'A', 'D', 'D', 'A', 'A', 'D', 'A', 'D', 'A'};
      private static readonly char[] barTypeD = new[] {'A', 'A', 'A', 'D', 'D', 'A', 'D', 'A', 'A', 'D', 'D', 'D', 'A'};
      private static readonly char[] barTypeE = new[] {'D', 'A', 'D', 'A', 'D', 'A', 'D', 'D', 'A', 'A', 'A', 'D', 'A'};
      private static readonly char[] barTypeF = new[] {'D', 'D', 'A', 'A', 'D', 'D', 'A', 'A', 'D', 'D', 'D', 'D', 'A'};
      private static readonly char[] barTypeG = new[] {'D', 'A', 'D', 'D', 'D', 'D', 'A', 'A', 'D', 'A', 'D', 'A', 'D'};
      private static readonly char[] barTypeH = new[] {'D', 'D', 'D', 'D', 'A', 'A', 'A', 'A', 'D', 'A', 'D', 'D', 'A'};
      private static readonly char[] barTypeI = new[] {'A', 'A', 'A', 'D', 'D', 'D', 'A', 'D', 'D', 'D', 'A', 'D', 'A'};
      private static readonly char[] barTypeJ = new[] {'A', 'D', 'A', 'D', 'A', 'D', 'A', 'A', 'D', 'A', 'D', 'A', 'D'};
      private static readonly char[][] barType = new[] {barTypeA, barTypeB, barTypeC, barTypeD, barTypeE, barTypeF, barTypeG, barTypeH, barTypeI, barTypeJ};

      private const int A = 0, B = 1, C = 2, D = 3, E = 4, F = 5, G = 6, H = 7, I = 8, J = 9;

      private static readonly IDictionary<int, int> table1Check;
      private static readonly IDictionary<int, int> table2Check;

      private BinaryBitmap currentBitmap;

      static IMBReader()
      {
         ushort[] table1 = {31, 7936, 47, 7808, 55, 7552, 59, 7040, 61, 6016, 62, 3968, 79, 7744, 87, 7488, 91, 6976, 93, 5952, 94, 3904, 103, 7360, 107, 6848, 109, 5824, 110, 3776, 115, 6592, 117, 5568, 118, 3520, 121, 5056, 122, 3008, 124, 1984, 143, 7712, 151, 7456, 155, 6944, 157, 5920, 158, 3872, 167, 7328, 171, 6816, 173, 5792, 174, 3744, 179, 6560, 181, 5536, 182, 3488, 185, 5024, 186, 2976, 188, 1952, 199, 7264, 203, 6752, 205, 5728, 206, 3680, 211, 6496, 213, 5472, 214, 3424, 217, 4960, 218, 2912, 220, 1888, 227, 6368, 229, 5344, 230, 3296, 233, 4832, 234, 2784, 236, 1760, 241, 4576, 242, 2528, 244, 1504, 248, 992, 271, 7696, 279, 7440, 283, 6928, 285, 5904, 286, 3856, 295, 7312, 299, 6800, 301, 5776, 302, 3728, 307, 6544, 309, 5520, 310, 3472, 313, 5008, 314, 2960, 316, 1936, 327, 7248, 331, 6736, 333, 5712, 334, 3664, 339, 6480, 341, 5456, 342, 3408, 345, 4944, 346, 2896, 348, 1872, 355, 6352, 357, 5328, 358, 3280, 361, 4816, 362, 2768, 364, 1744, 369, 4560, 370, 2512, 372, 1488, 376, 976, 391, 7216, 395, 6704, 397, 5680, 398, 3632, 403, 6448, 405, 5424, 406, 3376, 409, 4912, 410, 2864, 412, 1840, 419, 6320, 421, 5296, 422, 3248, 425, 4784, 426, 2736, 428, 1712, 433, 4528, 434, 2480, 436, 1456, 440, 944, 451, 6256, 453, 5232, 454, 3184, 457, 4720, 458, 2672, 460, 1648, 465, 4464, 466, 2416, 468, 1392, 472, 880, 481, 4336, 482, 2288, 484, 1264, 488, 752, 527, 7688, 535, 7432, 539, 6920, 541, 5896, 542, 3848, 551, 7304, 555, 6792, 557, 5768, 558, 3720, 563, 6536, 565, 5512, 566, 3464, 569, 5000, 570, 2952, 572, 1928, 583, 7240, 587, 6728, 589, 5704, 590, 3656, 595, 6472, 597, 5448, 598, 3400, 601, 4936, 602, 2888, 604, 1864, 611, 6344, 613, 5320, 614, 3272, 617, 4808, 618, 2760, 620, 1736, 625, 4552, 626, 2504, 628, 1480, 632, 968, 647, 7208, 651, 6696, 653, 5672, 654, 3624, 659, 6440, 661, 5416, 662, 3368, 665, 4904, 666, 2856, 668, 1832, 675, 6312, 677, 5288, 678, 3240, 681, 4776, 682, 2728, 684, 1704, 689, 4520, 690, 2472, 692, 1448, 696, 936, 707, 6248, 709, 5224, 710, 3176, 713, 4712, 714, 2664, 716, 1640, 721, 4456, 722, 2408, 724, 1384, 728, 872, 737, 4328, 738, 2280, 740, 1256, 775, 7192, 779, 6680, 781, 5656, 782, 3608, 787, 6424, 789, 5400, 790, 3352, 793, 4888, 794, 2840, 796, 1816, 803, 6296, 805, 5272, 806, 3224, 809, 4760, 810, 2712, 812, 1688, 817, 4504, 818, 2456, 820, 1432, 824, 920, 835, 6232, 837, 5208, 838, 3160, 841, 4696, 842, 2648, 844, 1624, 849, 4440, 850, 2392, 852, 1368, 865, 4312, 866, 2264, 868, 1240, 899, 6200, 901, 5176, 902, 3128, 905, 4664, 906, 2616, 908, 1592, 913, 4408, 914, 2360, 916, 1336, 929, 4280, 930, 2232, 932, 1208, 961, 4216, 962, 2168, 964, 1144, 1039, 7684, 1047, 7428, 1051, 6916, 1053, 5892, 1054, 3844, 1063, 7300, 1067, 6788, 1069, 5764, 1070, 3716, 1075, 6532, 1077, 5508, 1078, 3460, 1081, 4996, 1082, 2948, 1084, 1924, 1095, 7236, 1099, 6724, 1101, 5700, 1102, 3652, 1107, 6468, 1109, 5444, 1110, 3396, 1113, 4932, 1114, 2884, 1116, 1860, 1123, 6340, 1125, 5316, 1126, 3268, 1129, 4804, 1130, 2756, 1132, 1732, 1137, 4548, 1138, 2500, 1140, 1476, 1159, 7204, 1163, 6692, 1165, 5668, 1166, 3620, 1171, 6436, 1173, 5412, 1174, 3364, 1177, 4900, 1178, 2852, 1180, 1828, 1187, 6308, 1189, 5284, 1190, 3236, 1193, 4772, 1194, 2724, 1196, 1700, 1201, 4516, 1202, 2468, 1204, 1444, 1219, 6244, 1221, 5220, 1222, 3172, 1225, 4708, 1226, 2660, 1228, 1636, 1233, 4452, 1234, 2404, 1236, 1380, 1249, 4324, 1250, 2276, 1287, 7188, 1291, 6676, 1293, 5652, 1294, 3604, 1299, 6420, 1301, 5396, 1302, 3348, 1305, 4884, 1306, 2836, 1308, 1812, 1315, 6292, 1317, 5268, 1318, 3220, 1321, 4756, 1322, 2708, 1324, 1684, 1329, 4500, 1330, 2452, 1332, 1428, 1347, 6228, 1349, 5204, 1350, 3156, 1353, 4692, 1354, 2644, 1356, 1620, 1361, 4436, 1362, 2388, 1377, 4308, 1378, 2260, 1411, 6196, 1413, 5172, 1414, 3124, 1417, 4660, 1418, 2612, 1420, 1588, 1425, 4404, 1426, 2356, 1441, 4276, 1442, 2228, 1473, 4212, 1474, 2164, 1543, 7180, 1547, 6668, 1549, 5644, 1550, 3596, 1555, 6412, 1557, 5388, 1558, 3340, 1561, 4876, 1562, 2828, 1564, 1804, 1571, 6284, 1573, 5260, 1574, 3212, 1577, 4748, 1578, 2700, 1580, 1676, 1585, 4492, 1586, 2444, 1603, 6220, 1605, 5196, 1606, 3148, 1609, 4684, 1610, 2636, 1617, 4428, 1618, 2380, 1633, 4300, 1634, 2252, 1667, 6188, 1669, 5164, 1670, 3116, 1673, 4652, 1674, 2604, 1681, 4396, 1682, 2348, 1697, 4268, 1698, 2220, 1729, 4204, 1730, 2156, 1795, 6172, 1797, 5148, 1798, 3100, 1801, 4636, 1802, 2588, 1809, 4380, 1810, 2332, 1825, 4252, 1826, 2204, 1857, 4188, 1858, 2140, 1921, 4156, 1922, 2108, 2063, 7682, 2071, 7426, 2075, 6914, 2077, 5890, 2078, 3842, 2087, 7298, 2091, 6786, 2093, 5762, 2094, 3714, 2099, 6530, 2101, 5506, 2102, 3458, 2105, 4994, 2106, 2946, 2119, 7234, 2123, 6722, 2125, 5698, 2126, 3650, 2131, 6466, 2133, 5442, 2134, 3394, 2137, 4930, 2138, 2882, 2147, 6338, 2149, 5314, 2150, 3266, 2153, 4802, 2154, 2754, 2161, 4546, 2162, 2498, 2183, 7202, 2187, 6690, 2189, 5666, 2190, 3618, 2195, 6434, 2197, 5410, 2198, 3362, 2201, 4898, 2202, 2850, 2211, 6306, 2213, 5282, 2214, 3234, 2217, 4770, 2218, 2722, 2225, 4514, 2226, 2466, 2243, 6242, 2245, 5218, 2246, 3170, 2249, 4706, 2250, 2658, 2257, 4450, 2258, 2402, 2273, 4322, 2311, 7186, 2315, 6674, 2317, 5650, 2318, 3602, 2323, 6418, 2325, 5394, 2326, 3346, 2329, 4882, 2330, 2834, 2339, 6290, 2341, 5266, 2342, 3218, 2345, 4754, 2346, 2706, 2353, 4498, 2354, 2450, 2371, 6226, 2373, 5202, 2374, 3154, 2377, 4690, 2378, 2642, 2385, 4434, 2401, 4306, 2435, 6194, 2437, 5170, 2438, 3122, 2441, 4658, 2442, 2610, 2449, 4402, 2465, 4274, 2497, 4210, 2567, 7178, 2571, 6666, 2573, 5642, 2574, 3594, 2579, 6410, 2581, 5386, 2582, 3338, 2585, 4874, 2586, 2826, 2595, 6282, 2597, 5258, 2598, 3210, 2601, 4746, 2602, 2698, 2609, 4490, 2627, 6218, 2629, 5194, 2630, 3146, 2633, 4682, 2641, 4426, 2657, 4298, 2691, 6186, 2693, 5162, 2694, 3114, 2697, 4650, 2705, 4394, 2721, 4266, 2753, 4202, 2819, 6170, 2821, 5146, 2822, 3098, 2825, 4634, 2833, 4378, 2849, 4250, 2881, 4186, 2945, 4154, 3079, 7174, 3083, 6662, 3085, 5638, 3086, 3590, 3091, 6406, 3093, 5382, 3094, 3334, 3097, 4870, 3107, 6278, 3109, 5254, 3110, 3206, 3113, 4742, 3121, 4486, 3139, 6214, 3141, 5190, 3145, 4678, 3153, 4422, 3169, 4294, 3203, 6182, 3205, 5158, 3209, 4646, 3217, 4390, 3233, 4262, 3265, 4198, 3331, 6166, 3333, 5142, 3337, 4630, 3345, 4374, 3361, 4246, 3393, 4182, 3457, 4150, 3587, 6158, 3589, 5134, 3593, 4622, 3601, 4366, 3617, 4238, 3649, 4174, 3713, 4142, 3841, 4126, 4111, 7681, 4119, 7425, 4123, 6913, 4125, 5889, 4135, 7297, 4139, 6785, 4141, 5761, 4147, 6529, 4149, 5505, 4153, 4993, 4167, 7233, 4171, 6721, 4173, 5697, 4179, 6465, 4181, 5441, 4185, 4929, 4195, 6337, 4197, 5313, 4201, 4801, 4209, 4545, 4231, 7201, 4235, 6689, 4237, 5665, 4243, 6433, 4245, 5409, 4249, 4897, 4259, 6305, 4261, 5281, 4265, 4769, 4273, 4513, 4291, 6241, 4293, 5217, 4297, 4705, 4305, 4449, 4359, 7185, 4363, 6673, 4365, 5649, 4371, 6417, 4373, 5393, 4377, 4881, 4387, 6289, 4389, 5265, 4393, 4753, 4401, 4497, 4419, 6225, 4421, 5201, 4425, 4689, 4483, 6193, 4485, 5169, 4489, 4657, 4615, 7177, 4619, 6665, 4621, 5641, 4627, 6409, 4629, 5385, 4633, 4873, 4643, 6281, 4645, 5257, 4649, 4745, 4675, 6217, 4677, 5193, 4739, 6185, 4741, 5161, 4867, 6169, 4869, 5145, 5127, 7173, 5131, 6661, 5133, 5637, 5139, 6405, 5141, 5381, 5155, 6277, 5157, 5253, 5187, 6213, 5251, 6181, 5379, 6165, 5635, 6157, 6151, 7171, 6155, 6659, 6163, 6403, 6179, 6275, 6211, 5189, 4681, 4433, 4321, 3142, 2634, 2386, 2274, 1612, 1364, 1252, 856, 744, 496};
         ushort[] table2 = {3, 6144, 5, 5120, 6, 3072, 9, 4608, 10, 2560, 12, 1536, 17, 4352, 18, 2304, 20, 1280, 24, 768, 33, 4224, 34, 2176, 36, 1152, 40, 640, 48, 384, 65, 4160, 66, 2112, 68, 1088, 72, 576, 80, 320, 96, 192, 129, 4128, 130, 2080, 132, 1056, 136, 544, 144, 288, 257, 4112, 258, 2064, 260, 1040, 264, 528, 513, 4104, 514, 2056, 516, 1032, 1025, 4100, 1026, 2052, 2049, 4098, 4097, 2050, 1028, 520, 272, 160};

         // create tables to check decFcsChars
         table1Check = new Dictionary<int, int>(2000);
         table2Check = new Dictionary<int, int>(200);
         for (int k = 0; k < table1.Length; k++)
            table1Check.Add(table1[k], k);
         for (int k = 0; k < table2.Length; k++)
            table2Check.Add(table2[k], k);
      }

      protected override Result doDecode(BinaryBitmap image, IDictionary<DecodeHintType, object> hints)
      {
         currentBitmap = image;
         return base.doDecode(image, hints);
      }

      public override void reset()
      {
         base.reset();
         currentBitmap = null;
      }

      private ushort binaryStringToDec(string binary)
      {
         ushort factor = (ushort) Math.Pow(2, binary.Length - 1);
         ushort result = 0;

         foreach (char bit in binary)
         {
            if (bit == '1')
               result += factor;

            factor /= 2;
         }

         return result;
      }

      private string invertedBinaryString(string binary)
      {
         string result = "";

         foreach (char bit in binary)
         {
            if (bit == '1')
               result += '0';
            else
               result += '1';
         }

         return result;
      }

      private bool getCodeWords(out int[] codeWord, string imb, IDictionary<int, int> table1Check, IDictionary<int, int> table2Check, int[][] barPos, char[][] barType)
      {
         // initialize the binaryFcsChars to 0 (has 13 bits)
         StringBuilder[] binaryFcsChars = new StringBuilder[10];
         for (int c = 0; c < 10; c++)
            binaryFcsChars[c] = new StringBuilder("0000000000000");

         // fill in the binaryFcsChars
         for (int pos = 0; pos < 65; pos++)
         {
            if (imb[pos] != 'D' && imb[pos] != 'A' && imb[pos] != 'F')
               continue;

            int offset = pos + 1;
            for (int a = 0; a < 10; a++) // int[][] barPos
            {
               int i;
               for (i = 0; i < 13; i++)
               {
                  if (barPos[a][i] == offset)
                  {
                     if (barType[a][i] == imb[pos] || imb[pos] == 'F')
                        binaryFcsChars[a][12 - i] = '1';
                  }
               }
            }
         }

         // convert each binaryFcsChar into decimal format
         ushort[] decFcsChars = new ushort[10];
         for (int k = 0; k < 10; k++)
            decFcsChars[k] = binaryStringToDec(binaryFcsChars[k].ToString());

         // change decFcsChars according to whether FCS rules (whether it is the decFcsChars value is contained in one of the tables)
         for (int k = 0; k < decFcsChars.Length; k++)
         {
            if (!table1Check.ContainsKey(decFcsChars[k]) && !table2Check.ContainsKey(decFcsChars[k]))
            {
               binaryFcsChars[k].Replace(binaryFcsChars[k].ToString(), invertedBinaryString(binaryFcsChars[k].ToString()));
               decFcsChars[k] = binaryStringToDec(binaryFcsChars[k].ToString());
            }
         }

         // get codewords A-J
         codeWord = new int[10];
         for (int k = 0; k < 10; k++)
         {
            if (!table1Check.ContainsKey(decFcsChars[k]))
            {
               if (table2Check.ContainsKey(decFcsChars[k]))
                  codeWord[k] = table2Check[decFcsChars[k]] + 1287;
               else
                  return false; // invert the imb
            }
            else
               codeWord[k] = table1Check[decFcsChars[k]];
         }

         return true;
      }

      private string getTrackingNumber(string imb)
      {

         // get codewords A-J
         int[] codeWord;
         if (!getCodeWords(out codeWord, imb, table1Check, table2Check, barPos, barType))
         {
            // imb is upside down
            StringBuilder invertedImb = new StringBuilder(imb.Length);
            for (int k = imb.Length - 1; k >= 0; k--)
            {
               if (imb[k] == 'A')
                  invertedImb.Append('D');
               else if (imb[k] == 'D')
                  invertedImb.Append('A');
               else
                  invertedImb.Append(imb[k]);
            }

            if (!getCodeWords(out codeWord, invertedImb.ToString(), table1Check, table2Check, barPos, barType))
               return null;
         }


         if (codeWord[A] > 658)
            codeWord[A] -= 659;
         codeWord[J] /= 2;

         // codewords to binaryData
         BigInteger binaryData = codeWord[A];
         for (int k = 1; k <= 8; k++)
            binaryData = (binaryData*1365) + codeWord[k];
         binaryData = (binaryData*636) + codeWord[J];

         // get tracking code
         int[] tCode = new int[20];
         for (int i = 19; i >= 2; i--)
         {
            tCode[i] = (int) (binaryData%10);
            binaryData /= 10;
         }
         tCode[1] = (int) (binaryData%5);
         binaryData /= 5;
         tCode[0] = (int) (binaryData%10);
         binaryData /= 10;

         // get routing code and imb number
         string imbTrackingNumber = "";
         foreach (int t in tCode)
            imbTrackingNumber += t.ToString();
         ulong rCode;
         if (binaryData > 1000000000)
         {
            rCode = (ulong) (binaryData - 1000000000 - 100000 - 1);
            imbTrackingNumber += rCode.ToString().PadLeft(11, '0');
         }
         else if (binaryData > 100000)
         {
            rCode = (ulong) (binaryData - 100000 - 1);
            imbTrackingNumber += rCode.ToString().PadLeft(9, '0');
         }
         else if (binaryData > 0)
         {
            rCode = (ulong) (binaryData - 1);
            imbTrackingNumber += rCode.ToString().PadLeft(5, '0');
         }

         return imbTrackingNumber;
      }

      private void fillLists(BitArray row, BitArray topRow, BitArray botRow, ref List<int> listRow, ref List<int> listTop, ref List<int> listBot, int start, int stop) // list: 1=black  0=white
      {
         const bool isWhite = false;
         bool insideBar = false;

         for (int i = start; i <= stop; i++)
         {
            if (row[i] ^ isWhite) // if current pixel is black
            {
               if (!insideBar)
               {
                  insideBar = true;

                  listRow.Add(1);

                  if (topRow[i] ^ isWhite)
                     listTop.Add(1);
                  else
                     listTop.Add(0);

                  if (botRow[i] ^ isWhite)
                     listBot.Add(1);
                  else
                     listBot.Add(0);
               }
            }
            else // if current pixel is white
            {
               if (insideBar)
               {
                  listRow.Add(0);

                  if (topRow[i] ^ isWhite)
                     listTop.Add(1);
                  else
                     listTop.Add(0);

                  if (botRow[i] ^ isWhite)
                     listBot.Add(1);
                  else
                     listBot.Add(0);
               }

               insideBar = false;
            }
         }
      }

      private int isIMB(BitArray row, ref int pixelStartOffset, ref int pixelStopOffset, ref int pixelBarLength)
      {
         int width = row.Size;
         int rowOffset = row.getNextSet(0);
         pixelStartOffset = rowOffset;
         int previousPixelOffset = pixelStartOffset;
         const bool isWhite = false;

         int countBars = 0;
         bool insideBar = false;
         int currBarLength = 0;
         int prevBarLength = 0;

         bool insideWS = false;
         int numWSBetween = 0;
         int currWSLength = 0;
         int prevWSLength = 0;

         for (int i = rowOffset; i < width; i++)
         {
            if (row[i] ^ isWhite) // if current pixel is black
            {
               insideWS = false;

               if (!insideBar)
               {
                  if (countBars <= 1)
                  {
                     prevWSLength = currWSLength;
                  }
                  else
                  {
                     if (prevWSLength != currWSLength)
                     {
                        numWSBetween = 1;
                        prevWSLength = currWSLength;
                        countBars = 1;
                        pixelStartOffset = previousPixelOffset;
                     }
                  }
                  countBars++;

                  insideBar = true;
                  previousPixelOffset = i;
               }

               currWSLength = 0;

               currBarLength++;
            }
            else // if current pixel is white
            {
               insideBar = false;

               if (!insideWS)
               {
                  numWSBetween++;
                  insideWS = true;

                  if (countBars <= 1)
                     prevBarLength = currBarLength;
                  else
                  {
                     if (prevBarLength != currBarLength)
                     {
                        countBars = 1;
                        numWSBetween = 1;
                        prevWSLength = 0;
                        pixelStartOffset = previousPixelOffset;
                        prevBarLength = currBarLength;
                     }
                     else
                     {
                        if (countBars == 65) // made it this far, so break
                        {
                           pixelStopOffset = i;
                           //pixelBarLength = prevBarLength;
                           break;
                        }
                     }
                  }
                  currBarLength = 0;
               }


               currWSLength++;
            }


         }

         pixelBarLength = prevBarLength;
         return (countBars);
      }

      private int getNumberBars(BitArray row, int start, int stop, int barWidth)
      {
         const bool isWhite = false;
         bool insideBar = false;
         int countBars = 0;
         int currentBarWidth = 0;

         for (int i = start; i <= stop; i++)
         {
            if (row[i] ^ isWhite) // if current pixel is black
            {
               if (!insideBar)
               {
                  //countBars++;
                  insideBar = true;
               }
               currentBarWidth++;

               if (i == stop)
               {
                  if (currentBarWidth == barWidth)
                     countBars++;
               }
            }
            else // if current pixel is white
            {
               if (insideBar)
               {
                  if (currentBarWidth == barWidth)
                     countBars++;
               }
               insideBar = false;
               currentBarWidth = 0;
            }
         }

         return countBars;
      }

      public override Result decodeRow(int rowNumber, BitArray row, IDictionary<DecodeHintType, object> hints)
      {
         if (currentBitmap == null)
            return null;

         int pixelStartOffset = 0;
         int pixelStopOffset = currentBitmap.Width - 1;
         int pixelBarLength = 0;
         int numBars = isIMB(row, ref pixelStartOffset, ref pixelStopOffset, ref pixelBarLength);
         if (numBars != NUM_BARS_IMB)
            return null;

         // create the two bitarrays to check top and bottom
         BitArray topRow = new BitArray(currentBitmap.Width);
         BitArray botRow = new BitArray(currentBitmap.Width);
         int rowNumberTop = rowNumber;
         int rowNumberBot = rowNumber;

         do
         {
            if (rowNumberTop <= 0)
               return null;
            rowNumberTop--;
            topRow = currentBitmap.getBlackRow(rowNumberTop, topRow);
         } while (getNumberBars(topRow, pixelStartOffset, pixelStopOffset, pixelBarLength) >= NUM_BARS_IMB);
         do
         {
            if (rowNumberBot >= (currentBitmap.Height - 1))
               return null;
            rowNumberBot++;
            botRow = currentBitmap.getBlackRow(rowNumberBot, botRow);
         } while (getNumberBars(botRow, pixelStartOffset, pixelStopOffset, pixelBarLength) >= NUM_BARS_IMB);

         List<int> listRow = new List<int>();
         List<int> listTop = new List<int>();
         List<int> listBot = new List<int>();
         fillLists(row, topRow, botRow, ref listRow, ref listTop, ref listBot, pixelStartOffset, pixelStopOffset);

         string symbolCode = "";
         for (int k = 0; k < listRow.Count; k++)
         {
            if (listRow[k] == 0)
               continue;

            if (listBot[k] == 1 && listTop[k] == 1)
               symbolCode += "F";
            else if (listBot[k] == 1)
               symbolCode += "D";
            else if (listTop[k] == 1)
               symbolCode += "A";
            else
               symbolCode += "T";
         }

         string trackingNumber = getTrackingNumber(symbolCode);
         if (trackingNumber == null)
            return null;

         var resultPointCallback = hints == null || !hints.ContainsKey(DecodeHintType.NEED_RESULT_POINT_CALLBACK)
                                      ? null
                                      : (ResultPointCallback) hints[DecodeHintType.NEED_RESULT_POINT_CALLBACK];
         if (resultPointCallback != null)
         {
            resultPointCallback(new ResultPoint(pixelStartOffset, rowNumber));
            resultPointCallback(new ResultPoint(pixelStopOffset, rowNumber));
         }

         return new Result(
            trackingNumber,
            null,
            new[]
               {
                  new ResultPoint(pixelStartOffset, rowNumber),
                  new ResultPoint(pixelStopOffset, rowNumber)
               },
            BarcodeFormat.IMB);
      }
   }
}
