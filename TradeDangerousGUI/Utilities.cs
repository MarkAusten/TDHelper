using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
    }
}
