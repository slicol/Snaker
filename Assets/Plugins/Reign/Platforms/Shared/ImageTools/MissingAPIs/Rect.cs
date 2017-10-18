#region Assembly System.Windows.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\System.Windows.dll
#endregion

using System;

namespace System.Windows.Reign
{
	// Summary:
	//     Describes the width, height, and point origin of a rectangle.
	public struct Rect : IFormattable
	{
		private Point point1, point2;

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
		//     Initializes a System.Windows.Rect structure that is exactly large enough
		//     to contain the two specified points.
		//
		// Parameters:
		//   point1:
		//     The first point that the new rectangle must contain.
		//
		//   point2:
		//     The second point that the new rectangle must contain.
		public Rect(Point point1, Point point2)
		{
			this.point1 = point1;
			this.point2 = point2;
		}

		//
		// Summary:
		//     Initializes a System.Windows.Rect structure based on an origin and size.
		//
		// Parameters:
		//   location:
		//     The origin of the new System.Windows.Rect.
		//
		//   size:
		//     The size of the new System.Windows.Rect.
		public Rect(Point location, Size size)
		{
			this.point1 = location;
			this.point2 = new Point(location.X + size.Width, location.Y + size.Height);
		}

		//
		// Summary:
		//     Initializes a System.Windows.Rect structure that has the specified x-coordinate,
		//     y-coordinate, width, and height.
		//
		// Parameters:
		//   x:
		//     The x-coordinate of the top-left corner of the rectangle.
		//
		//   y:
		//     The y-coordinate of the top-left corner of the rectangle.
		//
		//   width:
		//     The width of the rectangle.
		//
		//   height:
		//     The height of the rectangle.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     width or height are less than 0.
		public Rect(double x, double y, double width, double height)
		{
			this.point1 = new Point(x, y);
			this.point2 = new Point(x + width, y + height);
		}

		// Summary:
		//     Compares two System.Windows.Rect structures for inequality.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to compare.
		//
		//   rect2:
		//     The second rectangle to compare.
		//
		// Returns:
		//     true if the System.Windows.Rect structures do not have the same x,y,width,height
		//     property values; otherwise, false.
		public static bool operator !=(Rect rect1, Rect rect2)
		{
			return
				(rect1.point1 != rect2.point1) ||
				(rect1.point2 != rect2.point2);
		}

		//
		// Summary:
		//     Compares two System.Windows.Rect structures for equality.
		//
		// Parameters:
		//   rect1:
		//     The first rectangle to compare.
		//
		//   rect2:
		//     The second rectangle to compare.
		//
		// Returns:
		//     true if the System.Windows.Rect structures have the same x,y,width,height
		//     property values; otherwise, false.
		public static bool operator ==(Rect rect1, Rect rect2)
		{
			return
				(rect1.point1 == rect2.point1) &&
				(rect1.point2 == rect2.point2);
		}

		// Summary:
		//     Gets the y-axis value of the bottom of the rectangle.
		//
		// Returns:
		//     The y-axis value of the bottom of the rectangle. If the rectangle is empty,
		//     the value is System.Double.NegativeInfinity .
		public double Bottom { get{return point1.Y;} }

		//
		// Summary:
		//     Gets a special value that represents a rectangle with no position or area.
		//
		// Returns:
		//     The empty rectangle, which has System.Windows.Rect.X and System.Windows.Rect.Y
		//     property values of System.Double.PositiveInfinity, and has System.Windows.Rect.Width
		//     and System.Windows.Rect.Height property values of System.Double.NegativeInfinity.
		public static Rect Empty { get{return new Rect();} }
		//
		// Summary:
		//     Gets or sets the height of the rectangle.
		//
		// Returns:
		//     A value that represents the height of the rectangle. The default is 0.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     Specified a value less than 0.
		public double Height
		{
			get {return point2.Y - point1.Y;}
			set {point2.Y = point1.Y + value;}
		}

		//
		// Summary:
		//     Gets a value that indicates whether the rectangle is the System.Windows.Rect.Empty
		//     rectangle.
		//
		// Returns:
		//     true if the rectangle is the System.Windows.Rect.Empty rectangle; otherwise,
		//     false.
		public bool IsEmpty
		{
			get
			{
				return
					(point1.X == 0 && point1.Y == 0) &&
					(point2.X == 0 && point2.Y == 0);
			}
		}

