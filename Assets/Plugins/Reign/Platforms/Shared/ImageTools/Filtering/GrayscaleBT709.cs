#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// GrayscaleBT709.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.Filtering
{    
    /// <summary>
    /// Grayscale image using BT709 algorithm.
    /// </summary>
    /// <remarks>The class uses BT709 algorithm to convert color image to grayscale. 
    /// The conversion coefficients are:
    /// <list type="table">
    ///     <listheader>
    ///         <term>Channel</term>
    ///         <description>Coefficient</description>
    ///     </listheader>
    ///     <item>
    ///         <term>Red</term>
    ///         <description>0.2125</description>
    ///     </item>
    ///     <item>
    ///         <term>Green</term>
    ///         <description>0.7154</description>
    ///     </item>
    ///     <item>
    ///         <term>Blue</term>
    ///         <description>0.0721</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public sealed class GrayscaleBT709 : Grayscale
    {        
        /// <summary>
        /// Initializes a new instance of the <see cref="GrayscaleBT709"/> class.
        /// </summary>
        public GrayscaleBT709()
            : base(0.2125, 0.7154, 0.0721)
        {
        }
    }
}
