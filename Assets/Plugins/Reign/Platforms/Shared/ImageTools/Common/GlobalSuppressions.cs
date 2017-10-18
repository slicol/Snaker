#define WINDOWS_PHONE
#define SILVERLIGHT

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "ImageTools.ExtendedImage.#LoadAsync(System.IO.Stream)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1023:IndexersShouldNotBeMultidimensional", Scope = "member", Target = "ImageTools.ImageBase.#Item[System.Int32,System.Int32]")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "ImageTools.ImageBase.#Pixels")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "bytes", Scope = "member", Target = "ImageTools.Helpers.Extensions.#ToArrayByBitsLength(System.Byte[],System.Int32)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "ImageTools.IO.Decoders.#GetAvailableDecoders()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "ImageTools.IO.Encoders.#GetAvailableEncoders()")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Scope = "member", Target = "ImageTools.Helpers.Extensions.#Swap`1(!!0&,!!0&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "1#", Scope = "member", Target = "ImageTools.Helpers.Extensions.#Swap`1(!!0&,!!0&)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Scope = "member", Target = "ImageTools.Helpers.Guard.#NotNullOrEmpty(System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1820:TestForEmptyStringsUsingStringLength", Scope = "member", Target = "ImageTools.Helpers.Guard.#NotNullOrEmpty(System.String,System.String,System.String)")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "ImageTools.ExtendedImage.#Apply(ImageTools.ExtendedImage,ImageTools.IImageFilter[])")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "ImageTools.ExtendedImage.#Apply(ImageTools.ExtendedImage,ImageTools.Rectangle,ImageTools.IImageFilter[])")]
