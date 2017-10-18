#region Assembly System.Windows.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\System.Windows.dll
#endregion

using System;

namespace System.Windows.Reign
{
	// Summary:
	//     Represents an x- and y-coordinate pair in two-dimensional space. Can also
	//     represent a logical point for certain property usages.
	public struct Point : IFormattable
	{
		private double x, y;

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return base.Equals(obj);
		}

		//
		// Summary:
		//     Initializes a System.Windows.Point structure that contains the specified
		//     values.
		//
		// Parameters:
		//   x:
		//     The x-coordinate value of the System.Windows.Point structure.
		//
		//   y:
		//     The y-coordinate value of the System.Windows.Point structure.
		public Point(double x, double y)
		{
			this.x = x;
			this.y = y;
		}

		// Summary:
		//     Compares two System.Windows.Point structures for inequality
		//
		// Parameters:
		//   point1:
		//     The first point to compare.
		//
		//   point2:
		//     The second point to compare.
		//
		// Returns:
		//     true if point1 and point2 have different System.Windows.Point.X or System.Windows.Point.Y
		//     values; false if point1 and point2 have the same System.Windows.Point.X and
		//     System.Windows.Point.Y values.
		public static bool operator !=(Point point1, Point point2)
		{
			return
				(point1.x != point2.x) ||
				(point1.y != point2.y);
		}

		//
		// Summary:
		//     Compares two System.Windows.Point structures for equality.
		//
		// Parameters:
		//   point1:
		//     The first System.Windows.Point structure to compare.
		//
		//   point2:
		//     The second System.Windows.Point structure to compare.
		//
		// Returns:
		//     true if both the System.Windows.Point.X and System.Windows.Point.Y values
		//     of point1 and point2 are equal; otherwise, false.
		public static bool operator ==(Point point1, Point point2)
		{
			return
				(point1.x == point2.x) &&
				(point1.y == point2.y);
		}

		// Summary:
		//     Gets or sets the System.Windows.Point.X-coordinate value of this System.Windows.Point
		//     structure.
		//
		// Returns:
		//     The System.Windows.Point.X-coordinate value of this System.Windows.Point
		//     structure. The default value is 0.
		public double X
		{
			get {return x;}
			set {x = value;}
		}

		//
		// Summary:
		//     Gets or sets the System.Windows.Point.Y-coordinate value of this System.Windows.Point.
		//
		// Returns:
		//     The System.Windows.Point.Y-coordinate value of this System.Windows.Point
		//     structure. The default value is 0.
		public double Y
		{
			get {return y;}
			set {y = value;}
		}

		// Summary:
		//     Determines whether the specified object is a System.Windows.Point and whether
		//     it contains the same values as this System.Windows.Point.
		//
		// Parameters:
		//   o:
		//     The object to compare.
		//
		// Returns:
		//     true if obj is a System.Windows.Point and contains the same System.Windows.Point.X
		//     and System.Windows.Point.Y values as this System.Windows.Point; otherwise,
		//     false.
		//public override bool Equals(object o);

		//
		// Summary:
		//     Compares two System.Windows.Point structures for equality.
		//
		// Parameters:
		//   value:
		//     The point to compare to this instance.
		//
		// Returns:
		//     true if both System.Windows.Point structures contain the same System.Windows.Point.X
		//     and System.Windows.Point.Y values; otherwise, false.
		public bool Equals(Point value)
		{
			return value == this;
		}

		//
		// Summary:
		//     Returns the hash code for this System.Windows.Point.
		//
		// Returns:
		//     The hash code for this System.Windows.Point structure.
		//public override int GetHashCode();

		//
		// Summary:
		//     Creates a System.String representation of this System.Windows.Point.
		//
		// Returns:
		//     A System.String containing the System.Windows.Point.X and System.Windows.Point.Y
		//     values of this System.Windows.Point structure.
		//public override string ToString();

		//
		// Summary:
		//     Creates a System.String representation of this System.Windows.Point.
		//
		// Parameters:
		//   provider:
		//     Culture-specific formatting information.
		//
		// Returns:
		//     A System.String containing the System.Windows.Point.X and System.Windows.Point.Y
		//     values of this System.Windows.Point structure.
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