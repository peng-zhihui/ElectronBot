namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Sprites;
	using UnityEngine.UI;

	/// <summary>
	/// Line builder.
	/// </summary>
	public class LineBuilder : ILineBuilder
	{
		/// <summary>
		/// Connector.
		/// </summary>
		protected ConnectorBase Connector;

		/// <summary>
		/// Source.
		/// </summary>
		protected RectTransform Source;

		/// <summary>
		/// Vertex helper.
		/// </summary>
		protected VertexHelper VertexHelper;

		/// <summary>
		/// Line.
		/// </summary>
		protected ConnectorLine Line;

		/// <summary>
		/// Vertex index.
		/// </summary>
		protected int VertexIndex;

		/// <summary>
		/// Points list.
		/// </summary>
		protected List<Vector3> Points = new List<Vector3>();

		/// <summary>
		/// Path.
		/// </summary>
		protected RectangularPath Path;

		/// <summary>
		/// Initializes a new instance of the <see cref="LineBuilder"/> class.
		/// </summary>
		public LineBuilder()
		{
			Path = new RectangularPath(Points);
		}

		/// <summary>
		/// Build line mesh.
		/// </summary>
		/// <param name="connector">Connector.</param>
		/// <param name="source">Source.</param>
		/// <param name="line">Line.</param>
		/// <param name="vh">Vertex Helper.</param>
		/// <param name="index">Start vertex index.</param>
		/// <returns>Count of added vertexes.</returns>
		public virtual int Build(ConnectorBase connector, RectTransform source, ConnectorLine line, VertexHelper vh, int index)
		{
			if ((line == null) || (line.Target == null) || (!line.Target.gameObject.activeInHierarchy))
			{
				return 0;
			}

			Connector = connector;
			Source = source;
			Line = line;
			VertexHelper = vh;
			VertexIndex = index;

			var start_point = Connector.GetPoint(source, line.Start);
			var end_point = Connector.GetPoint(line.Target, line.End);

			int points;

			switch (line.Type)
			{
				case ConnectorType.Straight:
					points = Straight(start_point, end_point, VertexIndex);
					break;
				case ConnectorType.Rectangular:
					points = RectangularStart(Line.Start, start_point, end_point);
					break;
				default:
					throw new NotSupportedException(string.Format("Unsupported line type: {0}", EnumHelper<ConnectorType>.ToString(line.Type)));
			}

			Connector = null;
			Source = null;
			Line = null;
			VertexHelper = null;
			VertexIndex = 0;

			return points;
		}

		/// <summary>
		/// Build straight line mesh.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		/// <param name="index">Index.</param>
		protected virtual int Straight(Vector3 startPoint, Vector3 endPoint, int index)
		{
			var width = Line.Thickness;
			var angle_z = Mathf.Atan2(endPoint.y - startPoint.y, endPoint.x - startPoint.x) * 180f / Mathf.PI;
			var angle = Quaternion.Euler(new Vector3(0, 0, angle_z));

			var vertex1 = startPoint + new Vector3(0, -width / 2);
			var vertex2 = startPoint + new Vector3(0, +width / 2);
			var vertex3 = endPoint + new Vector3(0, +width / 2);
			var vertex4 = endPoint + new Vector3(0, -width / 2);

			vertex1 = ConnectorBase.RotatePoint(vertex1, startPoint, angle);
			vertex2 = ConnectorBase.RotatePoint(vertex2, startPoint, angle);
			vertex3 = ConnectorBase.RotatePoint(vertex3, endPoint, angle);
			vertex4 = ConnectorBase.RotatePoint(vertex4, endPoint, angle);

			var uv = (Connector.Sprite != null) ? DataUtility.GetOuterUV(Connector.Sprite) : new Vector4(0f, 0f, 1f, 1f);

			Color32 color32 = Connector.color;
			VertexHelper.AddVert(vertex1, color32, new Vector2(uv.x, uv.y));
			VertexHelper.AddVert(vertex2, color32, new Vector2(uv.x, uv.w));
			VertexHelper.AddVert(vertex3, color32, new Vector2(uv.z, uv.w));
			VertexHelper.AddVert(vertex4, color32, new Vector2(uv.z, uv.y));

			VertexHelper.AddTriangle(index + 0, index + 1, index + 2);
			VertexHelper.AddTriangle(index + 2, index + 3, index + 0);

			return 4;
		}

		/// <summary>
		/// Build rectangular line mesh from the specified start position.
		/// </summary>
		/// <param name="start">Start position.</param>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		/// <returns>Count of added vertexes.</returns>
		protected int RectangularStart(ConnectorPosition start, Vector3 startPoint, Vector3 endPoint)
		{
			switch (start)
			{
				case ConnectorPosition.Left:
					return RectangularLeft(startPoint, endPoint);
				case ConnectorPosition.Right:
					return RectangularRight(startPoint, endPoint);
				case ConnectorPosition.Top:
					return RectangularTop(startPoint, endPoint);
				case ConnectorPosition.Bottom:
					return RectangularBottom(startPoint, endPoint);
				case ConnectorPosition.Center:
					return RectangularCenter(startPoint, endPoint);
				default:
					throw new NotSupportedException(string.Format("Unsupported line start: {0}", EnumHelper<ConnectorPosition>.ToString(start)));
			}
		}

		/// <summary>
		/// Get nearest end position.
		/// </summary>
		/// <param name="delta">Positions difference.</param>
		/// <returns>Position.</returns>
		protected ConnectorPosition NearestEnd(Vector2 delta)
		{
			if (delta.x > 0)
			{
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					return ConnectorPosition.Left;
				}

				return delta.y > 0 ? ConnectorPosition.Bottom : ConnectorPosition.Top;
			}

			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				return ConnectorPosition.Right;
			}

			return delta.y > 0 ? ConnectorPosition.Bottom : ConnectorPosition.Top;
		}

		/// <summary>
		/// Get nearest start position.
		/// </summary>
		/// <param name="delta">Positions difference.</param>
		/// <returns>Position.</returns>
		protected ConnectorPosition NearestStart(Vector2 delta)
		{
			if (delta.x > 0)
			{
				if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
				{
					return ConnectorPosition.Right;
				}

				return delta.y > 0 ? ConnectorPosition.Top : ConnectorPosition.Bottom;
			}

			if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
			{
				return ConnectorPosition.Left;
			}

			return delta.y > 0 ? ConnectorPosition.Top : ConnectorPosition.Bottom;
		}

		/// <summary>
		/// Build rectangular line mesh starting from the left.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		protected int RectangularLeft(Vector3 startPoint, Vector3 endPoint)
		{
			var delta = endPoint - startPoint;

			var end = Line.End;
			if (end == ConnectorPosition.Center)
			{
				end = NearestEnd(delta);
				endPoint = Connector.GetPoint(Line.Target, end);
				delta = endPoint - startPoint;
			}

			Path.Start(startPoint);

			switch (end)
			{
				case ConnectorPosition.Left:
					if (delta.x > 0)
					{
						Path.X(-Line.Margin);
						Path.Y(delta.y);
					}
					else
					{
						Path.X(delta.x - Line.Margin);
						Path.Y(delta.y);
					}

					break;
				case ConnectorPosition.Right:
					if (delta.x > (-Line.Margin * 2))
					{
						Path.X(-Line.Margin);
						Path.Y(delta.y / 2f);
						Path.X(delta.x + (Line.Margin * 2));
						Path.Y(delta.y / 2f);
					}
					else
					{
						Path.X(-Line.Margin);
						Path.Y(delta.y);
					}

					break;
				case ConnectorPosition.Top:
					if ((delta.x > 0) || (delta.y > 0))
					{
						Path.X(-Line.Margin);
						Path.Y(delta.y + Line.Margin);
						Path.X(delta.x + Line.Margin);
					}
					else
					{
						Path.X(delta.x);
					}

					break;
				case ConnectorPosition.Bottom:
					if ((delta.x > 0) || (delta.y < 0))
					{
						Path.X(-Line.Margin);
						Path.Y(delta.y - Line.Margin);
						Path.X(delta.x + Line.Margin);
					}
					else
					{
						Path.X(delta.x);
					}

					break;
				default:
					throw new NotSupportedException(string.Format("Unsupported line end: {0}", EnumHelper<ConnectorPosition>.ToString(Line.End)));
			}

			Path.End(endPoint);

			return Points2Lines();
		}

		/// <summary>
		/// Build rectangular line mesh starting from the right.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		protected int RectangularRight(Vector3 startPoint, Vector3 endPoint)
		{
			var delta = endPoint - startPoint;

			var end = Line.End;
			if (end == ConnectorPosition.Center)
			{
				end = NearestEnd(delta);
				endPoint = Connector.GetPoint(Line.Target, end);
				delta = endPoint - startPoint;
			}

			Path.Start(startPoint);

			switch (end)
			{
				case ConnectorPosition.Left:
					if (delta.x > (Line.Margin * 2))
					{
						Path.X(Line.Margin);
						Path.Y(delta.y);
					}
					else
					{
						Path.X(Line.Margin);
						Path.Y(delta.y / 2f);
						Path.X(delta.x - (Line.Margin * 2));
						Path.Y(delta.y / 2f);
					}

					break;
				case ConnectorPosition.Right:
					if (delta.x > 0)
					{
						Path.X(delta.x + Line.Margin);
						Path.Y(delta.y);
					}
					else
					{
						Path.X(Line.Margin);
						Path.Y(delta.y);
					}

					break;
				case ConnectorPosition.Top:
					if ((delta.x > 0) && (delta.y < 0))
					{
						Path.X(delta.x);
					}
					else
					{
						Path.X(Line.Margin);
						Path.Y(delta.y + Line.Margin);
						Path.X(delta.x - Line.Margin);
					}

					break;
				case ConnectorPosition.Bottom:
					if ((delta.x > 0) && (delta.y > 0))
					{
						Path.X(delta.x);
					}
					else
					{
						Path.X(Line.Margin);
						Path.Y(delta.y - Line.Margin);
						Path.X(delta.x - Line.Margin);
					}

					break;
				default:
					throw new NotSupportedException(string.Format("Unsupported line end: {0}", EnumHelper<ConnectorPosition>.ToString(Line.End)));
			}

			Path.End(endPoint);

			return Points2Lines();
		}

		/// <summary>
		/// Build rectangular line mesh starting from the top.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		protected int RectangularTop(Vector3 startPoint, Vector3 endPoint)
		{
			var delta = endPoint - startPoint;

			var end = Line.End;

			if (end == ConnectorPosition.Center)
			{
				end = NearestEnd(delta);
				endPoint = Connector.GetPoint(Line.Target, end);
				delta = endPoint - startPoint;
			}

			Path.Start(startPoint);

			switch (end)
			{
				case ConnectorPosition.Left:
					if ((delta.x > 0) && (delta.y > 0))
					{
						Path.Y(delta.y);
					}
					else
					{
						Path.Y(Line.Margin);
						Path.X(delta.x - Line.Margin);
						Path.Y(delta.y - Line.Margin);
					}

					break;
				case ConnectorPosition.Right:
					if ((delta.x < 0) && (delta.y > 0))
					{
						Path.Y(delta.y);
					}
					else
					{
						Path.Y(Line.Margin);
						Path.X(delta.x + Line.Margin);
						Path.Y(delta.y - Line.Margin);
					}

					break;
				case ConnectorPosition.Top:
					if (delta.y > 0)
					{
						Path.Y(delta.y + (Line.Margin * 2));
						Path.X(delta.x);
					}
					else
					{
						Path.Y(Line.Margin);
						Path.X(delta.x);
						Path.Y(delta.y + Line.Margin);
					}

					break;
				case ConnectorPosition.Bottom:
					if (delta.y > 0)
					{
						Path.Y(Line.Margin);
						Path.X(delta.x);
						Path.Y(delta.y + Line.Margin);
					}
					else
					{
						Path.Y(Line.Margin);
						Path.X(delta.x / 2f);
						Path.Y(delta.y - (Line.Margin * 2));
						Path.X(delta.x / 2f);
					}

					break;
				default:
					throw new NotSupportedException(string.Format("Unsupported line end: {0}", EnumHelper<ConnectorPosition>.ToString(Line.End)));
			}

			Path.End(endPoint);

			return Points2Lines();
		}

		/// <summary>
		/// Build rectangular line mesh starting from the bottom.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		protected int RectangularBottom(Vector3 startPoint, Vector3 endPoint)
		{
			var delta = endPoint - startPoint;

			var end = Line.End;

			if (end == ConnectorPosition.Center)
			{
				end = NearestEnd(delta);
				endPoint = Connector.GetPoint(Line.Target, end);
				delta = endPoint - startPoint;
			}

			Path.Start(startPoint);

			switch (end)
			{
				case ConnectorPosition.Left:
					if ((delta.x > 0) && (delta.y < 0))
					{
						Path.Y(delta.y);
					}
					else
					{
						Path.Y(-Line.Margin);
						Path.X(delta.x - Line.Margin);
						Path.Y(delta.y + Line.Margin);
					}

					break;
				case ConnectorPosition.Right:
					if ((delta.x < 0) && (delta.y < 0))
					{
						Path.Y(delta.y);
					}
					else
					{
						Path.Y(-Line.Margin);
						Path.X(delta.x + Line.Margin);
						Path.Y(delta.y + Line.Margin);
					}

					break;
				case ConnectorPosition.Top:
					if (delta.y > 0)
					{
						Path.Y(-Line.Margin);
						Path.X(delta.x / 2f);
						Path.Y(delta.y + (Line.Margin * 2));
						Path.X(delta.x / 2f);
					}
					else
					{
						Path.Y(-Line.Margin);
						Path.X(delta.x);
						Path.Y(delta.y + (Line.Margin * 2));
					}

					break;
				case ConnectorPosition.Bottom:
					if (delta.y > 0)
					{
						Path.Y(-Line.Margin);
						Path.X(delta.x);
						Path.Y(delta.y + Line.Margin);
					}
					else
					{
						Path.Y(delta.y - Line.Margin);
						Path.X(delta.x);
					}

					break;
				default:
					throw new NotSupportedException(string.Format("Unsupported line end: {0}", EnumHelper<ConnectorPosition>.ToString(Line.End)));
			}

			Path.End(endPoint);

			return Points2Lines();
		}

		/// <summary>
		/// Build rectangular line mesh starting from the center.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		/// <param name="startPoint">Start point.</param>
		/// <param name="endPoint">End point.</param>
		protected int RectangularCenter(Vector3 startPoint, Vector3 endPoint)
		{
			var delta = endPoint - startPoint;

			var start = NearestStart(delta);
			startPoint = Connector.GetPoint(Source, start);

			return RectangularStart(start, startPoint, endPoint);
		}

		/// <summary>
		/// Build line mesh from points.
		/// </summary>
		/// <returns>Count of added vertexes.</returns>
		protected virtual int Points2Lines()
		{
			var total = 0;

			for (var i = 1; i < Points.Count; i++)
			{
				total += Straight(Points[i - 1], Points[i], VertexIndex + total);
			}

			return total;
		}
	}
}