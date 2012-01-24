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

/*
 * These authors would like to acknowledge the Spanish Ministry of Industry,
 * Tourism and Trade, for the support in the project TSI020301-2008-2
 * "PIRAmIDE: Personalizable Interactions with Resources on AmI-enabled
 * Mobile Dynamic Environments", led by Treelogic
 * ( http://www.treelogic.com/ ):
 *
 *   http://www.piramidepse.com/
 */

using System;
using System.Collections.Generic;

namespace com.google.zxing.oned.rss.expanded.decoders
{


   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   sealed class FieldParser
   {

      private static Object VARIABLE_LENGTH = new Object();

      private static Object[][] TWO_DIGIT_DATA_LENGTH = new Object[][]
                                                           {
                                                              // "DIGITS", new Integer(LENGTH)
                                                              //    or
                                                              // "DIGITS", VARIABLE_LENGTH, new Integer(MAX_SIZE)

                                                              new object[] {"00", 18},
                                                              new object[] {"01", 14},
                                                              new object[] {"02", 14},

                                                              new object[] {"10", VARIABLE_LENGTH, 20},
                                                              new object[] {"11", 6},
                                                              new object[] {"12", 6},
                                                              new object[] {"13", 6},
                                                              new object[] {"15", 6},
                                                              new object[] {"17", 6},

                                                              new object[] {"20", 2},
                                                              new object[] {"21", VARIABLE_LENGTH, 20},
                                                              new object[] {"22", VARIABLE_LENGTH, 29},

                                                              new object[] {"30", VARIABLE_LENGTH, 8},
                                                              new object[] {"37", VARIABLE_LENGTH, 8},

                                                              //internal company codes
                                                              new object[] {"90", VARIABLE_LENGTH, 30},
                                                              new object[] {"91", VARIABLE_LENGTH, 30},
                                                              new object[] {"92", VARIABLE_LENGTH, 30},
                                                              new object[] {"93", VARIABLE_LENGTH, 30},
                                                              new object[] {"94", VARIABLE_LENGTH, 30},
                                                              new object[] {"95", VARIABLE_LENGTH, 30},
                                                              new object[] {"96", VARIABLE_LENGTH, 30},
                                                              new object[] {"97", VARIABLE_LENGTH, 30},
                                                              new object[] {"98", VARIABLE_LENGTH, 30},
                                                              new object[] {"99", VARIABLE_LENGTH, 30},
                                                           };

      private static Object[][] THREE_DIGIT_DATA_LENGTH = new Object[][]
                                                             {
                                                                // Same format as above

                                                                new object[] {"240", VARIABLE_LENGTH, 30},
                                                                new object[] {"241", VARIABLE_LENGTH, 30},
                                                                new object[] {"242", VARIABLE_LENGTH, 6},
                                                                new object[] {"250", VARIABLE_LENGTH, 30},
                                                                new object[] {"251", VARIABLE_LENGTH, 30},
                                                                new object[] {"253", VARIABLE_LENGTH, 17},
                                                                new object[] {"254", VARIABLE_LENGTH, 20},

                                                                new object[] {"400", VARIABLE_LENGTH, 30},
                                                                new object[] {"401", VARIABLE_LENGTH, 30},
                                                                new object[] {"402", 17},
                                                                new object[] {"403", VARIABLE_LENGTH, 30},
                                                                new object[] {"410", 13},
                                                                new object[] {"411", 13},
                                                                new object[] {"412", 13},
                                                                new object[] {"413", 13},
                                                                new object[] {"414", 13},
                                                                new object[] {"420", VARIABLE_LENGTH, 20},
                                                                new object[] {"421", VARIABLE_LENGTH, 15},
                                                                new object[] {"422", 3},
                                                                new object[] {"423", VARIABLE_LENGTH, 15},
                                                                new object[] {"424", 3},
                                                                new object[] {"425", 3},
                                                                new object[] {"426", 3},
                                                             };

      private static Object[][] THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH = new Object[][]
                                                                        {
                                                                           // Same format as above

                                                                           new object[] {"310", 6},
                                                                           new object[] {"311", 6},
                                                                           new object[] {"312", 6},
                                                                           new object[] {"313", 6},
                                                                           new object[] {"314", 6},
                                                                           new object[] {"315", 6},
                                                                           new object[] {"316", 6},
                                                                           new object[] {"320", 6},
                                                                           new object[] {"321", 6},
                                                                           new object[] {"322", 6},
                                                                           new object[] {"323", 6},
                                                                           new object[] {"324", 6},
                                                                           new object[] {"325", 6},
                                                                           new object[] {"326", 6},
                                                                           new object[] {"327", 6},
                                                                           new object[] {"328", 6},
                                                                           new object[] {"329", 6},
                                                                           new object[] {"330", 6},
                                                                           new object[] {"331", 6},
                                                                           new object[] {"332", 6},
                                                                           new object[] {"333", 6},
                                                                           new object[] {"334", 6},
                                                                           new object[] {"335", 6},
                                                                           new object[] {"336", 6},
                                                                           new object[] {"340", 6},
                                                                           new object[] {"341", 6},
                                                                           new object[] {"342", 6},
                                                                           new object[] {"343", 6},
                                                                           new object[] {"344", 6},
                                                                           new object[] {"345", 6},
                                                                           new object[] {"346", 6},
                                                                           new object[] {"347", 6},
                                                                           new object[] {"348", 6},
                                                                           new object[] {"349", 6},
                                                                           new object[] {"350", 6},
                                                                           new object[] {"351", 6},
                                                                           new object[] {"352", 6},
                                                                           new object[] {"353", 6},
                                                                           new object[] {"354", 6},
                                                                           new object[] {"355", 6},
                                                                           new object[] {"356", 6},
                                                                           new object[] {"357", 6},
                                                                           new object[] {"360", 6},
                                                                           new object[] {"361", 6},
                                                                           new object[] {"362", 6},
                                                                           new object[] {"363", 6},
                                                                           new object[] {"364", 6},
                                                                           new object[] {"365", 6},
                                                                           new object[] {"366", 6},
                                                                           new object[] {"367", 6},
                                                                           new object[] {"368", 6},
                                                                           new object[] {"369", 6},
                                                                           new object[] {"390", VARIABLE_LENGTH, 15},
                                                                           new object[] {"391", VARIABLE_LENGTH, 18},
                                                                           new object[] {"392", VARIABLE_LENGTH, 15},
                                                                           new object[] {"393", VARIABLE_LENGTH, 18},
                                                                           new object[] {"703", VARIABLE_LENGTH, 30}
                                                                        };

      private static Object[][] FOUR_DIGIT_DATA_LENGTH = new Object[][]
                                                            {
                                                               // Same format as above

                                                               new object[] {"7001", 13},
                                                               new object[] {"7002", VARIABLE_LENGTH, 30},
                                                               new object[] {"7003", 10},

                                                               new object[] {"8001", 14},
                                                               new object[] {"8002", VARIABLE_LENGTH, 20},
                                                               new object[] {"8003", VARIABLE_LENGTH, 30},
                                                               new object[] {"8004", VARIABLE_LENGTH, 30},
                                                               new object[] {"8005", 6},
                                                               new object[] {"8006", 18},
                                                               new object[] {"8007", VARIABLE_LENGTH, 30},
                                                               new object[] {"8008", VARIABLE_LENGTH, 12},
                                                               new object[] {"8018", 18},
                                                               new object[] {"8020", VARIABLE_LENGTH, 25},
                                                               new object[] {"8100", 6},
                                                               new object[] {"8101", 10},
                                                               new object[] {"8102", 2},
                                                               new object[] {"8110", VARIABLE_LENGTH, 30},
                                                            };

      private FieldParser()
      {
      }

      internal static String parseFieldsInGeneralPurpose(String rawInformation)
      {
         if (rawInformation.Length == 0)
         {
            return null;
         }

         // Processing 2-digit AIs

         if (rawInformation.Length < 2)
         {
            throw NotFoundException.Instance;
         }

         String firstTwoDigits = rawInformation.Substring(0, 2);

         foreach (Object[] dataLength in TWO_DIGIT_DATA_LENGTH)
         {
            if (dataLength[0].Equals(firstTwoDigits))
            {
               if (dataLength[1] == VARIABLE_LENGTH)
               {
                  return processVariableAI(2, (int)dataLength[2], rawInformation);
               }
               return processFixedAI(2, (int)dataLength[1], rawInformation);
            }
         }

         if (rawInformation.Length < 3)
         {
            throw NotFoundException.Instance;
         }

         String firstThreeDigits = rawInformation.Substring(0, 3);

         foreach (Object[] dataLength in THREE_DIGIT_DATA_LENGTH)
         {
            if (dataLength[0].Equals(firstThreeDigits))
            {
               if (dataLength[1] == VARIABLE_LENGTH)
               {
                  return processVariableAI(3, (int)dataLength[2], rawInformation);
               }
               return processFixedAI(3, (int)dataLength[1], rawInformation);
            }
         }


         foreach (Object[] dataLength in THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH)
         {
            if (dataLength[0].Equals(firstThreeDigits))
            {
               if (dataLength[1] == VARIABLE_LENGTH)
               {
                  return processVariableAI(4, (int)dataLength[2], rawInformation);
               }
               return processFixedAI(4, (int)dataLength[1], rawInformation);
            }
         }

         if (rawInformation.Length < 4)
         {
            throw NotFoundException.Instance;
         }

         String firstFourDigits = rawInformation.Substring(0, 4);

         foreach (Object[] dataLength in FOUR_DIGIT_DATA_LENGTH)
         {
            if (dataLength[0].Equals(firstFourDigits))
            {
               if (dataLength[1] == VARIABLE_LENGTH)
               {
                  return processVariableAI(4, (int)dataLength[2], rawInformation);
               }
               return processFixedAI(4, (int)dataLength[1], rawInformation);
            }
         }

         throw NotFoundException.Instance;
      }

      private static String processFixedAI(int aiSize, int fieldSize, String rawInformation)
      {
         if (rawInformation.Length < aiSize)
         {
            throw NotFoundException.Instance;
         }

         String ai = rawInformation.Substring(0, aiSize);

         if (rawInformation.Length < aiSize + fieldSize)
         {
            throw NotFoundException.Instance;
         }

         String field = rawInformation.Substring(aiSize, fieldSize);
         String remaining = rawInformation.Substring(aiSize + fieldSize);
         String result = '(' + ai + ')' + field;
         String parsedAI = parseFieldsInGeneralPurpose(remaining);
         return parsedAI == null ? result : result + parsedAI;
      }

      private static String processVariableAI(int aiSize, int variableFieldSize, String rawInformation)
      {
         String ai = rawInformation.Substring(0, aiSize);
         int maxSize;
         if (rawInformation.Length < aiSize + variableFieldSize)
         {
            maxSize = rawInformation.Length;
         }
         else
         {
            maxSize = aiSize + variableFieldSize;
         }
         String field = rawInformation.Substring(aiSize, maxSize - aiSize);
         String remaining = rawInformation.Substring(maxSize);
         String result = '(' + ai + ')' + field;
         String parsedAI = parseFieldsInGeneralPurpose(remaining);
         return parsedAI == null ? result : result + parsedAI;
      }
   }
}
