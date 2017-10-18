#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// GrayscaleRMY.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.Filtering
{
    /// <summary>
    /// Grayscale image using R-Y algorithm.
    /// </summary>
    /// <remarks>The class uses R-Y algorithm to convert color image to grayscale. 
    /// The conversion coefficients are:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Channel</term>
    ///         <description>Coefficient</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Red</term>
    ///         <description>0.5</description>
    ///     </item>
    ///     <item>
    ///         <term>Green</term>
    ///         <description>0.419</description>
    ///     </item>
    ///     <item>
    ///         <term>Blue</term>
    ///         <description>0.081</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public sealed class GrayscaleRMY : Grayscale
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleRMY"/> class.
        /// </summary>
        public GrayscaleRMY() 
            : base(0.5, 0.419, 0.081) 
        {
        }
    }
}
