#region Assembly mscorlib.dll, v2.0.50727
// C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\Silverlight\v4.0\mscorlib.dll
#endregion

using System;
using System.Collections.Generic;
using System.Diagnostics;
//using System.Runtime.ConstrainedExecution;

namespace System.Diagnostics.Contracts.Reign
{
	// Summary:
	//     Contains static methods for representing program contracts such as preconditions,
	//     postconditions, and object invariants.
	public static class Contract
	{
		// Summary:
		//     Checks for a condition; if the condition is false, follows the escalation
		//     policy set for the analyzer.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[Conditional("DEBUG")]
		public static void Assert(bool condition)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Checks for a condition; if the condition is false, follows the escalation
		//     policy set by the analyzer and displays the specified message.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		//   userMessage:
		//     A message to display if the condition is not met.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[Conditional("DEBUG")]
		[Conditional("CONTRACTS_FULL")]
		public static void Assert(bool condition, string userMessage)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Instructs code analysis tools to assume that the specified condition is true,
		//     even if it cannot be statically proven to always be true.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to assume true.
		[Conditional("DEBUG")]
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Assume(bool condition)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Instructs code analysis tools to assume that a condition is true, even if
		//     it cannot be statically proven to always be true, and displays a message
		//     if the assumption fails.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to assume true.
		//
		//   userMessage:
		//     The message to post if the assumption fails.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[Conditional("DEBUG")]
		public static void Assume(bool condition, string userMessage)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Marks the end of the contract section when a method's contracts contain only
		//     preconditions in the if-then-throw form.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		[Conditional("CONTRACTS_FULL")]
		public static void EndContractBlock()
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a postcondition contract for the enclosing method or property.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test. The expression may include System.Diagnostics.Contracts.Contract.OldValue<T0>(T0),
		//     System.Diagnostics.Contracts.Contract.ValueAtReturn<T0>(T0@), and System.Diagnostics.Contracts.Contract.Result<T0>()
		//     values.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Ensures(bool condition)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a postcondition contract for a provided exit condition and a message
		//     to display if the condition is false.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test. The expression may include System.Diagnostics.Contracts.Contract.OldValue<T0>(T0)
		//     and System.Diagnostics.Contracts.Contract.Result<T0>() values.
		//
		//   userMessage:
		//     The message to display if the expression is not true.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[Conditional("CONTRACTS_FULL")]
		public static void Ensures(bool condition, string userMessage)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a postcondition contract for the enclosing method or property,
		//     based on the provided exception and condition.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		// Type parameters:
		//   TException:
		//     The type of exception that invokes the postcondition check.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void EnsuresOnThrow<TException>(bool condition) where TException : Exception
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a postcondition contract and a message to display if the condition
		//     is false for the enclosing method or property, based on the provided exception
		//     and condition.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		//   userMessage:
		//     The message to display if the expression is false.
		//
		// Type parameters:
		//   TException:
		//     The type of exception that invokes the postcondition check.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[Conditional("CONTRACTS_FULL")]
		public static void EnsuresOnThrow<TException>(bool condition, string userMessage) where TException : Exception
		{
			// do nothing...
		}

		//
		// Summary:
		//     Determines whether an element within a collection of elements exists within
		//     a function.
		//
		// Parameters:
		//   collection:
		//     The collection from which elements of type T will be drawn to pass to predicate.
		//
		//   predicate:
		//     The function to evaluate for an element in collection.
		//
		// Type parameters:
		//   T:
		//     The type that is contained in collection.
		//
		// Returns:
		//     true if and only if predicate returns true for any element of type T in collection.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     collection or predicate is null.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		//public static bool Exists<T>(IEnumerable<T> collection, Predicate<T> predicate)
		//{
		//	// do nothing...
		//}

		//
		// Summary:
		//     Determines whether a specified test is true for any integer within a range
		//     of integers.
		//
		// Parameters:
		//   fromInclusive:
		//     The first integer to pass to predicate.
		//
		//   toExclusive:
		//     One more than the last integer to pass to predicate.
		//
		//   predicate:
		//     The function to evaluate for any value of the integer in the specified range.
		//
		// Returns:
		//     true if predicate returns true for any integer starting from fromInclusive
		//     to toExclusive - 1.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     predicate is null.
		//
		//   System.ArgumentException:
		//     toExclusive is less than fromInclusive.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		//public static bool Exists(int fromInclusive, int toExclusive, Predicate<int> predicate)
		//{
		//	// do nothing...
		//}

