/*
 * Copyright 2012 ZXing authors
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

namespace ZXing.PDF417.Internal.EC
{
   /// <summary>
   /// <p>PDF417 error correction implementation.</p>
   /// <p>This <a href="http://en.wikipedia.org/wiki/Reed%E2%80%93Solomon_error_correction#Example">example</a>
   /// is quite useful in understanding the algorithm.</p>
   /// <author>Sean Owen</author>
   /// <see cref="ZXing.Common.ReedSolomon.ReedSolomonDecoder" />
   /// </summary>
   public sealed class ErrorCorrection
   {
      private readonly ModulusGF field;

      /// <summary>
      /// Initializes a new instance of the <see cref="ErrorCorrection"/> class.
      /// </summary>
      public ErrorCorrection()
      {
         this.field = ModulusGF.PDF417_GF;
      }

      /// <summary>
      /// Decodes the specified received.
      /// </summary>
      /// <param name="received">received codewords</param>
      /// <param name="numECCodewords">number of those codewords used for EC</param>
      /// <param name="erasures">location of erasures</param>
      /// <param name="errorLocationsCount">The error locations count.</param>
      /// <returns></returns>
      public bool decode(int[] received, int numECCodewords, int[] erasures, out int errorLocationsCount)
      {
         ModulusPoly poly = new ModulusPoly(field, received);
         int[] S = new int[numECCodewords];
         bool error = false;
         errorLocationsCount = 0;
         for (int i = numECCodewords; i > 0; i--)
         {
            int eval = poly.evaluateAt(field.exp(i));
            S[numECCodewords - i] = eval;
            if (eval != 0)
            {
               error = true;
            }
         }

         if (!error)
         {
            return true;
         }

         ModulusPoly knownErrors = field.One;
         if (erasures != null)
         {
            foreach (int erasure in erasures)
            {
               int b = field.exp(received.Length - 1 - erasure);
               // Add (1 - bx) term:
               ModulusPoly term = new ModulusPoly(field, new int[] {field.subtract(0, b), 1});
               knownErrors = knownErrors.multiply(term);
            }
         }

         ModulusPoly syndrome = new ModulusPoly(field, S);
         //syndrome = syndrome.multiply(knownErrors);

         ModulusPoly[] sigmaOmega = runEuclideanAlgorithm(field.buildMonomial(numECCodewords, 1), syndrome, numECCodewords);

         if (sigmaOmega == null)
         {
            return false;
         }

         ModulusPoly sigma = sigmaOmega[0];
         ModulusPoly omega = sigmaOmega[1];

         if (sigma == null || omega == null)
         {
            return false;
         }

         //sigma = sigma.multiply(knownErrors);

         int[] errorLocations = findErrorLocations(sigma);

         if (errorLocations == null)
         {
            return false;
         }

         int[] errorMagnitudes = findErrorMagnitudes(omega, sigma, errorLocations);

         for (int i = 0; i < errorLocations.Length; i++)
         {
            int position = received.Length - 1 - field.log(errorLocations[i]);
            if (position < 0)
            {
               return false;

            }
            received[position] = field.subtract(received[position], errorMagnitudes[i]);
         }
         errorLocationsCount = errorLocations.Length;
         return true;
      }

      /// <summary>
      /// Runs the euclidean algorithm (Greatest Common Divisor) until r's degree is less than R/2
      /// </summary>
      /// <returns>The euclidean algorithm.</returns>
      private ModulusPoly[] runEuclideanAlgorithm(ModulusPoly a, ModulusPoly b, int R)
      {
         // Assume a's degree is >= b's
         if (a.Degree < b.Degree)
         {
            ModulusPoly temp = a;
            a = b;
            b = temp;
         }

         ModulusPoly rLast = a;
         ModulusPoly r = b;
         ModulusPoly tLast = field.Zero;
         ModulusPoly t = field.One;

         // Run Euclidean algorithm until r's degree is less than R/2
         while (r.Degree >= R / 2)
         {
            ModulusPoly rLastLast = rLast;
            ModulusPoly tLastLast = tLast;
            rLast = r;
            tLast = t;

            // Divide rLastLast by rLast, with quotient in q and remainder in r
            if (rLast.isZero)
            {
               // Oops, Euclidean algorithm already terminated?
               return null;
            }
            r = rLastLast;
            ModulusPoly q = field.Zero;
            int denominatorLeadingTerm = rLast.getCoefficient(rLast.Degree);
            int dltInverse = field.inverse(denominatorLeadingTerm);
            while (r.Degree >= rLast.Degree && !r.isZero)
            {
               int degreeDiff = r.Degree - rLast.Degree;
               int scale = field.multiply(r.getCoefficient(r.Degree), dltInverse);
               q = q.add(field.buildMonomial(degreeDiff, scale));
               r = r.subtract(rLast.multiplyByMonomial(degreeDiff, scale));
            }

            t = q.multiply(tLast).subtract(tLastLast).getNegative();
         }

         int sigmaTildeAtZero = t.getCoefficient(0);
         if (sigmaTildeAtZero == 0)
         {
            return null;
         }

         int inverse = field.inverse(sigmaTildeAtZero);
         ModulusPoly sigma = t.multiply(inverse);
         ModulusPoly omega = r.multiply(inverse);
         return new ModulusPoly[] { sigma, omega };
      }

      /// <summary>
      /// Finds the error locations as a direct application of Chien's search
      /// </summary>
      /// <returns>The error locations.</returns>
      /// <param name="errorLocator">Error locator.</param>
      private int[] findErrorLocations(ModulusPoly errorLocator)
      {
         // This is a direct application of Chien's search
         int numErrors = errorLocator.Degree;
         int[] result = new int[numErrors];
         int e = 0;
         for (int i = 1; i < field.Size && e < numErrors; i++)
         {
            if (errorLocator.evaluateAt(i) == 0)
            {
               result[e] = field.inverse(i);
               e++;
            }
         }
         if (e != numErrors)
         {
            return null;
         }
         return result;
      }

      /// <summary>
      /// Finds the error magnitudes by directly applying Forney's Formula
      /// </summary>
      /// <returns>The error magnitudes.</returns>
      /// <param name="errorEvaluator">Error evaluator.</param>
      /// <param name="errorLocator">Error locator.</param>
      /// <param name="errorLocations">Error locations.</param>
      private int[] findErrorMagnitudes(ModulusPoly errorEvaluator,
                                        ModulusPoly errorLocator,
                                        int[] errorLocations)
      {
         int errorLocatorDegree = errorLocator.Degree;
         int[] formalDerivativeCoefficients = new int[errorLocatorDegree];
         for (int i = 1; i <= errorLocatorDegree; i++)
         {
            formalDerivativeCoefficients[errorLocatorDegree - i] =
                field.multiply(i, errorLocator.getCoefficient(i));
         }
         ModulusPoly formalDerivative = new ModulusPoly(field, formalDerivativeCoefficients);

         // This is directly applying Forney's Formula
         int s = errorLocations.Length;
         int[] result = new int[s];
         for (int i = 0; i < s; i++)
         {
            int xiInverse = field.inverse(errorLocations[i]);
            int numerator = field.subtract(0, errorEvaluator.evaluateAt(xiInverse));
            int denominator = field.inverse(formalDerivative.evaluateAt(xiInverse));
            result[i] = field.multiply(numerator, denominator);
         }
         return result;
      }
   }
}