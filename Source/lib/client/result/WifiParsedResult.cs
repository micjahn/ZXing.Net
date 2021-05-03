/*
 * Copyright 2010 ZXing authors
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
    /// <summary>
    /// Represents a parsed result that encodes wifi network information, like SSID and password.
    /// </summary>
    /// <author>Vikram Aggarwal</author>
    public class WifiParsedResult : ParsedResult
    {
        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="networkEncryption"></param>
        /// <param name="ssid"></param>
        /// <param name="password"></param>
        public WifiParsedResult(String networkEncryption, String ssid, String password)
           : this(networkEncryption, ssid, password, false)
        {
        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="networkEncryption"></param>
        /// <param name="ssid"></param>
        /// <param name="password"></param>
        /// <param name="hidden"></param>
        public WifiParsedResult(String networkEncryption, String ssid, String password, bool hidden)
           : this(networkEncryption, ssid, password, hidden, null, null, null, null)
        {

        }

        /// <summary>
        /// initializing constructor
        /// </summary>
        /// <param name="networkEncryption"></param>
        /// <param name="ssid"></param>
        /// <param name="password"></param>
        /// <param name="hidden"></param>
        /// <param name="identity"></param>
        /// <param name="anonymousIdentity"></param>
        /// <param name="eapMethod"></param>
        /// <param name="phase2Method"></param>
        public WifiParsedResult(String networkEncryption, String ssid, String password, bool hidden, String identity, String anonymousIdentity, String eapMethod, String phase2Method)
           : base(ParsedResultType.WIFI)
        {
            Ssid = ssid;
            NetworkEncryption = networkEncryption;
            Password = password;
            Hidden = hidden;
            Identity = identity;
            AnonymousIdentity = anonymousIdentity;
            EapMethod = eapMethod;
            Phase2Method = phase2Method;

            var result = new StringBuilder(80);
            maybeAppend(Ssid, result);
            maybeAppend(NetworkEncryption, result);
            maybeAppend(Password, result);
            maybeAppend(Hidden.ToString(), result);
            maybeAppend(Identity, result);
            maybeAppend(AnonymousIdentity, result);
            maybeAppend(EapMethod, result);
            maybeAppend(Phase2Method, result);
            displayResultValue = result.ToString();
        }

        /// <summary>
        /// SSID
        /// </summary>
        public String Ssid { get; private set; }
        /// <summary>
        /// network encryption
        /// </summary>
        public String NetworkEncryption { get; private set; }
        /// <summary>
        /// password
        /// </summary>
        public String Password { get; private set; }
        /// <summary>
        /// hidden flag
        /// </summary>
        public bool Hidden { get; private set; }
        /// <summary>
        /// identity
        /// </summary>
        public String Identity { get; private set; }

        /// <summary>
        /// anonymous
        /// </summary>
        public String AnonymousIdentity { get; private set; }
        /// <summary>
        /// eap
        /// </summary>
        public String EapMethod { get; private set; }
        /// <summary>
        /// phase 2 method
        /// </summary>
        public String Phase2Method { get; private set; }
    }
}