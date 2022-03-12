#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using System;
	using TMPro;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Dialog info.
	/// </summary>
	public class DialogInfoTMPro : DialogInfoBase, IUpgradeable
	{
		/// <summary>
		/// The title.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with TitleAdapter.")]
		public TextMeshProUGUI Title;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with MessageAdapter.")]
		public TextMeshProUGUI Message;

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
#endif