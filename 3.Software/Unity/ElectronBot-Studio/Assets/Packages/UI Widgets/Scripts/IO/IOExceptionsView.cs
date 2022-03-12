namespace UIWidgets
{
	using System;
	using System.IO;
	using System.Security;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// IOExceptionsView.
	/// Handle IO exceptions - catch exception and display following errors.
	/// </summary>
	public class IOExceptionsView : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Invalid argument error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorArgument;

		/// <summary>
		/// Long path error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorLongPath;

		/// <summary>
		/// Unauthorized access error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorUnauthorizedAccess;

		/// <summary>
		/// Security error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorSecurity;

		/// <summary>
		/// Directory not found error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorDirectoryNotFound;

		/// <summary>
		/// IO error message.
		/// </summary>
		[SerializeField]
		public GameObject ErrorIO;

		/// <summary>
		/// Current error message.
		/// </summary>
		protected GameObject currentError;

		/// <summary>
		/// Current error message.
		/// </summary>
		public virtual GameObject CurrentError
		{
			get
			{
				return currentError;
			}

			set
			{
				if (currentError != null)
				{
					currentError.gameObject.SetActive(false);
				}

				currentError = value;

				if (currentError != null)
				{
					currentError.gameObject.SetActive(true);
				}
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
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			ErrorArgument.SetActive(false);
			ErrorLongPath.SetActive(false);
			ErrorUnauthorizedAccess.SetActive(false);
			ErrorSecurity.SetActive(false);
			ErrorDirectoryNotFound.SetActive(false);
			ErrorIO.SetActive(false);
		}

		/// <summary>
		/// Execute action and handle exceptions if raised.
		/// </summary>
		/// <typeparam name="TResult">Type of the result.</typeparam>
		/// <typeparam name="TArgument">Type of the function argument.</typeparam>
		/// <param name="func">Function.</param>
		/// <param name="argument">Function argument.</param>
		/// <returns>Function result.</returns>
		public virtual TResult Execute<TResult, TArgument>(Action<TResult, TArgument> func, TArgument argument)
			where TResult : class, new()
		{
			Init();
			CurrentError = null;

			var result = new TResult();
			try
			{
				func(result, argument);
			}
			catch (UnauthorizedAccessException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorUnauthorizedAccess;
			}
			catch (SecurityException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorSecurity;
			}
			catch (DirectoryNotFoundException)
			{
				// The path encapsulated in the DirectoryInfo object is invalid, such as being on an unmapped drive.
				CurrentError = ErrorDirectoryNotFound;
			}
			catch (PathTooLongException)
			{
				// The specified path, file name, or both exceed the system-defined maximum length.
				// For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
				CurrentError = ErrorLongPath;
			}
			catch (IOException)
			{
				// Path is a file name -or- network error has occurred.
				CurrentError = ErrorIO;
			}
			catch (ArgumentNullException)
			{
				// Path is null.
				CurrentError = ErrorArgument;
			}
			catch (ArgumentException)
			{
				// Path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the GetInvalidPathChars method.
				CurrentError = ErrorArgument;
			}

			return result;
		}

		/// <summary>
		/// Execute action and handle exceptions if raised.
		/// </summary>
		/// <typeparam name="TResult">Type of result.</typeparam>
		/// <param name="func">Function.</param>
		/// <returns>Function result.</returns>
		public virtual TResult Execute<TResult>(Action<TResult> func)
			where TResult : class, new()
		{
			Init();
			CurrentError = null;

			var result = new TResult();
			try
			{
				func(result);
			}
			catch (UnauthorizedAccessException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorUnauthorizedAccess;
			}
			catch (SecurityException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorSecurity;
			}
			catch (DirectoryNotFoundException)
			{
				// The path encapsulated in the DirectoryInfo object is invalid, such as being on an unmapped drive.
				CurrentError = ErrorDirectoryNotFound;
			}
			catch (PathTooLongException)
			{
				// The specified path, file name, or both exceed the system-defined maximum length.
				// For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
				CurrentError = ErrorLongPath;
			}
			catch (IOException)
			{
				// Path is a file name -or- network error has occurred.
				CurrentError = ErrorIO;
			}
			catch (ArgumentNullException)
			{
				// Path is null.
				CurrentError = ErrorArgument;
			}
			catch (ArgumentException)
			{
				// Path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the GetInvalidPathChars method.
				CurrentError = ErrorArgument;
			}

			return result;
		}

		/// <summary>
		/// Execute action and handle exceptions if raised.
		/// </summary>
		/// <param name="action">Action.</param>
		public virtual void Execute(Action action)
		{
			Init();
			CurrentError = null;

			try
			{
				action();
			}
			catch (UnauthorizedAccessException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorUnauthorizedAccess;
			}
			catch (SecurityException)
			{
				// The caller does not have the required permission.
				CurrentError = ErrorSecurity;
			}
			catch (DirectoryNotFoundException)
			{
				// The path encapsulated in the DirectoryInfo object is invalid, such as being on an unmapped drive.
				CurrentError = ErrorDirectoryNotFound;
			}
			catch (PathTooLongException)
			{
				// The specified path, file name, or both exceed the system-defined maximum length.
				// For example, on Windows-based platforms, paths must be less than 248 characters and file names must be less than 260 characters.
				CurrentError = ErrorLongPath;
			}
			catch (IOException)
			{
				// Path is a file name -or- network error has occurred.
				CurrentError = ErrorIO;
			}
			catch (ArgumentNullException)
			{
				// Path is null.
				CurrentError = ErrorArgument;
			}
			catch (ArgumentException)
			{
				// Path is a zero-length string, contains only white space, or contains one or more invalid characters. You can query for invalid characters by using the GetInvalidPathChars method.
				CurrentError = ErrorArgument;
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.IOCollectionsErrors.ApplyTo(ErrorArgument);
			style.IOCollectionsErrors.ApplyTo(ErrorLongPath);
			style.IOCollectionsErrors.ApplyTo(ErrorUnauthorizedAccess);
			style.IOCollectionsErrors.ApplyTo(ErrorSecurity);
			style.IOCollectionsErrors.ApplyTo(ErrorDirectoryNotFound);
			style.IOCollectionsErrors.ApplyTo(ErrorIO);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.IOCollectionsErrors.GetFrom(ErrorArgument);
			style.IOCollectionsErrors.GetFrom(ErrorLongPath);
			style.IOCollectionsErrors.GetFrom(ErrorUnauthorizedAccess);
			style.IOCollectionsErrors.GetFrom(ErrorSecurity);
			style.IOCollectionsErrors.GetFrom(ErrorDirectoryNotFound);
			style.IOCollectionsErrors.GetFrom(ErrorIO);

			return true;
		}
		#endregion
	}
}