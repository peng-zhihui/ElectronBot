#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Synchronizers;
	using UnityEngine;

	/// <summary>
	/// Synchronizer for the SelectedNodes of a TreeView.
	/// </summary>
	public class TreeViewSelectedNodesSynchronizer : ComponentDataSynchronizer<UIWidgets.TreeView, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.TreeViewItem>>>
	{
		TreeViewSelectedNodesObserver observer;

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
				observer = new TreeViewSelectedNodesObserver
				{
					Target = target,
				};
				observer.ValueChanged += OnObserverValueChanged;
			}
		}

		/// <inheritdoc />
		protected override void SetTargetValue(UIWidgets.TreeView target, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.TreeViewItem>> newContextValue)
		{
			target.SelectedNodes = newContextValue;
		}

		void OnObserverValueChanged()
		{
			OnComponentValueChanged(Target.SelectedNodes);
		}
	}
}
#endif