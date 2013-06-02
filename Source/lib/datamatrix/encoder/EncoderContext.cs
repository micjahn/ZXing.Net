/*
 * Copyright 2006-2007 Jeremias Maerki.
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

namespace ZXing.Datamatrix.Encoder
{
   internal sealed class EncoderContext
   {
      private readonly String msg;
      private SymbolShapeHint shape;
      private Dimension minSize;
      private Dimension maxSize;
      private readonly StringBuilder codewords;
      private int pos;
      private int newEncoding;
      private SymbolInfo symbolInfo;
      private int skipAtEnd;
      private static readonly Encoding encoding;

      static EncoderContext()
      {
#if !(WindowsCE || SILVERLIGHT4 || SILVERLIGHT5 || NETFX_CORE || PORTABLE)
         encoding = Encoding.GetEncoding("ISO-8859-1");
#elif WindowsCE
         try
         {
            encoding = Encoding.GetEncoding("ISO-8859-1");
         }
         catch (PlatformNotSupportedException)
         {
            encoding = Encoding.GetEncoding(1252);
         }
#else
         // not fully correct but what else
         encoding = Encoding.GetEncoding("UTF-8");
#endif
      }

      public EncoderContext(String msg)
      {
         //From this point on Strings are not Unicode anymore!
         var msgBinary = encoding.GetBytes(msg);
         var sb = new StringBuilder(msgBinary.Length);
         var c = msgBinary.Length;
         for (int i = 0; i < c; i++)
         {
            // TODO: does it works in .Net the same way?
            var ch = (char)(msgBinary[i] & 0xff);
            if (ch == '?' && msg[i] != '?')
            {
               throw new ArgumentException("Message contains characters outside " + encoding.WebName + " encoding.");
            }
            sb.Append(ch);
         }
         this.msg = sb.ToString(); //Not Unicode here!
         shape = SymbolShapeHint.FORCE_NONE;
         this.codewords = new StringBuilder(msg.Length);
         newEncoding = -1;
      }

      public void setSymbolShape(SymbolShapeHint shape)
      {
         this.shape = shape;
      }

      public void setSizeConstraints(Dimension minSize, Dimension maxSize)
      {
         this.minSize = minSize;
         this.maxSize = maxSize;
      }

      public void setSkipAtEnd(int count)
      {
         this.skipAtEnd = count;
      }

      public char CurrentChar
      {
         get { return msg[pos]; }
      }

      public char Current
      {
         get { return msg[pos]; }
      }

      public void writeCodewords(String codewords)
      {
         this.codewords.Append(codewords);
      }

      public void writeCodeword(char codeword)
      {
         this.codewords.Append(codeword);
      }

      public int CodewordCount
      {
         get { return this.codewords.Length; }
      }

      public void signalEncoderChange(int encoding)
      {
         this.newEncoding = encoding;
      }

      public void resetEncoderSignal()
      {
         this.newEncoding = -1;
      }

      public bool HasMoreCharacters
      {
         get { return pos < TotalMessageCharCount; }
      }

      private int TotalMessageCharCount
      {
         get { return msg.Length - skipAtEnd; }
      }

      public int RemainingCharacters
      {
         get { return TotalMessageCharCount - pos; }
      }

      public void updateSymbolInfo()
      {
         updateSymbolInfo(CodewordCount);
      }

      public void updateSymbolInfo(int len)
      {
         if (this.symbolInfo == null || len > this.symbolInfo.dataCapacity)
         {
            this.symbolInfo = SymbolInfo.lookup(len, shape, minSize, maxSize, true);
         }
      }

      public void resetSymbolInfo()
      {
         this.symbolInfo = null;
      }

      public int Pos
      {
         get { return pos; }
         set { pos = value; }
      }

      public StringBuilder Codewords
      {
         get { return codewords; }
      }

      public SymbolInfo SymbolInfo
      {
         get { return symbolInfo; }
      }

      public int NewEncoding
      {
         get { return newEncoding; }
      }

      public String Message
      {
         get { return msg; }
      }
   }
}