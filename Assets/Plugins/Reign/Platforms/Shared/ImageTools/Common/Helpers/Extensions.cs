#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Extensions.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if !UNITY_METRO || UNITY_EDITOR
using System.Collections.ObjectModel.Reign;
#endif
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts.Reign;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Reign;
using System.Windows.Resources.Reign;

namespace ImageTools.Helpers
{
    /// <summary>
    /// A collection of simple helper extension methods.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Defines a constant rectangle where all values are zero.
        /// </summary>
        public static readonly Rect ZeroRect = new Rect(0, 0, 0, 0);

        /// <summary>
        /// Converts byte array to a new array where each value in the original array is represented 
        /// by a the specified number of bits.
        /// </summary>
        /// <param name="bytes">The bytes to convert from. Cannot be null.</param>
        /// <param name="bits">The number of bits per value.</param>
        /// <returns>The resulting byte array. Is never null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="bytes"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="bits"/> is greater or equals than zero.</exception>
        public static byte[] ToArrayByBitsLength(this byte[] bytes, int bits)
        {
            Contract.Requires<ArgumentNullException>(bytes != null, "Bytes cannot be null.");
            Contract.Requires<ArgumentException>(bits > 0, "Bits must be greater than zero.");
            Contract.Ensures(Contract.Result<byte[]>() != null);

            byte[] result = null;

            if (bits < 8)
            {
                result = new byte[bytes.Length * 8 / bits];

                int factor = (int)Math.Pow(2, bits) - 1;
                int mask = (0xFF >> (8 - bits));
                int resultOffset = 0;

                for (int i = 0; i < bytes.Length; i++)
                {
                    for (int shift = 0; shift < 8; shift += bits)
                    {
                        Contract.Assume(resultOffset < result.Length);
                        Contract.Assume(factor > 0);

                        int colorIndex = (((bytes[i]) >> (8 - bits - shift)) & mask) * (255 / factor);

                        result[resultOffset] = (byte)colorIndex;

                        resultOffset++;
                    }

                }
            }
            else
            {
                result = bytes;
            }

            return result;
        }

        /// <summary>
        /// Gets the stream to a local resource
        /// </summary>
        /// <param name="uri">The path to the local stream. Cannot be null.</param>
        /// <returns>The stream to the local resource when such a resource exists or null otherwise.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="uri"/> is null.</exception>
		//public static Stream GetLocalResourceStream(Uri uri)
		//{
		//	Contract.Requires<ArgumentNullException>(uri != null, "Uri cannot be null.");

		//	StreamResourceInfo info = Application.GetResourceStream(uri);
		//	if (info == null)
		//	{
		//		Application app = Application.Current;

		//		if (app != null)
		//		{
		//			Type appplicationType = app.GetType();

		//			string assemblyName = appplicationType.Assembly.FullName.Split(new char[] { ',' })[0];

		//			string uriString = uri.OriginalString;
		//			uriString = string.Format(CultureInfo.CurrentCulture, "{0};component/{1}", assemblyName, uriString);
		//			uriString = uriString.Replace("\\", "/");
		//			uriString = uriString.Replace("//", "/");

		//			Uri resourceUri = new Uri(uriString, UriKind.Relative);

		//			info = Application.GetResourceStream(resourceUri);
		//		}
		//	}

		//	Stream stream = info != null ? info.Stream : null;
		//	return stream;
		//}

        /// <summary>
        /// Multiplies the values of the specified rectangle by the factor.
        /// </summary>
        /// <param name="rectangle">The rectangle to multiply with the factor.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The new rectangle.</returns>
        public static Rectangle Multiply(Rectangle rectangle, double factor)
        {
            rectangle.X = (int)(rectangle.X * factor);
            rectangle.Y = (int)(rectangle.Y * factor);

            rectangle.Width  = (int)(rectangle.Width * factor);
            rectangle.Height = (int)(rectangle.Height * factor);

            return rectangle;
        }

        /// <summary>
        /// Multiplies the values of the specified rectangle by the factor.
        /// </summary>
        /// <param name="rectangle">The rectangle to multiply with the factor.</param>
        /// <param name="factor">The factor.</param>
        /// <returns>The new rectangle.</returns>
        public static Rect Multiply(Rect rectangle, double factor)
        {
            rectangle.X = rectangle.X * factor;
            rectangle.Y = rectangle.Y * factor;

            rectangle.Width  = rectangle.Width * factor;
            rectangle.Height = rectangle.Height * factor;

            return rectangle;
        }

        /// <summary>
        /// Determines whether the specified value is a valid number.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>A flag indicating whether the specified value is a number.</returns>
        public static bool IsNumber(this double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }

        /// <summary>
        /// Determines whether the specified value is a valid number.
        /// </summary>
        /// <param name="value">The number to check.</param>
        /// <returns>A flag indicating whether the specified value is a number.</returns>
        public static bool IsNumber(this float value)
        {
            return !float.IsInfinity(value) && !float.IsNaN(value);
        }

