// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StateMachineItemEventReceiverBase.cs" company="SANDs">
//   Copyright © 2014 SANDs. All rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------
namespace StateMachine.SharePoint
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Microsoft.SharePoint;

    using Newtonsoft.Json;

    using StateMachine.SharePoint.Attributes;
    using StateMachine.SharePoint.Exceptions;
    using StateMachine.SharePoint.Extensions;
    using StateMachine.SharePoint.Interfaces;

    /// <summary>
    ///     The state machine item event receiver base.
    /// </summary>
    public class StateMachineItemEventReceiverBase : SPItemEventReceiver
    {
        #region Methods

        /// <summary>
        /// The item updated.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public override void ItemUpdated(SPItemEventProperties properties)
        {
            base.ItemUpdated(properties);
            if (properties.ListItem.Fields.ContainsField(SystemFieldNames.ChangeDetectionFieldName) && properties.ListItem[SystemFieldNames.ChangeDetectionFieldName] != null)
            {
                var changedFieldsIndexes = JsonConvert.DeserializeObject<List<int>>((string)properties.ListItem[SystemFieldNames.ChangeDetectionFieldName]);
                foreach (int idx in changedFieldsIndexes)
                {
                    this.ItemFieldUpdated(properties.ListItem.Fields[idx].StaticName, properties);
                }
            }
        }

        /// <summary>
        /// The item updating.
        /// </summary>
        /// <param name="properties">
        /// The properties.
        /// </param>
        public override void ItemUpdating(SPItemEventProperties properties)
        {
            base.ItemUpdating(properties);
            if (properties.ListItem != null)
            {
                int idx = 0;
                var changedFieldsIndexes = new List<int>();
                foreach (SPField field in properties.ListItem.Fields)
                {
                    if (field.StaticName != SystemFieldNames.ChangeDetectionFieldName && !field.ReadOnlyField && properties.AfterProperties[field.StaticName] != null && !Equals(properties.ListItem[field.StaticName], properties.AfterProperties[field.StaticName]))
                    {
                        this.ItemFieldUpdating(field.StaticName, properties.AfterProperties[field.StaticName], properties);

                        if (properties.Status == SPEventReceiverStatus.Continue)
                        {
                            if (!properties.AfterProperties.ChangedProperties.ContainsKey(field.StaticName))
                            {
                                changedFieldsIndexes.Add(idx);
                                properties.AfterProperties.ChangedProperties.Add(field.StaticName, properties.AfterProperties[field.StaticName]);
                            }
                            else
                            {
                                properties.AfterProperties.ChangedProperties[field.StaticName] = properties.AfterProperties[field.StaticName];
                            }
                        }
                    }

                    idx++;
                }

                if (properties.ListItem.Fields.ContainsField(SystemFieldNames.ChangeDetectionFieldName))
                {
                    properties.AfterProperties[SystemFieldNames.ChangeDetectionFieldName] = JsonConvert.SerializeObject(changedFieldsIndexes);
                }
            }
        }

        /// <summary>
        /// The item field updated.
        /// </summary>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        private void ItemFieldUpdated(string fieldName, SPItemEventProperties properties)
        {
            Type type = this.GetType();
            var stateMachineAttribute = type.GetCustomAttribute<StateMachineAttribute>();
            if (stateMachineAttribute != null && stateMachineAttribute.ColumnName == fieldName)
            {
                MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                MethodInfo stateMethodInfo = (from m in methodInfos let stateAttribute = m.GetCustomAttribute<StateAttribute>() where stateAttribute != null && Equals(stateAttribute.State, properties.ListItem[fieldName]) select m).FirstOrDefault();
                if (stateMethodInfo != null)
                {
                    var disableFiring = stateMethodInfo.GetCustomAttribute<DisableFiringAttribute>() != null;
                    bool eventFiringEnabled = this.EventFiringEnabled;
                    try
                    {
                        this.EventFiringEnabled = !disableFiring;
                        stateMethodInfo.Invoke(this, new object[] { properties });
                    }
                    finally
                    {
                        this.EventFiringEnabled = eventFiringEnabled;
                    }
                }
            }
        }

        /// <summary>
        /// The item field updating.
        /// </summary>
        /// <param name="fieldName">
        /// The field name.
        /// </param>
        /// <param name="afterFieldValue">
        /// The after field value.
        /// </param>
        /// <param name="properties">
        /// The properties.
        /// </param>
        private void ItemFieldUpdating(string fieldName, object afterFieldValue, SPItemEventProperties properties)
        {
            Type type = this.GetType();
            var stateMachineAttribute = type.GetCustomAttribute<StateMachineAttribute>();
            if (stateMachineAttribute != null && stateMachineAttribute.ColumnName == fieldName)
            {
                try
                {
                    if (!object.Equals(properties.ListItem[fieldName], afterFieldValue))
                    {
                        var instance = (IStateMachineValidator)Activator.CreateInstance(stateMachineAttribute.ValidatorType);
                        instance.ValidateTransition(properties.ListItem[fieldName], afterFieldValue);
                    }
                }
                catch (StateMachineValidationException e)
                {
                    properties.Status = SPEventReceiverStatus.CancelWithError;
                    properties.ErrorMessage = e.Message;
                }
            }
        }

        #endregion
    }
}