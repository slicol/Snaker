#region Assembly mscorlib.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\mscorlib.dll
#endregion

using System;
using System.Diagnostics;

namespace System.Diagnostics.Contracts.Reign
{
	// Summary:
	//     Instructs analysis tools to assume the correctness of an assembly, type,
	//     or member without performing static verification.
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Constructor | AttributeTargets.Method | AttributeTargets.Property)]
	public sealed class ContractVerificationAttribute : Attribute
	{
		// Summary:
		//     Initializes a new instance of the System.Diagnostics.Contracts.ContractVerificationAttribute
		//     class.
		//
		// Parameters:
		//   value:
		//     true to require verification; otherwise, false.
		public ContractVerificationAttribute(bool value)
		{
			this.Value = value;
		}

		// Summary:
		//     Gets the value that indicates whether to verify the contract of the target.
		//
		// Returns:
		//     true if verification is required; otherwise, false.
		public bool Value { get; private set; }
	}
}