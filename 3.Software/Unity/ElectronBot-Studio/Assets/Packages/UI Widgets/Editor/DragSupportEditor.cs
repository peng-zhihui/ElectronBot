#if UNITY_EDITOR
namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// DragSupport editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(BaseDragSupport), true)]
	public class DragSupportEditor : OrderedEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DragSupportEditor"/> class.
		/// </summary>
		public DragSupportEditor()
		{
			Cursors = new List<string>()
			{
				"AllowDropCursor",
				"AllowDropCursorHotSpot",
				"DeniedDropCursor",
				"DeniedDropCursorHotSpot",
				"DefaultCursorTexture",
				"DefaultCursorHotSpot",
			};
		}
	}
}
#endif