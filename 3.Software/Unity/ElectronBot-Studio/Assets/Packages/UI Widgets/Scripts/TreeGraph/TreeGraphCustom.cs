namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UIWidgets.Attributes;
	using UIWidgets.Extensions;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeGraph directions.
	/// </summary>
	public enum TreeGraphDirections
	{
		/// <summary>
		/// Top to bottom.
		/// </summary>
		TopToBottom = 0,

		/// <summary>
		/// Bottom to top
		/// </summary>
		BottomToTop = 1,

		/// <summary>
		/// Left to right.
		/// </summary>
		LeftToRight = 2,

		/// <summary>
		/// Right to left.
		/// </summary>
		RightToLeft = 3,
	}

	/// <summary>
	/// Base class for TreeGraph's.
	/// </summary>
	/// <typeparam name="TItem">Item type.</typeparam>
	/// <typeparam name="TComponent">Component type.</typeparam>
	[DataBindSupport]
	public class TreeGraphCustom<TItem, TComponent> : MonoBehaviour, IStylable, IUpdatable
		where TComponent : TreeGraphComponent<TItem>
	{
		/// <summary>
		/// Instance.
		/// </summary>
		[Serializable]
		protected class Instance
		{
			/// <summary>
			/// Node.
			/// </summary>
			public TreeNode<TItem> Node
			{
				get;
				protected set;
			}

			/// <summary>
			/// View.
			/// </summary>
			[SerializeField]
			protected TComponent view;

			/// <summary>
			/// View.
			/// </summary>
			public TComponent View
			{
				get
				{
					return view;
				}
			}

			/// <summary>
			/// View.RectTransform.
			/// </summary>
			[SerializeField]
			protected RectTransform rectTransform;

			/// <summary>
			/// View.RectTransform.
			/// </summary>
			public RectTransform RectTransform
			{
				get
				{
					return rectTransform;
				}
			}

			/// <summary>
			/// Connector.
			/// </summary>
			[SerializeField]
			protected MultipleConnector connector;

			/// <summary>
			/// Connector.
			/// </summary>
			public MultipleConnector Connector
			{
				get
				{
					return connector;
				}
			}

			/// <summary>
			/// Size of the View.RectTransform.
			/// </summary>
			public Vector2 Size;

			/// <summary>
			/// Set data.
			/// </summary>
			/// <param name="node">Node.</param>
			public void SetData(TreeNode<TItem> node)
			{
				Node = node;

				view.gameObject.SetActive(true);
				view.SetData(Node);
			}

			/// <summary>
			/// Set view.
			/// </summary>
			/// <param name="newView">New view.</param>
			public void SetView(TComponent newView)
			{
				view = newView;
				rectTransform = view.transform as RectTransform;
				connector = view.GetComponent<MultipleConnector>();
			}

			/// <summary>
			/// Set position.
			/// </summary>
			/// <param name="isHorizontal">Is horizontal direction.</param>
			/// <param name="position">Position.</param>
			public void SetPosition(bool isHorizontal, Vector2 position)
			{
				rectTransform.anchoredPosition = isHorizontal
					? new Vector2(position.x, -position.y - (Size.y / 2f))
					: new Vector2(position.x + (Size.x / 2f), -position.y);
			}

			/// <summary>
			/// Disable.
			/// </summary>
			public void Disable()
			{
				if (connector)
				{
					connector.Lines.Clear();
				}

				if (view != null)
				{
					view.MovedToCache();
					view.gameObject.SetActive(false);
				}
			}
		}

		/// <summary>
		/// Instances list.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Instance> InstancesList = new List<Instance>();

		/// <summary>
		/// Instances.
		/// </summary>
		protected Dictionary<TreeNode<TItem>, Instance> Instances = new Dictionary<TreeNode<TItem>, Instance>();

		[SerializeField]
		[HideInInspector]
		ObservableList<TreeNode<TItem>> nodes = new ObservableList<TreeNode<TItem>>();

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		[DataBindField]
		public virtual ObservableList<TreeNode<TItem>> Nodes
		{
			get
			{
				return nodes;
			}

			set
			{
				if (nodes != null)
				{
					nodes.OnChange -= NodesChanged;

#pragma warning disable 0219
					var temp = new TreeNode<TItem>(default(TItem))
					{
						Nodes = nodes,
					};
#pragma warning restore 0219
				}

				nodes = value;
				Refresh();
				if (nodes != null)
				{
					nodes.OnChange += NodesChanged;
				}
			}
		}

		[SerializeField]
		TreeGraphDirections direction;

		/// <summary>
		/// Direction.
		/// </summary>
		public TreeGraphDirections Direction
		{
			get
			{
				return direction;
			}

			set
			{
				if (direction != value)
				{
					direction = value;

					Refresh();
				}
			}
		}

		[SerializeField]
		TComponent defaultItem;

		/// <summary>
		/// Default item component.
		/// </summary>
		public TComponent DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				if (defaultItem != value)
				{
					defaultItem = value;

					if (isInited)
					{
						defaultItem.gameObject.SetActive(false);

						var rt = defaultItem.transform as RectTransform;
						rt.anchorMin = Vector2.zero;
						rt.anchorMax = Vector2.zero;
						rt.pivot = new Vector2(0.5f, 0.5f);

						ResetInstances();

						foreach (var c in Cache)
						{
							DestroyGameObject(c);
						}

						Cache.Clear();

						ComponentSize = GetComponentSize(value);

						Refresh();
					}
				}
			}
		}

		[SerializeField]
		RectTransform container;

		/// <summary>
		/// Container.
		/// </summary>
		public RectTransform Container
		{
			get
			{
				return container;
			}

			set
			{
				if (container != value)
				{
					ChangeContainer(value);

					ResetInstances();

					foreach (var c in Cache)
					{
						DestroyGameObject(c);
					}

					Cache.Clear();

					Refresh();
				}
			}
		}

		/// <summary>
		/// Container LayoutElement.
		/// </summary>
		protected LayoutElement ContainerLayoutElement;

		[SerializeField]
		[Tooltip("Empty space between nodes.")]
		Vector2 spacing;

		/// <summary>
		/// Spacing.
		/// </summary>
		public Vector2 Spacing
		{
			get
			{
				return spacing;
			}

			set
			{
				if (spacing != value)
				{
					spacing = value;

					Refresh();
				}
			}
		}

		[SerializeField]
		ConnectorType lineType = ConnectorType.Rectangular;

		/// <summary>
		/// Line type.
		/// </summary>
		public ConnectorType LineType
		{
			get
			{
				return lineType;
			}

			set
			{
				if (lineType != value)
				{
					lineType = value;

					UpdateLines();
				}
			}
		}

		[SerializeField]
		float lineThickness = 1f;

		/// <summary>
		/// Line thickness.
		/// </summary>
		public float LineThickness
		{
			get
			{
				return lineThickness;
			}

			set
			{
				if (lineThickness != value)
				{
					lineThickness = value;

					UpdateLines();
				}
			}
		}

		[SerializeField]
		float lineMargin = 10f;

		/// <summary>
		/// Line margin.
		/// </summary>
		public float LineMargin
		{
			get
			{
				return lineMargin;
			}

			set
			{
				if (lineMargin != value)
				{
					lineMargin = value;

					UpdateLines();
				}
			}
		}

		/// <summary>
		/// Component size.
		/// </summary>
		protected Vector2 ComponentSize = Vector2.zero;

		/// <summary>
		/// Component cache.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<Instance> Cache = new List<Instance>();

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Start()
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

			isInited = true;

			ChangeContainer(container);

			var rt = DefaultItem.transform as RectTransform;
			rt.anchorMin = Vector2.zero;
			rt.anchorMax = Vector2.zero;
			rt.pivot = new Vector2(0.5f, 0.5f);

			DefaultItem.gameObject.SetActive(false);

			ComponentSize = GetComponentSize(defaultItem);

			Refresh();

			Localization.OnLocaleChanged += LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			foreach (var instance in InstancesList)
			{
				instance.View.LocaleChanged();
			}
		}

		/// <summary>
		/// Change container.
		/// </summary>
		/// <param name="value">New container.</param>
		protected virtual void ChangeContainer(RectTransform value)
		{
			if (value == null)
			{
				value = transform as RectTransform;
			}

			if (container != null)
			{
				var resize_listener = container.GetComponent<ResizeListener>();
				if (resize_listener != null)
				{
					resize_listener.OnResizeNextFrame.RemoveListener(SizeChanged);
				}
			}

			container = value;

			if (container != null)
			{
				var resize_listener = Utilities.GetOrAddComponent<ResizeListener>(container);
				resize_listener.OnResizeNextFrame.AddListener(SizeChanged);

				ContainerLayoutElement = Utilities.GetOrAddComponent<LayoutElement>(container);
			}
		}

		/// <summary>
		/// Get component size.
		/// </summary>
		/// <param name="defaultItemComponent">Default item component.</param>
		/// <returns>Component size.</returns>
		protected virtual Vector2 GetComponentSize(TComponent defaultItemComponent)
		{
			LayoutRebuilder.ForceRebuildLayoutImmediate(defaultItemComponent.RectTransform);

			return defaultItemComponent.RectTransform.rect.size;
		}

		/// <summary>
		/// Is container size changed?
		/// </summary>
		protected bool ContainerSizeChanged;

		/// <summary>
		/// Container size.
		/// </summary>
		protected Vector2 ContainerSize;

		/// <summary>
		/// Handle container size changed event?
		/// </summary>
		protected virtual void SizeChanged()
		{
			if (!ContainerSizeChanged && (ContainerSize != Container.rect.size))
			{
				ContainerSizeChanged = true;
				Updater.RunOnceNextFrame(this);
			}
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			ContainerSizeChanged = false;
			SetPositions(Nodes, GetStartPosition());
		}

		/// <summary>
		/// Handle nodes changed event.
		/// </summary>
		protected virtual void NodesChanged()
		{
			Refresh();
		}

		/// <summary>
		/// Refresh displayed nodes.
		/// </summary>
		public virtual void Refresh()
		{
			if (!isInited)
			{
				return;
			}

			if (Nodes == null)
			{
				return;
			}

			ResetInstances();

			var size = GetNodesSize(Nodes);
			ContainerLayoutElement.minWidth = size.x;
			ContainerLayoutElement.minHeight = size.y;

			DisplayNodes(Nodes);
		}

		/// <summary>
		/// Display nodes.
		/// </summary>
		/// <param name="nodesToDisplay">Nodes to display.</param>
		protected virtual void DisplayNodes(ObservableList<TreeNode<TItem>> nodesToDisplay)
		{
			if (nodesToDisplay == null)
			{
				return;
			}

			CreateInstances(nodesToDisplay);
			SetPositions(nodesToDisplay, GetStartPosition());
		}

		/// <summary>
		/// Set positions.
		/// </summary>
		/// <param name="targetNodes">Nodes.</param>
		/// <param name="position">Base position.</param>
		protected virtual void SetPositions(ObservableList<TreeNode<TItem>> targetNodes, Vector2 position)
		{
			if (targetNodes == null)
			{
				return;
			}

			foreach (var node in targetNodes)
			{
				var instance = Instances[node];
				SetPosition(instance, position);
				position = GetNextPosition(position, instance.Size);
			}
		}

		/// <summary>
		/// Get start position.
		/// </summary>
		/// <returns>Start position.</returns>
		protected virtual Vector2 GetStartPosition()
		{
			var size = Container.rect.size;
			switch (Direction)
			{
				case TreeGraphDirections.TopToBottom:
					return new Vector2(0, -size.y + (ComponentSize.y / 2f));
				case TreeGraphDirections.BottomToTop:
					return new Vector2(0, -ComponentSize.y / 2f);
				case TreeGraphDirections.LeftToRight:
					return new Vector2(ComponentSize.x / 2f, -size.y);
				case TreeGraphDirections.RightToLeft:
					return new Vector2(size.x - (ComponentSize.x / 2f), -size.y);
				default:
#if NETFX_CORE
					throw new ArgumentException("Unsupported direction: " + Direction);
#else
					throw new InvalidEnumArgumentException(string.Format("Unsupported direction: {0}", EnumHelper<TreeGraphDirections>.ToString(Direction)));
#endif
			}
		}

		/// <summary>
		/// Get next position.
		/// </summary>
		/// <param name="position">Base position.</param>
		/// <param name="size">Size.</param>
		/// <returns>Next position.</returns>
		protected virtual Vector2 GetNextPosition(Vector2 position, Vector2 size)
		{
			return IsHorizontal()
				? new Vector2(position.x, position.y + size.y + spacing.y)
				: new Vector2(position.x + size.x + spacing.x, position.y);
		}

		/// <summary>
		/// Get next level position.
		/// </summary>
		/// <param name="position">Base position.</param>
		/// <returns>Next level position.</returns>
		protected virtual Vector2 GetNextLevelPosition(Vector2 position)
		{
			var delta = IsHorizontal() ? ComponentSize.x + spacing.x : ComponentSize.y + spacing.y;
			switch (Direction)
			{
				case TreeGraphDirections.TopToBottom:
					return new Vector2(position.x, position.y + delta);
				case TreeGraphDirections.BottomToTop:
					return new Vector2(position.x, position.y - delta);
				case TreeGraphDirections.LeftToRight:
					return new Vector2(position.x + delta, position.y);
				case TreeGraphDirections.RightToLeft:
					return new Vector2(position.x - delta, position.y);
				default:
#if NETFX_CORE
					throw new ArgumentException("Unsupported direction: " + Direction);
#else
					throw new InvalidEnumArgumentException(string.Format("Unsupported direction: {0}", EnumHelper<TreeGraphDirections>.ToString(Direction)));
#endif
			}
		}

		/// <summary>
		/// Display nodes.
		/// </summary>
		/// <param name="nodesToDisplay">Nodes to display.</param>
		/// <param name="connector">Connector.</param>
		protected virtual void CreateInstances(ObservableList<TreeNode<TItem>> nodesToDisplay, MultipleConnector connector = null)
		{
			if (nodesToDisplay == null)
			{
				return;
			}

			foreach (var node in nodesToDisplay)
			{
				if (!node.IsVisible)
				{
					continue;
				}

				var instance = CreateInstance(node, connector);
				Instances.Add(node, instance);
				InstancesList.Add(instance);
			}
		}

		/// <summary>
		/// Set position.
		/// </summary>
		/// <param name="instance">Instance.</param>
		/// <param name="position">Position.</param>
		protected virtual void SetPosition(Instance instance, Vector2 position)
		{
			instance.Size = GetNodeSize(instance.Node);
			instance.SetPosition(IsHorizontal(), position);

			if (instance.Node.IsExpanded)
			{
				var new_position = GetNextLevelPosition(position);
				SetPositions(instance.Node.Nodes, new_position);
			}
		}

		/// <summary>
		/// Display node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="connector">Connector.</param>
		/// <returns>Node size.</returns>
		protected virtual Instance CreateInstance(TreeNode<TItem> node, MultipleConnector connector)
		{
			var instance = GetInstance();
			instance.SetData(node);

			if (connector != null)
			{
				ConnectorPosition start;
				ConnectorPosition end;

				switch (Direction)
				{
					case TreeGraphDirections.TopToBottom:
						start = ConnectorPosition.Bottom;
						end = ConnectorPosition.Top;
						break;
					case TreeGraphDirections.BottomToTop:
						start = ConnectorPosition.Top;
						end = ConnectorPosition.Bottom;
						break;
					case TreeGraphDirections.LeftToRight:
						start = ConnectorPosition.Right;
						end = ConnectorPosition.Left;
						break;
					case TreeGraphDirections.RightToLeft:
						start = ConnectorPosition.Left;
						end = ConnectorPosition.Right;
						break;
					default:
#if NETFX_CORE
						throw new ArgumentException(string.Format("Unsupported direction: {0}", Direction.ToString()));
#else
						throw new InvalidEnumArgumentException(string.Format("Unsupported direction: {0}", EnumHelper<TreeGraphDirections>.ToString(Direction)));
#endif
				}

				var line = new ConnectorLine()
				{
					Target = instance.RectTransform,
					Start = start,
					End = end,
					Thickness = LineThickness,
					Type = LineType,
					Margin = LineMargin,
				};
				connector.Lines.Add(line);
			}

			if (node.IsExpanded)
			{
				CreateInstances(node.Nodes, instance.Connector);
			}

			return instance;
		}

		/// <summary>
		/// Update lines.
		/// </summary>
		protected virtual void UpdateLines()
		{
			foreach (var instance in InstancesList)
			{
				if (instance.Connector == null)
				{
					continue;
				}

				var lines = instance.Connector.Lines;
				lines.BeginUpdate();

				for (int i = 0; i < lines.Count; i++)
				{
					var line = lines[i];
					line.Thickness = LineThickness;
					line.Type = LineType;
					line.Margin = LineMargin;
				}

				lines.EndUpdate();
			}
		}

		/// <summary>
		/// Get new component instance.
		/// </summary>
		/// <returns>Component instance.</returns>
		protected Instance GetInstance()
		{
			Instance instance;
			if (Cache.Count > 0)
			{
				instance = Cache.Pop();
			}
			else
			{
				var component = Compatibility.Instantiate(defaultItem);
				component.transform.SetParent(container, false);
				Utilities.FixInstantiated(defaultItem, component);

				instance = new Instance();
				instance.SetView(component);
			}

			return instance;
		}

		/// <summary>
		/// Reset instances.
		/// </summary>
		protected void ResetInstances()
		{
			foreach (var i in InstancesList)
			{
				ResetInstance(i);
			}

			InstancesList.Clear();
			Instances.Clear();
		}

		/// <summary>
		/// Reset instance.
		/// </summary>
		/// <param name="instance">Instance.</param>
		protected void ResetInstance(Instance instance)
		{
			instance.Disable();
			Cache.Add(instance);
		}

		/// <summary>
		/// Get nodes size.
		/// </summary>
		/// <param name="displayNodes">Nodes.</param>
		/// <returns>Nodes size.</returns>
		protected virtual Vector2 GetNodesSize(ObservableList<TreeNode<TItem>> displayNodes)
		{
			var result = Vector2.zero;
			if (displayNodes == null)
			{
				return result;
			}

			foreach (var node in displayNodes)
			{
				if (!node.IsVisible)
				{
					continue;
				}

				var size = GetNodeSize(node);
				if (IsHorizontal())
				{
					result.y += size.y + spacing.y;
					result.x = Mathf.Max(result.x, size.x);
				}
				else
				{
					result.x += size.x + spacing.x;
					result.y = Mathf.Max(result.y, size.y);
				}
			}

			if (result != Vector2.zero)
			{
				if (IsHorizontal())
				{
					result.y -= spacing.y;
				}
				else
				{
					result.x -= spacing.x;
				}
			}

			return result;
		}

		/// <summary>
		/// Get node size.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Node size.</returns>
		protected virtual Vector2 GetNodeSize(TreeNode<TItem> node)
		{
			var result = Vector2.zero;
			if (node.IsExpanded && (node.Nodes != null))
			{
				foreach (var subnode in node.Nodes)
				{
					if (!subnode.IsVisible)
					{
						continue;
					}

					var subsize = GetNodeSize(subnode);
					if (IsHorizontal())
					{
						result.y += subsize.y + spacing.y;
						result.x = Mathf.Max(result.x, subsize.x);
					}
					else
					{
						result.x += subsize.x + spacing.x;
						result.y = Mathf.Max(result.y, subsize.y);
					}
				}
			}

			if (result == Vector2.zero)
			{
				return ComponentSize;
			}
			else
			{
				if (IsHorizontal())
				{
					result.y -= spacing.y;
					result.x += ComponentSize.x + spacing.x;
				}
				else
				{
					result.x -= spacing.x;
					result.y += ComponentSize.y + spacing.y;
				}
			}

			return result;
		}

		/// <summary>
		/// Is direction is horizontal?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise, false.</returns>
		protected bool IsHorizontal()
		{
			return Direction == TreeGraphDirections.LeftToRight || Direction == TreeGraphDirections.RightToLeft;
		}

		/// <summary>
		/// Destroy gameobject of the specified component.
		/// </summary>
		/// <param name="instance">Instance.</param>
		protected void DestroyGameObject(Instance instance)
		{
			Destroy(instance.View.gameObject);
		}

		/// <summary>
		/// Remove nodes parent.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		protected void RemoveNodes(ObservableList<TreeNode<TItem>> nodes)
		{
			if (nodes == null)
			{
				return;
			}

			nodes.OnChange -= NodesChanged;
			for (int i = nodes.Count - 1; i >= 0; i--)
			{
				nodes[i].Parent = null;
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Localization.OnLocaleChanged -= LocaleChanged;

			RemoveNodes(nodes);
			nodes = null;
		}

		#region IStylable implementation

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool SetStyle(Style style)
		{
			if (defaultItem != null)
			{
				defaultItem.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
			}

			if (InstancesList != null)
			{
				foreach (var instance in InstancesList)
				{
					instance.View.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
				}
			}

			if (Cache != null)
			{
				foreach (var instance in Cache)
				{
					instance.View.SetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
				}
			}

			return true;
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <returns><c>true</c>, if style was set for children gameobjects, <c>false</c> otherwise.</returns>
		/// <param name="style">Style data.</param>
		public virtual bool GetStyle(Style style)
		{
			if (defaultItem != null)
			{
				defaultItem.GetStyle(style.Collections.DefaultItemBackground, style.Collections.DefaultItemText, style);
			}

			return true;
		}
		#endregion
	}
}