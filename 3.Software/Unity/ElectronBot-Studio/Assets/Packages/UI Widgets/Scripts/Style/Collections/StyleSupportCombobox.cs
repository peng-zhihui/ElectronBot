namespace UIWidgets.Styles
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style support for the combobox.
	/// </summary>
	public class StyleSupportCombobox : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public GameObject Combobox;

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Combobox.Background.ApplyTo(GetComponent<Image>());

			if ((Combobox != null) && (Combobox.GetInstanceID() != gameObject.GetInstanceID()))
			{
				var stylable = Compatibility.GetComponent<IStylable>(Combobox);

				if (stylable != null)
				{
					stylable.SetStyle(style);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.Combobox.Background.GetFrom(GetComponent<Image>());

			if ((Combobox != null) && (Combobox.GetInstanceID() != gameObject.GetInstanceID()))
			{
				var stylable = Compatibility.GetComponent<IStylable>(Combobox);

				if (stylable != null)
				{
					stylable.GetStyle(style);
				}
			}

			return true;
		}
		#endregion
	}
}