namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Sample script how to add localization for ListView.
	/// </summary>
	public class LocalizationSampleListView : MonoBehaviour
	{
		[SerializeField]
		ListViewString targetListView;

		/// <summary>
		/// Target ListView.
		/// </summary>
		public ListViewString TargetListView
		{
			get
			{
				if (targetListView == null)
				{
					targetListView = GetComponent<ListViewString>();
				}

				return targetListView;
			}
		}

		/// <summary>
		/// Original DataSource.
		/// </summary>
		protected ObservableList<string> OriginalDataSource;

		/// <summary>
		/// Start this instance and adds listeners.
		/// </summary>
		protected virtual void Start()
		{
			TargetListView.Init();

			// save original data
			OriginalDataSource = TargetListView.DataSource;

			Localize();

			// Add callback on language change, if localization system support this.
			// LocalizationSystem.OnLanguageChange += Localize;
			// LocalizationSystem.OnLanguageChange.AddListener(Localize);
		}

		/// <summary>
		/// Localize.
		/// </summary>
		public void Localize()
		{
			TargetListView.DataSource = OriginalDataSource.Convert(x => GetLocalizedString(x));
		}

		static string GetLocalizedString(string str)
		{
			// return localized string
			return str;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			// Remove callback on language change, if localization system support this.
			// LocalizationSystem.OnLanguageChange -= Localize;
			// LocalizationSystem.OnLanguageChange.RemoveListener(Localize);
		}
	}
}