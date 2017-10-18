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
	public struct Bound3
	{
		/// <summary>
		/// Main field.
		/// </summary>
		public float left, right, bottom, top, back, front;

		/// <summary>
		/// Zero on all fields.
		/// </summary>
		public readonly static Bound3 zero = new Bound3();

		/// <summary>
		/// Min value.
		/// </summary>
		public Vector3 Min
		{
			get
			{
				Vector3 min;
				min.x = left;
				min.y = bottom;
				min.z = back;

				return min;
			}
		}

		/// <summary>
		/// Max value.
		/// </summary>
		public Vector3 Max
		{
			get
			{
				Vector3 min;
				min.x = right;
				min.y = top;
				min.z = front;

				return min;
			}
		}

		/// <summary>
		/// Bound constructor.
		/// </summary>
		/// <param name="left">Left.</param>
		/// <param name="right">Right.</param>
		/// <param name="bottom">Bottom.</param>
		/// <param name="top">Top.</param>
		/// <param name="back">Back.</param>
		/// <param name="front">Front.</param>
		public Bound3(float left, float right, float bottom, float top, float back, float front)
		{
			this.left = left;
			this.right = right;
			this.bottom = bottom;
			this.top = top;
			this.back = back;
			this.front = front;
		}

		/// <summary>
		/// Bound constructor.
		/// </summary>
		/// <param name="point">Center point.</param>
		public Bound3(Vector3 point)
		{
			this.left = point.x;
			this.right = point.x;
			this.bottom = point.y;
			this.top = point.y;
			this.back = point.z;
			this.front = point.z;
		}

		/// <summary>
		/// Create bound around points.
		/// </summary>
		/// <param name="point1">Point1.</param>
		/// <param name="point2">Point2.</param>
		/// <param name="result">Bound result.</param>
		public static void FromPoints(ref Vector3 point1, ref Vector3 point2, out Bound3 result)
		{
			result.left = point1.x;
			result.right = point1.x;
			result.bottom = point1.y;
			result.top = point1.y;
			result.back = point1.z;
			result.front = point1.z;

			if (point2.x < result.left) result.left = point2.x;
			if (point2.x > result.right) result.right = point2.x;
			if (point2.y < result.bottom) result.bottom = point2.y;
			if (point2.y > result.top) result.top = point2.y;
			if (point2.z < result.back) result.back = point2.z;
			if (point2.z > result.front) result.front = point2.z;
		}

		/// <summary>
		/// Adjust bound to include point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="radius">Point radius.</param>
		public void AddPoint(Vector3 point, float radius)
		{
			float value = (point.x - radius);
			if (value < left) left = value;

			value = (point.x + radius);
			if (value > right) right = value;

			value = (point.y - radius);
			if (value < bottom) bottom = value;

			value = (point.y + radius);
			if (value > top) top = value;

			value = (point.z - radius);
			if (value < back) back = value;

			value = (point.z + radius);
			if (value > front) front = value;
		}

		/// <summary>
		/// Check if bound intersects point.
		/// </summary>
		/// <param name="point">Point to intersect.</param>
		/// <returns>Returns true or false.</returns>
		public bool Intersects(Vector3 point)
		{
			return
				point.x >= left && point.x <= right &&
				point.y >= bottom && point.y <= top &&
				point.z >= back && point.z <= front;
		}

		/// <summary>
		/// Check if bound intersects point with radius.
		/// </summary>
		/// <param name="point">Point to intersect.</param>
		/// <param name="radius">Point radius.</param>
		/// <returns>Returns true or false.</returns>
		public bool Intersects(Vector3 point, float radius)
		{
			return
				(point.x + radius) >= left && (point.x - radius) <= right &&
				(point.y + radius) >= bottom && (point.y - radius) <= top &&
				(point.z + radius) >= back && (point.z - radius) <= front;
		}
	}
}