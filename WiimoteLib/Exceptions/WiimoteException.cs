using System;

namespace WiiMoteLib.Exceptions
{

    /// <summary>
    /// Represents errors that occur during the execution of the Wiimote library
    /// </summary>
    public class WiimoteException : Exception
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public WiimoteException()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        public WiimoteException(string message) : base(message)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="message">Error message</param>
        /// <param name="innerException">Inner exception</param>
        public WiimoteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
