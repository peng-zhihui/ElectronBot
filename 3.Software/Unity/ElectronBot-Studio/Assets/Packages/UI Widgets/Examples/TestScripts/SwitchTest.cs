namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test Switch.
	/// </summary>
	public class SwitchTest : MonoBehaviour
	{
		/// <summary>
		/// Switch.
		/// </summary>
		[SerializeField]
		protected Switch Switch;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			if (Switch != null)
			{
				Switch.OnValueChanged.AddListener(OnSwitchChanged);
			}
		}

		/// <summary>
		/// Handle switch changed event.
		/// </summary>
		/// <param name="status">Status.</param>
		protected virtual void OnSwitchChanged(bool status)
		{
			Debug.Log(string.Format("switch status: {0}", status.ToString()));
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (Switch != null)
			{
				Switch.OnValueChanged.RemoveListener(OnSwitchChanged);
			}
		}
	}
}