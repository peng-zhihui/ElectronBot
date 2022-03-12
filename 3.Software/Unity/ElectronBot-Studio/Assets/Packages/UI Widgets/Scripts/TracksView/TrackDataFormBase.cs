namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Base class for the data form.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	public abstract class TrackDataFormBase<TData, TPoint> : MonoBehaviour
		where TData : ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Data.
		/// </summary>
		public TData Data
		{
			get;
			protected set;
		}

		/// <summary>
		/// Create data.
		/// </summary>
		public abstract void Create();

		/// <summary>
		/// Create data with specified StartPoint.
		/// </summary>
		/// <param name="startPoint">SpartPoint.</param>
		public abstract void Create(TPoint startPoint);

		/// <summary>
		/// Edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		public abstract void Edit(TData data);

		/// <summary>
		/// Add listeners.
		/// </summary>
		public abstract void AddListeners();

		/// <summary>
		/// Remove listeners.
		/// </summary>
		public abstract void RemoveListeners();

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