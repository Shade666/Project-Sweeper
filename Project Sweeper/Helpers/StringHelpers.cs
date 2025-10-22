using System;
using System.Security;
using System.Xml;

namespace PKHL.ProjectSweeper.Helpers
{
    /// <summary>
    /// String utility methods
    /// </summary>
    public static class StringHelpers
    {
        /// <summary>
        /// Makes a string safe for use in XML by escaping special characters
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>XML-safe string</returns>
        public static string SafeForXML(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return SecurityElement.Escape(input);
        }

        /// <summary>
        /// Removes newlines from a string (for compatibility, but not actually used)
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>String with newlines removed</returns>
        public static string RemoveNewLines(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            return input.Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ");
        }
    }
}
