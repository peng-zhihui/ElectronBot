namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// TrackDataView.
	/// </summary>
	public class TrackDataView : TrackDataViewBase<TrackData, DateTime>
	{
		/// <summary>
		/// Drag component.
		/// </summary>
		[SerializeField]
		public TrackDataDragSupport Drag;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Add listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void AddListeners()
		{
			base.AddListeners();

			if (Drag != null)
			{
				Drag.StartDragEvent.AddListener(DisableResizable);
				Drag.EndDragEvent.AddListener(EnableResizable);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void RemoveListeners()
		{
			base.RemoveListeners();

			if (Drag != null)
			{
				Drag.StartDragEvent.RemoveListener(DisableResizable);
				Drag.EndDragEvent.RemoveListener(EnableResizable);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <param name="data">Data.</param>
		/// <param name="baseVerticalPostion">Base vertical position.</param>
		public override void SetData(Track<TrackData, DateTime> track, TrackData data, float baseVerticalPostion)
		{
			if (Drag != null)
			{
				Drag.Track = track;
				Drag.Owner = Owner;
			}

			Data = data;

			name = data.Name;
			Name.Value = data.Name;

			SetSizeAndPosition(baseVerticalPostion);
		}
	}
}