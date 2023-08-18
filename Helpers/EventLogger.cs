using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IW_ClimateControl
{
    /// <summary>
    /// Handles all messages to SMAPI.
    /// </summary>
    /// <remarks>This is the only way to print to the SMAPI console from outside the main class.</remarks>
    internal class EventLogger
    {
        /// <summary>
        /// The event handler which carries the message to the main mod.
        /// </summary>
        public EventHandler<EventMessage> Send;

        /// <summary>
        /// Initiates the event which sends the message to the main mod.
        /// </summary>
        /// <param name="message">The message to log with SMAPI.</param>
        /// <param name="type">The log level to feed to SMAPI. Determines colour and location of message. See <see cref="EventType"/></param>
        public void SendToSMAPI(string message)
        {
            EventMessage Message = new();
            Message.Message = message;
            Send.Invoke(this, Message);
        }
    }

    /// <summary>
    /// The event container for messages to SMAPI.
    /// </summary>
    internal class EventMessage : EventArgs
    {
        /// <summary>
        /// The message to log with SMAPI.
        /// </summary>
        public string Message { get; set; }
    }
}
