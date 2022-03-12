namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test SpinnerFloat.
	/// </summary>
	public class TestSpinnerFloat : MonoBehaviour
	{
		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("spinner")]
		protected SpinnerFloat Spinner;

		/// <summary>
		/// Change culture.
		/// </summary>
		public void ChangeCulture()
		{
			#if !NETFX_CORE
			// Culture names https://msdn.microsoft.com/ru-ru/goglobal/bb896001.aspx
			Spinner.Culture = System.Globalization.CultureInfo.GetCultureInfo("ru-RU");
			#endif
		}
	}
}