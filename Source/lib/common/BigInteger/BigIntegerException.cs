using System;
using ZXing;

namespace BigIntegerLibrary
{
   /// <summary>
   /// BigInteger-related exception class.
   /// </summary>
   [Serializable]
   public sealed class BigIntegerException : Exception
   {
      /// <summary>
      /// BigIntegerException constructor.
      /// </summary>
      /// <param name="message">The exception message</param>
      /// <param name="innerException">The inner exception</param>
      public BigIntegerException(string message, Exception innerException)
         : base(message, innerException)
      {
      }
   }
}