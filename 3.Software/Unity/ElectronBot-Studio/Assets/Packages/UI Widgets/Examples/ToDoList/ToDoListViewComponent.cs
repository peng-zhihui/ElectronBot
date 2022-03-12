namespace UIWidgets.Examples.ToDoList
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ToDoListView component.
	/// </summary>
	public class ToDoListViewComponent : ListViewItem, IViewData<ToDoListViewItem>
	{
		/// <summary>
		/// Toggle.
		/// </summary>
		[SerializeField]
		public Toggle Toggle;

		/// <summary>
		/// Task.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with TaskAdapter.")]
		public Text Task;

		/// <summary>
		/// Task.
		/// </summary>
		[SerializeField]
		public TextAdapter TaskAdapter;

		/// <summary>
		/// Item.
		/// </summary>
		[NonSerialized]
		public ToDoListViewItem Item;

		LayoutGroup layoutGroup;

		/// <summary>
		/// Current layout.
		/// </summary>
		public LayoutGroup LayoutGroup
		{
			get
			{
				if (layoutGroup == null)
				{
					layoutGroup = GetComponent<LayoutGroup>();
				}

				return layoutGroup;
			}
		}

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TaskAdapter), };
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void Start()
		{
			base.Start();
			Toggle.onValueChanged.AddListener(OnToggle);
		}

		void OnToggle(bool toggle)
		{
			Item.Done = toggle;
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ToDoListViewItem item)
		{
			Item = item;

			if (Item == null)
			{
				Toggle.isOn = false;
				TaskAdapter.text = string.Empty;
			}
			else
			{
				Toggle.isOn = Item.Done;
				TaskAdapter.text = Item.Task.Replace("\\n", "\n");
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (Toggle != null)
			{
				Toggle.onValueChanged.RemoveListener(OnToggle);
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Task, ref TaskAdapter);
#pragma warning restore 0612, 0618
		}
	}
}