// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineAttribute.cs" company="SANDs">
//   Copyright © 2014 SANDs. All rights reserved
// </copyright>
// <summary>
//   The state machine attribute.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace StateMachine.SharePoint.Attributes
{
    using System;

    /// <summary>
    ///     The state machine attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class StateMachineAttribute : Attribute
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="StateMachineAttribute"/> class.
        /// </summary>
        /// <param name="columnName">
        /// The column name.
        /// </param>
        /// <param name="validatorType">
        /// The validator type.
        /// </param>
        public StateMachineAttribute(string columnName, Type validatorType)
        {
            // Validation.ArgumentIsNotNullOrWhitespace(() => columnName);
            // Validation.ArgumentIsNotNull(() => validatorType);
            // Validation.ArgumentIsOfType(() => validatorType, typeof(IStateMachineValidator));

            this.ColumnName = columnName;
            this.ValidatorType = validatorType;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the column name.
        /// </summary>
        public string ColumnName { get; private set; }

        /// <summary>
        /// Gets the validator type.
        /// </summary>
        public Type ValidatorType { get; private set; }

        #endregion
    }
}