namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// UI cursor settings.
	/// </summary>
	[System.Obsolete("Replaced with CursorsSelector.")]
	public class UICursorSettings : MonoBehaviour
	{
		/// <summary>
		/// Default cursor.
		/// </summary>
		[SerializeField]
		protected Texture2D DefaultCursor;

		/// <summary>
		/// Default cursor hot spot.
		/// </summary>
		[SerializeField]
		protected Vector2 DefaultCursorHotSpot;

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			if (isInited)
			{
				return;
			}

			UICursor.Default = new Cursors.Cursor(DefaultCursor, DefaultCursorHotSpot);

			isInited = true;
		}
	}
}