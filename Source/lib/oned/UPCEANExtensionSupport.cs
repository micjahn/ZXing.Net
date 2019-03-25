﻿/*
 * Copyright (C) 2010 ZXing authors
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

using ZXing.Common;

namespace ZXing.OneD
{
    sealed class UPCEANExtensionSupport
    {
        private static readonly int[] EXTENSION_START_PATTERN = { 1, 1, 2 };

        private readonly UPCEANExtension2Support twoSupport = new UPCEANExtension2Support();
        private readonly UPCEANExtension5Support fiveSupport = new UPCEANExtension5Support();

        internal Result decodeRow(int rowNumber, BitArray row, int rowOffset)
        {
            int[] extensionStartRange = UPCEANReader.findGuardPattern(row, rowOffset, false, EXTENSION_START_PATTERN);
            if (extensionStartRange == null)
                return null;
            var result = fiveSupport.decodeRow(rowNumber, row, extensionStartRange);
            if (result == null)
                result = twoSupport.decodeRow(rowNumber, row, extensionStartRange);
            return result;
        }
    }
}