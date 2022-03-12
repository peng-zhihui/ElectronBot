namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test Accordion.
	/// </summary>
	public class AccordionTestToggle : MonoBehaviour
	{
		/// <summary>
		/// Accordion.
		/// </summary>
		[SerializeField]
		protected Accordion TestAccordion;

		/// <summary>
		/// Toggleable GameObject.
		/// </summary>
		[SerializeField]
		protected GameObject ToggleGameObject;

		/// <summary>
		/// Content GameObject.
		/// </summary>
		[SerializeField]
		protected GameObject ContentGameObject;

		/// <summary>
		/// Simple test. Get item and open it.
		/// </summary>
		public void Test()
		{
			// find required item by toggle object
			var item = FindItemByToggle(TestAccordion.DataSource, ToggleGameObject);

			// and expand item
			TestAccordion.Open(item);
		}

		/// <summary>
		/// Get item and close it.
		/// </summary>
		public void TestClose()
		{
			// find required item by content object
			var item = FindItemByToggle(TestAccordion.DataSource, ContentGameObject);

			// and close it
			TestAccordion.Close(item);
		}

		static AccordionItem FindItemByToggle(ObservableList<AccordionItem> items, GameObject toggle)
		{
			foreach (var item in items)
			{
				if (item.ToggleObject == toggle)
				{
					return item;
				}
			}

			return null;
		}

		static AccordionItem FindItemByContent(ObservableList<AccordionItem> items, GameObject content)
		{
			foreach (var item in items)
			{
				if (item.ContentObject == content)
				{
					return item;
				}
			}

			return null;
		}

		/// <summary>
		/// Get item and toggle it.
		/// </summary>
		public void TestToggle()
		{
			// get item by index
			var item = TestAccordion.DataSource[0];

			// and toggle item
			TestAccordion.ToggleItem(item);
		}

		/// <summary>
		/// Toggle with OnClick.Invoke();
		/// </summary>
		public void SimpleToggle()
		{
			var component = ToggleGameObject.GetComponent<AccordionItemComponent>();
			component.OnClick.Invoke();
		}
	}
}