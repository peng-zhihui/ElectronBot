namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// DialogInputHelper
	/// </summary>
	public class DialogInputHelper : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Username input.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with UsernameAdapter.")]
		public InputField Username;

		/// <summary>
		/// Password input.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with PasswordAdapter.")]
		public InputField Password;

		/// <summary>
		/// Username input.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter UsernameAdapter;

		/// <summary>
		/// Password input.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter PasswordAdapter;

		/// <summary>
		/// Reset input.
		/// </summary>
		public void Refresh()
		{
			UsernameAdapter.text = string.Empty;
			PasswordAdapter.text = string.Empty;
		}

		/// <summary>
		/// Validate input.
		/// </summary>
		/// <returns>true if input data is valid; otherwise, false.</returns>
		public bool Validate()
		{
			var valid_username = UsernameAdapter.text.Trim().Length > 0;
			var valid_password = PasswordAdapter.text.Length > 0;

			if (!valid_username)
			{
				UsernameAdapter.Select();
			}
			else if (!valid_password)
			{
				PasswordAdapter.Select();
			}

			return valid_username && valid_password;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Username, ref UsernameAdapter);
			Utilities.GetOrAddComponent(Password, ref PasswordAdapter);
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