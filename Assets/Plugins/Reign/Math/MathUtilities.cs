// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;
using System;

namespace Reign
{
	/// <summary>
	/// Helper methods and values.
	/// </summary>
	public static class MathUtilities
	{
		/// <summary>
		/// PI * 2
		/// </summary>
		public const float Pi2 = Mathf.PI * 2;

		/// <summary>
		/// Use to fit one object into another if the object is larger.
		/// </summary>
		/// <param name="objectWidth">Obj to fit width.</param>
		/// <param name="objectHeight">Obj to fit height.</param>
		/// <param name="viewWidth">View to fit Obj into width.</param>
		/// <param name="viewHeight">View to fit Obj into height.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FitInViewIfLarger(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return FitInViewIfLarger(objectSize, viewSize);
		}
	
		/// <summary>
		/// Use to fit one object into another if the object is larger.
		/// </summary>
		/// <param name="objectSize">Obj to fit size.</param>
		/// <param name="viewSize">View to fit Obj into size.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FitInViewIfLarger(Vector2 objectSize, Vector2 viewSize)
		{
			if (objectSize.x <= viewSize.x && objectSize.y <= viewSize.y) return objectSize;
			return FitInView(objectSize, viewSize);
		}

		/// <summary>
		/// Use to fit one object into another if the object is smaller.
		/// </summary>
		/// <param name="objectWidth">Obj to fit width.</param>
		/// <param name="objectHeight">Obj to fit height.</param>
		/// <param name="viewWidth">View to fit Obj into width.</param>
		/// <param name="viewHeight">View to fit Obj into height.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FitInViewIfSmaller(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return FitInViewIfSmaller(objectSize, viewSize);
		}
	
		/// <summary>
		/// Use to fit one object into another if the object is smaller.
		/// </summary>
		/// <param name="objectSize">Obj to fit size.</param>
		/// <param name="viewSize">View to fit Obj into size.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FitInViewIfSmaller(Vector2 objectSize, Vector2 viewSize)
		{
			if (objectSize.x >= viewSize.x || objectSize.y >= viewSize.y) return objectSize;
			return FitInView(objectSize, viewSize);
		}

		/// <summary>
		/// Use to fit one object into another.
		/// </summary>
		/// <param name="objectWidth">Obj to fit width.</param>
		/// <param name="objectHeight">Obj to fit height.</param>
		/// <param name="viewWidth">View to fit Obj into width.</param>
		/// <param name="viewHeight">View to fit Obj into height.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FitInView(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return FitInView(objectSize, viewSize);
		}
	
		/// <summary>
		/// Use to fit one object into another.
		/// </summary>
		/// <param name="objectSize">Obj to fit size.</param>
		/// <param name="viewSize">View to fit Obj into size.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FitInView(Vector2 objectSize, Vector2 viewSize)
		{
			#if DEBUG
			if (objectSize.x == 0 || objectSize.y == 0 || viewSize.x == 0 || viewSize.y == 0)
			{
				throw new Exception("Object and View sizes can't be 0.");
			}
			#endif
		
			float objectSlope = objectSize.y / objectSize.x;
			float viewSlope = viewSize.y / viewSize.x;
		
			if (objectSlope >= viewSlope) return new Vector2(objectSize.x/objectSize.y, 1) * viewSize.y;
			else return new Vector2(1, objectSize.y/objectSize.x) * viewSize.x;
		}
	
		/// <summary>
		/// Get scale value needed to fit one object into another.
		/// </summary>
		/// <param name="objectWidth">Obj to fit width.</param>
		/// <param name="objectHeight">Obj to fit height.</param>
		/// <param name="viewWidth">View to fit Obj into width.</param>
		/// <param name="viewHeight">View to fit Obj into height.</param>
		/// <returns>Returns scale.</returns>
		public static Vector2 ScaleToFitInView(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return ScaleToFitInView(objectSize, viewSize);
		}
	
		/// <summary>
		/// Get scale value needed to fit one object into another.
		/// </summary>
		/// <param name="objectSize">Obj to fit size.</param>
		/// <param name="viewSize">View to fit Obj into size.</param>
		/// <returns>Returns scale.</returns>
		public static Vector2 ScaleToFitInView(Vector2 objectSize, Vector2 viewSize)
		{
			var fitViewSize = FitInView(objectSize, viewSize);
			objectSize.x /= fitViewSize.x;
			objectSize.y /= fitViewSize.y;
			return objectSize;
		}
	
		/// <summary>
		/// Use to fill one object into another.
		/// </summary>
		/// <param name="objectWidth">Obj to fill width.</param>
		/// <param name="objectHeight">Obj to fill height.</param>
		/// <param name="viewWidth">View to fill Obj into width.</param>
		/// <param name="viewHeight">View to fill Obj into height.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FillView(float objectWidth, float objectHeight, float viewWidth, float viewHeight)
		{
			Vector2 objectSize, viewSize;
			objectSize.x = objectWidth;
			objectSize.y = objectHeight;
			viewSize.x = viewWidth;
			viewSize.y = viewHeight;
			return FillView(objectSize, viewSize);
		}
	
		/// <summary>
		/// Use to fill one object into another.
		/// </summary>
		/// <param name="objectSize">Obj to fill size.</param>
		/// <param name="viewSize">View to fill Obj into size.</param>
		/// <returns>Returns new size.</returns>
		public static Vector2 FillView(Vector2 objectSize, Vector2 viewSize)
		{
			#if DEBUG
			if (objectSize.x == 0 || objectSize.y == 0 || viewSize.x == 0 || viewSize.y == 0)
			{
				throw new Exception("Object and View sizes can't be 0.");
			}
			#endif
		
			float objectSlope = objectSize.y / objectSize.x;
			float viewSlope = viewSize.y / viewSize.x;
		
			if (objectSlope <= viewSlope) return new Vector2(objectSize.x/objectSize.y, 1) * viewSize.y;
			else return new Vector2(1, objectSize.y/objectSize.x) * viewSize.x;
		}
	}
}