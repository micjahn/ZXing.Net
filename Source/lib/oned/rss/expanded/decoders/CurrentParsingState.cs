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

namespace ZXing.OneD.RSS.Expanded.Decoders
{
   /// <summary>
   /// <author>Pablo Orduña, University of Deusto (pablo.orduna@deusto.es)</author>
   /// </summary>
   internal sealed class CurrentParsingState
   {
      private int position;
      private State encoding;

      private enum State
      {
         NUMERIC,
         ALPHA,
         ISO_IEC_646
      }

      internal CurrentParsingState()
      {
         this.position = 0;
         this.encoding = State.NUMERIC;
      }

      internal int getPosition()
      {
         return position;
      }

      internal void setPosition(int position)
      {
         this.position = position;
      }

      internal void incrementPosition(int delta)
      {
         position += delta;
      }

      internal bool isAlpha()
      {
         return this.encoding == State.ALPHA;
      }

      internal bool isNumeric()
      {
         return this.encoding == State.NUMERIC;
      }

      internal bool isIsoIec646()
      {
         return this.encoding == State.ISO_IEC_646;
      }

      internal void setNumeric()
      {
         this.encoding = State.NUMERIC;
      }

      internal void setAlpha()
      {
         this.encoding = State.ALPHA;
      }

      internal void setIsoIec646()
      {
         this.encoding = State.ISO_IEC_646;
      }
   }
}
