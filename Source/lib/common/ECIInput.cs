/*
 * Copyright 2021 ZXing authors
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

namespace ZXing.Common
{
    using System;

    /// <summary>
    /// Interface to navigate a sequence of ECIs and bytes.
    /// @author Alex Geller
    /// </summary>
    public interface ECIInput
    {
        /// <summary>
        /// Returns the length of this input.The length is the number of {@code byte}s in or ECIs in the sequence.
        /// </summary>
        int Length { get; }

        /// <summary>
        /// Returns the {@code byte} value at the specified index.  An index ranges from zero to {@code length() - 1}.  The first { @code byte}
        /// value of the sequence is at index zero, the next at index one, and so on, as for array indexing.
        /// </summary>
        /// <param name="index">the index of the {@code byte} value to be returned</param>
        /// <returns>the specified {@code byte} value as character or the FNC1 character</returns>
        /// <exception cref="IndexOutOfRangeException">if the {@code index} argument is negative or not less than {@code Length}</exception>
        /// <exception cref="ArgumentException">if the value at the {@code index} argument is an ECI (@see #isECI)</exception>
        char charAt(int index);

        /// <summary>
        /// Returns a {@code CharSequence} that is a subsequence of this sequence.
        /// The subsequence starts with the {@code char} value at the specified index and
        /// ends with the {@code char} value at index {@code end - 1}.  The length
        /// (in {@code char}s) of the
        /// returned sequence is {@code end - start}, so if {@code start == end}
        /// then an empty sequence is returned.
        /// </summary>
        /// <param name="start">the start index, inclusive</param>
        /// <param name="end">the start index, inclusive</param>
        /// <returns>the specified subsequence</returns>
        /// <exception cref="IndexOutOfRangeException">if {@code start} or {@code end} are negative,
        /// if {@code end} is greater than { @code length()},
        /// or if {@code start} is greater than { @code end}</exception>
        /// <exception cref="ArgumentException">if a value in the range {@code start}-{@code end} is an ECI (@see #isECI)</exception>
        String subSequence(int start, int end);

        /// <summary>
        /// Determines if a value is an ECI
        /// </summary>
        /// <param name="index">the index of the value</param>
        /// <returns>true if the value at position {@code index} is an ECI</returns>
        /// <exception cref="IndexOutOfRangeException">if the {@code index} argument is negative or not less than {@code length()}</exception>
        bool isECI(int index);

        /// <summary>
        /// Returns the {@code int} ECI value at the specified index.  An index ranges from zero
        /// to {@code length() - 1}.  The first {@code byte} value of the sequence is at
        /// index zero, the next at index one, and so on, as for array
        /// indexing.
        /// </summary>
        /// <param name="index">the index of the {@code int} value to be returned</param>
        /// <returns>the specified {@code int} ECI value. 
        /// The ECI specified the encoding of all bytes with a higher index until the
        /// next ECI or until the end of the input if no other ECI follows.</returns>
        /// <exception cref="IndexOutOfRangeException">if the value at the {@code index} argument is not an ECI (@see #isECI)</exception>
        /// <exception cref="ArgumentException"></exception>
        int getECIValue(int index);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        bool haveNCharacters(int index, int n);
    }
}