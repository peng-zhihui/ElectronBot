namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIcons component with Toggle.
	/// </summary>
	public class ListViewIconsToggleComponent : ListViewIconsItemComponent
	{
		/// <summary>
		/// Toggle.
		/// </summary>
		[SerializeField]
		public Toggle Toggle;

		bool isInited;

		/// <inheritdoc/>
		public override void SetData(ListViewIconsItemDescription item)
		{
			base.SetData(item);

			UpdateToggle();
		}

		void UpdateToggle(int index, ListViewItem item)
		{
			UpdateToggle();
		}

		bool pauseUpdateToggle;

		void UpdateToggle()
		{
			if (pauseUpdateToggle)
			{
				return;
			}

			if ((Toggle != null) && (Owner != null))
			{
				Toggle.isOn = Owner.IsSelected(Index);
			}
		}

		void SelectItem(bool isOn)
		{
			if (Owner == null)
			{
				return;
			}

			pauseUpdateToggle = true;

			if (isOn)
			{
				Owner.Select(Index);
			}
			else
			{
				Owner.Deselect(Index);
			}

			pauseUpdateToggle = false;
		}

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			Init();
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;
			ToggleOnClick = false;
			ToggleOnSubmit = false;

			if (Toggle != null)
			{
				Toggle.onValueChanged.AddListener(SelectItem);
			}

			if (Owner != null)
			{
				Owner.OnSelect.AddListener(UpdateToggle);
				Owner.OnDeselect.AddListener(UpdateToggle);
			}
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		protected override void OnDestroy()
		{
			if (Toggle != null)
			{
				Toggle.onValueChanged.RemoveListener(SelectItem);
			}

			if (Owner != null)
			{
				Owner.OnSelect.RemoveListener(UpdateToggle);
				Owner.OnDeselect.RemoveListener(UpdateToggle);
			}

			base.OnDestroy();
		}
	}
}