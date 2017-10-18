#region Assembly mscorlib.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\mscorlib.dll
#endregion

using System;
using System.Diagnostics;

namespace System.Diagnostics.Contracts.Reign
{
	// Summary:
	//     Specifies that a class is a contract for a type.
	[Conditional("CONTRACTS_FULL")]
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public sealed class ContractClassForAttribute : Attribute
	{
		// Summary:
		//     Initializes a new instance of the System.Diagnostics.Contracts.ContractClassForAttribute
		//     class, specifying the type the current class is a contract for.
		//
		// Parameters:
		//   typeContractsAreFor:
		//     The type the current class is a contract for.
		public ContractClassForAttribute(Type typeContractsAreFor)
		{
			this.TypeContractsAreFor = typeContractsAreFor;
		}

		// Summary:
		//     Gets the type that this code contract applies to.
		//
		// Returns:
		//     The type that this contract applies to.
		public Type TypeContractsAreFor { get; private set; }
	}
}