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

using ByteMatrix = com.google.zxing.common.ByteMatrix;
using EAN13Writer = com.google.zxing.oned.EAN13Writer;
using EAN8Writer = com.google.zxing.oned.EAN8Writer;
using QRCodeWriter = com.google.zxing.qrcode.QRCodeWriter;

namespace com.google.zxing
{
	/// <summary> This is a factory class which finds the appropriate Writer subclass for the BarcodeFormat
	/// requested and encodes the barcode with the supplied contents.
	/// 
	/// </summary>
	/// <author>  dswitkin@google.com (Daniel Switkin)
	/// </author>
	/// <author>www.Redivivus.in (suraj.supekar@redivivus.in) - Ported from ZXING Java Source 
	/// </author>
	public sealed class MultiFormatWriter : Writer
	{
	   private static readonly IDictionary<BarcodeFormat, Func<Writer>> formatMap;

      static MultiFormatWriter()
      {
         formatMap = new Dictionary<BarcodeFormat, Func<Writer>>
                        {
                           {BarcodeFormat.EAN_8, () => new EAN8Writer()},
                           {BarcodeFormat.EAN_13, () => new EAN13Writer()},
                           {BarcodeFormat.QR_CODE, () => new QRCodeWriter()}
                        };
      }

		public ByteMatrix encode(String contents, BarcodeFormat format, int width, int height)
		{
			
			return encode(contents, format, width, height, null);
		}

      public ByteMatrix encode(String contents, BarcodeFormat format, int width, int height, IDictionary<EncodeHintType, object> hints)
		{
         if (!formatMap.ContainsKey(format))
            throw new ArgumentException("No encoder available for format " + format);
         
         return formatMap[format]().encode(contents, format, width, height, hints);
		}
	}
}