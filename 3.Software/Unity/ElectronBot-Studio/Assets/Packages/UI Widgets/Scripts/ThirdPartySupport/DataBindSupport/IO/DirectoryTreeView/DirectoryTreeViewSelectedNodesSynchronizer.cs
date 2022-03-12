#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the SelectedNodes of a DirectoryTreeView.
	/// </summary>
	public class DirectoryTreeViewSelectedNodesSynchronizer : ComponentDataSynchronizer<UIWidgets.DirectoryTreeView, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>>>
	{
		DirectoryTreeViewSelectedNodesObserver observer;

		/// <inheritdoc />
		public override void Disable()
		{
			base.Disable();

			if (observer != null)
			{
				observer.ValueChanged -= OnObserverValueChanged;
				observer = null;
			}
		}

		/// <inheritdoc />
		public override void Enable()
		{
			base.Enable();

			var target = Target;
			if (target != null)
			{
				observer = new DirectoryTreeViewSelectedNodesObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.DirectoryTreeView target, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>> newContextValue)
		{
			target.SelectedNodes = newContextValue;
		}

		void OnObserverValueChanged()
		{
			this.OnComponentValueChanged(this.Target.SelectedNodes);
		}
	}
}
#endif