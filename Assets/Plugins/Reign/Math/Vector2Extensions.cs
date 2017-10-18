// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Reign
{
	/// <summary>
	/// Vector2 extensions helpers.
	/// </summary>
	public static class Vector2Extensions
	{
		#region Method Extensions
		/// <summary>
		/// Convert to Vector3.
		/// </summary>
		/// <param name="inVec">Local value.</param>
		/// <returns>Return value.</returns>
		public static Vector3 ToVector3(this Vector2 inVec)
		{
			Vector3 outVec;
			outVec.x = inVec.x;
			outVec.y = inVec.y;
			outVec.z = 0;
			return outVec;
		}
	
		/// <summary>
		/// Convert to Vector3.
		/// </summary>
		/// <param name="inVec">Local value.</param>
		/// <param name="z">Z value.</param>
		/// <returns>Return value.</returns>
		public static Vector3 ToVector3(this Vector2 inVec, float z)
		{
			Vector3 outVec;
			outVec.x = inVec.x;
			outVec.y = inVec.y;
			outVec.z = z;
			return outVec;
		}
		#endregion
	
		#region Methods
		/// <summary>
		/// Get the angle of a vector.
		/// </summary>
		/// <param name="vector">Vector.</param>
		/// <param name="result">Return value.</param>
		public static void Angle360(ref Vector2 vector, out float result)
		{
			var vec = vector.normalized;
			float value = ((float)Mathf.Atan2(-vec.y, vec.x)) % MathUtilities.Pi2;
			result = (value < 0) ? ((Mathf.PI+value)+Mathf.PI) : value;
		}
	
		/// <summary>
		/// Lerp a point between two vectors.
		/// </summary>
		/// <param name="p1">Point 1.</param>
		/// <param name="p2">Point 2.</param>
		/// <param name="interpolation">Interpolation value.</param>
		/// <param name="p3">Return value.</param>
		public static void Lerp(ref Vector2 p1, ref Vector2 p2, float interpolation, out Vector2 p3)
		{
			p3.x = Mathf.Lerp(p1.x, p2.x, interpolation);
			p3.y = Mathf.Lerp(p1.y, p2.y, interpolation);
		}
	
		/// <summary>
		/// Get a Quadratic Bezier Curve point.
		/// </summary>
		/// <param name="p1">Point 1.</param>
		/// <param name="p2">Point 2.</param>
		/// <param name="p3">Point 3.</param>
		/// <param name="interpolation">Interpolation value.</param>
		/// <returns>Return value.</returns>
		public static Vector2 QuadraticBezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, float interpolation)
		{
			Vector2 l1;
			l1.x = Mathf.Lerp(p1.x, p2.x, interpolation);
			l1.y = Mathf.Lerp(p1.y, p2.y, interpolation);
		
			Vector2 l2;
			l2.x = Mathf.Lerp(p2.x, p3.x, interpolation);
			l2.y = Mathf.Lerp(p2.y, p3.y, interpolation);
		
			Vector2 outPos;
			outPos.x = Mathf.Lerp(l1.x, l2.x, interpolation);
			outPos.y = Mathf.Lerp(l1.y, l2.y, interpolation);
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
		public static Vector2 CubicBezierCurve(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, float interpolation)
		{
			Vector2 l1;
			l1.x = Mathf.Lerp(p1.x, p2.x, interpolation);
			l1.y = Mathf.Lerp(p1.y, p2.y, interpolation);
		
			Vector2 l2;
			l2.x = Mathf.Lerp(p3.x, p4.x, interpolation);
			l2.y = Mathf.Lerp(p3.y, p4.y, interpolation);
		
			Vector2 l3;
			l3.x = Mathf.Lerp(l1.x, l2.x, interpolation);
			l3.y = Mathf.Lerp(l1.y, l2.y, interpolation);
		
			return QuadraticBezierCurve(l1, l3, l2, interpolation);
		}
	
		/// <summary>
		/// Get a ray intersect point.
		/// </summary>
		/// <param name="vector">Point.</param>
		/// <param name="rayOrigin">Ray Orgin.</param>
		/// <param name="rayDirection">Ray Direction.</param>
		/// <returns>Return value.</returns>
		public static Vector2 InersectRay(this Vector2 vector, Vector2 rayOrigin, Vector2 rayDirection)
		{
			return (rayDirection * Vector2.Dot(vector-rayOrigin, rayDirection)) + rayOrigin;
		}

		/// <summary>
		/// Get a line intersect point.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <param name="p1">Line Point 1.</param>
		/// <param name="p2">Line Point 2.</param>
		/// <returns>Return value.</returns>
		public static Vector2 IntersectLine(Vector2 point, Vector2 p1, Vector2 p2)
		{
			Vector2 pointOffset = (point-p1), vector = (p2-p1).normalized;
			return (vector * Vector2.Dot(pointOffset, vector)) + p1;
		}
	
		/// <summary>
		/// Get a line segment intersect point.
		/// </summary>
		/// <param name="vector">Point.</param>
		/// <param name="point1">Line Point 1.</param>
		/// <param name="point2">Line Point 2.</param>
		/// <returns>Return value.</returns>
		public static Vector2 IntersectLineSegment(this Vector2 vector, Vector2 point1, Vector2 point2)
		{
			var dir = point1 - point2;
			dir.Normalize();
			var point = (dir * Vector2.Dot(vector-point1, dir)) + point1;
			Bound2 bound;
			Bound2.FromPoints(ref point1, ref point2, out bound);
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
		public static void GetBounds(Vector2[] points, out Vector2 min, out Vector2 max)
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
			}
		}
	
		/// <summary>
		/// Rotate vectors around axis.
		/// </summary>
		/// <param name="vectors">Vectors.</param>
		/// <param name="angle">Rotation Angle.</param>
		public static void RotateVectors(Vector2[] vectors, float angle)
		{
			var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, new Vector3(0, 0, 1));
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
		public static void RotatePoints(Vector2[] points, float angle)
		{
			Vector2 min, max;
			GetBounds(points, out min, out max);
			Vector2 center = min + ((max - min) * .5f);
			var offset3 = center.ToVector3();
		
			var rotation = Quaternion.AngleAxis(angle * Mathf.Rad2Deg, new Vector3(0, 0, 1));
			for (int i = 0; i != points.Length; ++i)
			{
				points[i] = ((rotation * (points[i] - center)) + offset3).ToVector2();
			}
		}
	
		/// <summary>
		/// Flip all points over the center of the points.
		/// </summary>
		/// <param name="points">Points.</param>
		public static void FlipPointsX(Vector2[] points)
		{
			Vector2 min, max;
			GetBounds(points, out min, out max);
			Vector2 center = min + ((max - min) * .5f);
		
			for (int i = 0; i != points.Length; ++i)
			{
				points[i].x = (-(points[i].x - center.x)) + center.x;
			}
		}
	
		/// <summary>
		/// Flip all points over the center of the points.
		/// </summary>
		/// <param name="points">Points.</param>
		public static void FlipPointsY(Vector2[] points)
		{
			Vector2 min, max;
			GetBounds(points, out min, out max);
			Vector2 center = min + ((max - min) * .5f);
		
			for (int i = 0; i != points.Length; ++i)
			{
				points[i].y = (-(points[i].y - center.y)) + center.y;
			}
		}
		#endregion
	}
}