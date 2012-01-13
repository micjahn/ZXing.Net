/*
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

using System;
using System.Collections.Generic;

namespace com.google.zxing.oned.rss
{
   public abstract class AbstractRSSReader : OneDReader
   {
      private static int MAX_AVG_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.2f);
      private static int MAX_INDIVIDUAL_VARIANCE = (int)(PATTERN_MATCH_RESULT_SCALE_FACTOR * 0.4f);

      private static float MIN_FINDER_PATTERN_RATIO = 9.5f / 12.0f;
      private static float MAX_FINDER_PATTERN_RATIO = 12.5f / 14.0f;

      private int[] decodeFinderCounters;
      private int[] dataCharacterCounters;
      private float[] oddRoundingErrors;
      private float[] evenRoundingErrors;
      private int[] oddCounts;
      private int[] evenCounts;

      protected AbstractRSSReader()
      {
         decodeFinderCounters = new int[4];
         dataCharacterCounters = new int[8];
         oddRoundingErrors = new float[4];
         evenRoundingErrors = new float[4];
         oddCounts = new int[dataCharacterCounters.Length / 2];
         evenCounts = new int[dataCharacterCounters.Length / 2];
      }

      protected int[] getDecodeFinderCounters()
      {
         return decodeFinderCounters;
      }

      protected int[] getDataCharacterCounters()
      {
         return dataCharacterCounters;
      }

      protected float[] getOddRoundingErrors()
      {
         return oddRoundingErrors;
      }

      protected float[] getEvenRoundingErrors()
      {
         return evenRoundingErrors;
      }

      protected int[] getOddCounts()
      {
         return oddCounts;
      }

      protected int[] getEvenCounts()
      {
         return evenCounts;
      }

      protected static int parseFinderValue(int[] counters,
                                            int[][] finderPatterns)
      {
         for (int value = 0; value < finderPatterns.Length; value++)
         {
            if (patternMatchVariance(counters, finderPatterns[value], MAX_INDIVIDUAL_VARIANCE) <
                MAX_AVG_VARIANCE)
            {
               return value;
            }
         }
         throw NotFoundException.Instance;
      }

      protected static int count(int[] array)
      {
         int count = 0;
         foreach (int a in array)
         {
            count += a;
         }
         return count;
      }

      protected static void increment(int[] array, float[] errors)
      {
         int index = 0;
         float biggestError = errors[0];
         for (int i = 1; i < array.Length; i++)
         {
            if (errors[i] > biggestError)
            {
               biggestError = errors[i];
               index = i;
            }
         }
         array[index]++;
      }

      protected static void decrement(int[] array, float[] errors)
      {
         int index = 0;
         float biggestError = errors[0];
         for (int i = 1; i < array.Length; i++)
         {
            if (errors[i] < biggestError)
            {
               biggestError = errors[i];
               index = i;
            }
         }
         array[index]--;
      }

      protected static bool isFinderPattern(int[] counters)
      {
         int firstTwoSum = counters[0] + counters[1];
         int sum = firstTwoSum + counters[2] + counters[3];
         float ratio = (float)firstTwoSum / (float)sum;
         if (ratio >= MIN_FINDER_PATTERN_RATIO && ratio <= MAX_FINDER_PATTERN_RATIO)
         {
            // passes ratio test in spec, but see if the counts are unreasonable
            int minCounter = Int32.MaxValue;
            int maxCounter = Int32.MinValue;
            foreach (int counter in counters)
            {
               if (counter > maxCounter)
               {
                  maxCounter = counter;
               }
               if (counter < minCounter)
               {
                  minCounter = counter;
               }
            }
            return maxCounter < 10 * minCounter;
         }
         return false;
      }
   }
}
