namespace UIWidgets
{
	using System;

	/// <summary>
	/// Notification button info.
	/// </summary>
	public class NotificationButton : ButtonConfiguration<NotificationBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationButton"/> class.
		/// </summary>
		public NotificationButton()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationButton"/> class.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		public NotificationButton(string label, Func<NotificationBase, int, bool> action, int templateIndex = 0)
			: base(label, action, templateIndex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationButton"/> class.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		[Obsolete("Type of \"action\" parameter changed to Func<DialogBase, int, bool> from Func<int, bool>")]
		public NotificationButton(string label, Func<int, bool> action, int templateIndex = 0)
			: base(label, action, templateIndex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NotificationButton"/> class.
		/// Exists only to keep compatibility with previous versions.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		[Obsolete("Type of \"action\" parameter changed to Func<int, bool> from Func<bool>")]
		public NotificationButton(string label, Func<bool> action, int templateIndex = 0)
			: base(label, action, templateIndex)
		{
		}
	}
}