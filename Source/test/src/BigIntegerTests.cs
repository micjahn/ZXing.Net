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
   }
}
