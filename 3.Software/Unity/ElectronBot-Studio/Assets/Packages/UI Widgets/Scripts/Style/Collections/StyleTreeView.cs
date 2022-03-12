namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the TreeView.
	/// </summary>
	[Serializable]
	public class StyleTreeView : IStyleDefaultValues
	{
		/// <summary>
		/// The padding per level.
		/// </summary>
		[SerializeField]
		public int PaddingPerLevel = 30;

		/// <summary>
		/// Style for the toggle.
		/// </summary>
		[SerializeField]
		public StyleImage Toggle;

		/// <summary>
		/// The action type for toggle on node expand.
		/// </summary>
		[SerializeField]
		public NodeToggle OnNodeExpand = NodeToggle.Rotate;

		/// <summary>
		/// Is animate arrow?
		/// </summary>
		[SerializeField]
		public bool AnimateArrow = false;

		/// <summary>
		/// Sprite for expanded node.
		/// </summary>
		[SerializeField]
		public Sprite NodeOpened = null;

		/// <summary>
		/// Sprite for the collapsed node.
		/// </summary>
		[SerializeField]
		public Sprite NodeClosed = null;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Toggle.SetDefaultValues();
		}
#endif
	}
}