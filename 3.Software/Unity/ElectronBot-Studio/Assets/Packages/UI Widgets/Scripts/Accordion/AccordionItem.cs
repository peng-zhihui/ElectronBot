namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Accordion item.
	/// </summary>
	[Serializable]
	public class AccordionItem
	{
		/// <summary>
		/// The toggle object.
		/// </summary>
		public GameObject ToggleObject;

		/// <summary>
		/// The content object.
		/// </summary>
		public GameObject ContentObject;

		/// <summary>
		/// Default state of content object.
		/// </summary>
		public bool Open;

		/// <summary>
		/// Label.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public TextAdapter ToggleLabel;

		/// <summary>
		/// The current coroutine.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public Coroutine CurrentCoroutine;

		/// <summary>
		/// The content object RectTransform.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public RectTransform ContentObjectRect;

		/// <summary>
		/// The content LayoutElement.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public LayoutElement ContentLayoutElement;

		/// <summary>
		/// The height of the content object.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public float ContentObjectHeight;

		/// <summary>
		/// The width of the content object.
		/// </summary>
		[HideInInspector]
		[NonSerialized]
		public float ContentObjectWidth;
	}
}