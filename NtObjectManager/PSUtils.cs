﻿//  Copyright 2018 Google Inc. All Rights Reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.

using NtApiDotNet.Utilities.Text;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Text;

namespace NtObjectManager
{
    /// <para type="description">Enumeration to specify a text encoding.</para>
    public enum TextEncodingType
    {
        /// <summary>
        /// Binary raw text.
        /// </summary>
        Binary,
        /// <summary>
        /// 16 bit unicode text.
        /// </summary>
        Unicode,
        /// <summary>
        /// Big Endian 16 bit unicode text.
        /// </summary>
        BigEndianUnicode,
        /// <summary>
        /// UTF8
        /// </summary>
        UTF8,
        /// <summary>
        /// UTF32
        /// </summary>
        UTF32,
        /// <summary>
        /// UTF7
        /// </summary>
        UTF7
    }

    internal static class PSUtils
    {
        internal static T InvokeWithArg<T>(this ScriptBlock script_block, T default_value, params object[] args) 
        {
            try
            {
                List<PSVariable> vars = new List<PSVariable>();
                if (args.Length > 0)
                {
                    vars.Add(new PSVariable("_", args[0]));
                }
#if NETCORE
                // Work around issue with standard PS library until it's fixed.
                var os = script_block.Invoke(args);
#else
                var os = script_block.InvokeWithContext(null, vars, args);
#endif
                if (os.Count > 0)
                {
                    return (T)Convert.ChangeType(os[0].BaseObject, typeof(T));
                }
            }
            catch
            {
            }
            return default_value;
        }

        internal static Collection<PSObject> InvokeWithArg(this ScriptBlock script_block, params object[] args)
        {
            List<PSVariable> vars = new List<PSVariable>();
            if (args.Length > 0)
            {
                vars.Add(new PSVariable("_", args[0]));
            }
#if NETCORE
            // Work around issue with standard PS library until it's fixed.
            return script_block.Invoke(args);
#else
            return script_block.InvokeWithContext(null, vars, args);
#endif
        }

        internal static Encoding GetEncoding(TextEncodingType encoding)
        {
            switch (encoding)
            {
                case TextEncodingType.Binary:
                    return BinaryEncoding.Instance;
                case TextEncodingType.Unicode:
                    return Encoding.Unicode;
                case TextEncodingType.BigEndianUnicode:
                    return Encoding.BigEndianUnicode;
                case TextEncodingType.UTF8:
                    return Encoding.UTF8;
                case TextEncodingType.UTF32:
                    return Encoding.UTF32;
                case TextEncodingType.UTF7:
                    return Encoding.UTF7;
                default:
                    throw new ArgumentException("Unknown text encoding", nameof(encoding));
            }
        }
    }
}
