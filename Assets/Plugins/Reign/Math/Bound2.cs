// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Reign
{
	/// <summary>
	/// For bound calculations.
	/// </summary>
	public struct Bound2
	{
		/// <summary>
		/// Main field.
		/// </summary>
		public float left, right, bottom, top;

		/// <summary>
		/// Zero on all fields.
		/// </summary>
		public readonly static Bound2 zero = new Bound2();

		/// <summary>
		/// Min value.
		/// </summary>
		public Vector2 Min
		{
			get
			{
				Vector2 min;
				min.x = left;
				min.y = bottom;

				return min;
			}
		}

		/// <summary>
		/// Max value.
		/// </summary>
		public Vector2 Max
		{
			get
			{
				Vector2 min;
				min.x = right;
				min.y = top;

				return min;
			}
		}

		/// <summary>
		/// Bound constructor.
		/// </summary>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		/// <param name="top">Top.</param>=
		public Bound2(float left, float right, float bottom, float top)
		{
			this.left = left;
			this.right = right;
			this.bottom = bottom;
			this.top = top;
		}

		/// <summary>
		/// Bound constructor.
		/// </summary>
		/// <param name="point">Center point.</param>
		public Bound2(Vector2 point)
		{
			this.left = point.x;
			this.right = point.x;
			this.bottom = point.y;
			this.top = point.y;
		}
	
		/// <summary>
		/// Covert bound to rect.
		/// </summary>
		/// <returns>Returns rect</returns>
		public Rect ToRect()
		{
			return new Rect(left, top, right-left, bottom-top);
		}

		/// <summary>
		/// Create bound around points.
		/// </summary>
		/// <param name="point1">Point1.</param>
		/// <param name="point2">Point2.</param>
		/// <param name="result">Bound result.</param>
		public static void FromPoints(ref Vector2 point1, ref Vector2 point2, out Bound2 result)
		{
			result.left = point1.x;
			result.right = point1.x;
			result.bottom = point1.y;
			result.top = point1.y;

			if (point2.x < result.left) result.left = point2.x;
			if (point2.x > result.right) result.right = point2.x;
			if (point2.y < result.bottom) result.bottom = point2.y;
			if (point2.y > result.top) result.top = point2.y;
		}

		/// <summary>
		/// Adjust bound to include point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="radius">Point radius.</param>
		public void AddPoint(Vector2 point, float radius)
		{
			float value = (point.x - radius);
			if (value < left) left = value;

			value = (point.x + radius);
			if (value > right) right = value;

			value = (point.y - radius);
			if (value < top) top = value;

			value = (point.y + radius);
			if (value > bottom) bottom = value;
		}
	
		/// <summary>
		/// Adjust bound to include rect.
		/// </summary>
		/// <param name="rect">Rect.</param>
		public void AddRect(Rect rect)
		{
			if (rect.xMin < left) left = rect.xMin;
			if (rect.xMax > right) right = rect.xMax;
			if (rect.yMin < top) top = rect.yMin;
			if (rect.yMax > bottom) bottom = rect.yMax;
		}

		/// <summary>
		/// Check if bound intersects point.
		/// </summary>
		/// <param name="point">Point to intersect.</param>
		/// <returns>Returns true or false.</returns>
		public bool Intersects(Vector2 point)
		{
			return
				point.x >= left && point.x <= right &&
				point.y >= bottom && point.y <= top;
		}

		/// <summary>
		/// Check if bound intersects point with radius.
		/// </summary>
		/// <param name="point">Point to intersect.</param>
		/// <param name="radius">Point radius.</param>
		/// <returns>Returns true or false.</returns>
		public bool Intersects(Vector2 point, float radius)
		{
			return
				(point.x + radius) >= left && (point.x - radius) <= right &&
				(point.y + radius) >= bottom && (point.y - radius) <= top;
		}
	}
}