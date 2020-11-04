using System;
using System.Linq;

namespace YantraJS.Core
{
    /// <summary>
    /// State of an object
    /// </summary>
    public enum ObjectStatus
    {
        /// <summary>
        /// Default
        /// </summary>
        None = 0,

        /// <summary>
        /// Frozen
        /// </summary>
        Frozen = 1,

        /// <summary>
        /// Sealed
        /// </summary>
        Sealed = 2,

        /// <summary>
        /// Prevent Extension has been called
        /// </summary>
        NonExtensible = 4,

        /// <summary>
        /// Sealed and Frozen
        /// </summary>
        SealedOrFrozen = 3,

        /// <summary>
        /// Sealed and Forzen and Prevent extension has been called
        /// </summary>
        SealedFrozenNonExtensible = 7
    }
}
