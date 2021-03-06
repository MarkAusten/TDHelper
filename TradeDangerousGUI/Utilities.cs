﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace TDHelper
{
    public static class Utilities
    {
        /// <summary>
        /// Get a list of the child controls of the root object.
        /// </summary>
        /// <param name="root">The root object.</param>
        /// <returns>A list of all thr controls contained in the root.</returns>
        public static IEnumerable<Control> GetAllChildren(
            this Control root)
        {
            Stack<Control> stack = new Stack<Control>();

            stack.Push(root);

            while (stack.Any())
            {
                var next = stack.Pop();

                foreach (Control child in next.Controls)
                {
                    stack.Push(child);
                }

                yield return next;
            }
        }

        /// <summary>
        /// Get the message and all inner exception messages from the exception.
        /// </summary>
        /// <param name="ex">The exception with the messages.</param>
        /// <returns>A full list of messages.</returns>
        public static string GetFullMessage(this Exception ex)
        {
            string message = string.Empty;

            Exception exception = ex;

            do
            {
                message += "{0}{1}".With(Environment.NewLine, exception.Message);

                exception = exception.InnerException;
            }
            while (exception != null);

            return message.TrimStart(Environment.NewLine.ToCharArray());
        }

        /// <summary>
        /// Convert the number to string with a "." decimal point if required.
        /// </summary>
        /// <param name="number">The number to be converted.</param>
        /// <returns>The specified number as a string with a dot as a deciaml point.</returns>
        public static string ToEnglishString(this decimal number)
        {
            string result = number.ToString();

            // If the current culture uses commas as a decimal point, replace all deciml separators with a pipe, then all
            // group separators with a comma and finally all pipes with a dot.
            if (CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator != ".")
            {
                result = result
                    .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator, "|")
                    .Replace(CultureInfo.CurrentCulture.NumberFormat.NumberGroupSeparator, ",")
                    .Replace("|", ".");
            }

            return result;
        }

        /// <summary>
        /// Replacement for String.Format.
        /// </summary>
        /// <param name="format">The formatting string with placeholders.</param>
        /// <param name="args">The values to insert into the format string.</param>
        /// <returns>The string with the placeholders replaced.</returns>
        public static string With(
            this string format,
            params object[] args)
        {
            return string.Format(format, args);
        }
    }
}