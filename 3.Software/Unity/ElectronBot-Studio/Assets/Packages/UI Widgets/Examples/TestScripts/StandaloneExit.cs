namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Exit option for standalone build.
	/// </summary>
	public class StandaloneExit : MonoBehaviour
	{
		#if !UNITY_STANDALONE
		/// <summary>
		/// Disable gameobject if not standalone build.
		/// </summary>
		protected virtual void Start()
		{
			gameObject.SetActive(false);
		}
		#endif

		/// <summary>
		/// Quit.
		/// </summary>
		public virtual void Quit()
		{
			Application.Quit();
		}
	}
}