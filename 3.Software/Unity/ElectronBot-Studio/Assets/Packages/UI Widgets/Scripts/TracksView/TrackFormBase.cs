namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base class for the track form.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	public abstract class TrackFormBase<TData, TPoint> : MonoBehaviour
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Data.
		/// </summary>
		public Track<TData, TPoint> Data
		{
			get;
			protected set;
		}

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter Name;

		/// <summary>
		/// Process name changed event.
		/// </summary>
		/// <param name="name">Name.</param>
		protected void NameChanged(string name)
		{
			Data.Name = name;
		}

		/// <summary>
		/// Create new data.
		/// </summary>
		public virtual void Create()
		{
			Data = new Track<TData, TPoint>();

			SetValues();
		}

		/// <summary>
		/// Edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		public virtual void Edit(Track<TData, TPoint> data)
		{
			Data = new Track<TData, TPoint>();

			data.CopyTo(Data);

			SetValues();
		}

		/// <summary>
		/// Set values.
		/// </summary>
		protected virtual void SetValues()
		{
			Name.Value = Data.Name;
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		public virtual void AddListeners()
		{
			if (Name != null)
			{
				Name.onValueChanged.AddListener(NameChanged);
				Name.onEndEdit.AddListener(NameChanged);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		public virtual void RemoveListeners()
		{
			if (Name != null)
			{
				Name.onValueChanged.RemoveListener(NameChanged);
				Name.onEndEdit.RemoveListener(NameChanged);
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			AddListeners();
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}
	}
}