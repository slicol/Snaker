#define WINDOWS_PHONE
#define SILVERLIGHT

// ===============================================================================
// Encoders.cs
// .NET Image Tools
// ===============================================================================
// Copyright (c) .NET Image Tools Development Group. 
// All rights reserved.
// ===============================================================================

namespace ImageTools.IO
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts.Reign;

    /// <summary>
    /// Helper methods for encoders.
    /// </summary>
    public static class Encoders
    {
        private static List<Type> _encoderTypes = new List<Type>();

        /// <summary>
        /// Adds the type of the encoder to the list of available image encoder.
        /// </summary>
        /// <typeparam name="TEncoder">The type of the encoder.</typeparam>
        public static void AddEncoder<TEncoder>() where TEncoder : IImageEncoder
        {
            if (!_encoderTypes.Contains(typeof(TEncoder)))
            {
                _encoderTypes.Add(typeof(TEncoder));
            }
        }

        /// <summary>
        /// Gets a list of all available encoders.
        /// </summary>
        /// <returns>A list of all available encoders.</returns>
        public static ReadOnlyCollection<IImageEncoder> GetAvailableEncoders()
        {
            Contract.Ensures(Contract.Result<ReadOnlyCollection<IImageEncoder>>() != null);

            List<IImageEncoder> encoders = new List<IImageEncoder>();

            foreach (Type encoderType in _encoderTypes)
            {
                if (encoderType != null)
                {
                    encoders.Add(Activator.CreateInstance(encoderType) as IImageEncoder);
                }
            }

            return new ReadOnlyCollection<IImageEncoder>(encoders);
        }
    }
}
