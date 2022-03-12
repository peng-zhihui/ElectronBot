namespace UIWidgets
{
	using System;
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// FileDialog.
	/// </summary>
	public class FileDialog : Picker<string, FileDialog>, IUpgradeable
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public FileListView FileListView;

		/// <summary>
		/// Confirm Dialog.
		/// </summary>
		[SerializeField]
		public PickerBool ConfirmDialog;

		/// <summary>
		/// Filename Input.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with FilenameInputAdapter.")]
		public InputField FilenameInput;

		/// <summary>
		/// Proxy for InputField.
		/// Required to improve compatibility between different InputFields (like Unity.UI and TextMeshPro versions).
		/// </summary>
		[Obsolete("Replaced with FilenameInputAdapter.")]
		protected IInputFieldProxy FilenameInputProxy
		{
			get
			{
				return FilenameInputAdapter;
			}
		}

		/// <summary>
		/// Filename Input.
		/// </summary>
		[SerializeField]
		public InputFieldAdapter FilenameInputAdapter;

		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		public Button OkButton;

		/// <summary>
		/// Is specified file should exists?
		/// </summary>
		[SerializeField]
		public bool FileShouldExists = false;

		/// <summary>
		/// Request confirmation if file exists?
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("RequestConfirmationIsFileExists")]
		public bool RequestConfirmationIfFileExists = true;

		/// <summary>
		/// Init InputProxy.
		/// </summary>
		protected virtual void InitFilenameInputProxy()
		{
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(string defaultValue)
		{
			InitFilenameInputProxy();
			FileListView.OnSelectObject.AddListener(FileSelected);
			FilenameInputAdapter.onValueChanged.AddListener(FilenameChanged);
			OkButton.onClick.AddListener(OkClick);

			if (!string.IsNullOrEmpty(defaultValue))
			{
				FileListView.CurrentDirectory = Path.GetDirectoryName(defaultValue);
				FileListView.Select(FileListView.FullName2Index(defaultValue));
			}

			FilenameChanged(FilenameInputAdapter.text);
		}

		/// <summary>
		/// Callback when file selected.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void FileSelected(int index)
		{
			FilenameInputAdapter.text = FileListView.SelectedItem.IsFile
				? Path.GetFileName(FileListView.SelectedItem.FullName)
				: string.Empty;
		}

		/// <summary>
		/// Callback when filename changed.
		/// </summary>
		/// <param name="filename">Filename.</param>
		protected virtual void FilenameChanged(string filename)
		{
			OkButton.interactable = IsValidFile(filename);
		}

		/// <summary>
		/// Is file valid?
		/// </summary>
		/// <param name="filename">Filename.</param>
		/// <returns>true if file valid; otherwise, false.</returns>
		protected virtual bool IsValidFile(string filename)
		{
			if (string.IsNullOrEmpty(filename))
			{
				return false;
			}

			var fullname = Path.Combine(FileListView.CurrentDirectory, filename);
			if (FileShouldExists && !File.Exists(fullname))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Handle OK click event.
		/// </summary>
		public void OkClick()
		{
			var fullname = Path.Combine(FileListView.CurrentDirectory, FilenameInputAdapter.text);
			if (RequestConfirmationIfFileExists && File.Exists(fullname))
			{
				var confirm = ConfirmDialog.Clone();
				confirm.Show(false, ConfirmAction, DoNothing);
			}
			else
			{
				Selected(fullname);
			}
		}

		/// <summary>
		/// Process confirmation.
		/// </summary>
		/// <param name="isConfirmed">If action confirmed.</param>
		protected void ConfirmAction(bool isConfirmed)
		{
			if (isConfirmed)
			{
				var fullname = Path.Combine(FileListView.CurrentDirectory, FilenameInputAdapter.text);
				Selected(fullname);
			}
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		protected virtual void DoNothing()
		{
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			FileListView.OnSelectObject.RemoveListener(FileSelected);
			FilenameInputAdapter.onValueChanged.RemoveListener(FilenameChanged);
			OkButton.onClick.RemoveListener(OkClick);
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0612, 0618
			Utilities.GetOrAddComponent(FilenameInput, ref FilenameInputAdapter);
#pragma warning restore 0612, 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			FileListView.SetStyle(style);

			style.InputField.ApplyTo(FilenameInputAdapter);

			style.Dialog.Button.ApplyTo(OkButton.gameObject);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			ConfirmDialog.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			FileListView.GetStyle(style);

			style.InputField.GetFrom(FilenameInputAdapter);

			style.Dialog.Button.GetFrom(OkButton.gameObject);

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			ConfirmDialog.GetStyle(style);

			return true;
		}
		#endregion
	}
}