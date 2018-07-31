using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TDHelper
{
    public static class Utilities
    {
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
