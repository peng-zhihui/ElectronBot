namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Line builder interface.
	/// </summary>
	public interface ILineBuilder
	{
		/// <summary>
		/// Build line.
		/// </summary>
		/// <param name="connector">Connector.</param>
		/// <param name="source">Source.</param>
		/// <param name="line">Line.</param>
		/// <param name="vh">Vertex Helper.</param>
		/// <param name="index">Start vertex index.</param>
		/// <returns>Count of added vertexes.</returns>
		int Build(ConnectorBase connector, RectTransform source, ConnectorLine line, VertexHelper vh, int index);
	}
}