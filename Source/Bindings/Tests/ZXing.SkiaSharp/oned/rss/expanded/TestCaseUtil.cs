/*
 * Copyright (C) 2012 ZXing authors
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

/*
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", led by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 */

using System;
#if !SILVERLIGHT
using System.Drawing;
#else
using System.Windows.Media.Imaging;
#endif
using System.IO;

using ZXing.Common;
using ZXing.Common.Test;

namespace ZXing.OneD.RSS.Expanded.Test
{
   internal static class TestCaseUtil
   {
      internal static BinaryBitmap getBinaryBitmap(String directory, String path)
      {
         var bufferedImage = readImage(directory, path);
         var luminanceSource = new BitmapLuminanceSource(bufferedImage);
         return new BinaryBitmap(new GlobalHistogramBinarizer(luminanceSource));
      }

#if !SILVERLIGHT
      internal static Bitmap readImage(String directory, String fileName)
      {
         var path = AbstractBlackBoxTestCase.buildTestBase(directory);
         return new Bitmap(Image.FromFile(Path.Combine(path, fileName)));
      }
#else
      internal static WriteableBitmap readImage(String directory, String fileName)
      {
         var path = AbstractBlackBoxTestCase.buildTestBase(directory);
         var image = new WriteableBitmap(0, 0);
         image.SetSource(File.OpenRead(Path.Combine(path, fileName)));
         return image;
      }
#endif
   }
}