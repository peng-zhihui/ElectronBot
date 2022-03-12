namespace UIWidgets
{
	using System;
	using System.IO;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// FileView.
	/// </summary>
	public class FileListView : TileViewCustomSize<FileListViewComponentBase, FileSystemEntry>
	{
		/// <summary>
		/// Pattern separators.
		/// </summary>
		protected static char[] PatternSeparators = new char[] { ';' };

		/// <summary>
		/// Current directory.
		/// </summary>
		[SerializeField]
		protected string currentDirectory;

		/// <summary>
		/// Current directory.
		/// </summary>
		public string CurrentDirectory
		{
			get
			{
				return currentDirectory;
			}

			set
			{
				SetCurrentDirectory(value);
			}
		}

		/// <summary>
		/// Directory patterns.
		/// </summary>
		[SerializeField]
		protected string directoryPatterns = string.Empty;

		/// <summary>
		/// Gets or sets the directory patterns, semicolon used as separator.
		/// Directory will be displayed if it's match one of the pattern.
		/// Wild-cards:
		/// * - Zero or more characters in that position.
		/// ? - Zero or one character in that position.
		/// Warning: if directory match two or more patterns it will be displayed two or more times.
		/// </summary>
		/// <value>The directory patterns.</value>
		public string DirectoryPatterns
		{
			get
			{
				return directoryPatterns;
			}

			set
			{
				directoryPatterns = value;
			}
		}

		/// <summary>
		/// File patterns.
		/// </summary>
		[SerializeField]
		protected string filePatterns = string.Empty;

		/// <summary>
		/// Gets or sets the file patterns, semicolon used as separator between patterns.
		/// File will be displayed if it's match one of the pattern.
		/// Wild-cards:
		/// * - Zero or more characters in that position.
		/// ? - Zero or one character in that position.
		/// Warning: if file match two or more patterns it will be displayed two or more times.
		/// </summary>
		/// <value>The files patterns.</value>
		public string FilePatterns
		{
			get
			{
				return filePatterns;
			}

			set
			{
				filePatterns = value;
			}
		}

		/// <summary>
		/// Button Up.
		/// Open parent directory.
		/// </summary>
		[SerializeField]
		protected Button ButtonUp;

		/// <summary>
		/// Button to toggle DriversList.
		/// </summary>
		[SerializeField]
		protected Button ButtonToggleDrivers;

		/// <summary>
		/// FileListViewPath.
		/// Display path.
		/// </summary>
		[SerializeField]
		protected FileListViewPath PathView;

		/// <summary>
		/// DrivesListView.
		/// </summary>
		[SerializeField]
		protected DrivesListView DrivesListView;

		/// <summary>
		/// Display IO errors.
		/// </summary>
		[SerializeField]
		public IOExceptionsView ExceptionsView;

		/// <summary>
		/// Can display file system entry?
		/// </summary>
		public Func<FileSystemEntry, bool> CanDisplayEntry = DisplayAll;

		/// <summary>
		/// Default comparison.
		/// </summary>
		/// <param name="x">First FileSystemEntry.</param>
		/// <param name="y">Seconds FileSystemEntry.</param>
		/// <returns>Result of the comparison.</returns>
		protected virtual int ComparisonDefault(FileSystemEntry x, FileSystemEntry y)
		{
			if (x.IsFile == y.IsFile)
			{
				return UtilitiesCompare.Compare(x.DisplayName, y.DisplayName);
			}

			return x.IsFile.CompareTo(y.IsFile);
		}

		bool isInited;

		/// <summary>
		/// Init and adds listeners.
		/// </summary>
		public override void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			base.Init();

			DataSource.Comparison = ComparisonDefault;

			SetCurrentDirectory(currentDirectory);

			if (ButtonUp != null)
			{
				ButtonUp.onClick.AddListener(Up);
			}

			if (ButtonToggleDrivers != null)
			{
				ButtonToggleDrivers.onClick.AddListener(DrivesListView.Toggle);
			}

			if (DrivesListView != null)
			{
				DrivesListView.OnSelectObject.AddListener(ChangeDrive);
				DrivesListView.FileListView = this;
				DrivesListView.gameObject.SetActive(false);
			}

			ItemsEvents.DoubleClick.AddListener(ProcessDoubleClick);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected override void OnDestroy()
		{
			if (ButtonUp != null)
			{
				ButtonUp.onClick.RemoveListener(Up);
			}

			if (DrivesListView != null)
			{
				DrivesListView.OnSelectObject.RemoveListener(ChangeDrive);
			}

			ItemsEvents.DoubleClick.RemoveListener(ProcessDoubleClick);

			base.OnDestroy();
		}

		/// <summary>
		/// Callback when drive changed.
		/// </summary>
		/// <param name="index">Drive index.</param>
		protected virtual void ChangeDrive(int index)
		{
			CurrentDirectory = DrivesListView.DataSource[index].FullName;
			DrivesListView.Close();
		}

		/// <summary>
		/// Open parent directory.
		/// </summary>
		public virtual void Up()
		{
			var current = CurrentDirectory;
			var directory = Path.GetDirectoryName(current);
			if (!string.IsNullOrEmpty(directory))
			{
				CurrentDirectory = directory;
				Select(FullName2Index(current));
			}
		}

		/// <summary>
		/// Set current directory.
		/// </summary>
		/// <param name="directory">New directory.</param>
		protected virtual void SetCurrentDirectory(string directory)
		{
			currentDirectory = Path.GetFullPath(string.IsNullOrEmpty(directory) ? Application.persistentDataPath : directory);

			if (ButtonUp != null)
			{
				ButtonUp.gameObject.SetActive(!string.IsNullOrEmpty(Path.GetDirectoryName(CurrentDirectory)));
			}

			if (PathView != null)
			{
				PathView.FileView = this;
				PathView.Path = currentDirectory;
			}

			DataSource.BeginUpdate();
			DataSource.Clear();

			try
			{
				ExceptionsView.Execute(GetFiles);
			}
			finally
			{
				DataSource.EndUpdate();
			}
		}

		/// <summary>
		/// Get files.
		/// </summary>
		protected virtual void GetFiles()
		{
			if (!string.IsNullOrEmpty(directoryPatterns))
			{
				var patterns = directoryPatterns.Split(PatternSeparators);
				for (int i = 0; i < patterns.Length; i++)
				{
					var dirs = Directory.GetDirectories(currentDirectory, patterns[i]);
					foreach (var dir in dirs)
					{
						AddDirectory(dir);
					}
				}
			}
			else
			{
				var dirs = Directory.GetDirectories(currentDirectory);
				foreach (var dir in dirs)
				{
					AddDirectory(dir);
				}
			}

			if (!string.IsNullOrEmpty(filePatterns))
			{
				var patterns = filePatterns.Split(PatternSeparators);
				for (int i = 0; i < patterns.Length; i++)
				{
					var files = Directory.GetFiles(currentDirectory, patterns[i]);
					foreach (var file in files)
					{
						AddFile(file);
					}
				}
			}
			else
			{
				var files = Directory.GetFiles(currentDirectory);
				foreach (var file in files)
				{
					AddFile(file);
				}
			}
		}

		/// <summary>
		/// Add directory to DataSource.
		/// </summary>
		/// <param name="directory">Directory.</param>
		protected virtual void AddDirectory(string directory)
		{
			var item = new FileSystemEntry(directory, Path.GetFileName(directory), false);
			if (CanDisplayEntry(item))
			{
				DataSource.Add(item);
			}
		}

		/// <summary>
		/// Add files DataSource.
		/// </summary>
		/// <param name="file">File.</param>
		protected virtual void AddFile(string file)
		{
			var item = new FileSystemEntry(file, Path.GetFileName(file), true);
			if (CanDisplayEntry(item))
			{
				DataSource.Add(item);
			}
		}

		/// <summary>
		/// Get index by full name.
		/// </summary>
		/// <param name="fullname">Full name</param>
		/// <returns>Index.</returns>
		public int FullName2Index(string fullname)
		{
			for (int index = 0; index < DataSource.Count; index++)
			{
				if (DataSource[index].FullName == fullname)
				{
					return index;
				}
			}

			return -1;
		}

		int doubleClickFrame = -1;

		/// <summary>
		/// Handle double click event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Item.</param>
		/// <param name="eventData">Event data.</param>
		protected virtual void ProcessDoubleClick(int index, ListViewItem component, PointerEventData eventData)
		{
			if (doubleClickFrame == UtilitiesTime.GetFrameCount())
			{
				return;
			}

			doubleClickFrame = UtilitiesTime.GetFrameCount();

			var item = DataSource[index];
			if (item.IsDirectory && (CurrentDirectory != item.FullName))
			{
				CurrentDirectory = item.FullName;
			}
		}

		#region Display

		/// <summary>
		/// Display all file system entry.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true.</returns>
		public static bool DisplayAll(FileSystemEntry item)
		{
			return true;
		}

		/// <summary>
		/// Display only directories.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item is directory; otherwise, false.</returns>
		public static bool DisplayOnlyDirectories(FileSystemEntry item)
		{
			return item.IsDirectory;
		}

		/// <summary>
		/// Display only files.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item is file; otherwise, false.</returns>
		public static bool DisplayOnlyFiles(FileSystemEntry item)
		{
			return item.IsFile;
		}
		#endregion

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			if (ExceptionsView != null)
			{
				ExceptionsView.SetStyle(style);
			}

			if (ButtonUp != null)
			{
				style.FileListView.ButtonUp.ApplyTo(ButtonUp.GetComponent<Image>());
			}

			if (ButtonToggleDrivers != null)
			{
				style.FileListView.ButtonToggle.ApplyTo(ButtonToggleDrivers.GetComponent<Image>());
			}

			if (DrivesListView != null)
			{
				DrivesListView.SetStyle(style);
			}

			if (PathView != null)
			{
				PathView.SetStyle(style);
			}

			return base.SetStyle(style);
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			if (ExceptionsView != null)
			{
				ExceptionsView.GetStyle(style);
			}

			if (ButtonUp != null)
			{
				style.FileListView.ButtonUp.GetFrom(ButtonUp.GetComponent<Image>());
			}

			if (ButtonToggleDrivers != null)
			{
				style.FileListView.ButtonToggle.GetFrom(ButtonToggleDrivers.GetComponent<Image>());
			}

			if (DrivesListView != null)
			{
				DrivesListView.GetStyle(style);
			}

			if (PathView != null)
			{
				PathView.GetStyle(style);
			}

			return base.GetStyle(style);
		}
		#endregion
	}
}