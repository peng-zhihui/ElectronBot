namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIconsItem extended component.
	/// </summary>
	public class ListViewButtonsComponent : ListViewItem, IViewData<ListViewButtonsItem>
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Button 1.
		/// </summary>
		[SerializeField]
		public Button Button1;

		/// <summary>
		/// Button 2.
		/// </summary>
		[SerializeField]
		public Button Button2;

		/// <summary>
		/// Button 3.
		/// </summary>
		[SerializeField]
		public Button Button3;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public ListViewButtonsItem Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Get selectable GameObject by specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Selectable GameObject.</returns>
		public override GameObject GetSelectableObject(int index)
		{
			return GetFirstButton();
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ListViewButtonsItem item)
		{
			Item = item;

			if (Item == null)
			{
				name = "null";
				Name.Value = string.Empty;

				Button1.gameObject.SetActive(false);
				Button2.gameObject.SetActive(false);
				Button3.gameObject.SetActive(false);
			}
			else
			{
				name = Item.Name;
				Name.Value = Item.Name;

				Button1.name = string.Format("{0} B1", Item.Name);
				Button1.gameObject.SetActive(Item.Button1);

				Button2.name = string.Format("{0} B2", Item.Name);
				Button2.gameObject.SetActive(Item.Button2);

				Button3.name = string.Format("{0} B3", Item.Name);
				Button3.gameObject.SetActive(Item.Button3);
			}
		}

		/// <summary>
		/// Get next button.
		/// </summary>
		/// <param name="currentButton">Current button.</param>
		/// <returns>Next button.</returns>
		public GameObject GetNextButton(int currentButton)
		{
			if (Item.Button1 && (currentButton == 0))
			{
				return Button1.gameObject;
			}

			if (Item.Button2 && (currentButton <= 1))
			{
				return Button2.gameObject;
			}

			if (Item.Button3 && (currentButton <= 2))
			{
				return Button3.gameObject;
			}

			return null;
		}

		/// <summary>
		/// Get previous button.
		/// </summary>
		/// <param name="currentButton">Current button.</param>
		/// <returns>Previous button.</returns>
		public GameObject GetPrevButton(int currentButton)
		{
			if (Item.Button3 && (currentButton > 3))
			{
				return Button3.gameObject;
			}

			if (Item.Button2 && (currentButton > 2))
			{
				return Button2.gameObject;
			}

			if (Item.Button1 && (currentButton > 1))
			{
				return Button1.gameObject;
			}

			return null;
		}

		/// <summary>
		/// Get first button.
		/// </summary>
		/// <returns>First button.</returns>
		public GameObject GetFirstButton()
		{
			if (Item.Button1)
			{
				return Button1.gameObject;
			}

			if (Item.Button2)
			{
				return Button2.gameObject;
			}

			if (Item.Button3)
			{
				return Button3.gameObject;
			}

			return null;
		}

		/// <summary>
		/// Get last button.
		/// </summary>
		/// <returns>Last button.</returns>
		public GameObject GetLastButton()
		{
			if (Item.Button3)
			{
				return Button3.gameObject;
			}

			if (Item.Button2)
			{
				return Button2.gameObject;
			}

			if (Item.Button1)
			{
				return Button1.gameObject;
			}

			return null;
		}
	}
}