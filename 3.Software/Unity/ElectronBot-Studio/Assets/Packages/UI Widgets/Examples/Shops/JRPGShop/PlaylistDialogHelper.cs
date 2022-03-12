namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// PlaylistDialog helper.
	/// </summary>
	public class PlaylistDialogHelper : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Username input.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NameAdapter.")]
		public InputField Name;

		/// <summary>
		/// Username input.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter NameAdapter;

		/// <summary>
		/// Reset input.
		/// </summary>
		public void Refresh()
		{
			NameAdapter.text = string.Empty;
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <returns>true if input data is valid; otherwise, false.</returns>
		public bool Validate()
		{
			var valid_name = NameAdapter.text.Trim().Length > 0;

			if (!valid_name)
			{
				NameAdapter.Select();
			}

			return valid_name;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Name, ref NameAdapter);
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