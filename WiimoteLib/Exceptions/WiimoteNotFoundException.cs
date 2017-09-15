using System;

namespace WiimoteLib.Exceptions
{
    /// <summary>
    /// Thrown when no Wiimotes are found in the HID device list
    /// </summary>

    public class WiimoteNotFoundException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WiimoteNotFoundException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public WiimoteNotFoundException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public WiimoteNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
