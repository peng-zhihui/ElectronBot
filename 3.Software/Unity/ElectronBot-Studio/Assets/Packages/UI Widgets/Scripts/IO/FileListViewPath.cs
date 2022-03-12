namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// FileListViewPath.
	/// </summary>
	public class FileListViewPath : MonoBehaviour, IStylable
	{
		/// <summary>
		/// FileView.
		/// </summary>
		[HideInInspector]
		public FileListView FileView;

		/// <summary>
		/// Current path.
		/// </summary>
		protected string path;

		/// <summary>
		/// Current path.
		/// </summary>
		public string Path
		{
			get
			{
				return path;
			}

			set
			{
				SetPath(value);
			}
		}

		/// <summary>
		/// FileListViewPathComponent template.
		/// </summary>
		[SerializeField]
		public FileListViewPathComponentBase Template;

		/// <summary>
		/// Used components.
		/// </summary>
		[HideInInspector]
		protected List<FileListViewPathComponentBase> Components = new List<FileListViewPathComponentBase>();

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			Template.gameObject.SetActive(false);
		}

		/// <summary>
		/// Directories list from current to root.
		/// </summary>
		protected List<string> CurrentDirectories = new List<string>();

		/// <summary>
		/// Set path.
		/// </summary>
		/// <param name="newPath">New path.</param>
		protected virtual void SetPath(string newPath)
		{
			path = newPath;

			CurrentDirectories.Clear();
			do
			{
				CurrentDirectories.Add(newPath);
				newPath = System.IO.Path.GetDirectoryName(newPath);
			}
			while (!string.IsNullOrEmpty(newPath));
			CurrentDirectories.Reverse();

			for (int i = Components.Count - 1; i >= CurrentDirectories.Count; i--)
			{
				var c = Components[i];
				Components.RemoveAt(i);
				c.Owner = null;
				c.Free();
			}

			for (int i = Components.Count; i < CurrentDirectories.Count; i++)
			{
				Components.Add(Template.Instance());
			}

			for (int i = 0; i < CurrentDirectories.Count; i++)
			{
				SetCurrentDirectories(CurrentDirectories[i], i);
			}
		}

		/// <summary>
		/// Set current directories.
		/// </summary>
		/// <param name="path">Path.</param>
		/// <param name="index">Index of the component.</param>
		protected void SetCurrentDirectories(string path, int index)
		{
			Components[index].Owner = this;
			Components[index].SetPath(path);
		}

		/// <summary>
		/// Open directory.
		/// </summary>
		/// <param name="directory">Directory.</param>
		public virtual void Open(string directory)
		{
			var index = CurrentDirectories.IndexOf(directory);
			var select_directory = index == (CurrentDirectories.Count - 1) ? string.Empty : CurrentDirectories[index + 1];
			FileView.CurrentDirectory = directory;
			if (!string.IsNullOrEmpty(select_directory))
			{
				FileView.Select(FileView.FullName2Index(select_directory));
			}
		}

		/// <summary>
		/// Destroy created components.
		/// </summary>
		protected virtual void OnDestroy()
		{
			foreach (var c in Components)
			{
				FreeComponent(c);
			}

			Components.Clear();
		}

		/// <summary>
		/// Free component.
		/// </summary>
		/// <param name="component">Component.</param>
		protected virtual void FreeComponent(FileListViewPathComponentBase component)
		{
			component.Owner = null;
			component.Free();
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (Template != null)
			{
				Template.SetStyle(style.FileListView.PathItemBackground, style.FileListView.PathItemText, style);
			}

			for (int i = 0; i < Components.Count; i++)
			{
				Components[i].SetStyle(style.FileListView.PathItemBackground, style.FileListView.PathItemText, style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (Template != null)
			{
				Template.GetStyle(style.FileListView.PathItemBackground, style.FileListView.PathItemText, style);
			}

			return true;
		}
		#endregion
	}
}