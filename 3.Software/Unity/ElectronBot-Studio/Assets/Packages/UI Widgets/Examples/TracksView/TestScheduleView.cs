namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Test ScheduleView.
	/// </summary>
	[RequireComponent(typeof(ScheduleView))]
	public class TestScheduleView : MonoBehaviour
	{
		/// <summary>
		/// Start this instance.
		/// </summary>
		protected void Start()
		{
			var track1 = new Track<TrackData, DateTime>()
			{
				Name = "Track 1",
			};
			track1.Data.Add(new TrackData()
			{
				Name = "Current Task",
				StartPoint = DateTime.Today,
				EndPoint = DateTime.Today.AddDays(1),
			});
			track1.Data.Add(new TrackData()
			{
				Name = "Past Task",
				StartPoint = DateTime.Today.AddDays(-5),
				EndPoint = DateTime.Today.AddDays(-4),
			});
			track1.Data.Add(new TrackData()
			{
				Name = "Overlapping Task",
				StartPoint = DateTime.Today.AddDays(-5),
				EndPoint = DateTime.Today.AddDays(-2),
			});
			track1.Data.Add(new TrackData()
			{
				Name = "Future Task",
				StartPoint = DateTime.Today.AddDays(3),
				EndPoint = DateTime.Today.AddDays(6),
			});

			var track2 = new Track<TrackData, DateTime>()
			{
				Name = "Track 2",
			};
			track2.Data.Add(new TrackData()
			{
				Name = "Future Task",
				StartPoint = DateTime.Today.AddDays(2),
				EndPoint = DateTime.Today.AddDays(3),
			});
			track2.Data.Add(new TrackData()
			{
				Name = "Future Task",
				StartPoint = DateTime.Today.AddDays(3),
				EndPoint = DateTime.Today.AddDays(4),
			});
			track2.Data.Add(new TrackData()
			{
				Name = "Future Task",
				StartPoint = DateTime.Today.AddDays(4),
				EndPoint = DateTime.Today.AddDays(5),
			});

			var schedule = GetComponent<ScheduleView>();
			schedule.Tracks.BeginUpdate();
			schedule.Tracks.Add(track1);
			schedule.Tracks.Add(track2);

			for (int i = 3; i < 21; i++)
			{
				var track = new Track<TrackData, DateTime>()
				{
					Name = string.Format("Track {0}", i.ToString()),
				};
				schedule.Tracks.Add(track);
			}

			schedule.Tracks.EndUpdate();
		}
	}
}