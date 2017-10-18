// -------------------------------------------------------
//  Created by Andrew Witte.
// -------------------------------------------------------

using UnityEngine;
using System.Collections;

namespace Reign
{
	/// <summary>
	/// Vector4 extensions helpers.
	/// </summary>
	public static class Vector4Extensions
	{
		#region Method Extensions
		/// <summary>
		/// Convert to Vector2.
		/// </summary>
		/// <param name="inVec">Local value.</param>
		/// <returns>Return value.</returns>
		public static Vector2 ToVector2(this Vector4 inVec)
		{
			Vector2 outVec;
			outVec.x = inVec.x;
			outVec.y = inVec.y;
			return outVec;
		}
	
		/// <summary>
		/// Convert to Vector3
		/// </summary>
		/// <param name="inVec">Local value.</param>
		/// <returns>Return value.</returns>
		public static Vector3 ToVector3(this Vector4 inVec)
		{
			Vector3 outVec;
			outVec.x = inVec.x;
			outVec.y = inVec.y;
			outVec.z = inVec.z;
			return outVec;
		}
		#endregion
	}
}