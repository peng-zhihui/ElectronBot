namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Autocomplete test.
	/// </summary>
	public class AutocompleteTest : MonoBehaviour
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public AutocompleteString Autocomplete;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected virtual void Start()
		{
			// OptionSelected will be called when user select value
			Autocomplete.OnOptionSelectedItem.AddListener(OptionSelected);
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected virtual void OnDestroy()
		{
			Autocomplete.OnOptionSelectedItem.RemoveListener(OptionSelected);
		}

		/// <summary>
		/// Process selected text.
		/// </summary>
		/// <param name="text">Selected text.</param>
		protected virtual void OptionSelected(string text)
		{
			// do something with text
			Debug.Log(string.Format("Autocomplete selected value = {0}", text));
		}
	}
}