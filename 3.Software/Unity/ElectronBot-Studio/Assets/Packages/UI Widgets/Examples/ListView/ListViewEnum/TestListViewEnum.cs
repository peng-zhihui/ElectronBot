namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test ListViewEnum.
	/// </summary>
	public class TestListViewEnum : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		protected ListViewEnum ListView;

		ListViewEnum<AdditionalCanvasShaderChannels> WrapperWithFlags;

		ListViewEnum<ListViewType> Wrapper;

		ListViewEnum<TestEnumObsolete> WrapperObsolete;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			ListView.OnSelectObject.AddListener(ValueChanged);
			ListView.OnDeselectObject.AddListener(ValueChanged);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected void OnDestroy()
		{
			if (ListView != null)
			{
				ListView.OnSelectObject.RemoveListener(ValueChanged);
				ListView.OnDeselectObject.RemoveListener(ValueChanged);
			}
		}

		void ValueChanged(int index)
		{
			if (WrapperWithFlags != null)
			{
				Debug.Log(string.Format("selected: {0}", EnumHelper<AdditionalCanvasShaderChannels>.ToString(WrapperWithFlags.Selected)));
			}
			else if (Wrapper != null)
			{
				Debug.Log(string.Format("selected: {0}", EnumHelper<ListViewType>.ToString(Wrapper.Selected)));
			}
			else if (WrapperObsolete != null)
			{
				Debug.Log(string.Format("selected: {0}", EnumHelper<TestEnumObsolete>.ToString(WrapperObsolete.Selected)));
			}
		}

		void DeleteWrappers()
		{
			Wrapper = null;
			WrapperWithFlags = null;
			WrapperObsolete = null;
		}

		/// <summary>
		/// Show AdditionalCanvasShaderChannels.
		/// </summary>
		public void ShowCanvasChannels()
		{
			DeleteWrappers();
			WrapperWithFlags = ListView.UseEnum<AdditionalCanvasShaderChannels>(false, x => (AdditionalCanvasShaderChannels)x);
		}

		/// <summary>
		/// Select AdditionalCanvasShaderChannels.
		/// </summary>
		public void SelectCanvasChannels()
		{
			if (WrapperWithFlags != null)
			{
				WrapperWithFlags.Selected = AdditionalCanvasShaderChannels.Normal | AdditionalCanvasShaderChannels.TexCoord1;
			}
		}

		/// <summary>
		/// Show ListViewType.
		/// </summary>
		public void ShowListType()
		{
			DeleteWrappers();
			Wrapper = ListView.UseEnum<ListViewType>(false, x => (ListViewType)x);
		}

		/// <summary>
		/// Select ListViewType.
		/// </summary>
		public void SelectListType()
		{
			if (Wrapper != null)
			{
				Wrapper.Selected = ListViewType.TileViewStaggered;
			}
		}

		/// <summary>
		/// Show TestEnumObsolete.
		/// </summary>
		public void ShowObsolete()
		{
			DeleteWrappers();
			WrapperObsolete = ListView.UseEnum<TestEnumObsolete>(true, x => (TestEnumObsolete)x);
		}
	}
}