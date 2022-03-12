#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Notify info.
	/// </summary>
	public class NotifyInfoTMPro : NotifyInfoBase, IUpgradeable
	{
		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with MessageAdapter.")]
		public TextMeshProUGUI Message;

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
	}
}
#endif