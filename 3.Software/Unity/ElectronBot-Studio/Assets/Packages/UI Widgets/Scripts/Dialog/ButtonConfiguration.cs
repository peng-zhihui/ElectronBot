namespace UIWidgets
{
	using System;

	/// <summary>
	/// Button configuration.
	/// </summary>
	/// <typeparam name="TSender">Type of the sender.</typeparam>
	public class ButtonConfiguration<TSender>
	{
		/// <summary>
		/// Label.
		/// </summary>
		public string Label;

		Func<TSender, int, bool> actionSender;

		/// <summary>
		/// Action with sender and button index.
		/// </summary>
		public Func<TSender, int, bool> ActionSender
		{
			get
			{
				return actionSender;
			}

			set
			{
				actionSender = value;
			}
		}

		Func<int, bool> action;

		/// <summary>
		/// Action with button index.
		/// </summary>
		public Func<int, bool> Action
		{
			get
			{
				return action;
			}

			set
			{
				action = value;
			}
		}

		Func<bool> actionBool;

		/// <summary>
		/// Action without button index.
		/// Exists only to keep compatibility with previous versions.
		/// </summary>
		[Obsolete("Replaced with Action.")]
		public Func<bool> ActionBool
		{
			get
			{
				return actionBool;
			}

			set
			{
				actionBool = value;
			}
		}

		/// <summary>
		/// Template index.
		/// </summary>
		public int TemplateIndex;

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonConfiguration{TOwner}"/> class.
		/// </summary>
		public ButtonConfiguration()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonConfiguration{TOwner}"/> class.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		public ButtonConfiguration(string label, Func<TSender, int, bool> action, int templateIndex = 0)
		{
			Label = label;
			ActionSender = action;
			TemplateIndex = templateIndex;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonConfiguration{TOwner}"/> class.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		[Obsolete("Type of \"action\" parameter changed to Func<DialogBase, int, bool> from Func<int, bool>")]
		public ButtonConfiguration(string label, Func<int, bool> action, int templateIndex = 0)
		{
			Label = label;
			Action = action;
			TemplateIndex = templateIndex;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ButtonConfiguration{TOwner}"/> class.
		/// Exists only to keep compatibility with previous versions.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		[Obsolete("Type of \"action\" parameter changed to Func<int, bool> from Func<bool>")]
		public ButtonConfiguration(string label, Func<bool> action, int templateIndex = 0)
		{
			Label = label;
			ActionBool = action;
			TemplateIndex = templateIndex;
		}

		/// <summary>
		/// Process button click.
		/// </summary>
		/// <param name="sender">Sender.</param>
		/// <param name="index">Button index.</param>
		/// <returns>Action result; true if sender should be closed; otherwise false,</returns>
		public bool Process(TSender sender, int index)
		{
			if (ActionSender != null)
			{
				return ActionSender(sender, index);
			}

			if (Action != null)
			{
				return Action(index);
			}

#pragma warning disable 0618
			return ActionBool();
#pragma warning restore 0618
		}
	}
}