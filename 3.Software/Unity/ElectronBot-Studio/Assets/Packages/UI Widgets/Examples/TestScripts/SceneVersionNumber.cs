namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Display current version number.
	/// </summary>
	public class SceneVersionNumber : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// File with version number.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("versionFile")]
		public TextAsset VersionFile;

		/// <summary>
		/// Label to display version number.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with LabelAdapter.")]
		public Text Label;

		/// <summary>
		/// Label to display version number.
		/// </summary>
		[SerializeField]
		public TextAdapter LabelAdapter;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			DisplayVersion();
		}

		/// <summary>
		/// Display current version number.
		/// </summary>
		public void DisplayVersion()
		{
			if (LabelAdapter != null)
			{
				var version = string.Format("v{0}", VersionFile.text);
				if (LabelAdapter.text != version)
				{
					LabelAdapter.text = version;
				}
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Label, ref LabelAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);

			DisplayVersion();
		}
#endif
	}
}