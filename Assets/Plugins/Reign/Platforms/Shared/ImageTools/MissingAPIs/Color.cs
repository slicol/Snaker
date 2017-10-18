#region Assembly System.Windows.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\System.Windows.dll
#endregion

using System;

namespace System.Windows.Media.Reign
{
	// Summary:
	//     Describes a color in terms of alpha, red, green, and blue channels.
	public struct Color : IFormattable
	{
		private byte r, g, b, a;

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		// Summary:
		//     Tests whether two System.Windows.Media.Color structures are not identical.
		//
		// Parameters:
		//   color1:
		//     The first System.Windows.Media.Color structure to compare.
		//
		//   color2:
		//     The second System.Windows.Media.Color structure to compare.
		//
		// Returns:
		//     true if color1 and color2 are not equal; otherwise, false.
		public static bool operator !=(Color color1, Color color2)
		{
			return
				(color1.r != color2.r) ||
				(color1.g != color2.g) ||
				(color1.b != color2.b) ||
				(color1.a != color2.a);
		}

		//
		// Summary:
		//     Tests whether two System.Windows.Media.Color structures are identical.
		//
		// Parameters:
		//   color1:
		//     The first System.Windows.Media.Color structure to compare.
		//
		//   color2:
		//     The second System.Windows.Media.Color structure to compare.
		//
		// Returns:
		//     true if color1 and color2 are exactly identical; otherwise, false.
		public static bool operator ==(Color color1, Color color2)
		{
			return
				(color1.r == color2.r) &&
				(color1.g == color2.g) &&
				(color1.b == color2.b) &&
				(color1.a == color2.a);
		}

		// Summary:
		//     Gets or sets the sRGB alpha channel value of the color.
		//
		// Returns:
		//     The sRGB alpha channel value of the color, as a value between 0 and 255.
		public byte A
		{
			get {return a;}
			set {a = value;}
		}

		//
		// Summary:
		//     Gets or sets the sRGB blue channel value of the color.
		//
		// Returns:
		//     The sRGB blue channel value, as a value between 0 and 255.
		public byte B
		{
			get {return b;}
			set {b = value;}
		}

		//
		// Summary:
		//     Gets or sets the sRGB green channel value of the color.
		//
		// Returns:
		//     The sRGB green channel value, as a value between 0 and 255.
		public byte G
		{
			get {return g;}
			set {g = value;}
		}

		//
		// Summary:
		//     Gets or sets the sRGB red channel value of the color.
		//
		// Returns:
		//     The sRGB red channel value, as a value between 0 and 255.
		public byte R
		{
			get {return r;}
			set {r = value;}
		}

		// Summary:
		//     Tests whether the specified System.Windows.Media.Color structure is identical
		//     to the current color.
		//
		// Parameters:
		//   color:
		//     The System.Windows.Media.Color structure to compare to the current System.Windows.Media.Color
		//     structure.
		//
		// Returns:
		//     true if the specified System.Windows.Media.Color structure is identical to
		//     the current System.Windows.Media.Color structure; otherwise, false.
		//public bool Equals(Color color)
		//{
		//	return color == this;
		//}

		//
		// Summary:
		//     Tests whether the specified object is a System.Windows.Media.Color structure
		//     and is equivalent to the current color.
		//
		// Parameters:
		//   o:
		//     The object to compare to the current System.Windows.Media.Color structure.
		//
		// Returns:
		//     true if the specified object is a System.Windows.Media.Color structure and
		//     is identical to the current System.Windows.Media.Color structure; otherwise,
		//     false.
		//public override bool Equals(object o)
		//{
		//	if (o == null || o.GetType() != typeof(Color)) return false;
		//	return ((Color)o) == this;
		//}

		//
		// Summary:
		//     Creates a new System.Windows.Media.Color structure by using the specified
		//     sRGB alpha channel and color channel values.
		//
		// Parameters:
		//   a:
		//     The alpha channel, System.Windows.Media.Color.A, of the new color. The value
		//     must be between 0 and 255.
		//
		//   r:
		//     The red channel, System.Windows.Media.Color.R, of the new color. The value
		//     must be between 0 and 255.
		//
		//   g:
		//     The green channel, System.Windows.Media.Color.G, of the new color. The value
		//     must be between 0 and 255.
		//
		//   b:
		//     The blue channel, System.Windows.Media.Color.B, of the new color. The value
		//     must be between 0 and 255.
		//
		// Returns:
		//     A System.Windows.Media.Color structure with the specified values.
		public static Color FromArgb(byte a, byte r, byte g, byte b)
		{
			Color c;
			c.a = a;
			c.r = r;
			c.g = g;
			c.b = b;
			return c;
		}

		//
		// Summary:
		//     Gets a hash code for the current System.Windows.Media.Color structure.
		//
		// Returns:
		//     A hash code for the current System.Windows.Media.Color structure.
		//public override int GetHashCode();

		//
		// Summary:
		//     Creates a string representation of the color using the ARGB channels in hex
		//     notation.
		//
		// Returns:
		//     The string representation of the color.
		//public override string ToString();

		//
		// Summary:
		//     Creates a string representation of the color by using the ARGB channels and
		//     the specified format provider.
		//
		// Parameters:
		//   provider:
		//     Culture-specific formatting information.
		//
		// Returns:
		//     The string representation of the color.
		public string ToString(IFormatProvider provider)
		{
			return this.ToString();
		}

		// Summary:
		//     Formats the value of the current instance using the specified format.
		//
		// Parameters:
		//   format:
		//     The System.String specifying the format to use.  -or- null to use the default
		//     format defined for the type of the System.IFormattable implementation.
		//
		//   formatProvider:
		//     The System.IFormatProvider to use to format the value.  -or- null to obtain
		//     the numeric format information from the current locale setting of the operating
		//     system.
		//
		// Returns:
		//     A System.String containing the value of the current instance in the specified
		//     format.
		public string ToString(string format, IFormatProvider formatProvider)
		{
			return this.ToString();
		}
	}
}