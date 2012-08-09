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

using System;
using System.Text;

using NUnit.Framework;

namespace ZXing.Common.Test
{
   [TestFixture]
   public sealed class CharacterSetECITestCase
   {
      [Test]
      public void CharacterSetECI_Should_Return_Usable_Charactersets()
      {
         var errors = String.Empty;
         foreach (var charSetEntry in CharacterSetECI.VALUE_TO_ECI)
         {
            try
            {
               Encoding.GetEncoding(charSetEntry.Value.EncodingName);
            }
            catch (Exception exc)
            {
               errors += exc.Message + Environment.NewLine;
            }
         }
         Assert.IsEmpty(errors);
      }
   }
}
