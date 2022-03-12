namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test accordion.
	/// </summary>
	public class AccordionTestItems : MonoBehaviour
	{
		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		public Accordion Accordion;

		/// <summary>
		/// Header1.
		/// </summary>
		[SerializeField]
		public GameObject Header1;

		/// <summary>
		/// Content1.
		/// </summary>
		[SerializeField]
		public GameObject Content1;

		/// <summary>
		/// Header2.
		/// </summary>
		[SerializeField]
		public GameObject Header2;

		/// <summary>
		/// Content2.
		/// </summary>
		[SerializeField]
		public GameObject Content2;

		/// <summary>
		/// Header3.
		/// </summary>
		[SerializeField]
		public GameObject Header3;

		/// <summary>
		/// Content3.
		/// </summary>
		[SerializeField]
		public GameObject Content3;

		/// <summary>
		/// Set accordion items.
		/// </summary>
		public void SetItems()
		{
			Accordion.DataSource = new ObservableList<AccordionItem>()
			{
				new AccordionItem()
				{
					ToggleObject = Header1,
					ContentObject = Content1,
					Open = true,
				},
				new AccordionItem()
				{
					ToggleObject = Header2,
					ContentObject = Content2,
					Open = false,
				},
				new AccordionItem()
				{
					ToggleObject = Header3,
					ContentObject = Content3,
					Open = false,
				},
			};
		}
	}
}