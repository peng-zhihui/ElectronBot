namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// AudioPlayer.
	/// Play single AudioClip.
	/// </summary>
	public class AudioPlayer : MonoBehaviour, IStylable, IUpdatable
	{
		/// <summary>
		/// Slider to display play progress.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("progress")]
		protected Slider Progress;

		/// <summary>
		/// Play button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("playButton")]
		protected Button PlayButton;

		/// <summary>
		/// Pause button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("pauseButton")]
		protected Button PauseButton;

		/// <summary>
		/// Stop button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("stopButton")]
		protected Button StopButton;

		/// <summary>
		/// Toggle button.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("toggleButton")]
		protected Button ToggleButton;

		/// <summary>
		/// AudioSource to play AudioClip.
		/// </summary>
		[SerializeField]
		protected AudioSource source;

		/// <summary>
		/// AudioSource to play AudioClip.
		/// </summary>
		public AudioSource Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<AudioSource>();

					if (source != null)
					{
						CurrentClip = source.clip;
					}
				}

				return source;
			}

			set
			{
				source = value;
			}
		}

		/// <summary>
		/// Current clip.
		/// </summary>
		[NonSerialized]
		protected AudioClip CurrentClip;

		/// <summary>
		/// Is Source playing current clip?
		/// </summary>
		public bool IsUsedCurrentClip
		{
			get
			{
				return (Source.clip != null) && (CurrentClip != null) && (Source.clip.GetInstanceID() == CurrentClip.GetInstanceID());
			}
		}

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init AudioPlayer and attach listeners.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			if (PlayButton != null)
			{
				PlayButton.onClick.AddListener(Play);
			}

			if (PauseButton != null)
			{
				PauseButton.onClick.AddListener(Pause);
			}

			if (StopButton != null)
			{
				StopButton.onClick.AddListener(Stop);
			}

			if (ToggleButton != null)
			{
				ToggleButton.onClick.AddListener(Toggle);
			}

			if (Progress != null)
			{
				Progress.onValueChanged.AddListener(SetCurrentTimeSample);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (PlayButton != null)
			{
				PlayButton.onClick.RemoveListener(Play);
			}

			if (PauseButton != null)
			{
				PauseButton.onClick.RemoveListener(Pause);
			}

			if (StopButton != null)
			{
				StopButton.onClick.RemoveListener(Stop);
			}

			if (ToggleButton != null)
			{
				ToggleButton.onClick.RemoveListener(Toggle);
			}

			if (Progress != null)
			{
				Progress.onValueChanged.RemoveListener(SetCurrentTimeSample);
			}
		}

		/// <summary>
		/// Set playback position in seconds.
		/// </summary>
		/// <param name="time">Playback position in seconds.</param>
		public virtual void SetTime(float time)
		{
			if (!IsUsedCurrentClip)
			{
				return;
			}

			if (Source.clip != null)
			{
				Source.time = Mathf.Min(time, Source.clip.length);
			}
		}

		/// <summary>
		/// Set playback position in PCM samples.
		/// </summary>
		/// <param name="timesample">Playback position in PCM samples.</param>
		protected virtual void SetCurrentTimeSample(float timesample)
		{
			SetCurrentTimeSample(Mathf.RoundToInt(timesample));
		}

		/// <summary>
		/// Set playback position in PCM samples.
		/// </summary>
		/// <param name="timesample">Playback position in PCM samples.</param>
		public virtual void SetCurrentTimeSample(int timesample)
		{
			if (!IsUsedCurrentClip)
			{
				return;
			}

			if (Source.clip != null)
			{
				Source.timeSamples = Mathf.Min(timesample, Source.clip.samples - 1);
			}
		}

		/// <summary>
		/// Set AudioClip to play.
		/// </summary>
		/// <param name="clip">AudioClip to play.</param>
		public virtual void SetAudioClip(AudioClip clip)
		{
			if (IsUsedCurrentClip)
			{
				CurrentClip = clip;

				if (Source.isPlaying)
				{
					Source.Stop();
					Source.clip = CurrentClip;
					Source.Play();
				}
				else
				{
					Source.clip = CurrentClip;
				}

				RunUpdate();
			}
			else
			{
				CurrentClip = clip;
			}
		}

		/// <summary>
		/// Play specified AudioClip.
		/// </summary>
		/// <param name="clip">AudioClip to play.</param>
		public virtual void Play(AudioClip clip)
		{
			CurrentClip = clip;

			Source.Stop();
			Source.clip = CurrentClip;
			Source.Play();
		}

		/// <summary>
		/// Play current AudioClip.
		/// </summary>
		public virtual void Play()
		{
			if (IsUsedCurrentClip)
			{
				if (Source.timeSamples >= (Source.clip.samples - 1))
				{
					Source.timeSamples = 0;
				}
			}
			else
			{
				Source.Stop();
				Source.clip = CurrentClip;
				Source.timeSamples = 0;
			}

			Source.Play();
			RunUpdate();
		}

		/// <summary>
		/// Pauses playing current AudioClip.
		/// </summary>
		public virtual void Pause()
		{
			if (!IsUsedCurrentClip)
			{
				return;
			}

			Source.Pause();
			RunUpdate();
		}

		/// <summary>
		/// Stops playing current AudioClip.
		/// </summary>
		public virtual void Stop()
		{
			if (!IsUsedCurrentClip)
			{
				return;
			}

			Source.Stop();
			RunUpdate();
		}

		/// <summary>
		/// Pauses current AudioClip, if it's playing, otherwise unpaused.
		/// </summary>
		public virtual void Toggle()
		{
			if (!IsUsedCurrentClip)
			{
				return;
			}

			if (Source.isPlaying)
			{
				Source.Pause();
				RunUpdate();
			}
			else
			{
				Play();
			}
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
		/// Update buttons state and playing progress.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (IsUsedCurrentClip)
			{
				if (PlayButton != null)
				{
					PlayButton.gameObject.SetActive(!Source.isPlaying);
				}

				if (PauseButton != null)
				{
					PauseButton.gameObject.SetActive(Source.isPlaying);
				}

				if (StopButton != null)
				{
					StopButton.gameObject.SetActive(Source.isPlaying);
				}

				if (Progress != null)
				{
					Progress.wholeNumbers = true;
					Progress.minValue = 0;

					if (Source.clip != null)
					{
						Progress.maxValue = Source.clip.samples;
						Progress.value = Source.timeSamples;
					}
					else
					{
						Progress.maxValue = 100;
						Progress.value = 0;
					}
				}
			}
			else
			{
				if (PlayButton != null)
				{
					PlayButton.gameObject.SetActive(true);
				}

				if (PauseButton != null)
				{
					PauseButton.gameObject.SetActive(false);
				}

				if (StopButton != null)
				{
					StopButton.gameObject.SetActive(false);
				}
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.AudioPlayer.Progress.ApplyTo(Progress);

			if (PlayButton != null)
			{
				style.AudioPlayer.Play.ApplyTo(PlayButton.gameObject);
			}

			if (PauseButton != null)
			{
				style.AudioPlayer.Pause.ApplyTo(PauseButton.gameObject);
			}

			if (StopButton != null)
			{
				style.AudioPlayer.Stop.ApplyTo(StopButton.gameObject);
			}

			if (ToggleButton != null)
			{
				style.AudioPlayer.Toggle.ApplyTo(ToggleButton.gameObject);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.AudioPlayer.Progress.GetFrom(Progress);

			if (PlayButton != null)
			{
				style.AudioPlayer.Play.GetFrom(PlayButton.gameObject);
			}

			if (PauseButton != null)
			{
				style.AudioPlayer.Pause.GetFrom(PauseButton.gameObject);
			}

			if (StopButton != null)
			{
				style.AudioPlayer.Stop.GetFrom(StopButton.gameObject);
			}

			if (ToggleButton != null)
			{
				style.AudioPlayer.Toggle.GetFrom(ToggleButton.gameObject);
			}

			return true;
		}
		#endregion
	}
}