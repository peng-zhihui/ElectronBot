namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewBase.
	/// You can use it for creating custom ListViews.
	/// </summary>
	public abstract partial class ListViewBase : UIBehaviour,
			ISelectHandler, IDeselectHandler,
			ISubmitHandler, ICancelHandler,
			IStylable, IUpgradeable
	{
		/// <summary>
		/// Component template.
		/// </summary>
		/// <typeparam name="TComponent">Component type.</typeparam>
		[Serializable]
		public class ListViewItemTemplate<TComponent>
			where TComponent : ListViewItem
		{
			[SerializeField]
			private TComponent template;

			/// <summary>
			/// Template.
			/// </summary>
			public TComponent Template
			{
				get
				{
					return template;
				}

				protected set
				{
					template = value;
				}
			}

			[SerializeField]
			private int id;

			/// <summary>
			/// Id.
			/// </summary>
			public int Id
			{
				get
				{
					return id;
				}

				protected set
				{
					id = value;
				}
			}

			[SerializeField]
			private List<TComponent> instances = new List<TComponent>();

			/// <summary>
			/// Instances.
			/// </summary>
			public List<TComponent> Instances
			{
				get
				{
					return instances;
				}

				protected set
				{
					instances = value;
				}
			}

			[SerializeField]
			private List<TComponent> requested = new List<TComponent>();

			/// <summary>
			/// Requested components.
			/// </summary>
			public List<TComponent> Requested
			{
				get
				{
					return requested;
				}

				protected set
				{
					requested = value;
				}
			}

			[SerializeField]
			private List<TComponent> cache = new List<TComponent>();

			/// <summary>
			/// Cache.
			/// </summary>
			public List<TComponent> Cache
			{
				get
				{
					return cache;
				}

				protected set
				{
					cache = value;
				}
			}

			/// <summary>
			/// Required instances.
			/// </summary>
			[NonSerialized]
			public int RequiredInstances;

			/// <summary>
			/// The function to call after component instantiated.
			/// </summary>
			public Action<TComponent> ComponentCreated;

			/// <summary>
			/// The function to call before component destroyed.
			/// </summary>
			public Action<TComponent> ComponentDestroyed;

			/// <summary>
			/// The function to call after component activated.
			/// </summary>
			public Action<TComponent> ComponentActivated;

			/// <summary>
			/// The function to call after component moved to cache.
			/// </summary>
			public Action<TComponent> ComponentCached;

			[SerializeField]
			private RectTransform container;

			/// <summary>
			/// Container.
			/// </summary>
			public RectTransform Container
			{
				get
				{
					return container;
				}

				protected set
				{
					container = value;
				}
			}

			[SerializeField]
			private ListViewBase owner;

			/// <summary>
			/// Owner.
			/// </summary>
			public ListViewBase Owner
			{
				get
				{
					return owner;
				}

				protected set
				{
					owner = value;
				}
			}

			private Vector2 size;

			/// <summary>
			/// Template size.
			/// </summary>
			public Vector2 Size
			{
				get
				{
					return size;
				}
			}

			/// <summary>
			/// Layout elements.
			/// </summary>
			protected static List<ILayoutElement> LayoutElements = new List<ILayoutElement>();

			/// <summary>
			/// Compare LayoutElements by layoutPriority.
			/// </summary>
			/// <param name="x">First LayoutElement.</param>
			/// <param name="y">Second LayoutElement.</param>
			/// <returns>Result of the comparison.</returns>
			protected static int LayoutElementsComparison(ILayoutElement x, ILayoutElement y)
			{
				return -x.layoutPriority.CompareTo(y.layoutPriority);
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="ListViewItemTemplate{TComponent}"/> class.
			/// </summary>
			protected ListViewItemTemplate()
			{
			}

			/// <summary>
			/// Create template.
			/// </summary>
			/// <typeparam name="TTemplate">Type of template.</typeparam>
			/// <param name="template">template.</param>
			/// <param name="componentCreated">The function to call after component instantiated.</param>
			/// <param name="componentDestroyed">The function to call before component destroyed.</param>
			/// <param name="componentActivated">The function to call after component activated.</param>
			/// <param name="componentCached">The function to call after component moved to cache.</param>
			/// <param name="container">Container.</param>
			/// <param name="owner">Owner.</param>
			/// <returns>Template.</returns>
			public static TTemplate Create<TTemplate>(
				TComponent template,
				Action<TComponent> componentCreated,
				Action<TComponent> componentDestroyed,
				Action<TComponent> componentActivated,
				Action<TComponent> componentCached,
				RectTransform container,
				ListViewBase owner)
				where TTemplate : ListViewItemTemplate<TComponent>, new()
			{
				var result = new TTemplate()
				{
					Template = template,
					Id = template.GetInstanceID(),
					ComponentCreated = componentCreated,
					ComponentDestroyed = componentDestroyed,
					ComponentActivated = componentActivated,
					ComponentCached = componentCached,
					Container = container,
					Owner = owner,
				};

				result.size = result.GetSize();

				return result;
			}

			/// <summary>
			/// Returns an enumerator that iterates through the <see cref="ListViewItemTemplate{TComponent}" />.
			/// </summary>
			/// <param name="mode">Mode.</param>
			/// <returns>A <see cref="PoolEnumerator{TComponent}" /> for the <see cref="ListViewItemTemplate{TComponent}" />.</returns>
			public PoolEnumerator<TComponent> GetEnumerator(PoolEnumeratorMode mode)
			{
				return new PoolEnumerator<TComponent>(mode, Template, Instances, Cache);
			}

			/// <summary>
			/// Update Id.
			/// </summary>
			public void UpdateId()
			{
				Id = Template.GetInstanceID();
			}

			/// <summary>
			/// Update callbacks.
			/// </summary>
			/// <param name="componentCreated">The function to call after component instantiated.</param>
			/// <param name="componentDestroyed">The function to call before component destroyed.</param>
			/// <param name="componentActivated">The function to call after component activated.</param>
			/// <param name="componentCached">The function to call after component moved to cache.</param>
			public void SetCallbacks(
				Action<TComponent> componentCreated,
				Action<TComponent> componentDestroyed,
				Action<TComponent> componentActivated,
				Action<TComponent> componentCached)
			{
				ComponentCreated = componentCreated;
				ComponentDestroyed = componentDestroyed;
				ComponentActivated = componentActivated;
				ComponentCached = componentCached;

				foreach (var instance in Cache)
				{
					ComponentCreated(instance);
				}

				foreach (var instance in Instances)
				{
					ComponentCreated(instance);
					ComponentActivated(instance);
				}
			}

			/// <summary>
			/// Get size.
			/// </summary>
			/// <returns>Size.</returns>
			protected virtual Vector2 GetSize()
			{
				Template.gameObject.SetActive(true);

				var rt = Template.transform as RectTransform;

				LayoutElements.Clear();
				Compatibility.GetComponents<ILayoutElement>(Template.gameObject, LayoutElements);
				LayoutElements.Sort(LayoutElementsComparison);

				var size = Vector2.zero;

				size.x = Mathf.Max(Mathf.Max(PreferredWidth(LayoutElements), rt.rect.width), 1f);
				if (float.IsNaN(size.x))
				{
					size.x = 1f;
				}

				size.y = Mathf.Max(Mathf.Max(PreferredHeight(LayoutElements), rt.rect.height), 1f);
				if (float.IsNaN(size.y))
				{
					size.y = 1f;
				}

				Template.gameObject.SetActive(false);

				return size;
			}

			static float PreferredHeight(List<ILayoutElement> elems)
			{
				if (elems.Count == 0)
				{
					return 0f;
				}

				var priority = elems[0].layoutPriority;
				var result = -1f;
				for (int i = 0; i < elems.Count; i++)
				{
					if ((result > -1f) && (elems[i].layoutPriority < priority))
					{
						break;
					}

					result = Mathf.Max(Mathf.Max(result, elems[i].minHeight), elems[i].preferredHeight);
					priority = elems[i].layoutPriority;
				}

				return result;
			}

			static float PreferredWidth(List<ILayoutElement> elems)
			{
				if (elems.Count == 0)
				{
					return 0f;
				}

				var priority = elems[0].layoutPriority;
				var result = -1f;
				for (int i = 0; i < elems.Count; i++)
				{
					if ((result > -1f) && (elems[i].layoutPriority < priority))
					{
						break;
					}

					result = Mathf.Max(Mathf.Max(result, elems[i].minWidth), elems[i].preferredWidth);
					priority = elems[i].layoutPriority;
				}

				return result;
			}

			/// <summary>
			/// Process locale changes.
			/// </summary>
			public virtual void LocaleChanged()
			{
				for (int i = 0; i < Instances.Count; i++)
				{
					Instances[i].LocaleChanged();
				}
			}

			/// <summary>
			/// Disable instance.
			/// </summary>
			/// <param name="instance">Instance.</param>
			protected virtual void Disable(TComponent instance)
			{
				if (instance == null)
				{
					return;
				}

				ComponentCached(instance);
				instance.MovedToCache();
				instance.Index = -1;
				instance.gameObject.SetActive(false);

				Cache.Add(instance);
			}

			/// <summary>
			/// Request instances.
			/// </summary>
			public virtual void RequestInstances()
			{
				foreach (var instance in Instances)
				{
					instance.MovedToCache();
					instance.Index = -1;
				}

				Requested.AddRange(Instances);
				Instances.Clear();

				if (Requested.Count == RequiredInstances)
				{
					return;
				}

				for (var i = Requested.Count; i < RequiredInstances; i++)
				{
					Requested.Add(Create());
				}

				// try to disable components except dragged one
				var index = Requested.Count - 1;
				while ((Requested.Count > RequiredInstances) && (index >= 0))
				{
					var component = Requested[index];
					#pragma warning disable 0618
					var disable_recycling = component.DisableRecycling || component.IsDragged;
					#pragma warning restore 0618
					if (!disable_recycling)
					{
						Disable(component);
						Requested.RemoveAt(index);
					}

					index--;
				}

				// if too much dragged components then disable any components
				index = Requested.Count - 1;
				while ((Requested.Count > RequiredInstances) && (index >= 0))
				{
					var component = Requested[index];
					Disable(component);
					Requested.RemoveAt(index);

					index--;
				}

				Requested.Sort(ComponentsComparison);
			}

			static readonly Comparison<TComponent> ComponentsComparison = (x, y) =>
			{
				if (x.Index == y.Index)
				{
					return 0;
				}

				if (x.Index == -1)
				{
					return 1;
				}

				if (y.Index == -1)
				{
					return -1;
				}

				return x.Index.CompareTo(y.Index);
			};

			/// <summary>
			/// Create instance.
			/// </summary>
			/// <returns>Instance.</returns>
			protected virtual TComponent Create()
			{
				TComponent instance;

				if (Cache.Count > 0)
				{
					instance = Cache[Cache.Count - 1];
					Cache.RemoveAt(Cache.Count - 1);
				}
				else
				{
					instance = Compatibility.Instantiate(Template, Container);
					Utilities.FixInstantiated(Template, instance);
					instance.Owner = Owner;

					instance.Index = -2;
					instance.transform.SetAsLastSibling();

					ComponentCreated(instance);
				}

				instance.gameObject.SetActive(true);
				ComponentActivated(instance);

				return instance;
			}

			/// <summary>
			/// Move component with the specified index to the Requested.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>true if component was moved; otherwise false.</returns>
			public virtual bool Require(int index)
			{
				RequiredInstances += 1;

				for (var i = 0; i < Instances.Count; i++)
				{
					var instance = Instances[i];
					if (instance.Index == index)
					{
						Requested.Add(instance);
						Instances.RemoveAt(i);

						return true;
					}
				}

				return false;
			}

			/// <summary>
			/// Request instance.
			/// </summary>
			/// <returns>Instance.</returns>
			public virtual TComponent RequestInstance()
			{
				var n = Requested.Count - 1;
				var instance = Requested[n];
				Requested.RemoveAt(n);
				Instances.Add(instance);

				return instance;
			}

			/// <summary>
			/// Request instance with the specified index.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="isNew">true if instance with the specified index was found; otherwise false.</param>
			/// <returns>Instance.</returns>
			public virtual TComponent RequestInstance(int index, out bool isNew)
			{
				for (var i = 0; i < Requested.Count; i++)
				{
					var instance = Requested[i];
					if (instance.Index < 0)
					{
						break;
					}

					if (instance.Index == index)
					{
						Requested.RemoveAt(i);
						Instances.Add(instance);

						isNew = false;
						return instance;
					}
				}

				var n = Requested.Count - 1;
				var result = Requested[n];
				Requested.RemoveAt(n);
				Instances.Add(result);

				isNew = true;
				return result;
			}

			/// <summary>
			/// Apply function for each active component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEach(Action<TComponent> action)
			{
				foreach (var instance in Instances)
				{
					action(instance);
				}
			}

			/// <summary>
			/// Apply function for each active and cached components.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachAll(Action<TComponent> action)
			{
				ForEach(action);
				ForEachCache(action);
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<TComponent> action)
			{
				action(Template);

				foreach (var c in Cache)
				{
					action(c);
				}
			}

			/// <summary>
			/// Apply function for each cached component.
			/// </summary>
			/// <param name="action">Action.</param>
			public void ForEachCache(Action<ListViewItem> action)
			{
				action(Template);

				foreach (var instance in Cache)
				{
					action(instance);
				}
			}

			/// <summary>
			/// Set size of the components.
			/// </summary>
			/// <param name="size">Size.</param>
			public void SetSize(Vector2 size)
			{
				SetSize(Template, size);

				foreach (var instance in Instances)
				{
					SetSize(instance, size);
				}

				foreach (var instance in Cache)
				{
					SetSize(instance, size);
				}
			}

			/// <summary>
			/// Set size.
			/// </summary>
			/// <param name="component">Component.</param>
			/// <param name="size">Size.</param>
			protected void SetSize(TComponent component, Vector2 size)
			{
				var item_rt = component.RectTransform;
				item_rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
				item_rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
			}

			/// <summary>
			/// Set the style.
			/// </summary>
			/// <param name="styleBackground">Style for the background.</param>
			/// <param name="styleText">Style for the text.</param>
			/// <param name="style">Full style data.</param>
			public void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
			{
				Template.SetStyle(styleBackground, styleText, style);

				foreach (var instance in Instances)
				{
					instance.SetStyle(styleBackground, styleText, style);
				}

				foreach (var instance in Cache)
				{
					instance.SetStyle(styleBackground, styleText, style);
				}
			}

			/// <summary>
			/// Destroy all instances.
			/// </summary>
			public virtual void Destroy()
			{
				foreach (var i in Instances)
				{
					Disable(i);
				}

				Instances.Clear();

				foreach (var r in Requested)
				{
					Disable(r);
				}

				Requested.Clear();

				foreach (var c in Cache)
				{
					Destroy(c);
				}

				Cache.Clear();
			}

			/// <summary>
			/// Enable Template.
			/// </summary>
			public virtual void EnableTemplate()
			{
				if (!Template.gameObject.activeSelf)
				{
					Template.gameObject.SetActive(true);
				}
			}

			/// <summary>
			/// Disable Template.
			/// </summary>
			public virtual void DisableTemplate()
			{
				if (template != null)
				{
					template.gameObject.SetActive(false);
				}
			}

			/// <summary>
			/// Destroy the instance.
			/// </summary>
			/// <param name="instance">Instance.</param>
			protected void Destroy(TComponent instance)
			{
				ComponentDestroyed(instance);

				if (Application.isPlaying)
				{
					UnityEngine.Object.Destroy(instance.gameObject);
				}
#if UNITY_EDITOR
				else
				{
					UnityEngine.Object.DestroyImmediate(instance.gameObject);
				}
#endif
			}
		}
	}
}