		//
		// Summary:
		//     Determines whether all the elements in a collection exist within a function.
		//
		// Parameters:
		//   collection:
		//     The collection from which elements of type T will be drawn to pass to predicate.
		//
		//   predicate:
		//     The function to evaluate for the existence of all the elements in collection.
		//
		// Type parameters:
		//   T:
		//     The type that is contained in collection.
		//
		// Returns:
		//     true if and only if predicate returns true for all elements of type T in
		//     collection.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     collection or predicate is null.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		//public static bool ForAll<T>(IEnumerable<T> collection, Predicate<T> predicate)
		//{
		//	// do nothing...
		//}

		//
		// Summary:
		//     Determines whether a particular condition is valid for all integers in a
		//     specified range.
		//
		// Parameters:
		//   fromInclusive:
		//     The first integer to pass to predicate.
		//
		//   toExclusive:
		//     One more than the last integer to pass to predicate.
		//
		//   predicate:
		//     The function to evaluate for the existence of the integers in the specified
		//     range.
		//
		// Returns:
		//     true if predicate returns true for all integers starting from fromInclusive
		//     to toExclusive - 1.
		//
		// Exceptions:
		//   System.ArgumentNullException:
		//     predicate is null.
		//
		//   System.ArgumentException:
		//     toExclusive is less than fromInclusive.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		//public static bool ForAll(int fromInclusive, int toExclusive, Predicate<int> predicate)
		//{
		//	// do nothing...
		//}

		//
		// Summary:
		//     Specifies an invariant contract for the enclosing class.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Invariant(bool condition)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies an invariant contract for the enclosing class, and displays a message
		//     if the condition for the contract fails.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		//   userMessage:
		//     The message to display if the condition is false.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Invariant(bool condition, string userMessage)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Represents values as they were at the start of a method or property.
		//
		// Parameters:
		//   value:
		//     The value to represent (field or parameter).
		//
		// Type parameters:
		//   T:
		//     The type of value.
		//
		// Returns:
		//     The value of the parameter or field at the start of a method or property.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		//public static T OldValue<T>(T value)
		//{
		//	// do nothing...
		//}

		//
		// Summary:
		//     Specifies a precondition contract for the enclosing method or property.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		[Conditional("CONTRACTS_FULL")]
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Requires(bool condition)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a precondition contract for the enclosing method or property, and
		//     throws an exception if the condition for the contract fails.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		// Type parameters:
		//   TException:
		//     The exception to throw if the condition is false.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Requires<TException>(bool condition) where TException : Exception
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a precondition contract for the enclosing method or property, and
		//     throws an exception with the provided message if the condition for the contract
		//     fails.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		//   userMessage:
		//     The message to display if the condition is false.
		//
		// Type parameters:
		//   TException:
		//     The exception to throw if the condition is false.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		public static void Requires<TException>(bool condition, string userMessage) where TException : Exception
		{
			// do nothing...
		}

		//
		// Summary:
		//     Specifies a precondition contract for the enclosing method or property, and
		//     displays a message if the condition for the contract fails.
		//
		// Parameters:
		//   condition:
		//     The conditional expression to test.
		//
		//   userMessage:
		//     The message to display if the condition is false.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
		[Conditional("CONTRACTS_FULL")]
		public static void Requires(bool condition, string userMessage)
		{
			// do nothing...
		}

		//
		// Summary:
		//     Represents the return value of a method or property.
		//
		// Type parameters:
		//   T:
		//     Type of return value of the enclosing method or property.
		//
		// Returns:
		//     Return value of the enclosing method or property.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		public static T Result<T>()
		{
			return default(T);
		}

		//
		// Summary:
		//     Represents the final (output) value of an out parameter when returning from
		//     a method.
		//
		// Parameters:
		//   value:
		//     The out parameter.
		//
		// Type parameters:
		//   T:
		//     The type of the out parameter.
		//
		// Returns:
		//     The output value of the out parameter.
		//[ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
		//public static T ValueAtReturn<T>(out T value)
		//{
		//	T = null;
		//	return T;
		//}

	}
}