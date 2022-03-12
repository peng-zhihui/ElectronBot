namespace UIWidgets.Examples
{
	using System.Collections;
	using UnityEngine;

	/// <summary>
	/// Accordion arrow animation.
	/// </summary>
	[RequireComponent(typeof(Accordion))]
	public class AccordionArrowAnimation : MonoBehaviour
	{
		Accordion accordion;

		/// <summary>
		/// Accordion.
		/// </summary>
		protected Accordion Accordion
		{
			get
			{
				if (accordion == null)
				{
					accordion = GetComponent<Accordion>();
				}

				return accordion;
			}
		}

		IEnumerator AnimationCoroutine;

		bool isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instances.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			Accordion.OnStartToggleAnimation.AddListener(AnimateArrow);
			Accordion.OnToggleItem.AddListener(UpdateArrow);
			Accordion.OnDataSourceChanged.AddListener(UpdateArrows);

			UpdateArrows();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (accordion != null)
			{
				accordion.OnStartToggleAnimation.RemoveListener(AnimateArrow);
				Accordion.OnToggleItem.RemoveListener(UpdateArrow);
				accordion.OnDataSourceChanged.RemoveListener(UpdateArrows);
			}
		}

		void UpdateArrows()
		{
			foreach (var item in Accordion.DataSource)
			{
				UpdateArrow(item);
			}
		}

		RectTransform Item2Arrow(AccordionItem item)
		{
			var component = item.ToggleObject.GetComponent<AccordionArrow>();
			if (component == null)
			{
				return null;
			}

			return component.Arrow;
		}

		void AnimateArrow(AccordionItem item)
		{
			var arrow = Item2Arrow(item);
			if (arrow == null)
			{
				return;
			}

			if (AnimationCoroutine != null)
			{
				StopCoroutine(AnimationCoroutine);
			}

			AnimationCoroutine = item.Open ? CloseCoroutine(arrow) : OpenCoroutine(arrow);
			StartCoroutine(AnimationCoroutine);
		}

		void UpdateArrow(AccordionItem item)
		{
			var arrow = Item2Arrow(item);
			if (arrow == null)
			{
				return;
			}

			SetArrowState(arrow, item.Open);
		}

		void SetArrowState(RectTransform arrow, bool open)
		{
			arrow.localRotation = Quaternion.Euler(0, 0, open ? -90 : 0);
		}

		IEnumerator OpenCoroutine(RectTransform arrow)
		{
			yield return StartCoroutine(Animations.RotateZ(arrow, Accordion.AnimationDuration, -90, 0));
		}

		IEnumerator CloseCoroutine(RectTransform arrow)
		{
			yield return StartCoroutine(Animations.RotateZ(arrow, Accordion.AnimationDuration, 0, -90));
		}
	}
}