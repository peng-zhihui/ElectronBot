namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// SimpleTable component.
	/// </summary>
	public class SimpleTableComponent : ListViewItem, IViewData<SimpleTableItem>, IUpgradeable
	{
		/// <summary>
		/// Field1.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Field1Adapter.")]
		public Text Field1;

		/// <summary>
		/// Field2.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Field2Adapter.")]
		public Text Field2;

		/// <summary>
		/// Field3.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Field3Adapter.")]
		public Text Field3;

		/// <summary>
		/// Field1.
		/// </summary>
		[SerializeField]
		public TextAdapter Field1Adapter;

		/// <summary>
		/// Field2.
		/// </summary>
		[SerializeField]
		public TextAdapter Field2Adapter;

		/// <summary>
		/// Field3.
		/// </summary>
		[SerializeField]
		public TextAdapter Field3Adapter;

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[]
				{
					UtilitiesUI.GetGraphic(Field1Adapter),
					UtilitiesUI.GetGraphic(Field2Adapter),
					UtilitiesUI.GetGraphic(Field3Adapter),
				};
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Init graphics background.
		/// </summary>
		protected override void GraphicsBackgroundInit()
		{
			if (GraphicsBackgroundVersion == 0)
			{
				graphicsBackground = Compatibility.EmptyArray<Graphic>();
				GraphicsBackgroundVersion = 1;
			}
		}

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				return new[]
				{
					Field1Adapter.transform.parent.gameObject,
					Field2Adapter.transform.parent.gameObject,
					Field3Adapter.transform.parent.gameObject,
				};
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(SimpleTableItem item)
		{
			Field1Adapter.text = item.Field1;
			Field2Adapter.text = item.Field2;
			Field3Adapter.text = item.Field3;
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Field1, ref Field1Adapter);
			Utilities.GetOrAddComponent(Field2, ref Field2Adapter);
			Utilities.GetOrAddComponent(Field3, ref Field3Adapter);
#pragma warning restore 0612, 0618
		}
	}
}