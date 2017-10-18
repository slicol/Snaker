#region Assembly System.Windows.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\System.Windows.dll
#endregion

using System;

namespace System.Windows.Reign
{
	// Summary:
	//     Describes the width and height of an object.
	public struct Size
	{
		private double width, height;

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
		//     Initializes a new instance of the System.Windows.Size structure and assigns
		//     it an initial width and height.
		//
		// Parameters:
		//   width:
		//     The initial width of the instance of System.Windows.Size.
		//
		//   height:
		//     The initial height of the instance of System.Windows.Size.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     width or height are less than 0.
		public Size(double width, double height)
		{
			this.width = width;
			this.height = height;
		}

		// Summary:
		//     Compares two instances of System.Windows.Size for inequality.
		//
		// Parameters:
		//   size1:
		//     The first instance of System.Windows.Size to compare.
		//
		//   size2:
		//     The second instance of System.Windows.Size to compare.
		//
		// Returns:
		//     true if the instances of System.Windows.Size are not equal; otherwise false.
		public static bool operator !=(Size size1, Size size2)
		{
			return
				(size1.width != size2.width) ||
				(size1.height != size2.height);
		}

		//
		// Summary:
		//     Compares two instances of System.Windows.Size for equality.
		//
		// Parameters:
		//   size1:
		//     The first instance of System.Windows.Size to compare.
		//
		//   size2:
		//     The second instance of System.Windows.Size to compare.
		//
		// Returns:
		//     true if the two instances of System.Windows.Size are equal; otherwise false.
		public static bool operator ==(Size size1, Size size2)
		{
			return
				(size1.width == size2.width) &&
				(size1.height == size2.height);
		}

		// Summary:
		//     Gets a value that represents a static empty System.Windows.Size.
		//
		// Returns:
		//     An empty instance of System.Windows.Size.
		//public static Size Empty { get; }

		//
		// Summary:
		//     Gets or sets the height of this instance of System.Windows.Size.
		//
		// Returns:
		//     The System.Windows.Size.Height of this instance of System.Windows.Size, in
		//     pixels. The default is 0. The value cannot be negative.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     Specified a value less than 0.
		public double Height
		{
			get {return height;}
			set {height = value;}
		}

		//
		// Summary:
		//     Gets a value that indicates whether this instance of System.Windows.Size
		//     is System.Windows.Size.Empty.
		//
		// Returns:
		//     true if this instance of size is System.Windows.Size.Empty; otherwise false.
		//public bool IsEmpty { get; }

		//
		// Summary:
		//     Gets or sets the width of this instance of System.Windows.Size.
		//
		// Returns:
		//     The System.Windows.Size.Width of this instance of System.Windows.Size, in
		//     pixels. The default value is 0. The value cannot be negative.
		//
		// Exceptions:
		//   System.ArgumentException:
		//     Specified a value less than 0.
		public double Width
		{
			get {return width;}
			set {width = value;}
		}

		// Summary:
		//     Compares an object to an instance of System.Windows.Size for equality.
		//
		// Parameters:
		//   o:
		//     The System.Object to compare.
		//
		// Returns:
		//     true if the sizes are equal; otherwise, false.
		//public override bool Equals(object o);

		//
		// Summary:
		//     Compares a value to an instance of System.Windows.Size for equality.
		//
		// Parameters:
		//   value:
		//     The size to compare to this current instance of System.Windows.Size.
		//
		// Returns:
		//     true if the instances of System.Windows.Size are equal; otherwise, false.
		//public bool Equals(Size value);

		//
		// Summary:
		//     Gets the hash code for this instance of System.Windows.Size.
		//
		// Returns:
		//     The hash code for this instance of System.Windows.Size.
		//public override int GetHashCode();

		//
		// Summary:
		//     Returns a string representation of this System.Windows.Size.
		//
		// Returns:
		//     A string representation of this System.Windows.Size.
		//public override string ToString();
	}
}