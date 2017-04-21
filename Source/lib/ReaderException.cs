/*
* Copyright 2007 ZXing authors
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

namespace ZXing
{
	/// <summary>
	/// The general exception class throw when something goes wrong during decoding of a barcode.
	/// This includes, but is not limited to, failing checksums / error correction algorithms, being
	/// unable to locate finder timing patterns, and so on.
	/// </summary>
	/// <author>Sean Owen</author>
	[Serializable]
	public class ReaderException : Exception
	{
      /// <summary>
      /// Initializes a new instance of the <see cref="ReaderException"/> class.
      /// </summary>
		public ReaderException()
		{
		}

      /// <summary>
      /// Initializes a new instance of the <see cref="ReaderException"/> class.
      /// </summary>
      /// <param name="message"></param>
      public ReaderException(String message)
         : base(message)
      {
      }
      /// <summary>
      /// Initializes a new instance of the <see cref="ReaderException"/> class.
      /// </summary>
      /// <param name="innerException">The inner exception.</param>
      public ReaderException(Exception innerException)
         : base(innerException.Message, innerException)
      {
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ReaderException"/> class.
      /// </summary>
      /// <param name="innerException">The inner exception.</param>
      /// <param name="message"></param>
      public ReaderException(String message, Exception innerException)
         : base(message, innerException)
      {
      }
   }
}