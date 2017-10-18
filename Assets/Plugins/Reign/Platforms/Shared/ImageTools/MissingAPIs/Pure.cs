#region Assembly mscorlib.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\mscorlib.dll
#endregion

using System;
using System.Diagnostics;

namespace System.Diagnostics.Contracts.Reign
{
	// Summary:
	//     Indicates that a type or method is pure, that is, it does not make any visible
	//     state changes.
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Parameter | AttributeTargets.Delegate, AllowMultiple = false, Inherited = true)]
	public sealed class PureAttribute : Attribute
	{
		// Summary:
		//     Initializes a new instance of the System.Diagnostics.Contracts.PureAttribute
		//     class.
		public PureAttribute()
		{
			
		}
	}
}