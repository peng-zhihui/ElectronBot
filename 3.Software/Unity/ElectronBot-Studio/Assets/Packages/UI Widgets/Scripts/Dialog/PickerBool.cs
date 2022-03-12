namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// PickerBool.
	/// </summary>
	public class PickerBool : Picker<bool, PickerBool>, IUpgradeable
	{
		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with MessageAdapter.")]
		protected Text Message;

		/// <summary>
		/// Message.
		/// </summary>
		[SerializeField]
		protected TextAdapter MessageAdapter;

		/// <summary>
		/// Set message.
		/// </summary>
		/// <param name="message">Message text.</param>
		public virtual void SetMessage(string message)
		{
			MessageAdapter.text = message;
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(bool defaultValue)
		{
		}

		/// <summary>
		/// Select true value.
		/// </summary>
		public void Yes()
		{
			Selected(true);
		}

		/// <summary>
		/// Select false value.
		/// </summary>
		public void No()
		{
			Selected(false);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Message, ref MessageAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			if (MessageAdapter != null)
			{
				style.Dialog.ContentText.ApplyTo(MessageAdapter.gameObject);
			}

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Yes"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/No"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			if (MessageAdapter != null)
			{
				style.Dialog.ContentText.GetFrom(MessageAdapter.gameObject);
			}

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Yes"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/No"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}