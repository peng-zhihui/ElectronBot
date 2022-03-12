namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// LVInputFields component.
	/// </summary>
	public class LVInputFieldsComponent : ListViewItem, IViewData<LVInputFieldsItem>
	{
		/// <summary>
		/// Input1.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Input1Adapter.")]
		public InputField Input1;

		/// <summary>
		/// Input2.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with Input2Adapter.")]
		public InputField Input2;

		/// <summary>
		/// Input1.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter Input1Adapter;

		/// <summary>
		/// Input2.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter Input2Adapter;

		/// <summary>
		/// Toggle.
		/// </summary>
		[SerializeField]
		public Toggle Toggle;

		LVInputFieldsItem currentItem;

		/// <summary>
		/// Current item.
		/// </summary>
		public LVInputFieldsItem Item
		{
			get
			{
				return currentItem;
			}

			set
			{
				if (currentItem != null)
				{
					// unsubscribe
					currentItem.OnChange -= UpdateInputFields;
				}

				currentItem = value;

				if (currentItem != null)
				{
					// subscribe to event
					// when item properties changed OnChange will called
					currentItem.OnChange += UpdateInputFields;

					// and update InputFields values
					UpdateInputFields();
				}
			}
		}

		void UpdateInputFields()
		{
			Input1Adapter.text = Item.Text1;
			Input2Adapter.text = Item.Text2;
			Toggle.isOn = Item.IsOn;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(LVInputFieldsItem item)
		{
			Item = item;
		}

		/// <summary>
		/// Handle Input1.OnEndEdit event.
		/// Attached in Inspector window to InputField1.EndEdit.
		/// </summary>
		public void Text1Changed()
		{
			Item.Text1 = Input1Adapter.text;
		}

		/// <summary>
		/// Handle Input2.OnEndEdit event.
		/// Attached in Inspector window to InputField2.EndEdit.
		/// </summary>
		public void Text2Changed()
		{
			Item.Text2 = Input2Adapter.text;
		}

		/// <summary>
		/// Handle Toggle.OnValueChanged event.
		/// Attached in Inspector window to Toggle.OnValueChanged.
		/// </summary>
		public void IsOnChanged()
		{
			Item.IsOn = Toggle.isOn;
		}

		/// <summary>
		/// Reset current item.
		/// </summary>
		public override void MovedToCache()
		{
			Item = null;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Input1, ref Input1Adapter);
			Utilities.GetOrAddComponent(Input2, ref Input2Adapter);
#pragma warning restore 0612, 0618
		}
	}
}