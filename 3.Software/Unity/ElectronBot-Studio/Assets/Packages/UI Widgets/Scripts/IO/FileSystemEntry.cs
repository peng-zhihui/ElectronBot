namespace UIWidgets
{
	/// <summary>
	/// FileSystem entry.
	/// </summary>
	public class FileSystemEntry
	{
		/// <summary>
		/// Full name.
		/// </summary>
		public string FullName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Name to display.
		/// </summary>
		public string DisplayName
		{
			get;
			protected set;
		}

		/// <summary>
		/// Is entry is directory?
		/// </summary>
		public bool IsDirectory
		{
			get;
			protected set;
		}

		/// <summary>
		/// is entry is file?
		/// </summary>
		public bool IsFile
		{
			get;
			protected set;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileSystemEntry"/> class.
		/// </summary>
		/// <param name="fullName">Full name.</param>
		/// <param name="displayName">Name to display.</param>
		/// <param name="isFile">Entry is file.</param>
		public FileSystemEntry(string fullName, string displayName, bool isFile)
		{
			FullName = fullName;
			DisplayName = displayName;
			IsDirectory = !isFile;
			IsFile = isFile;
		}

		/// <summary>
		/// Convert instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return FullName;
		}
	}
}