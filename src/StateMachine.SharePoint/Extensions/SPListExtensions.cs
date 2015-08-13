// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SPListExtensions.cs" company="SANDs">
//   Copyright © 2015 SANDs. All rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace StateMachine.SharePoint.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;

    using Microsoft.SharePoint;

    using StateMachine.SharePoint.Attributes;
    using StateMachine.SharePoint.Interfaces;

    /// <summary>
    ///     The <see cref="SPList" /> extension.
    /// </summary>
    public static class SPListExtensions
    {
        #region Public Methods and Operators


        /// <summary>
        /// Registers an event receiver if required.
        /// </summary>
        /// <param name="this">
        /// The this.
        /// </param>
        /// <param name="eventReceiverType">
        /// The event receiver type.
        /// </param>
        /// <param name="assemblyName">
        /// The assembly name.
        /// </param>
        /// <param name="className">
        /// The class name.
        /// </param>
        /// <param name="synchronization">
        /// The synchronization.
        /// </param>
        public static void RegisterEventReceiverIfRequired(this SPList @this, SPEventReceiverType eventReceiverType, string assemblyName, string className, SPEventReceiverSynchronization synchronization = SPEventReceiverSynchronization.Default)
        {
            // Validation.ArgumentIsNotNull(() => @this);
            // Validation.ArgumentIsNotNullOrWhitespace(() => assemblyName);
            // Validation.ArgumentIsNotNullOrWhitespace(() => className);

            SPEventReceiverDefinition eventReceiverDefinition = @this.EventReceivers.OfType<SPEventReceiverDefinition>().FirstOrDefault(receiver => receiver.Type == eventReceiverType && receiver.Assembly.Equals(assemblyName) && receiver.Class.Equals(className));
            if (eventReceiverDefinition == null)
            {
                @this.EventReceivers.Add(eventReceiverType, assemblyName, className);

                eventReceiverDefinition = @this.EventReceivers.OfType<SPEventReceiverDefinition>().First(receiver => receiver.Type == eventReceiverType && receiver.Assembly.Equals(assemblyName) && receiver.Class.Equals(className));
                eventReceiverDefinition.Synchronization = synchronization;
                eventReceiverDefinition.Update();

                Type type = Type.GetType(string.Format(CultureInfo.InvariantCulture, "{0}, {1}", className, assemblyName));
                if (type != null && typeof(StateMachineItemEventReceiverBase).IsAssignableFrom(type) && !@this.Fields.ContainsField(SystemFieldNames.ChangeDetectionFieldName))
                {
                    @this.Fields.Add(SystemFieldNames.ChangeDetectionFieldName, SPFieldType.Text, false);
                    @this.Update();

                    IEnumerable<SPField> changeDetectionFields = @this.Fields.OfType<SPField>().Where(field => SystemFieldNames.ChangeDetectionFieldName.Equals(field.Title)).ToList();
                    foreach (SPField changeDetectionField in changeDetectionFields)
                    {
                        changeDetectionField.Hidden = true;
                        changeDetectionField.Update();
                    }

                    @this.Update();
                }
            }
        }


        /// <summary>
        /// Tries to get state machine validator by field name.
        /// </summary>
        /// <param name="this">
        /// The this.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="stateMachineValidator">
        /// The state machine validator.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool TryGetStateMachineValidatorByFieldName(this SPList @this, string fieldName, out IStateMachineValidator stateMachineValidator)
        {
            // Validation.ArgumentIsNotNull(() => @this);
            // Validation.ArgumentIsNotNullOrWhitespace(() => fieldName);

            return (stateMachineValidator = @this.GetStateMachineValidatorByFieldName(fieldName)) != null;
        }


        #endregion

        #region Methods

        /// <summary>
        /// Gets the state machine validator by field name.
        /// </summary>
        /// <param name="this">
        /// The this.
        /// </param>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <returns>
        /// The <see cref="IStateMachineValidator"/>.
        /// </returns>
        private static IStateMachineValidator GetStateMachineValidatorByFieldName(this SPList @this, string fieldName)
        {
            IStateMachineValidator stateMachineValidator = null;
            SPEventReceiverDefinition eventReceiverDefinition = @this.EventReceivers.OfType<SPEventReceiverDefinition>().FirstOrDefault(definition => Type.GetType(string.Format("{0}, {1}", definition.Class, definition.Assembly)).GetCustomAttribute(typeof(StateMachineAttribute)) != null);
            if (eventReceiverDefinition != null)
            {
                Type eventHandlerType = Type.GetType(string.Format("{0}, {1}", eventReceiverDefinition.Class, eventReceiverDefinition.Assembly));
                var stateMachineAttribute = eventHandlerType.GetCustomAttribute<StateMachineAttribute>();
                if (stateMachineAttribute.ColumnName == fieldName)
                {
                    stateMachineValidator = (IStateMachineValidator)Activator.CreateInstance(stateMachineAttribute.ValidatorType);
                }
            }

            return stateMachineValidator;
        }

        #endregion
    }
}