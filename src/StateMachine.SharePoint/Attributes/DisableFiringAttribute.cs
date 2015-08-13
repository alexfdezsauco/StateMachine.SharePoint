// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DisableFiringAttribute.cs" company="SANDs">
//   Copyright © 2015 SANDs. All rights reserved
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace StateMachine.SharePoint.Attributes
{
    using System;

    /// <summary>
    /// The disable firing.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public sealed class DisableFiringAttribute : Attribute
    {
    }
}