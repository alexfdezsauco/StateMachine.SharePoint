// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateAttribute.cs" company="SANDs">
//   Copyright © 2015 SANDs. All rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace StateMachine.SharePoint.Attributes
{
    using System;

    /// <summary>
    ///     The state attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class StateAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateAttribute"/> class.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        public StateAttribute(object state)
        {
            this.State = state;
        }

        #endregion

        #region Public Properties

        /// <summary>
        ///     The state.
        /// </summary>
        public object State { get; private set; }

        #endregion
    }
}