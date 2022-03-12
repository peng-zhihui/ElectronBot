namespace UIWidgets
{
	using System;
	using System.Collections;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Node toggle type.
	/// </summary>
	public enum NodeToggle
	{
		/// <summary>
		/// Rotate.
		/// </summary>
		Rotate = 0,

		/// <summary>
		/// ChangeSprite.
		/// </summary>
		ChangeSprite = 1,
	}

	/// <summary>
	/// Tree view component base.
	/// </summary>
	/// <typeparam name="T">Type of node item.</typeparam>
	public class TreeViewComponentBase<T> : ListViewItem, IViewData<ListNode<T>>
	{
		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), };
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// The toggle.
		/// </summary>
		public TreeNodeToggle Toggle;

		Image toggleImage;

		/// <summary>
		/// Gets the toggle image.
		/// </summary>
		/// <value>The toggle image.</value>
		protected Image ToggleImage
		{
			get
			{
				if (toggleImage == null)
				{
					toggleImage = Toggle.GetComponent<Image>();
				}

				return toggleImage;
			}
		}

		/// <summary>
		/// The toggle event.
		/// </summary>
		public NodeToggleEvent ToggleEvent = new NodeToggleEvent();

		/// <summary>
		/// Indentation.
		/// </summary>
		[FormerlySerializedAs("Filler")]
		public LayoutElement Indentation;

		/// <summary>
		/// Gets or sets the indentation.
		/// </summary>
		/// <value>The indentation.</value>
		[Obsolete("Use Indentation instead.")]
		public LayoutElement Filler
		{
			get
			{
				return Indentation;
			}

			set
			{
				Indentation = value;
			}
		}

		/// <summary>
		/// The on node expand.
		/// </summary>
		public NodeToggle OnNodeExpand = NodeToggle.Rotate;

		/// <summary>
		/// Is need animate arrow?
		/// </summary>
		public bool AnimateArrow;

		/// <summary>
		/// Sprite when node opened.
		/// </summary>
		public Sprite NodeOpened;

		/// <summary>
		/// Sprite when node closed.
		/// </summary>
		public Sprite NodeClosed;

		/// <summary>
		/// The node.
		/// </summary>
		public TreeNode<T> Node
		{
			get;
			protected set;
		}

		/// <summary>
		/// The padding per level.
		/// </summary>
		public float PaddingPerLevel = 30;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (Toggle != null)
			{
				Toggle.OnClick.AddListener(ToggleNode);
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Toggle != null)
			{
				Toggle.OnClick.RemoveListener(ToggleNode);
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Toggles the node.
		/// </summary>
		protected virtual void ToggleNode()
		{
			if (AnimationCoroutine != null)
			{
				Owner.StopCoroutine(AnimationCoroutine);
			}

			SetToggle(Node.IsExpanded);

			ToggleEvent.Invoke(Index);

			if (Owner != null)
			{
				Owner.ItemsEvents.NodeToggleClick.Invoke(Index, this);
			}

			if (OnNodeExpand == NodeToggle.Rotate)
			{
				if (AnimateArrow)
				{
					AnimationCoroutine = Node.IsExpanded ? CloseCoroutine() : OpenCoroutine();
					Owner.StartCoroutine(AnimationCoroutine);
				}
			}
			else
			{
				SetToggle(Node.IsExpanded);
			}
		}

		/// <summary>
		/// Set the toggle sprite.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggleSprite(bool isExpanded)
		{
			ToggleImage.sprite = isExpanded ? NodeOpened : NodeClosed;
		}

		/// <summary>
		/// The animation coroutine.
		/// </summary>
		protected IEnumerator AnimationCoroutine;

		/// <summary>
		/// Animate arrow on open.
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator OpenCoroutine()
		{
			var rect = Toggle.transform as RectTransform;
			yield return Owner.StartCoroutine(Animations.RotateZ(rect, 0.2f, -90, 0));
		}

		/// <summary>
		/// Animate arrow on close.
		/// </summary>
		/// <returns>The coroutine.</returns>
		protected virtual IEnumerator CloseCoroutine()
		{
			var rect = Toggle.transform as RectTransform;
			yield return Owner.StartCoroutine(Animations.RotateZ(rect, 0.2f, 0, -90));
		}

		/// <summary>
		/// Sets the toggle rotation.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggleRotation(bool isExpanded)
		{
			if (Toggle == null)
			{
				return;
			}

			Toggle.transform.localRotation = Quaternion.Euler(0, 0, isExpanded ? -90 : 0);
		}

		/// <summary>
		/// Sets the toggle.
		/// </summary>
		/// <param name="isExpanded">If set to <c>true</c> is expanded.</param>
		protected virtual void SetToggle(bool isExpanded)
		{
			if (OnNodeExpand == NodeToggle.Rotate)
			{
				SetToggleRotation(isExpanded);
			}
			else
			{
				SetToggleSprite(isExpanded);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(ListNode<T> item)
		{
			SetData(item.Node, item.Depth);
		}

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public virtual void SetData(TreeNode<T> node, int depth)
		{
			if (node != null)
			{
				Node = node;
				SetToggle(Node.IsExpanded);
			}

			if (Indentation != null)
			{
				Indentation.minWidth = depth * PaddingPerLevel;
				Indentation.preferredWidth = depth * PaddingPerLevel;
				Indentation.flexibleWidth = 0;
			}

			if ((Toggle != null) && (Toggle.gameObject != null))
			{
				var toggle_active = (node != null) && (node.Nodes != null) && (node.Nodes.Count > 0);
				if (Toggle.gameObject.activeSelf != toggle_active)
				{
					Toggle.gameObject.SetActive(toggle_active);
				}
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public override void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(Text, ref TextAdapter);
#pragma warning restore 0612, 0618
		}

		/// <inheritdoc/>
		public override void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.SetStyle(styleBackground, styleText, style);

			PaddingPerLevel = style.TreeView.PaddingPerLevel;

			style.TreeView.Toggle.ApplyTo(ToggleImage);

			OnNodeExpand = style.TreeView.OnNodeExpand;

			AnimateArrow = style.TreeView.AnimateArrow;

			NodeOpened = style.TreeView.NodeOpened;

			NodeClosed = style.TreeView.NodeClosed;

			if (TextAdapter != null)
			{
				styleText.ApplyTo(TextAdapter.gameObject);
			}
		}

		/// <inheritdoc/>
		public override void GetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.GetStyle(styleBackground, styleText, style);

			style.TreeView.PaddingPerLevel = Mathf.RoundToInt(PaddingPerLevel);

			style.TreeView.Toggle.GetFrom(ToggleImage);

			style.TreeView.OnNodeExpand = OnNodeExpand;

			style.TreeView.AnimateArrow = AnimateArrow;

			style.TreeView.NodeOpened = NodeOpened;

			style.TreeView.NodeClosed = NodeClosed;

			if (TextAdapter != null)
			{
				styleText.GetFrom(TextAdapter.gameObject);
			}
		}
	}
}