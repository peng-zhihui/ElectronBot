namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test spinner.
	/// </summary>
	public class TestSpinner : MonoBehaviour
	{
		/// <summary>
		/// The spinner.
		/// </summary>
		[SerializeField]
		public Spinner Spinner;

		/// <summary>
		/// Test this instance.
		/// </summary>
		public void Test()
		{
			Spinner.Value = 5;
		}
	}
}