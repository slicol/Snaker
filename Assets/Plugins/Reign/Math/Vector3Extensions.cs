// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;

namespace Reign
{
	/// <summary>
	/// Vector3 extensions helpers.
	/// </summary>
	public static class Vector3Extensions
	{
		#region Method Extensions
		/// <summary>
		/// Convert to Vector2
		/// </summary>
		/// <param name="inVec">Local value.</param>
		/// <returns>Return value.</returns>
		public static Vector2 ToVector2(this Vector3 inVec)
		{
			Vector2 outVec;
			outVec.x = inVec.x;
			outVec.y = inVec.y;
			return outVec;
		}
		#endregion
	
		#region Methods
		/// <summary>
		/// Lerp a point between two vectors.
		/// </summary>
		/// <param name="p1">Point 1.</param>
		/// <param name="p2">Point 2.</param>
		/// <param name="interpolation">Interpolation value.</param>
		/// <param name="p3">Return value.</param>
		public static void Lerp(ref Vector3 p1, ref Vector3 p2, float interpolation, out Vector3 p3)
		{
			p3.x = Mathf.Lerp(p1.x, p2.x, interpolation);
			p3.y = Mathf.Lerp(p1.y, p2.y, interpolation);
			p3.z = Mathf.Lerp(p1.z, p2.z, interpolation);
		}
	
		/// <summary>
		/// Get a Quadratic Bezier Curve point.
		/// </summary>
		/// <param name="p1">Point 1.</param>
		/// <param name="p2">Point 2.</param>
		/// <param name="p3">Point 3.</param>
		/// <param name="interpolation">Interpolation value.</param>
		/// <returns>Return value.</returns>
		public static Vector3 QuadraticBezierCurve(Vector3 p1, Vector3 p2, Vector3 p3, float interpolation)
		{
			Vector3 l1;
			l1.x = Mathf.Lerp(p1.x, p2.x, interpolation);
			l1.y = Mathf.Lerp(p1.y, p2.y, interpolation);
			l1.z = Mathf.Lerp(p1.z, p2.z, interpolation);
		
			Vector3 l2;
			l2.x = Mathf.Lerp(p2.x, p3.x, interpolation);
			l2.y = Mathf.Lerp(p2.y, p3.y, interpolation);
			l2.z = Mathf.Lerp(p2.z, p3.z, interpolation);
	
			Vector3 outPos;
			outPos.x = Mathf.Lerp(l1.x, l2.x, interpolation);
			outPos.y = Mathf.Lerp(l1.y, l2.y, interpolation);
			outPos.z = Mathf.Lerp(l1.z, l2.z, interpolation);
			return outPos;
		}
	
		/// <summary>
		/// Get a Cubic Bezier Curve point.
		/// </summary>
		/// <param name="p1">Point 1.</param>
		/// <param name="p2">Point 2.</param>
		/// <param name="p3">Point 3.</param>
		/// <param name="p4">Point 4.</param>
		/// <param name="interpolation">Interpolation value.</param>
		/// <returns>Return value.</returns>
		public static Vector3 CubicBezierCurve(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, float interpolation)
		{
			Vector3 l1;
			l1.x = Mathf.Lerp(p1.x, p2.x, interpolation);
			l1.y = Mathf.Lerp(p1.y, p2.y, interpolation);
			l1.z = Mathf.Lerp(p1.z, p2.z, interpolation);
		
			Vector3 l2;
			l2.x = Mathf.Lerp(p3.x, p4.x, interpolation);
			l2.y = Mathf.Lerp(p3.y, p4.y, interpolation);
			l2.z = Mathf.Lerp(p3.z, p4.z, interpolation);
		
			Vector3 l3;
			l3.x = Mathf.Lerp(l1.x, l2.x, interpolation);
			l3.y = Mathf.Lerp(l1.y, l2.y, interpolation);
			l3.z = Mathf.Lerp(l1.z, l2.z, interpolation);
		
			return QuadraticBezierCurve(l1, l3, l2, interpolation);
		}
	
		/// <summary>
		/// Get a ray intersect point.
		/// </summary>
		/// <param name="vector">Point.</param>
		/// <param name="rayOrigin">Ray Orgin.</param>
		/// <param name="rayDirection">Ray Direction.</param>
		/// <returns>Return value.</returns>
		public static Vector3 InersectRay(this Vector3 vector, Vector3 rayOrigin, Vector3 rayDirection)
		{
			return (rayDirection * Vector3.Dot(vector-rayOrigin, rayDirection)) + rayOrigin;
		}

		/// <summary>
		/// Get a line intersect point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="p1">Line Point 1.</param>
		/// <param name="p2">Line Point 2.</param>
		/// <returns>Return value.</returns>
		public static Vector3 IntersectLine(Vector3 point, Vector3 p1, Vector3 p2)
		{
			Vector3 pointOffset = (point-p1), vector = (p2-p1).normalized;
			return (vector * Vector3.Dot(pointOffset, vector)) + p1;
		}

		/// <summary>
		/// Get a line segment intersect point.
		/// </summary>
		/// <param name="vector">Point.</param>
		/// <param name="point1">Line Point 1.</param>
		/// <param name="point2">Line Point 2.</param>
		/// <returns>Return value.</returns>
		public static Vector3 IntersectLineSegment(this Vector3 vector, Vector3 point1, Vector3 point2)
		{
			var dir = point1 - point2;
			dir.Normalize();
			var point = (dir * Vector3.Dot(vector-point1, dir)) + point1;
			Bound3 bound;
			Bound3.FromPoints(ref point1, ref point2, out bound);
			if (!bound.Intersects(point))
			{
				if ((vector - point1).magnitude <= (vector - point2).magnitude) return point1;
				else return point2;
			}

			return point;
		}
	
		/// <summary>
		/// Get bounds of point cluster.
		/// </summary>
		/// <param name="points">Points.</param>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		public static void GetBounds(Vector3[] points, out Vector3 min, out Vector3 max)
		{
			min = points[0];
			max = points[0];
			for (int i = 0; i != points.Length; ++i)
			{
				var point = points[i];
				if (point.x < min.x) min.x = point.x;
				if (point.x > max.x) max.x = point.x;
			
				if (point.y < min.y) min.y = point.y;
				if (point.y > max.y) max.y = point.y;
			
				if (point.z < min.z) min.z = point.z;
				if (point.z > max.z) max.z = point.z;
			}
		}
	
		/// <summary>
		/// Rotate vectors around axis.
		/// </summary>
		/// <param name="vectors">Vectors.</param>
		/// <param name="angle">Rotation Angle.</param>
		/// <param name="axis">Ratoation Axis.</param>
		public static void RotateVectors(Vector3[] vectors, float angle, Vector3 axis)
		{
			var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
			for (int i = 0; i != vectors.Length; ++i)
			{
				vectors[i] = rotation * vectors[i];
			}
		}
	
		/// <summary>
		/// Rotate points around axis.
		/// </summary>
		/// <param name="points">Points.</param>
		/// <param name="angle">Rotation Angle.</param>
		/// <param name="axis">Ratoation Axis.</param>
		public static void RotatePoints(Vector3[] points, float angle, Vector3 axis)
		{
			Vector3 min, max;
			GetBounds(points, out min, out max);
			Vector3 offset = min + ((max - min) * .5f);
	
			var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, axis);
			for (int i = 0; i != points.Length; ++i)
			{
				points[i] = (rotation * (points[i] - offset)) + offset;
			}
		}
		#endregion
	}
}