		//
		// Summary:
		//     Gets the x-axis value of the left side of the rectangle.
		//
		// Returns:
		//     The x-axis value of the left side of the rectangle.
		public double Left { get{return point1.X;} }

		//
		// Summary:
		//     Gets the x-axis value of the right side of the rectangle.
		//
		// Returns:
		//     The x-axis value of the right side of the rectangle.
		public double Right { get{return point2.X;} }

		//
		// Summary:
		//     Gets the y-axis position of the top of the rectangle.
		//
		// Returns:
		//     The y-axis position of the top of the rectangle.
		public double Top { get{return point2.Y;} }

		//
		// Summary:
		//     Gets or sets the width of the rectangle.
		//
		// Returns:
		//     A value that represents the width of the rectangle in pixels. The default
		//     is 0.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     Specified a value less than 0.
		public double Width
		{
			get {return point2.X - point1.X;}
			set {point2.X = point1.X + value;}
		}

		//
		// Summary:
		//     Gets or sets the x-axis value of the left side of the rectangle.
		//
		// Returns:
		//     The x-axis value of the left side of the rectangle. This value is interpreted
		//     as pixels within the coordinate space.
		public double X
		{
			get {return point1.X;}
			set
			{
				point2.X = value + (point2.X - point1.X);
				point1.X = value;
			}
		}

		//
		// Summary:
		//     Gets or sets the y-axis value of the top side of the rectangle.
		//
		// Returns:
		//     The y-axis value of the top side of the rectangle. This value is interpreted
		//     as pixels within the coordinate space..
		public double Y
		{
			get {return point1.Y;}
			set
			{
				point2.Y = value + (point2.Y - point1.Y);
				point1.Y = value;
			}
		}

		// Summary:
		//     Indicates whether the rectangle described by the System.Windows.Rect contains
		//     the specified point.
		//
		// Parameters:
		//   point:
		//     The point to check.
		//
		// Returns:
		//     true if the rectangle described by the System.Windows.Rect contains the specified
		//     point; otherwise, false.
		//public bool Contains(Point point);

		//
		// Summary:
		//     Indicates whether the specified object is equal to the current System.Windows.Rect.
		//
		// Parameters:
		//   o:
		//     The object to compare to the current rectangle.
		//
		// Returns:
		//     true if o is a System.Windows.Rect and has the same x,y,width,height values
		//     as the current System.Windows.Rect; otherwise, false.
		//public override bool Equals(object o);

		//
		// Summary:
		//     Indicates whether the specified System.Windows.Rect is equal to the current
		//     System.Windows.Rect.
		//
		// Parameters:
		//   value:
		//     The rectangle to compare to the current rectangle.
		//
		// Returns:
		//     true if the specified System.Windows.Rect has the same x,y,width,height property
		//     values as the current System.Windows.Rect; otherwise, false.
		//public bool Equals(Rect value);

		//
		// Summary:
		//     Creates a hash code for the System.Windows.Rect.
		//
		// Returns:
		//     A hash code for the current System.Windows.Rect structure.
		//public override int GetHashCode();

		//
		// Summary:
		//     Finds the intersection of the rectangle represented by the current System.Windows.Rect
		//     and the rectangle represented by the specified System.Windows.Rect, and stores
		//     the result as the current System.Windows.Rect.
		//
		// Parameters:
		//   rect:
		//     The rectangle to intersect with the current rectangle.
		//public void Intersect(Rect rect);

		//
		// Summary:
		//     Returns a string representation of the System.Windows.Rect structure.
		//
		// Returns:
		//     A string representation of the current System.Windows.Rect structure. The
		//     string has the following form: "System.Windows.Rect.X,System.Windows.Rect.Y,System.Windows.Rect.Width,System.Windows.Rect.Height".
		//public override string ToString();

		//
		// Summary:
		//     Returns a string representation of the rectangle by using the specified format
		//     provider.
		//
		// Parameters:
		//   provider:
		//     Culture-specific formatting information.
		//
		// Returns:
		//     A string representation of the current rectangle that is determined by the
		//     specified format provider.
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

		//
		// Summary:
		//     Expands the rectangle represented by the current System.Windows.Rect exactly
		//     enough to contain the specified point.
		//
		// Parameters:
		//   point:
		//     The point to include.
		//public void Union(Point point);

		//
		// Summary:
		//     Expands the rectangle represented by the current System.Windows.Rect exactly
		//     enough to contain the specified rectangle.
		//
		// Parameters:
		//   rect:
		//     The rectangle to include.
		//public void Union(Rect rect);
	}
}