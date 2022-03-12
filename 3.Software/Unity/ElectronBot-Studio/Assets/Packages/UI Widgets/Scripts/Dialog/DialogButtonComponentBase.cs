namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// DialogInfoBase.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class DialogButtonComponentBase : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public TextAdapter NameAdapter;

		/// <summary>
		/// Sets button label.
		/// </summary>
		/// <param name="name">Name.</param>
		public virtual void SetButtonName(string name)
		{
			NameAdapter.text = name;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
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