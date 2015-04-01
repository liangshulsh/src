// Ai Software Library.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;

namespace Ai.Common {
    /// <summary>
    /// Provides commonly used objects and functions.
    /// </summary>
    public static class Common {
        public static CultureInfo ci_en_us = new CultureInfo("en-us");
        public static CultureInfo ci_id_id = new CultureInfo("id-id");
        public static CultureInfo ci_app;
        public static Huffman huffman = new Huffman();
        #region Public Methods
        /// <summary>
        /// Gets current method name that executed by the application.
        /// </summary>
        /// <returns>String value represent current method name in a class name.</returns>
        public static string getCaller() {
            StackTrace st = new StackTrace();
            StackFrame[] sfs = st.GetFrames();
            if (sfs.Length == 0) return "";
            System.Reflection.MethodBase mb;
            if (sfs.Length >= 2) mb = sfs[1].GetMethod();
            else mb = sfs[0].GetMethod();
            return mb.Name + " in " + mb.DeclaringType.Name;
        }
        /// <summary>
        /// Gets current method name that executed by the application with specified stack level.
        /// </summary>
        /// <param name="level">A number represent the level of the current stack.</param>
        /// <returns>String value represent current method name in a class name.</returns>
        public static string getCaller(int level) {
            StackTrace st = new StackTrace();
            StackFrame[] sfs = st.GetFrames();
            if (sfs.Length == 0) return "";
            System.Reflection.MethodBase mb;
            if (sfs.Length >= level + 1) mb = sfs[level].GetMethod();
            else return "";
            return mb.Name + " in " + mb.DeclaringType.Name;
        }
        /// <summary>
        /// Converts an int value to its binary value.
        /// </summary>
        /// <param name="value">An int value to be converted.</param>
        /// <returns>A string value represent the binary value of an int value.</returns>
        public static string binary(int value) {
            bool sign;
            string result = "";
            sign = false;
            if (value < 0) {
                value = -value;
                sign = true;
            }
            while (value > 0) {
                result = (value % 2) + result;
                value = value / 2;
            }
            while (result.Length % 8 != 0 || result.Length == 0) result = "0" + result;
            if (sign) result = "1" + result.Substring(1);
            return result;
        }
        /// <summary>
        /// Gets fully qualified domain name of the current machine.
        /// </summary>
        /// <returns>A string value represent the FQDN of the machine.</returns>
        public static string getFQDN() { 
            string domainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
            string hostName = Dns.GetHostName();
            string fqdn = "";
            if (!hostName.Contains(domainName)) fqdn = hostName + "." + domainName;
            else fqdn = hostName;
            return fqdn;
        }
        #endregion
    }
}