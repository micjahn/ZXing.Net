/*
* Copyright 2007 ZXing authors
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

namespace ZXing.Client.Result
{
    /// <summary> <p>Abstract class representing the result of decoding a barcode, as more than
    /// a String -- as some type of structured data. This might be a subclass which represents
    /// a URL, or an e-mail address. {@link ResultParser#parseResult(Result)} will turn a raw
    /// decoded string into the most appropriate type of structured representation.</p>
    /// 
    /// <p>Thanks to Jeff Griffin for proposing rewrite of these classes that relies less
    /// on exception-based mechanisms during parsing.</p>
    /// </summary>
    /// <author>Sean Owen</author>
    public abstract class ParsedResult
    {
        /// <summary>
        /// user friendly value
        /// </summary>
        protected string displayResultValue;
        /// <summary>
        /// gets the type of the parsed result
        /// </summary>
        public virtual ParsedResultType Type { get; private set; }
        /// <summary>
        /// user friendly value
        /// </summary>
        public virtual String DisplayResult { get { return displayResultValue; } }
        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="type"></param>
        protected ParsedResult(ParsedResultType type)
        {
            Type = type;
        }
        /// <summary>
        /// gets a user friendly value
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return DisplayResult;
        }
        /// <summary>
        /// compare two objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as ParsedResult;
            if (other == null)
                return false;
            return other.Type.Equals(Type) && other.DisplayResult.Equals(DisplayResult);
        }
        /// <summary>
        /// gets the hashcode
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Type.GetHashCode() + DisplayResult.GetHashCode();
        }
        /// <summary>
        /// append to result if not null or empty
        /// </summary>
        /// <param name="value"></param>
        /// <param name="result"></param>
        public static void maybeAppend(String value, StringBuilder result)
        {
            if (String.IsNullOrEmpty(value))
                return;

            // Don't add a newline before the first value
            if (result.Length > 0)
            {
                result.Append('\n');
            }
            result.Append(value);
        }
        /// <summary>
        /// append to result if not null or empty
        /// </summary>
        /// <param name="values"></param>
        /// <param name="result"></param>
        public static void maybeAppend(String[] values, StringBuilder result)
        {
            if (values != null)
            {
                foreach (String value in values)
                {
                    maybeAppend(value, result);
                }
            }
        }
    }
}