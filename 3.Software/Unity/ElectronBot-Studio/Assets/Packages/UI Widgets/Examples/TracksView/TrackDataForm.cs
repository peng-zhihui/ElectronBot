namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Form to add/edit data.
	/// </summary>
	public class TrackDataForm : TrackDataFormBase<TrackData, DateTime>
	{
		/// <summary>
		/// DateTime picker.
		/// </summary>
		[SerializeField]
		protected DateTimePicker Picker;

		/// <summary>
		/// Text to display StartPoint.
		/// </summary>
		[SerializeField]
		protected TextAdapter StartPoint;

		/// <summary>
		/// Text to display EndPoint.
		/// </summary>
		[SerializeField]
		protected TextAdapter EndPoint;

		/// <summary>
		/// Button to change StartPoint.
		/// </summary>
		[SerializeField]
		protected Button StartPointChange;

		/// <summary>
		/// Button to change EndPoint.
		/// </summary>
		[SerializeField]
		protected Button EndPointChange;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter Name;

		/// <summary>
		/// Description.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter Description;

		/// <summary>
		/// Date format.
		/// </summary>
		[SerializeField]
		protected string DateFormat = "dd MM yyyy HH:mm:ss";

		/// <summary>
		/// Create data.
		/// </summary>
		public override void Create()
		{
			Data = new TrackData();
		}

		/// <summary>
		/// Create data with specified StartPoint.
		/// </summary>
		/// <param name="startPoint">SpartPoint.</param>
		public override void Create(DateTime startPoint)
		{
			Data = new TrackData()
			{
				StartPoint = startPoint,
				EndPoint = startPoint.AddDays(+1),
			};

			SetValues();
		}

		/// <summary>
		/// Edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		public override void Edit(TrackData data)
		{
			Data = new TrackData();

			data.CopyTo(Data);

			SetValues();
		}

		/// <summary>
		/// Set values.
		/// </summary>
		protected virtual void SetValues()
		{
			StartPoint.Value = Data.StartPoint.ToString(DateFormat, UtilitiesCompare.Culture);
			EndPoint.Value = Data.EndPoint.ToString(DateFormat, UtilitiesCompare.Culture);
			Name.Value = Data.Name;
			Description.Value = Data.Description;
		}

		/// <summary>
		/// Process name changed event.
		/// </summary>
		/// <param name="name">Name.</param>
		protected void NameChanged(string name)
		{
			Data.Name = name;
		}

		/// <summary>
		/// Process description changed event.
		/// </summary>
		/// <param name="description">Description.</param>
		protected void DescriptionChanged(string description)
		{
			Data.Description = description;
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void AddListeners()
		{
			if (StartPointChange != null)
			{
				StartPointChange.onClick.AddListener(OpenStartPointPicker);
			}

			if (EndPointChange != null)
			{
				EndPointChange.onClick.AddListener(OpenEndPointPicker);
			}

			if (Name != null)
			{
				Name.onValueChanged.AddListener(NameChanged);
				Name.onEndEdit.AddListener(NameChanged);
			}

			if (Description != null)
			{
				Description.onValueChanged.AddListener(DescriptionChanged);
				Description.onEndEdit.AddListener(DescriptionChanged);
			}
		}

		/// <summary>
		/// Open picker to change StartPoint.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OpenStartPointPicker()
		{
			Picker.Clone().Show(Data.StartPoint, StartPointChanged);
		}

		/// <summary>
		/// Process StartPoint changed event.
		/// </summary>
		/// <param name="dt">DateTime.</param>
		protected void StartPointChanged(DateTime dt)
		{
			Data.StartPoint = dt;
			StartPoint.Value = Data.StartPoint.ToString(DateFormat);
		}

		/// <summary>
		/// Open picker to change EndPoint.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OpenEndPointPicker()
		{
			Picker.Clone().Show(Data.EndPoint, OnEndPointChange);
		}

		/// <summary>
		/// Process EndPoint changed event.
		/// </summary>
		/// <param name="dt">DateTime.</param>
		protected void OnEndPointChange(DateTime dt)
		{
			Data.EndPoint = dt;
			EndPoint.Value = Data.EndPoint.ToString(DateFormat);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public override void RemoveListeners()
		{
			if (StartPointChange != null)
			{
				StartPointChange.onClick.RemoveListener(OpenStartPointPicker);
			}

			if (EndPointChange != null)
			{
				EndPointChange.onClick.RemoveListener(OpenEndPointPicker);
			}

			if (Name != null)
			{
				Name.onValueChanged.RemoveListener(NameChanged);
				Name.onEndEdit.RemoveListener(NameChanged);
			}

			if (Description != null)
			{
				Description.onValueChanged.RemoveListener(DescriptionChanged);
				Description.onEndEdit.RemoveListener(DescriptionChanged);
			}
		}
	}
}