        /// <summary>
        /// Invokes the specified foreach item in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the items in the enumerable.</typeparam>
        /// <param name="items">The enumerable that is iterated through this method. Cannot be null.</param>
        /// <param name="action">The action to invoke foreach item. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="items"/> is null.</para>
        /// <para>- or -</para>
        /// <para><paramref name="action"/> is null.</para>
        /// </exception>
        public static void Foreach<T>(this IEnumerable<T> items, Action<T> action)
        {
            Contract.Requires<ArgumentNullException>(items != null, "Items cannot be null");
            Contract.Requires<ArgumentNullException>(action != null, "Action cannot be null.");

            foreach (T item in items)
            {
                if (item != null)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Invokes the specified foreach item in the enumerable.
        /// </summary>
        /// <param name="items">The enumerable that is iterated through this method. Cannot be null.</param>
        /// <param name="action">The action to invoke foreach item. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// 	<para><paramref name="items"/> is null.</para>
        /// 	<para>- or -</para>
        /// 	<para><paramref name="action"/> is null.</para>
        /// </exception>
        public static void Foreach(this IEnumerable items, Action<object> action)
        {
            Contract.Requires<ArgumentNullException>(items != null, "Items cannot be null");
            Contract.Requires<ArgumentNullException>(action != null, "Action cannot be null.");

            foreach (object item in items)
            {
                if (item != null)
                {
                    action(item);
                }
            }
        }

        /// <summary>
        /// Adds the the specified elements to the target collection object.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the source and target.</typeparam>
        /// <param name="target">The target, where the items should be inserted to. Cannot be null.</param>
        /// <param name="elements">The elements to add to the collection. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="target"/> is null.</para>
        /// <para>- or -</para>
        /// <para><paramref name="elements"/> is null.</para>
        /// </exception>
        public static void AddRange<TItem>(this ObservableCollection<TItem> target, IEnumerable<TItem> elements)
        {
            Contract.Requires<ArgumentNullException>(target != null, "Target cannot be null");
            Contract.Requires<ArgumentNullException>(elements != null, "Elements cannot be null.");

            foreach (TItem item in elements)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// Adds the the specified elements to the target collection object.
        /// </summary>
        /// <typeparam name="TItem">The type of the items in the source and target.</typeparam>
        /// <param name="target">The target, where the items should be inserted to. Cannot be null.</param>
        /// <param name="elements">The elements to add to the collection. Cannot be null.</param>
        /// <exception cref="ArgumentNullException">
        /// <para><paramref name="target"/> is null.</para>
        /// <para>- or -</para>
        /// <para><paramref name="elements"/> is null.</para>
        /// </exception>
        public static void AddRange<TItem>(this Collection<TItem> target, IEnumerable<TItem> elements)
        {
            Contract.Requires<ArgumentNullException>(target != null, "Target cannot be null");
            Contract.Requires<ArgumentNullException>(elements != null, "Elements cannot be null.");

            foreach (TItem item in elements)
            {
                target.Add(item);
            }
        }

        /// <summary>
        /// Determines whether the specified value is between two other
        /// values.
        /// </summary>
        /// <typeparam name="TValue">The type of the values to check.
        /// Must implement <see cref="IComparable"/>.</typeparam>
        /// <param name="value">The value which should be between the other values.</param>
        /// <param name="low">The lower value.</param>
        /// <param name="high">The higher value.</param>
        /// <returns>
        /// 	<c>true</c> if the specified value is between the other values; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsBetween<TValue>(this TValue value, TValue low, TValue high) where TValue : IComparable
        {
            return (Comparer<TValue>.Default.Compare(low, value) <= 0
                 && Comparer<TValue>.Default.Compare(high, value) >= 0);
        }

        /// <summary>
        /// Arranges the value, so that it is not greater than the high value and
        /// not lower than the low value and returns the result.
        /// </summary>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="low">The lower value.</param>
        /// <param name="high">The higher value.</param>
        /// <returns>The arranged value.</returns>
        /// <remarks>If the specified lower value is greater than the higher value. The low value
        /// will be returned.</remarks>
        public static TValue RemainBetween<TValue>(this TValue value, TValue low, TValue high) where TValue : IComparable
        {
            TValue result = value;

            if (Comparer<TValue>.Default.Compare(high, low) < 0)
            {
                result = low;
            }

            else if (Comparer<TValue>.Default.Compare(value, low) <= 0)
            {
                result = low;
            }

            else if (Comparer<TValue>.Default.Compare(value, high) >= 0)
            {
                result = high;
            }

            return result;
        }

        /// <summary>
        /// Swaps two references.
        /// </summary>
        /// <typeparam name="TRef">The type of the references to swap.</typeparam>
        /// <param name="lhs">The first reference.</param>
        /// <param name="rhs">The second reference.</param>
        public static void Swap<TRef>(ref TRef lhs, ref TRef rhs) where TRef : class
        {
            TRef tmp = lhs;

            lhs = rhs;
            rhs = tmp;
        }
    }
}
