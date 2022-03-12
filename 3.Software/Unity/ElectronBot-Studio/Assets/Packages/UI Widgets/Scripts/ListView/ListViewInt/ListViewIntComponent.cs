namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewInt component.
	/// </summary>
	public class ListViewIntComponent : ListViewIntComponentBase, IViewData<int>
	{
		/// <summary>
		/// The number.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with NumberAdapter.")]
		public Text Number;

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Number, ref NumberAdapter);
#pragma warning restore 0612, 0618
		}
	}
}