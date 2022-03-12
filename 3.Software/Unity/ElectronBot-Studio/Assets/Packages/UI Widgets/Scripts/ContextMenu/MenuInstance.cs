namespace UIWidgets.Menu
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Context menu.
	/// Contains menu items and reference to the menu template.
	/// </summary>
	public partial class ContextMenu : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// Menu instance.
		/// </summary>
		protected class MenuInstance : IMenuSubscriber, IStylable
		{
			ContextMenuView template;

			/// <summary>
			/// Template.
			/// </summary>
			public ContextMenuView Template
			{
				get
				{
					return template;
				}

				set
				{
					if (value == null)
					{
						throw new ArgumentNullException("value");
					}

					if (template == value)
					{
						return;
					}

					if (template != null)
					{
						ResetMenu();

						Template.Unsubscribe(this);
					}

					template = value;

					ResetMenu();
					Template.Subscribe(this);

					if (IsOpened)
					{
						UpdateItems();
					}

					if (submenu != null)
					{
						submenu.Template = Template;
					}
				}
			}

			/// <summary>
			/// Preferred menu position.
			/// </summary>
			protected Vector2 Position;

			/// <summary>
			/// Menu items.
			/// </summary>
			protected ReadOnlyCollection<MenuItem> Items;

			/// <summary>
			/// Owner.
			/// </summary>
			protected ContextMenu Owner;

			/// <summary>
			/// The modal key.
			/// </summary>
			protected int? ModalKey;

			/// <summary>
			/// Is menu opened?
			/// </summary>
			public bool IsOpened
			{
				get;
				protected set;
			}

			/// <summary>
			/// Menu gameobject.
			/// </summary>
			protected ContextMenuView View;

			/// <summary>
			/// Menu items view.
			/// </summary>
			protected List<MenuItemView> ItemViews = new List<MenuItemView>();

			/// <summary>
			/// Is pointer over view?
			/// </summary>
			protected bool PointerOver
			{
				get
				{
					return (View != null) ? View.IsPointerOver : false;
				}
			}

			MenuInstance parent;

			MenuInstance submenu;

			int submenuItemIndex;

			int depth = 0;

			Coroutine openCoroutine;

			Coroutine closeCoroutine;

			RectTransform parentCanvas;

			/// <summary>
			/// Parent canvas.
			/// </summary>
			public RectTransform ParentCanvas
			{
				get
				{
					return parentCanvas;
				}

				set
				{
					parentCanvas = value;

					if (submenu != null)
					{
						submenu.ParentCanvas = value;
					}
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="MenuInstance"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="template">Template.</param>
			/// <param name="items">Items.</param>
			/// <param name="parentCanvas">Parent canvas.</param>
			public MenuInstance(ContextMenu owner, ContextMenuView template, ReadOnlyCollection<MenuItem> items, RectTransform parentCanvas)
			{
				Owner = owner;
				Template = template;
				Items = items;
				ParentCanvas = parentCanvas;
			}

			/// <summary>
			/// Process locale changes.
			/// </summary>
			public virtual void LocaleChanged()
			{
				for (int i = 0; i < ItemViews.Count; i++)
				{
					ItemViews[i].LocaleChanged();
				}

				if (submenu != null)
				{
					submenu.LocaleChanged();
				}
			}

			/// <summary>
			/// Update menu items.
			/// </summary>
			/// <param name="items">Menu items.</param>
			public void UpdateItems(ReadOnlyCollection<MenuItem> items)
			{
				ResetItems();

				Items = items;

				UpdateItems();
			}

			/// <summary>
			/// Update menu items.
			/// </summary>
			public void UpdateItems()
			{
				if (!IsOpened)
				{
					return;
				}

				ResetItems();

				for (int index = 0; index < Items.Count; index++)
				{
					var item = Items[index];
					if (!item.Visible)
					{
						continue;
					}

					var view = Template.GetItemView(item);

					view.transform.SetParent(View.RectTransform, false);
					AddListeners(view);
					SetData(view, index);

					ItemViews.Add(view);
				}

				View.RectTransform.SetParent(ParentCanvas, false);
				View.RectTransform.SetAsLastSibling();
				View.RectTransform.anchorMin = new Vector2(0, 1);
				View.RectTransform.anchorMax = new Vector2(0, 1);
				View.RectTransform.localScale = Vector3.one;
				View.RectTransform.localPosition = Vector3.zero;

				LayoutRebuilder.ForceRebuildLayoutImmediate(View.RectTransform);

				var canvas_size = ParentCanvas.rect.size;
				var width = LayoutUtility.GetPreferredWidth(View.RectTransform);
				var height = LayoutUtility.GetPreferredHeight(View.RectTransform);

				if ((Position.x + width) > canvas_size.x)
				{
					Position.x = parent != null
						? Position.x - parent.View.RectTransform.rect.width - width
						: canvas_size.x - width;
				}

				if ((-(Position.y - height)) > canvas_size.y)
				{
					Position.y = Position.y + height;
				}

				View.RectTransform.anchoredPosition = Position;
			}

			/// <summary>
			/// Open menu.
			/// </summary>
			/// <param name="position">Preferred position.</param>
			public void Open(Vector2 position)
			{
				if (IsOpened)
				{
					return;
				}

				IsOpened = true;
				Position = position;

				if (parent == null)
				{
					ModalKey = ModalHelper.Open(Owner.RectTransform, null, Color.clear, Close, ParentCanvas);
				}

				CreateView();

				UpdateItems();

				if (depth == 0)
				{
					Owner.OnOpen.Invoke(Owner);
				}
			}

			/// <summary>
			/// Fully close menu.
			/// </summary>
			protected void AllClose()
			{
				if (parent != null)
				{
					parent.AllClose();
				}
				else
				{
					Close();
				}
			}

			/// <summary>
			/// Close.
			/// </summary>
			public void Close()
			{
				if (!IsOpened)
				{
					return;
				}

				if (ModalKey.HasValue)
				{
					ModalHelper.Close(ModalKey.Value);
				}

				if ((submenu != null) && submenu.IsOpened)
				{
					submenu.Close();
				}

				ResetMenu();

				IsOpened = false;

				if (depth == 0)
				{
					Owner.OnClose.Invoke(Owner);
				}
			}

			/// <summary>
			/// Add listeners to the MenuItem view.
			/// </summary>
			/// <param name="view">MenuItem view.</param>
			protected virtual void AddListeners(MenuItemView view)
			{
				view.OnClick.AddListener(ItemClick);
				view.OnEnter.AddListener(ItemEnter);
				view.OnExit.AddListener(ItemExit);
				view.OnAxisMove.AddListener(ItemMove);
			}

			/// <summary>
			/// Remove listeners from the MenuItem view.
			/// </summary>
			/// <param name="view">MenuItem view.</param>
			protected virtual void RemoveListeners(MenuItemView view)
			{
				view.OnClick.RemoveListener(ItemClick);
				view.OnEnter.RemoveListener(ItemEnter);
				view.OnExit.RemoveListener(ItemExit);
				view.OnAxisMove.RemoveListener(ItemMove);
			}

			/// <summary>
			/// Set data.
			/// </summary>
			/// <param name="view">MenuItem view.</param>
			/// <param name="index">MenuItem index.</param>
			protected virtual void SetData(MenuItemView view, int index)
			{
				view.Index = index;
				view.Item = Items[index];
			}

			/// <summary>
			/// Reset menu.
			/// </summary>
			protected void ResetMenu()
			{
				ResetItems();

				if (View != null)
				{
					Template.Return(View);
					View = null;
				}
			}

			/// <summary>
			/// Reset menu items.
			/// </summary>
			public void ResetItems()
			{
				foreach (var view in ItemViews)
				{
					RemoveListeners(view);

					if (!Items[view.Index].Visible)
					{
						continue;
					}

					Template.Return(Items[view.Index], view);
				}

				ItemViews.Clear();
			}

			/// <summary>
			/// Process the item click event.
			/// </summary>
			/// <param name="item">Item.</param>
			protected virtual void ItemClick(MenuItem item)
			{
				if (item.Interactable)
				{
					item.Action.Invoke(item);

					if (!item.HasVisibleItems)
					{
						AllClose();
					}
				}
			}

			/// <summary>
			/// Process the item enter event.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="pointerEvent">Pointer event.</param>
			protected virtual void ItemEnter(int index, bool pointerEvent)
			{
				var item = Items[index];
				Owner.OnItemSelect.Invoke(item);

				if (submenu != null)
				{
					StartCloseSubmenu();
				}

				if (item.Interactable && item.HasVisibleItems && pointerEvent)
				{
					StartOpenSubmenu(index);
				}
			}

			/// <summary>
			/// Start open sub-menu.
			/// </summary>
			/// <param name="index">Index.</param>
			protected void StartOpenSubmenu(int index)
			{
				if (openCoroutine != null)
				{
					Owner.StopCoroutine(openCoroutine);
				}

				openCoroutine = Owner.StartCoroutine(OpenSubmenuCoroutine(index));
			}

			/// <summary>
			/// Start close sub-menu coroutine.
			/// </summary>
			protected void StartCloseSubmenu()
			{
				if (closeCoroutine != null)
				{
					Owner.StopCoroutine(closeCoroutine);
				}

				closeCoroutine = Owner.StartCoroutine(CloseSubmenuCoroutine());
			}

			/// <summary>
			/// Open sub-menu coroutine.
			/// </summary>
			/// <param name="index">Index of the MenuItem.</param>
			/// <returns>IEnumerator.</returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
			protected IEnumerator OpenSubmenuCoroutine(int index)
			{
				yield return UtilitiesTime.Wait(Owner.SubmenuDelay, Owner.UnscaledTime);
				OpenSubmenu(index);
			}

			/// <summary>
			/// Close sub-menu coroutine.
			/// </summary>
			/// <returns>IEnumerator.</returns>
			[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
			protected IEnumerator CloseSubmenuCoroutine()
			{
				yield return UtilitiesTime.Wait(Owner.SubmenuDelay, Owner.UnscaledTime);
				CloseSubmenu();
			}

			/// <summary>
			/// Process the item exit event.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="pointerEvent">Pointer event.</param>
			protected virtual void ItemExit(int index, bool pointerEvent)
			{
				var item = Items[index];
				if (pointerEvent)
				{
					StartCloseSubmenu();
				}

				Owner.OnItemDeselect.Invoke(item);
			}

			/// <summary>
			/// Create view.
			/// </summary>
			protected virtual void CreateView()
			{
				if (View == null)
				{
					View = Template.Instance();
					View.name = string.Format("{0} {1}", Owner.name, depth.ToString());
				}
			}

			/// <summary>
			/// Open sub menu.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="selectItem">Select the item after open.</param>
			protected virtual void OpenSubmenu(int index, bool selectItem = false)
			{
				if (closeCoroutine != null)
				{
					Owner.StopCoroutine(closeCoroutine);
					closeCoroutine = null;
				}

				submenuItemIndex = index;

				if (submenu == null)
				{
					submenu = new MenuInstance(Owner, Template, Items[index].Items.AsReadOnly(), ParentCanvas);
					submenu.parent = this;
					submenu.depth = depth + 1;
				}

				submenu.UpdateItems(Items[index].Items.AsReadOnly());

				var item_view = Index2View(index);

				var distance = DistanceTopLeft(item_view.RectTransform);
				distance.x += item_view.RectTransform.rect.width;
				var position = View.RectTransform.anchoredPosition + distance;

				submenu.Open(position);

				if (selectItem)
				{
					submenu.SelectFirst();
				}
			}

			/// <summary>
			/// Get distance from top left corner of the target to the top left corner of the target's parent.
			/// </summary>
			/// <param name="target">Target.</param>
			/// <returns>Distance.</returns>
			protected Vector2 DistanceTopLeft(RectTransform target)
			{
				var parent_size = (target.parent as RectTransform).rect.size;
				var size = target.rect.size;
				var pos = target.anchoredPosition;
				var pivot = target.pivot;

				pos.x -= size.x * pivot.x;
				pos.y += size.y * (1f - pivot.y);
				pos.x += parent_size.x * ((target.anchorMax.x + target.anchorMin.x) / 2f);
				pos.y -= parent_size.y * ((target.anchorMax.y + target.anchorMin.y) / 2f);

				return pos;
			}

			/// <summary>
			/// Close sub menu.
			/// </summary>
			/// <param name="selectItem">Select the item after close.</param>
			protected virtual void CloseSubmenu(bool selectItem = false)
			{
				if (submenu == null)
				{
					return;
				}

				if (!selectItem && submenu.PointerOver)
				{
					return;
				}

				submenu.Close();

				if (selectItem)
				{
					SelectItemView(submenuItemIndex);
					submenuItemIndex = -1;
				}
			}

			/// <summary>
			/// Index to MenuItem view.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <returns>MenuItem view.</returns>
			protected MenuItemView Index2View(int index)
			{
				for (int i = 0; i < ItemViews.Count; i++)
				{
					var view = ItemViews[i];
					if (view.Index == index)
					{
						return view;
					}
				}

				return null;
			}

			/// <summary>
			/// Select the first item.
			/// </summary>
			public void SelectFirst()
			{
				SelectItemView(FindNextItem(-1));
			}

			/// <summary>
			/// Select the MenuItem view by index.
			/// </summary>
			/// <param name="index">Index.</param>
			protected void SelectItemView(int index)
			{
				if ((index < 0) && (index >= Items.Count))
				{
					return;
				}

				var view = Index2View(index);
				if (view != null)
				{
					EventSystem.current.SetSelectedGameObject(view.gameObject);
				}
			}

			/// <summary>
			/// Find next visible and interactable item after the specified index.
			/// </summary>
			/// <param name="start">Start index.</param>
			/// <returns>Next item index.</returns>
			protected int FindNextItem(int start)
			{
				for (int index = start + 1; index < Items.Count; index++)
				{
					if (Items[index].Visible && Items[index].Interactable)
					{
						return index;
					}
				}

				return -1;
			}

			/// <summary>
			/// Find previous visible and interactable item before the specified index.
			/// </summary>
			/// <param name="start">Start index.</param>
			/// <returns>Previous item index.</returns>
			protected int FindPreviousItem(int start)
			{
				for (int index = start - 1; index >= 0; index--)
				{
					if (Items[index].Visible && Items[index].Interactable)
					{
						return index;
					}
				}

				return -1;
			}

			/// <summary>
			/// Process the item move event.
			/// </summary>
			/// <param name="index">Index.</param>
			/// <param name="eventData">Event data.</param>
			protected virtual void ItemMove(int index, AxisEventData eventData)
			{
				if (!Owner.Navigation)
				{
					return;
				}

				switch (eventData.moveDir)
				{
					case MoveDirection.Left:
						if (parent != null)
						{
							parent.CloseSubmenu(true);
						}

						break;
					case MoveDirection.Right:
						if (Items[index].Interactable && Items[index].HasVisibleItems)
						{
							OpenSubmenu(index, true);
						}

						break;
					case MoveDirection.Up:
						var prev_index = FindPreviousItem(index);
						if (prev_index >= 0)
						{
							SelectItemView(prev_index);
						}

						break;
					case MoveDirection.Down:
						var next_index = FindNextItem(index);
						if (next_index < Items.Count)
						{
							SelectItemView(next_index);
						}

						break;
				}
			}

			/// <summary>
			/// Set widget properties from specified style.
			/// </summary>
			/// <param name="style">Style data.</param>
			/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
			public bool SetStyle(Style style)
			{
				if (View != null)
				{
					View.SetStyle(style);
				}

				for (int i = 0; i < ItemViews.Count; i++)
				{
					ItemViews[i].SetStyle(style);
				}

				if (submenu != null)
				{
					submenu.SetStyle(style);
				}

				return false;
			}

			/// <summary>
			/// Set style options from widget properties.
			/// </summary>
			/// <param name="style">Style data.</param>
			/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
			public bool GetStyle(Style style)
			{
				if (View != null)
				{
					View.GetStyle(style);
				}

				for (int i = 0; i < ItemViews.Count; i++)
				{
					ItemViews[i].GetStyle(style);
				}

				if (submenu != null)
				{
					submenu.GetStyle(style);
				}

				return false;
			}
		}
	}
}