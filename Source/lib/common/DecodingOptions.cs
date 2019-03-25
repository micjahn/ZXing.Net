﻿/*
 * Copyright 2013 ZXing.Net authors
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
using System.ComponentModel;

namespace ZXing.Common
{
    /// <summary>
    /// Defines an container for encoder options
    /// </summary>
    [Serializable]
    public class DecodingOptions
    {
        /// <summary>
        /// Gets the data container for all options
        /// </summary>
#if !UNITY
        [Browsable(false)]
#endif
        public IDictionary<DecodeHintType, object> Hints { get; private set; }

        /// <summary>
        /// event is raised when a value is changed
        /// </summary>
        [field: NonSerialized]
        public event Action<object, EventArgs> ValueChanged;

        /// <summary>
        /// Gets or sets a flag which cause a deeper look into the bitmap
        /// </summary>
        /// <value>
        ///   <c>true</c> if [try harder]; otherwise, <c>false</c>.
        /// </value>
        public bool TryHarder
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.TRY_HARDER))
                    return (bool)Hints[DecodeHintType.TRY_HARDER];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.TRY_HARDER] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.TRY_HARDER))
                    {
                        Hints.Remove(DecodeHintType.TRY_HARDER);
                    }
                }
            }
        }

        /// <summary>
        /// Image is a pure monochrome image of a barcode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if monochrome image of a barcode; otherwise, <c>false</c>.
        /// </value>
        public bool PureBarcode
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.PURE_BARCODE))
                    return (bool)Hints[DecodeHintType.PURE_BARCODE];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.PURE_BARCODE] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.PURE_BARCODE))
                    {
                        Hints.Remove(DecodeHintType.PURE_BARCODE);
                    }
                }
            }
        }

        /// <summary>
        /// Specifies what character encoding to use when decoding, where applicable (type String)
        /// </summary>
        /// <value>
        /// The character set.
        /// </value>
        public string CharacterSet
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.CHARACTER_SET))
                    return (string)Hints[DecodeHintType.CHARACTER_SET];
                return null;
            }
            set
            {
                if (value != null)
                {
                    Hints[DecodeHintType.CHARACTER_SET] = value;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.CHARACTER_SET))
                    {
                        Hints.Remove(DecodeHintType.CHARACTER_SET);
                    }
                }
            }
        }

        /// <summary>
        /// Image is known to be of one of a few possible formats.
        /// Maps to a {@link java.util.List} of {@link BarcodeFormat}s.
        /// </summary>
        /// <value>
        /// The possible formats.
        /// </value>
        public IList<BarcodeFormat> PossibleFormats
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS))
                    return (IList<BarcodeFormat>)Hints[DecodeHintType.POSSIBLE_FORMATS];
                return null;
            }
            set
            {
                if (value != null)
                {
                    Hints[DecodeHintType.POSSIBLE_FORMATS] = value;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.POSSIBLE_FORMATS))
                    {
                        Hints.Remove(DecodeHintType.POSSIBLE_FORMATS);
                    }
                }
            }
        }

        /// <summary>
        /// if Code39 could be detected try to use extended mode for full ASCII character set
        /// </summary>
        public bool UseCode39ExtendedMode
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.USE_CODE_39_EXTENDED_MODE))
                    return (bool)Hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE];
                return false;
            }
            set
            {
                // don't remove the hint. Otherwise it would not be possible to override the default behaviour of the Code39 reader
                Hints[DecodeHintType.USE_CODE_39_EXTENDED_MODE] = value;
            }
        }

        /// <summary>
        /// Don't fail if a Code39 is detected but can't be decoded in extended mode.
        /// Return the raw Code39 result instead. Maps to <see cref="bool" />.
        /// </summary>
        public bool UseCode39RelaxedExtendedMode
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE))
                    return (bool)Hints[DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE))
                    {
                        Hints.Remove(DecodeHintType.RELAXED_CODE_39_EXTENDED_MODE);
                    }
                }
            }
        }

        /// <summary>
        /// Assume Code 39 codes employ a check digit. Maps to <see cref="bool" />.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it should assume a Code 39 check digit; otherwise, <c>false</c>.
        /// </value>
        public bool AssumeCode39CheckDigit
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT))
                    return (bool)Hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT))
                    {
                        Hints.Remove(DecodeHintType.ASSUME_CODE_39_CHECK_DIGIT);
                    }
                }
            }
        }

        /// <summary>
        /// If true, return the start and end digits in a Codabar barcode instead of stripping them. They
        /// are alpha, whereas the rest are numeric. By default, they are stripped, but this causes them
        /// to not be. Doesn't matter what it maps to; use <see cref="bool" />.
        /// </summary>
        public bool ReturnCodabarStartEnd
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.RETURN_CODABAR_START_END))
                    return (bool)Hints[DecodeHintType.RETURN_CODABAR_START_END];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.RETURN_CODABAR_START_END] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.RETURN_CODABAR_START_END))
                    {
                        Hints.Remove(DecodeHintType.RETURN_CODABAR_START_END);
                    }
                }
            }
        }

        /// <summary>
        /// Assume the barcode is being processed as a GS1 barcode, and modify behavior as needed.
        /// For example this affects FNC1 handling for Code 128 (aka GS1-128).
        /// </summary>
        /// <value>
        ///   <c>true</c> if it should assume GS1; otherwise, <c>false</c>.
        /// </value>
        public bool AssumeGS1
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.ASSUME_GS1))
                    return (bool)Hints[DecodeHintType.ASSUME_GS1];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.ASSUME_GS1] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.ASSUME_GS1))
                    {
                        Hints.Remove(DecodeHintType.ASSUME_GS1);
                    }
                }
            }
        }

        /// <summary>
        /// Assume MSI codes employ a check digit. Maps to <see cref="bool" />.
        /// </summary>
        /// <value>
        ///   <c>true</c> if it should assume a MSI check digit; otherwise, <c>false</c>.
        /// </value>
        public bool AssumeMSICheckDigit
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.ASSUME_MSI_CHECK_DIGIT))
                    return (bool)Hints[DecodeHintType.ASSUME_MSI_CHECK_DIGIT];
                return false;
            }
            set
            {
                if (value)
                {
                    Hints[DecodeHintType.ASSUME_MSI_CHECK_DIGIT] = true;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.ASSUME_MSI_CHECK_DIGIT))
                    {
                        Hints.Remove(DecodeHintType.ASSUME_MSI_CHECK_DIGIT);
                    }
                }
            }
        }

        /// <summary>
        /// Allowed lengths of encoded data -- reject anything else. Maps to an int[].
        /// </summary>
        public int[] AllowedLengths
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.ALLOWED_LENGTHS))
                    return (int[])Hints[DecodeHintType.ALLOWED_LENGTHS];
                return null;
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    Hints[DecodeHintType.ALLOWED_LENGTHS] = value;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.ALLOWED_LENGTHS))
                    {
                        Hints.Remove(DecodeHintType.ALLOWED_LENGTHS);
                    }
                }
            }
        }

        /// <summary>
        /// Allowed extension lengths for EAN or UPC barcodes. Other formats will ignore this.
        /// Maps to an int[] of the allowed extension lengths, for example [2], [5], or [2, 5].
        /// If it is optional to have an extension, do not set this hint. If this is set,
        /// and a UPC or EAN barcode is found but an extension is not, then no result will be returned
        /// at all.
        /// </summary>
        public int[] AllowedEANExtensions
        {
            get
            {
                if (Hints.ContainsKey(DecodeHintType.ALLOWED_EAN_EXTENSIONS))
                    return (int[])Hints[DecodeHintType.ALLOWED_EAN_EXTENSIONS];
                return null;
            }
            set
            {
                if (value != null && value.Length > 0)
                {
                    Hints[DecodeHintType.ALLOWED_EAN_EXTENSIONS] = value;
                }
                else
                {
                    if (Hints.ContainsKey(DecodeHintType.ALLOWED_EAN_EXTENSIONS))
                    {
                        Hints.Remove(DecodeHintType.ALLOWED_EAN_EXTENSIONS);
                    }
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DecodingOptions"/> class.
        /// </summary>
        public DecodingOptions()
        {
            var hints = new ChangeNotifyDictionary<DecodeHintType, object>();
            Hints = hints;
            UseCode39ExtendedMode = true;
            UseCode39RelaxedExtendedMode = true;
            hints.ValueChanged += (o, args) => { if (ValueChanged != null) ValueChanged(this, EventArgs.Empty); };
        }

        [Serializable]
        private class ChangeNotifyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
        {
            private readonly IDictionary<TKey, TValue> values;

            [field: NonSerialized]
            public event Action<object, EventArgs> ValueChanged;

            public ChangeNotifyDictionary()
            {
                values = new Dictionary<TKey, TValue>();
            }

            private void OnValueChanged()
            {
                if (ValueChanged != null)
                    ValueChanged(this, EventArgs.Empty);
            }

            public void Add(TKey key, TValue value)
            {
                values.Add(key, value);
                OnValueChanged();
            }

            public bool ContainsKey(TKey key)
            {
                return values.ContainsKey(key);
            }

            public ICollection<TKey> Keys
            {
                get { return values.Keys; }
            }

            public bool Remove(TKey key)
            {
                var result = values.Remove(key);
                OnValueChanged();
                return result;
            }

            public bool TryGetValue(TKey key, out TValue value)
            {
                return values.TryGetValue(key, out value);
            }

            public ICollection<TValue> Values
            {
                get { return values.Values; }
            }

            public TValue this[TKey key]
            {
                get
                {
                    return values[key];
                }
                set
                {
                    values[key] = value;
                    OnValueChanged();
                }
            }

            public void Add(KeyValuePair<TKey, TValue> item)
            {
                values.Add(item);
                OnValueChanged();
            }

            public void Clear()
            {
                values.Clear();
                OnValueChanged();
            }

            public bool Contains(KeyValuePair<TKey, TValue> item)
            {
                return values.Contains(item);
            }

            public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            {
                values.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get { return values.Count; }
            }

            public bool IsReadOnly
            {
                get { return values.IsReadOnly; }
            }

            public bool Remove(KeyValuePair<TKey, TValue> item)
            {
                var result = values.Remove(item);
                OnValueChanged();

                return result;
            }

            public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            {
                return values.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return ((System.Collections.IEnumerable)values).GetEnumerator();
            }
        }
    }
}
