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
        public static void Argument([NotNull] object? value, [CallerArgumentExpression(nameof(value))] string? name = null)
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

        public static void Positive(int value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(name, value, "Value must be greater than zero.");
            }
        }

        public static void Positive(int? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            Positive(value.Value, name);
        }

        public static void PositiveIfProvided(int? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value != null)
            {
                Positive(value.Value, name);
            }
        }

        public static void NonNegative(int value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(name, value, "Value cannot be negative.");
            }
        }

        public static void NonNegative(int? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value == null)
            {
                throw new ArgumentNullException(name);
            }

            NonNegative(value.Value, name);
        }

        public static void NonNegativeIfProvided(int? value, [CallerArgumentExpression(nameof(value))] string? name = null)
        {
            if (value != null)
            {
                NonNegative(value.Value, name);
            }
        }
    }
}
