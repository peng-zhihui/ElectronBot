namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Dialog info.
	/// </summary>
	public class DialogInfo : DialogInfoBase, IUpgradeable
	{
		/// <summary>
		/// The title.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with TitleAdapter.")]
		public Text Title;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with MessageAdapter.")]
		public Text Message;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Title, ref TitleAdapter);
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
	}
}