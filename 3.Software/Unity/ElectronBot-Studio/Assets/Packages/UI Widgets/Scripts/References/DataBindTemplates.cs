namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Scripts templates.
	/// </summary>
	public class DataBindTemplates : ScriptableObject
	{
#if UNITY_EDITOR
		static DataBindTemplates instance;

		/// <summary>
		/// Instance.
		/// </summary>
		public static DataBindTemplates Instance
		{
			get
			{
				if (instance == null)
				{
					instance = UtilitiesEditor.LoadAssetWithGUID<DataBindTemplates>(ReferenceGUID.DataBindTemplates);
				}

				return instance;
			}

			set
			{
				instance = value;
			}
		}
#endif

		/// <summary>
		/// Setter.
		/// </summary>
		[SerializeField]
		public TextAsset Setter;

		/// <summary>
		/// Provider.
		/// </summary>
		[SerializeField]
		public TextAsset Provider;

		/// <summary>
		/// Observer.
		/// </summary>
		[SerializeField]
		public TextAsset Observer;

		/// <summary>
		/// Synchronizer.
		/// </summary>
		[SerializeField]
		public TextAsset Synchronizer;
	}
}