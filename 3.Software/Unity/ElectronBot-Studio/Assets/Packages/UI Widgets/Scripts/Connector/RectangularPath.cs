namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Path builder for the rectangular line.
	/// </summary>
	public class RectangularPath
	{
		List<Vector3> points;

		bool ended;

		/// <summary>
		/// Is path ended.
		/// </summary>
		public bool Ended
		{
			get
			{
				return ended;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="RectangularPath"/> class.
		/// </summary>
		/// <param name="points">Points/</param>
		public RectangularPath(List<Vector3> points)
		{
			this.points = points;
			ended = false;
		}

		/// <summary>
		/// Set start point.
		/// </summary>
		/// <param name="start">Start point.</param>
		public void Start(Vector3 start)
		{
			ended = false;
			points.Clear();
			points.Add(start);
		}

		/// <summary>
		/// Move on X axis.
		/// </summary>
		/// <param name="x">X.</param>
		public void X(float x)
		{
			if (ended)
			{
				throw new InvalidOperationException("Path already ended.");
			}

			var last = points[points.Count - 1];
			points.Add(last + new Vector3(x, 0, 0));
		}

		/// <summary>
		/// Move on Y axis.
		/// </summary>
		/// <param name="y">Y.</param>
		public void Y(float y)
		{
			if (ended)
			{
				throw new InvalidOperationException("Path already ended.");
			}

			var last = points[points.Count - 1];
			points.Add(last + new Vector3(0, y, 0));
		}

		/// <summary>
		/// End path.
		/// </summary>
		public void End()
		{
			ended = true;
		}

		/// <summary>
		/// End path.
		/// </summary>
		/// <param name="end">End point.</param>
		public void End(Vector3 end)
		{
			if (ended)
			{
				throw new InvalidOperationException("Path already ended.");
			}

			points.Add(end);
			ended = true;
		}
	}
}