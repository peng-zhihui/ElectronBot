namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// GroupMultipleComponent.
	/// </summary>
	public class GroupMultipleComponent : ListViewItem, IViewData<GroupMultipleItem>
	{
		/// <summary>
		/// Group name.
		/// </summary>
		[SerializeField]
		public TextAdapter GroupName;

		/// <summary>
		/// Group description.
		/// </summary>
		[SerializeField]
		public TextAdapter GroupDescription;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Checkbox.
		/// </summary>
		[SerializeField]
		public Toggle Checkbox;

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		public TextAdapter Value;

		/// <summary>
		/// Item.
		/// </summary>
		public GroupMultipleItem Item
		{
			get;
			protected set;
		}

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			if (Checkbox != null)
			{
				Checkbox.onValueChanged.AddListener(CheckboxChanged);
			}
		}

		void CheckboxChanged(bool isOn)
		{
			Item.IsOn = isOn;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(GroupMultipleItem item)
		{
			Item = item;
			switch (Item.Mode)
			{
				case GroupMultipleItem.ItemMode.Group:
					GroupName.text = Item.Name;
					GroupDescription.text = Item.Desription;
					break;
				case GroupMultipleItem.ItemMode.Checkbox:
					Name.text = Item.Name;
					Checkbox.isOn = Item.IsOn;
					break;
				case GroupMultipleItem.ItemMode.Value:
					Name.text = Item.Name;
					Value.text = Item.Value;
					break;
			}
		}
	}
}