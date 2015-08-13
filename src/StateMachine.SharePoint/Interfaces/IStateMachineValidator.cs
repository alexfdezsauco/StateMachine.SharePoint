// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStateMachineValidator.cs" company="SANDs">
//   Copyright © 2015 SANDs. All rights reserved
// </copyright>
// <summary>
//   The StateMachineValidator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StateMachine.SharePoint.Interfaces
{
    using StateMachine.SharePoint.Exceptions;

    /// <summary>
    ///     The StateMachineValidator interface.
    /// </summary>
    public interface IStateMachineValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="fromState">
        /// The from state.
        /// </param>
        /// <param name="toState">
        /// The to state.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsAllowedTransition(object fromState, object toState);

        /// <summary>
        /// The is final state.
        /// </summary>
        /// <param name="state">
        /// The from state.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsFinalState(object state);

        /// <summary>
        /// The is transitional state.
        /// </summary>
        /// <param name="state">
        /// The from state.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool IsTransitionalState(object state);

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="fromState">
        /// The from status.
        /// </param>
        /// <param name="toState">
        /// The to status.
        /// </param>
        /// <exception cref="StateMachineValidationException">
        /// When the <paramref name="toState"/> is not valid from the from <paramref name="toState"/>.
        /// </exception>
        void ValidateTransition(object fromState, object toState);

        #endregion
    }
}