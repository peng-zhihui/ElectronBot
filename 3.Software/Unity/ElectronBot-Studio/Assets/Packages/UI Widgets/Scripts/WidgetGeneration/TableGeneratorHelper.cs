namespace UIWidgets.WidgetGeneration
{
	using UnityEngine.Serialization;

	/// <summary>
	/// Table generator helper.
	/// </summary>
	public class TableGeneratorHelper : ListViewGeneratorHelper
	{
		/// <summary>
		/// TableHeader.
		/// </summary>
		[FormerlySerializedAs("ResizableHeader")]
		public TableHeader TableHeader;
	}
}