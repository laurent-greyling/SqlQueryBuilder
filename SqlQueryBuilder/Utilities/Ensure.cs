using System;

namespace SqlQueryBuilder.Utilities
{
    /// <summary>
    /// Class to do basic parameter checks
    /// </summary>
    public class Ensure
    {
        /// <summary>
        /// This allows us to defer parameter checking to seperate class and prevent CA1062 unnecessarily
        /// </summary>
        [AttributeUsage(AttributeTargets.Parameter)]
        private sealed class ValidateNotNullAttribute : Attribute { }

        /// <summary>
        /// Throws <see cref="ArgumentNullException"/> if specified value is <see langword="null"/>
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="name">name of the parameter, which will appear in the exception message</param>
        public static void ArgumentNotNull([ValidateNotNull] object value, string name)
        {
            if (value is null)
            {
                throw new ArgumentNullException(name);
            }
        }

        /// <summary>
        /// Throws <see cref="ArgumentException"/> if specified value is <see langword="null"/> or empty string
        /// </summary>
        /// <param name="value">value to test</param>
        /// <param name="name">name of the parameter, which will appear in the exception message</param>
        public static void ArgumentNotNullOrEmptyString([ValidateNotNull] string value, string name)
        {
            ArgumentNotNull(value, name);

            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"The argument {name} cannot be an empty string");
            }
        }
    }
}
