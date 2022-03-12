namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Disable Combobox input.
	/// </summary>
	[RequireComponent(typeof(Combobox))]
	[System.Obsolete]
	public class ComboboxDisableDefaultInput : MonoBehaviour
	{
		/// <summary>
		/// Disable combobox input.
		/// </summary>
		protected virtual void Start()
		{
			// disable default combobox input, this also disable input field
			var combobox = GetComponent<Combobox>();
			combobox.Init();
			combobox.Editable = false;

			// enable input field back
			var adapter = GetComponent<InputFieldAdapter>();
			if (adapter != null)
			{
				adapter.interactable = true;
			}
			else
			{
				var input = GetComponent<InputField>();
				if (input != null)
				{
					input.interactable = true;
				}
			}
		}
	}
}