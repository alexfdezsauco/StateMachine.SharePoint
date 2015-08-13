// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineValidator.cs" company="SANDs">
//   Copyright © 2015 SANDs. All rights reserved
// </copyright>
// <summary>
//   The state machine validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StateMachine.SharePoint
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using StateMachine.SharePoint.Exceptions;
    using StateMachine.SharePoint.Interfaces;

    /// <summary>
    ///     The state machine validator.
    /// </summary>
    public class StateMachineValidator : IStateMachineValidator
    {
        #region Static Fields

        /// <summary>
        ///     The log.
        /// </summary>
        // private static readonly ILog Log = LogManager.GetCurrentClassLogger();
        #endregion

        #region Fields

        /// <summary>
        ///     The sync obj.
        /// </summary>
        private readonly object syncObj = new object();

        /// <summary>
        ///     The transitions.
        /// </summary>
        private readonly List<TransitionInfo> transitions = new List<TransitionInfo>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The add transition.
        /// </summary>
        /// <param name="fromStatus">
        /// The from status.
        /// </param>
        /// <param name="toStatus">
        /// The to status.
        /// </param>
        public void AddAllowedTransition(object fromStatus, object toStatus)
        {
            lock (this.syncObj)
            {
                TransitionInfo transitionInfo = this.transitions.FirstOrDefault(info => info.FromState.Equals(fromStatus) && info.ToState.Equals(toStatus));
                if (transitionInfo == null)
                {
                    this.transitions.Add(new TransitionInfo(fromStatus, toStatus));
                }
            }
        }

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
        public bool IsAllowedTransition(object fromState, object toState)
        {
            bool result = false;
            try
            {
                this.ValidateTransition(fromState, toState);
                result = true;
            }
            catch (StateMachineValidationException e)
            {
                // Log.Debug(e);
            }

            return result;
        }

        /// <summary>
        /// The is final state.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsFinalState(object state)
        {
            lock (this.syncObj)
            {
                return this.transitions.All(info => !info.FromState.Equals(state));
            }
        }

        /// <summary>
        /// The is transitional state.
        /// </summary>
        /// <param name="state">
        /// The state.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool IsTransitionalState(object state)
        {
            return !this.IsFinalState(state);
        }

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
        /// When the transition is not allowed.
        /// </exception>
        public void ValidateTransition(object fromState, object toState)
        {
            lock (this.syncObj)
            {
                TransitionInfo transitionInfo = this.transitions.FirstOrDefault(info => info.FromState.Equals(fromState) && info.ToState.Equals(toState));
                if (transitionInfo == null)
                {
                    var stateMachineValidationException = new StateMachineValidationException(string.Format(CultureInfo.InvariantCulture, "The transition from state:'{0}' to state:'{1}' is not allowed", fromState, toState));

                    // Log.Error(stateMachineValidationException);
                    
                    throw stateMachineValidationException;
                }
            }
        }

        #endregion

        /// <summary>
        ///     The transition info.
        /// </summary>
        private class TransitionInfo
        {
            #region Constructors and Destructors

            /// <summary>
            /// Initializes a new instance of the <see cref="TransitionInfo"/> class.
            /// </summary>
            /// <param name="fromState">
            /// The from status.
            /// </param>
            /// <param name="toState">
            /// The to status.
            /// </param>
            public TransitionInfo(object fromState, object toState)
            {
                this.FromState = fromState;
                this.ToState = toState;
            }

            #endregion

            #region Public Properties

            /// <summary>
            ///     Gets the from status.
            /// </summary>
            public object FromState { get; private set; }

            /// <summary>
            ///     Gets the to status.
            /// </summary>
            public object ToState { get; private set; }

            #endregion
        }
    }

    /// <summary>
    /// The state machine validator.
    /// </summary>
    /// <typeparam name="TState">
    /// The status type.
    /// </typeparam>
    public class StateMachineValidator<TState> : StateMachineValidator
    {
        #region Public Methods and Operators

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="fromState">
        /// The from status.
        /// </param>
        /// <param name="toState">
        /// The to status.
        /// </param>
        public void ValidateTransition(TState fromState, TState toState)
        {
            this.ValidateTransition(fromState, toState);
        }

        #endregion

        #region Methods

        /// <summary>
        /// The add transition.
        /// </summary>
        /// <param name="fromState">
        /// The from status.
        /// </param>
        /// <param name="toState">
        /// The to status.
        /// </param>
        protected void AddAllowedTransition(TState fromState, TState toState)
        {
            this.AddAllowedTransition(fromState, toState);
        }

        #endregion
    }

    /// <summary>
    /// The state machine validator.
    /// </summary>
    /// <typeparam name="TItemType">
    /// The item type.
    /// </typeparam>
    /// <typeparam name="TState">
    /// The status type.
    /// </typeparam>
    public class StateMachineValidator<TItemType, TState> : StateMachineValidator<TState>
    {
        #region Fields

        /// <summary>
        ///     The property info.
        /// </summary>
        private readonly PropertyInfo propertyInfo;

        #endregion

        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineValidator{TItemType,TState}"/> class. 
        /// </summary>
        /// <param name="expression">
        /// The func.
        /// </param>
        public StateMachineValidator(Expression<Func<TItemType, TState>> expression)
        {
            // Validation.ArgumentIsOfOneOfTheTypes(() => expression.Body, new[] { typeof(UnaryExpression), typeof(MemberExpression) });
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null && expression.Body is UnaryExpression)
            {
                var unaryExpression = expression.Body as UnaryExpression;

                // Validation.ArgumentIsOfType(() => unaryExpression.Operand, typeof(MemberExpression));
                memberExpression = (MemberExpression)unaryExpression.Operand;
            }

            if (memberExpression != null)
            {
                string name = memberExpression.Member.Name;
                this.propertyInfo = typeof(TItemType).GetProperty(name);
            }

            // Validation.ArgumentIsNotNull(() => this.propertyInfo);
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="fromObject">
        /// The from object.
        /// </param>
        /// <param name="toObject">
        /// The to object.
        /// </param>
        /// <exception cref="StateMachineValidationException">
        /// When the <paramref name="fromObject"/> status property can't turn into the status property of the
        ///     <paramref name="toObject"/>.
        /// </exception>
        public void ValidateTransition(TItemType fromObject, TItemType toObject)
        {
            var toStatus = (TState)this.propertyInfo.GetValue(toObject);
            this.ValidateTransition(fromObject, toStatus);
        }

        /// <summary>
        /// The validate.
        /// </summary>
        /// <param name="fromObject">
        /// The from object.
        /// </param>
        /// <param name="toState">
        /// The to status.
        /// </param>
        /// <exception cref="StateMachineValidationException">
        /// When the <paramref name="fromObject"/> status property can't move to the <paramref name="toState"/>.
        /// </exception>
        public void ValidateTransition(TItemType fromObject, TState toState)
        {
            var fromStatus = (TState)this.propertyInfo.GetValue(fromObject);
            this.ValidateTransition(fromStatus, toState);
        }

        #endregion
    }
}