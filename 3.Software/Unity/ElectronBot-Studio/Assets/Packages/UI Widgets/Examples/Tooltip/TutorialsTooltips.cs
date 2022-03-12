namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Tutorials tooltip.
	/// </summary>
	public class TutorialsTooltips : MonoBehaviour
	{
		/// <summary>
		/// Target.
		/// </summary>
		[Serializable]
		protected class Target
		{
			/// <summary>
			/// Gameobject.
			/// </summary>
			[SerializeField]
			public GameObject GO;

			/// <summary>
			/// Tooltip text.
			/// </summary>
			[SerializeField]
			public string TooltipText;

			/// <summary>
			/// Parent.
			/// </summary>
			public Transform Parent
			{
				get;
				private set;
			}

			List<Graphic> graphics = new List<Graphic>();

			List<bool> raycastTargets = new List<bool>();

			/// <summary>
			/// Init.
			/// </summary>
			public void Init()
			{
				Parent = GO.transform.parent;

				GO.GetComponentsInChildren<Graphic>(true, graphics);

				raycastTargets.Clear();
				foreach (var g in graphics)
				{
					raycastTargets.Add(g.raycastTarget);
				}
			}

			/// <summary>
			/// Bring to front.
			/// </summary>
			/// <param name="parent">Parent.</param>
			public void BringToFront(Transform parent)
			{
				GO.transform.SetParent(parent, true);

				foreach (var g in graphics)
				{
					g.raycastTarget = false;
				}
			}

			/// <summary>
			/// Restore.
			/// </summary>
			public void Restore()
			{
				for (int i = 0; i < graphics.Count; i++)
				{
					graphics[i].raycastTarget = raycastTargets[i];
				}

				GO.transform.SetParent(Parent, true);
			}
		}

		/// <summary>
		/// Targets.
		/// </summary>
		[SerializeField]
		protected List<Target> Targets = new List<Target>();

		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		protected TooltipString Tooltip;

		/// <summary>
		/// Container.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("TargetsContainer")]
		protected RectTransform Container;

		/// <summary>
		/// Start button.
		/// </summary>
		[SerializeField]
		protected Button ButtonStart;

		/// <summary>
		/// Next button.
		/// </summary>
		[SerializeField]
		protected Button ButtonNext;

		/// <summary>
		/// Hide button.
		/// </summary>
		[SerializeField]
		protected Button ButtonHide;

		int index = -1;

		int? modalKey;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected void Start()
		{
			foreach (var target in Targets)
			{
				target.Init();
				Tooltip.Register(target.GO, target.TooltipText, new TooltipSettings(TooltipPosition.TopCenter));
			}

			ButtonStart.onClick.AddListener(Show);
			ButtonStart.gameObject.SetActive(true);

			ButtonNext.onClick.AddListener(Next);
			ButtonNext.gameObject.SetActive(false);

			ButtonHide.onClick.AddListener(Hide);
			ButtonHide.gameObject.SetActive(false);
		}

		/// <summary>
		/// Show tooltip.
		/// </summary>
		public void Show()
		{
			if (index == -1)
			{
				modalKey = ModalHelper.Open(Container, color: new Color32(128, 128, 128, 128), parent: Container);
				Next();

				ButtonStart.gameObject.SetActive(false);
				ButtonNext.gameObject.SetActive(true);
				ButtonHide.gameObject.SetActive(true);
			}
		}

		/// <summary>
		/// Next tooltip.
		/// </summary>
		protected void Next()
		{
			var is_last = index == (Targets.Count - 1);
			if (is_last)
			{
				Hide();
				return;
			}

			if (index > -1)
			{
				Targets[index].Restore();
				ModalHelper.SetAsLastSibling(modalKey);
			}

			index += 1;
			Targets[index].BringToFront(Container.parent);
			Tooltip.Show(Targets[index].GO);
		}

		/// <summary>
		/// Hide tooltip.
		/// </summary>
		public void Hide()
		{
			if (index > -1)
			{
				Targets[index].Restore();
				ModalHelper.SetAsLastSibling(modalKey);

				index = -1;
			}

			if (modalKey.HasValue)
			{
				ModalHelper.Close(modalKey.Value);
				Tooltip.Hide();
			}

			ButtonStart.gameObject.SetActive(true);
			ButtonNext.gameObject.SetActive(false);
			ButtonHide.gameObject.SetActive(false);
		}
	}
}