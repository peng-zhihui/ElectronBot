namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Range slider and audio track.
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class RangeSliderAudioTrack : MonoBehaviour, IUpdatable
	{
		/// <summary>
		/// The slider.
		/// </summary>
		[SerializeField]
		public RangeSlider Slider;

		/// <summary>
		/// The indicator bar.
		/// </summary>
		[SerializeField]
		public RectTransform IndicatorBar;

		[SerializeField]
		AudioClip track;

		/// <summary>
		/// Gets or sets the track.
		/// </summary>
		/// <value>The track.</value>
		public AudioClip Track
		{
			get
			{
				return track;
			}

			set
			{
				SetTrack(value);
			}
		}

		AudioSource source;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected void Start()
		{
			source = GetComponent<AudioSource>();

			SetTrack(track);
		}

		/// <summary>
		/// Sets the track.
		/// </summary>
		/// <param name="newTrack">New track.</param>
		protected void SetTrack(AudioClip newTrack)
		{
			Stop();

			track = newTrack;

			Slider.LimitMax = newTrack.samples;

			Slider.ValueMin = 0;
			Slider.ValueMax = newTrack.samples;
		}

		/// <summary>
		/// Play audio clip.
		/// </summary>
		public void Play()
		{
			source.clip = Track;
			source.timeSamples = Slider.ValueMin;

			IndicatorBar.gameObject.SetActive(true);

			source.Play();

			RunUpdate();
		}

		/// <summary>
		/// Stop playing.
		/// </summary>
		public void Stop()
		{
			if (source.isPlaying)
			{
				source.Stop();
			}

			IndicatorBar.gameObject.SetActive(false);
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Updater.Add(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Updater.Remove(this);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public void RunUpdate()
		{
			if (!source.isPlaying)
			{
				return;
			}

			var length = ((float)(source.timeSamples - Slider.ValueMin)) / ((float)(Slider.ValueMax - Slider.ValueMin));
			IndicatorBar.anchorMax = new Vector2(length, IndicatorBar.anchorMax.y);

			if (source.timeSamples >= Slider.ValueMax)
			{
				Stop();
			}
		}
	}
}