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

namespace ZXing.OneD.RSS.Expanded.Decoders
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// <author>Eduardo Castillejo, University of Deusto (eduardo.castillejo@deusto.es)</author>
   /// </summary>
   static class FieldParser
   {
      private static readonly Object VARIABLE_LENGTH = new Object();

      // "DIGITS", new Integer(LENGTH)
      //    or
      // "DIGITS", VARIABLE_LENGTH, new Integer(MAX_SIZE)
      private static readonly IDictionary<string, object[]> TWO_DIGIT_DATA_LENGTH;
      private static readonly IDictionary<string, object[]> THREE_DIGIT_DATA_LENGTH;
      private static readonly IDictionary<string, object[]> THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH;
      private static readonly IDictionary<string, object[]> FOUR_DIGIT_DATA_LENGTH;

      static FieldParser()
      {
         TWO_DIGIT_DATA_LENGTH = new Dictionary<string, object[]>
                                    {
                                       {"00", new object[] {18}},
                                       {"01", new object[] {14}},
                                       {"02", new object[] {14}},
                                       {"10", new object[] {VARIABLE_LENGTH, 20}},
                                       {"11", new object[] {6}},
                                       {"12", new object[] {6}},
                                       {"13", new object[] {6}},
                                       {"15", new object[] {6}},
                                       {"17", new object[] {6}},
                                       {"20", new object[] {2}},
                                       {"21", new object[] {VARIABLE_LENGTH, 20}},
                                       {"22", new object[] {VARIABLE_LENGTH, 29}},
                                       {"30", new object[] {VARIABLE_LENGTH, 8}},
                                       {"37", new object[] {VARIABLE_LENGTH, 8}},
                                       //internal company codes
                                       {"90", new object[] {VARIABLE_LENGTH, 30}},
                                       {"91", new object[] {VARIABLE_LENGTH, 30}},
                                       {"92", new object[] {VARIABLE_LENGTH, 30}},
                                       {"93", new object[] {VARIABLE_LENGTH, 30}},
                                       {"94", new object[] {VARIABLE_LENGTH, 30}},
                                       {"95", new object[] {VARIABLE_LENGTH, 30}},
                                       {"96", new object[] {VARIABLE_LENGTH, 30}},
                                       {"97", new object[] {VARIABLE_LENGTH, 30}},
                                       {"98", new object[] {VARIABLE_LENGTH, 30}},
                                       {"99", new object[] {VARIABLE_LENGTH, 30}}
                                    };
         THREE_DIGIT_DATA_LENGTH = new Dictionary<string, object[]>
                                      {
                                         // Same format as above

                                         {"240", new object[] {VARIABLE_LENGTH, 30}},
                                         {"241", new object[] {VARIABLE_LENGTH, 30}},
                                         {"242", new object[] {VARIABLE_LENGTH, 6}},
                                         {"250", new object[] {VARIABLE_LENGTH, 30}},
                                         {"251", new object[] {VARIABLE_LENGTH, 30}},
                                         {"253", new object[] {VARIABLE_LENGTH, 17}},
                                         {"254", new object[] {VARIABLE_LENGTH, 20}},

                                         {"400", new object[] {VARIABLE_LENGTH, 30}},
                                         {"401", new object[] {VARIABLE_LENGTH, 30}},
                                         {"402", new object[] {17}},
                                         {"403", new object[] {VARIABLE_LENGTH, 30}},
                                         {"410", new object[] {13}},
                                         {"411", new object[] {13}},
                                         {"412", new object[] {13}},
                                         {"413", new object[] {13}},
                                         {"414", new object[] {13}},
                                         {"420", new object[] {VARIABLE_LENGTH, 20}},
                                         {"421", new object[] {VARIABLE_LENGTH, 15}},
                                         {"422", new object[] {3}},
                                         {"423", new object[] {VARIABLE_LENGTH, 15}},
                                         {"424", new object[] {3}},
                                         {"425", new object[] {3}},
                                         {"426", new object[] {3}},
                                      };
         THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH = new Dictionary<string, object[]>
                                                 {
                                                    {"310", new object[] {6}},
                                                    {"311", new object[] {6}},
                                                    {"312", new object[] {6}},
                                                    {"313", new object[] {6}},
                                                    {"314", new object[] {6}},
                                                    {"315", new object[] {6}},
                                                    {"316", new object[] {6}},
                                                    {"320", new object[] {6}},
                                                    {"321", new object[] {6}},
                                                    {"322", new object[] {6}},
                                                    {"323", new object[] {6}},
                                                    {"324", new object[] {6}},
                                                    {"325", new object[] {6}},
                                                    {"326", new object[] {6}},
                                                    {"327", new object[] {6}},
                                                    {"328", new object[] {6}},
                                                    {"329", new object[] {6}},
                                                    {"330", new object[] {6}},
                                                    {"331", new object[] {6}},
                                                    {"332", new object[] {6}},
                                                    {"333", new object[] {6}},
                                                    {"334", new object[] {6}},
                                                    {"335", new object[] {6}},
                                                    {"336", new object[] {6}},
                                                    {"340", new object[] {6}},
                                                    {"341", new object[] {6}},
                                                    {"342", new object[] {6}},
                                                    {"343", new object[] {6}},
                                                    {"344", new object[] {6}},
                                                    {"345", new object[] {6}},
                                                    {"346", new object[] {6}},
                                                    {"347", new object[] {6}},
                                                    {"348", new object[] {6}},
                                                    {"349", new object[] {6}},
                                                    {"350", new object[] {6}},
                                                    {"351", new object[] {6}},
                                                    {"352", new object[] {6}},
                                                    {"353", new object[] {6}},
                                                    {"354", new object[] {6}},
                                                    {"355", new object[] {6}},
                                                    {"356", new object[] {6}},
                                                    {"357", new object[] {6}},
                                                    {"360", new object[] {6}},
                                                    {"361", new object[] {6}},
                                                    {"362", new object[] {6}},
                                                    {"363", new object[] {6}},
                                                    {"364", new object[] {6}},
                                                    {"365", new object[] {6}},
                                                    {"366", new object[] {6}},
                                                    {"367", new object[] {6}},
                                                    {"368", new object[] {6}},
                                                    {"369", new object[] {6}},
                                                    {"390", new object[] {VARIABLE_LENGTH, 15}},
                                                    {"391", new object[] {VARIABLE_LENGTH, 18}},
                                                    {"392", new object[] {VARIABLE_LENGTH, 15}},
                                                    {"393", new object[] {VARIABLE_LENGTH, 18}},
                                                    {"703", new object[] {VARIABLE_LENGTH, 30}}

                                                 };
         FOUR_DIGIT_DATA_LENGTH = new Dictionary<string, object[]>
                                     {
                                        {"7001", new object[] {13}},
                                        {"7002", new object[] {VARIABLE_LENGTH, 30}},
                                        {"7003", new object[] {10}},

                                        {"8001", new object[] {14}},
                                        {"8002", new object[] {VARIABLE_LENGTH, 20}},
                                        {"8003", new object[] {VARIABLE_LENGTH, 30}},
                                        {"8004", new object[] {VARIABLE_LENGTH, 30}},
                                        {"8005", new object[] {6}},
                                        {"8006", new object[] {18}},
                                        {"8007", new object[] {VARIABLE_LENGTH, 30}},
                                        {"8008", new object[] {VARIABLE_LENGTH, 12}},
                                        {"8018", new object[] {18}},
                                        {"8020", new object[] {VARIABLE_LENGTH, 25}},
                                        {"8100", new object[] {6}},
                                        {"8101", new object[] {10}},
                                        {"8102", new object[] {2}},
                                        {"8110", new object[] {VARIABLE_LENGTH, 70}},
                                        {"8200", new object[] {VARIABLE_LENGTH, 70}},
                                     };
      }

      internal static String parseFieldsInGeneralPurpose(String rawInformation)
      {
         if (String.IsNullOrEmpty(rawInformation))
         {
            return null;
         }

         // Processing 2-digit AIs

         if (rawInformation.Length < 2)
         {
            return null;
         }

         String firstTwoDigits = rawInformation.Substring(0, 2);

         if (TWO_DIGIT_DATA_LENGTH.ContainsKey(firstTwoDigits))
         {
            var dataLength = TWO_DIGIT_DATA_LENGTH[firstTwoDigits];
            if (dataLength[0] == VARIABLE_LENGTH)
            {
               return processVariableAI(2, (int)dataLength[1], rawInformation);
            }
            return processFixedAI(2, (int)dataLength[0], rawInformation);
         }

         if (rawInformation.Length < 3)
         {
            return null;
         }

         String firstThreeDigits = rawInformation.Substring(0, 3);

         if (THREE_DIGIT_DATA_LENGTH.ContainsKey(firstThreeDigits))
         {
            var dataLength = THREE_DIGIT_DATA_LENGTH[firstThreeDigits];
            if (dataLength[0] == VARIABLE_LENGTH)
            {
               return processVariableAI(3, (int)dataLength[1], rawInformation);
            }
            return processFixedAI(3, (int)dataLength[0], rawInformation);
         }

         if (THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH.ContainsKey(firstThreeDigits))
         {
            var dataLength = THREE_DIGIT_PLUS_DIGIT_DATA_LENGTH[firstThreeDigits];
            if (dataLength[0] == VARIABLE_LENGTH)
            {
               return processVariableAI(4, (int)dataLength[1], rawInformation);
            }
            return processFixedAI(4, (int)dataLength[0], rawInformation);
         }

         if (rawInformation.Length < 4)
         {
            return null;
         }

         String firstFourDigits = rawInformation.Substring(0, 4);

         if (FOUR_DIGIT_DATA_LENGTH.ContainsKey(firstFourDigits))
         {
            var dataLength = FOUR_DIGIT_DATA_LENGTH[firstFourDigits];
            if (dataLength[0] == VARIABLE_LENGTH)
            {
               return processVariableAI(4, (int)dataLength[1], rawInformation);
            }
            return processFixedAI(4, (int)dataLength[0], rawInformation);
         }

         return null;
      }

      private static String processFixedAI(int aiSize, int fieldSize, String rawInformation)
      {
         if (rawInformation.Length < aiSize)
         {
            return null;
         }

         String ai = rawInformation.Substring(0, aiSize);

         if (rawInformation.Length < aiSize + fieldSize)
         {
            return null;
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