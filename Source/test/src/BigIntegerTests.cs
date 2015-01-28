/*
* Copyright 2012 ZXing.Net authors
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

using System.Numerics;

using NUnit.Framework;

namespace ZXing.Test
{
   [TestFixture]
   public class BigIntegerTests
   {
      [TestCase("1234567890")]
      [TestCase("12345678901234567890")]
      [TestCase("-12345678901234567890")]
      public void Extensions_Parse_Should_Return_The_Same_Result_Like_Internal_BigInteger_Parse(string number)
      {
         var left = BigIntegerExtensions.Parse(number).ToString();
         var right = BigInteger.Parse(number).ToString();

         Assert.AreEqual(left, right);
      }

      [TestCase("1234567890")]
      [TestCase("12345678901234567890")]
      [TestCase("-12345678901234567890")]
      public void System_Numerics_BigInteger_And_BigIntegerLibrary_Should_Be_The_Same(string number)
      {
         var systemBigInt900 = new BigInteger(900);
         var libraryBigInt900 = new BigIntegerLibrary.BigInteger(900);
         var systemBigInt = BigInteger.Parse(number);
         var libraryBigInt = BigIntegerLibrary.BigInteger.Parse(number);

         var systemResult = systemBigInt%systemBigInt900;
         var libraryResult = BigIntegerLibrary.BigInteger.Modulo(libraryBigInt, libraryBigInt900);

         Assert.AreEqual(systemResult.ToString(), libraryResult.ToString());
      }

      [TestCase(0)]
      [TestCase(1)]
      [TestCase(int.MaxValue)]
      [TestCase(987345)]
      [TestCase(-1)]
      [TestCase(int.MinValue)]
      [TestCase(-987345)]
      public void Conversion_Of_Int_To_BigInteger_And_Back_Should_Give_The_Same_Value(int number)
      {
         var bigInt = new BigInteger(number);
         var castBack = (int) bigInt;
         Assert.That(castBack, Is.EqualTo(number));
      }

      [TestCase((ulong)0)]
      [TestCase((ulong)1)]
      [TestCase(ulong.MaxValue)]
      [TestCase((ulong)987345)]
      [TestCase(ulong.MinValue)]
      public void Conversion_Of_Int_To_BigInteger_And_Back_Should_Give_The_Same_Value(ulong number)
      {
         var bigInt = new BigInteger(number);
         var castBack = (ulong)bigInt;
         Assert.That(castBack, Is.EqualTo(number));
      }
   }
}
