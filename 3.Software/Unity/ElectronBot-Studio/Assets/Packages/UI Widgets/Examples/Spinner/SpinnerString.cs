namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Spinner with string values.
	/// Display strings instead numeric value.
	/// </summary>
	public class SpinnerString : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		protected Spinner Spinner;

		/// <summary>
		/// Text component to display selected option.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		protected Text Text;

		/// <summary>
		/// Text component to display selected option.
		/// </summary>
		[SerializeField]
		protected TextAdapter TextAdapter;

		/// <summary>
		/// Options list.
		/// </summary>
		[SerializeField]
		protected List<string> Options = new List<string>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Init()
		{
			Spinner.Min = -1;
			Spinner.Max = Options.Count;
			Spinner.Step = 1;

			// add callback
			Spinner.onValueChangeInt.AddListener(Changed);

			// display initial option
			Changed(Spinner.Value);
		}

		/// <summary>
		/// Handle change event.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void Changed(int value)
		{
			if (value == -1)
			{
				Spinner.Value = Options.Count - 1;
			}
			else if (value == Options.Count)
			{
				Spinner.Value = 0;
			}
			else
			{
				// display option
				TextAdapter.text = Options[Spinner.Value];
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Text, ref TextAdapter);
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