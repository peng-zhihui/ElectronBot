namespace UIWidgets
{
	using System;

	/// <summary>
	/// Dialog button info.
	/// </summary>
	public class DialogButton : ButtonConfiguration<DialogBase>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DialogButton"/> class.
		/// </summary>
		public DialogButton()
			: base()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogButton"/> class.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		public DialogButton(string label, Func<DialogBase, int, bool> action, int templateIndex = 0)
			: base(label, action, templateIndex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogButton"/> class.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		[Obsolete("Type of \"action\" parameter changed to Func<DialogBase, int, bool> from Func<int, bool>")]
		public DialogButton(string label, Func<int, bool> action, int templateIndex = 0)
			: base(label, action, templateIndex)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogButton"/> class.
		/// Exists only to keep compatibility with previous versions.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="action">Action on button click.</param>
		/// <param name="templateIndex">Button template index.</param>
		[Obsolete("Type of \"action\" parameter changed to Func<int, bool> from Func<bool>")]
		public DialogButton(string label, Func<bool> action, int templateIndex = 0)
			: base(label, action, templateIndex)
		{
		}
	}
}