namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test Paginator.
	/// </summary>
	public class TestPaginator : MonoBehaviour
	{
		/// <summary>
		/// ScrollRectPaginator.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("paginator")]
		protected ScrollRectPaginator Paginator;

		/// <summary>
		/// Test.
		/// </summary>
		public void Test()
		{
			// pages count
			Debug.Log(Paginator.Pages.ToString());

			// navigate to page
			Paginator.CurrentPage = 2;
		}
	}
}