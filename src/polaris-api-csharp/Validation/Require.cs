using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Clc.Polaris.Api.Validation
{
    /// <summary>
    /// Allows us to require properties of parameter objects
    /// </summary>
    public class Require
    {
        /// <summary>
        /// Verify argument is provided
        /// </summary>
        /// <param name="value"></param>
        /// <param name="name"></param>
        public static void Argument(
            [NotNull] object? value,
            [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            if (value is string stringValue && string.IsNullOrWhiteSpace(stringValue))
            {
                throw new ArgumentException("Value cannot be empty or whitespace.", name);
            }
        }
    }
}
