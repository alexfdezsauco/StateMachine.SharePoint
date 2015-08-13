// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineValidationException.cs" company="SANDs">
//   Copyright © 2014 SANDs. All rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace StateMachine.SharePoint.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    ///     The state machine validation exception.
    /// </summary>
    public class StateMachineValidationException : Exception
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineValidationException"/> class.
        /// </summary>
        public StateMachineValidationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public StateMachineValidationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineValidationException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="innerException">
        /// The inner exception.
        /// </param>
        public StateMachineValidationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineValidationException"/> class.
        /// </summary>
        /// <param name="info">
        /// The info.
        /// </param>
        /// <param name="context">
        /// The context.
        /// </param>
        protected StateMachineValidationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}