namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test content addition to the accordion.
	/// </summary>
	public class TestAccordionContent : MonoBehaviour
	{
		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		protected Accordion Accordion;

		/// <summary>
		/// Content.
		/// </summary>
		[SerializeField]
		protected RectTransform Content;

		/// <summary>
		/// Add content to item.
		/// </summary>
		/// <param name="itemIndex">Item index.</param>
		public void AddContent(int itemIndex)
		{
			var container = Accordion.DataSource[itemIndex].ContentObjectRect;
			var clone = Instantiate(Content, container, false);
			clone.gameObject.SetActive(true);
		}
	}
}