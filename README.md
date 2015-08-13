# StateMachine.SharePoint

This library allow you simplify the implementation of a state machine based approach for monitor and validates the states of SharePoint list items.

For example, you have a custom publication process implemented through a publication request list item and you want to monitor and validates the state change of such requests. 

So, you can follow this tree basic steps:

1) Write the allowed transitions in a validator
	        
    public sealed class PublicationRequestStateMachineValidator : StateMachineValidator<string>
    {
        public PublicationRequestStateMachineValidator()
        {
            this.AddAllowedTransition("WaitingForApproval", "Approved");
            this.AddAllowedTransition("WaitingForApproval", "Rejected");
            this.AddAllowedTransition("Approved", "ReadyToBePublished");
            this.AddAllowedTransition("ReadyToBePublished", "Published");
            this.AddAllowedTransition("Published", "ReadyToBeUnpublished");
            this.AddAllowedTransition("ReadyToBeUnpublished", "Unpublished");
        }
    }

2) Write an event receiver that inherits from StateMachineItemEventReceiverBase:
	
	[StateMachine("State", typeof(PublicationRequestStateMachineValidator))]
	public sealed class PublicationRequestStateMachineItemEventReceiver : StateMachineItemEventReceiverBase
	{
		/// <summary>
		/// Called when the field <c>State</c> of an item of <c>PublicationRequestList</c> changed to Approved state.
		/// </summary>
		[State("Approved")]
		private void OnApproved(SPItemEventProperties properties)
		{
			/*...*/
		}
	
		/// <summary>
		/// Called when the field <c>State</c> of an item of <c>PublicationRequestList</c> changed to Rejected state.
		/// </summary>
		[State("Rejected")]
		private void OnRejected(SPItemEventProperties properties)
		{
			/*...*/	
		}
	
		/// <summary>
		/// Called when the field <c>State</c> of an item of <c>PublicationRequestList</c> changed to ReadyToBePublished state.
		/// </summary>
		[State("ReadyToBePublished")]
		private void OnPublished(SPItemEventProperties properties)
		{
			/*...*/
		}
	
		/// <summary>
		/// Called when the field <c>State</c> of an item of <c>PublicationRequestList</c> changed to Unpublished state.
		/// </summary>
		[State("ReadyToBeUnpublished")]
		private void OnUnpublished(SPItemEventProperties properties)
		{
			/*...*/	
		}
	}

3) Register the event receiver to the list in a feature activation just like this:

	publicationRequestList.RegisterEventReceiverIfRequired(SPEventReceiverType.ItemUpdating, typeof(PublicationRequestStateMachineItemEventReceiver).Assembly.FullName, typeof(PublicationRequestStateMachineItemEventReceiver).FullName);
	publicationRequestList.RegisterEventReceiverIfRequired(SPEventReceiverType.ItemUpdated, typeof(PublicationRequestStateMachineItemEventReceiver).Assembly.FullName, typeof(PublicationRequestStateMachineItemEventReceiver).FullName);