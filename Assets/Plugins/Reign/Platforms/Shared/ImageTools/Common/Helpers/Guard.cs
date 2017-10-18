#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Guard.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace ImageTools.Helpers
{
    /// <summary>
    /// A static class with a lot of helper methods, which guards 
    /// a method agains invalid parameters.
    /// </summary>
    public static class Guard
    {
        /// <summary>
        /// Verifies that the specified value is between a lower and a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not between
        /// the lower and the upper value.</exception>
        public static void Between<TValue>(TValue target, TValue lower, TValue upper, string parameterName) where TValue : IComparable
        {
            if (!target.IsBetween(lower, upper))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be between {0} and {1}", lower, upper), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is between a lower and a upper value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is not between
        /// the lower and the upper value.</exception>
        public static void Between<TValue>(TValue target, TValue lower, TValue upper, string parameterName, string message) where TValue : IComparable
        {
            if (!target.IsBetween(lower, upper))
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater than a lower value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterThan<TValue>(TValue target, TValue lower, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(lower) <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be greater than {0}", lower), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater than a lower value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterThan<TValue>(TValue target, TValue lower, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(lower) <= 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater or equals than a lower value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterEquals<TValue>(TValue target, TValue lower, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(lower) < 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be greater than {0}", lower), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is greater or equals than a lower value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="lower">The lower value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void GreaterEquals<TValue>(TValue target, TValue lower, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(lower) < 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less than a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessThan<TValue>(TValue target, TValue upper, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(upper) <= 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be less than {0}", upper), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less than a upper value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessThan<TValue>(TValue target, TValue upper, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(upper) <= 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less or equals than a upper value
        /// and throws an exception, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessEquals<TValue>(TValue target, TValue upper, string parameterName) where TValue : IComparable
        {
            if (target.CompareTo(upper) > 0)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, "Value must be less than {0}", upper), parameterName);
            }
        }

        /// <summary>
        /// Verifies that the specified value is less or equals than a upper value
        /// and throws an exception with the passed message, if not.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="target">The target value, which should be validated.</param>
        /// <param name="upper">The upper value.</param>
        /// <param name="parameterName">Name of the parameter, which should will be checked.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="target"/> is greater than
        /// the lower value.</exception>
        public static void LessEquals<TValue>(TValue target, TValue upper, string parameterName, string message) where TValue : IComparable
        {
            if (target.CompareTo(upper) > 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the collection method parameter with specified reference
        /// contains one or more elements and throws an exception
        /// if the object contains no elements.
        /// </summary>
        /// <typeparam name="TType">The type of the items in the collection.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentException"><paramref name="enumerable"/> is
        /// empty or contains only blanks.</exception>
        public static void NotEmpty<TType>(ICollection<TType> enumerable, string parameterName)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (enumerable.Count == 0)
            {
                throw new ArgumentException("Collection does not contain an item", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the collection method parameter with specified reference
        /// contains one or more elements and throws an exception with
        /// the passed message if the object contains no elements.
        /// </summary>
        /// <typeparam name="TType">The type of the items in the collection.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentException"><paramref name="enumerable"/> is
        /// empty or contains only blanks.</exception>
        public static void NotEmpty<TType>(ICollection<TType> enumerable, string parameterName, string message)
        {
            if (enumerable == null)
            {
                throw new ArgumentNullException("enumerable");
            }

            if (enumerable.Count == 0)
            {
                throw new ArgumentException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the method parameter with specified object value and message  
        /// is not null and throws an exception if the object is null.
        /// </summary>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        public static void NotNull(object target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the method parameter with specified object value and message
        /// is not null and throws an exception with the passed message if the object is null.
        /// </summary>
        /// <param name="target">The target object, which cannot be null.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <example>
        /// Use the following code to validate a parameter:
        /// <code>
        /// // A method with parameter 'name' which cannot be null.
        /// public void MyMethod(string name)
        /// {
        ///    Guard.NotNull(name, "name", "Name is null!");
        /// }
        /// </code>
        /// </example>
        public static void NotNull(object target, string parameterName, string message)
        {
            if (target == null)
            {
                throw new ArgumentNullException(message, parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanls and throws an exception 
        /// if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        public static void NotNullOrEmpty(string target, string parameterName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(target) || target.Trim().Equals(string.Empty))
            {
                throw new ArgumentException("String parameter cannot be null or empty and cannot contain only blanks.", parameterName);
            }
        }

        /// <summary>
        /// Verifies, that the string method parameter with specified object value and message
        /// is not null, not empty and does not contain only blanls and throws an exception with 
        /// the passed message if the object is null.
        /// </summary>
        /// <param name="target">The target string, which should be checked against being null or empty.</param>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="message">The message for the exception to throw.</param>
        /// <exception cref="ArgumentNullException"><paramref name="target"/> is
        /// null (Nothing in Visual Basic).</exception>
        /// <exception cref="ArgumentException"><paramref name="target"/> is
        /// empty or contains only blanks.</exception>
        public static void NotNullOrEmpty(string target, string parameterName, string message)
        {
            if (target == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            if (string.IsNullOrEmpty(target) || target.Trim().Equals(string.Empty))
            {
                throw new ArgumentException(message, parameterName);
            }
        }
    }
}
