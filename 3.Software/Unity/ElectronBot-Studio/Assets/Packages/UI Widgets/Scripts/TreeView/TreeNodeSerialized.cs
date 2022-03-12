namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Serialized TreeNode.
	/// </summary>
	/// <typeparam name="TItem">Type of node item.</typeparam>
	[Serializable]
	public class TreeNodeSerialized<TItem>
	{
		/// <summary>
		/// The pause observation.
		/// </summary>
		[SerializeField]
		public bool PauseObservation;

		/// <summary>
		/// Is node visible?
		/// </summary>
		[SerializeField]
		public bool IsVisible;

		/// <summary>
		/// Is node expanded?
		/// </summary>
		[SerializeField]
		public bool IsExpanded;

		/// <summary>
		/// The item.
		/// </summary>
		[SerializeField]
		public TItem Item;

		/// <summary>
		/// The sub nodes count.
		/// </summary>
		public int SubNodesCount;

		/// <summary>
		/// The index of the first sub node.
		/// </summary>
		public int FirstSubNodeIndex;

		/// <summary>
		/// The depth.
		/// </summary>
		public int Depth;

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNodeSerialized{TItem}"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="firstSubNodeIndex">Index of first subnode.</param>
		/// <param name="depth">Node depth.</param>
		public TreeNodeSerialized(TreeNode<TItem> node, int firstSubNodeIndex, int depth)
		{
			PauseObservation = node.PauseObservation;
			IsVisible = node.IsVisible;
			IsExpanded = node.IsExpanded;
			Item = node.Item;

			SubNodesCount = node.Nodes == null ? 0 : node.Nodes.Count;
			FirstSubNodeIndex = firstSubNodeIndex;
			Depth = depth;
		}
	}
}