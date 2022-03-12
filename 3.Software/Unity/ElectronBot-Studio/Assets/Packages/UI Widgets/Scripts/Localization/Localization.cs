namespace UIWidgets.l10n
{
	using System;
	using System.Globalization;
	using UnityEngine;

	/// <summary>
	/// Locale changed event handler.
	/// </summary>
	public delegate void LocaleChangedEventHandler();

	/// <summary>
	/// Localization class.
	/// </summary>
	public static class Localization
	{
		/// <summary>
		/// Locale changed event.
		/// </summary>
		public static event LocaleChangedEventHandler OnLocaleChanged = DoNothing;

		/// <summary>
		/// Translate the specified string.
		/// </summary>
		public static Func<string, string> GetTranslation;

		/// <summary>
		/// Get country code.
		/// Used to instantiate System.Globalization.CultureInfo.
		/// </summary>
		/// <returns>Country code.</returns>
		public static Func<string> GetCountryCode;

		static CultureInfo сulture;

		/// <summary>
		/// Get culture info (based on country code).
		/// </summary>
		public static CultureInfo Culture
		{
			get
			{
				if (сulture == null)
				{
					var code = GetCountryCode();
					сulture = GetCulture(code);
				}

				return сulture;
			}
		}

		/// <summary>
		/// Get culture.
		/// </summary>
		public static Func<string, CultureInfo> GetCulture = DefaultGetCulture;

		/// <summary>
		/// Default implementation of get culture.
		/// </summary>
		/// <param name="code">Country code.</param>
		/// <returns>Culture.</returns>
		public static CultureInfo DefaultGetCulture(string code)
		{
			if (string.IsNullOrEmpty(code))
			{
				return CultureInfo.InvariantCulture;
			}

			var new_code = string.Format("{0}-{1}", code, code.ToUpperInvariant());
			try
			{
				var culture = new CultureInfo(code);
				if (culture.IsNeutralCulture)
				{
					return new CultureInfo(new_code);
				}
			}
			catch (NotSupportedException)
			{
				try
				{
					return new CultureInfo(new_code);
				}
				catch (NotSupportedException)
				{
				}
			}

			return CultureInfo.InvariantCulture;
		}

#if I2_LOCALIZATION_SUPPORT
		/// <summary>
		/// Translate the input string using I2 Localization.
		/// </summary>
		/// <param name="input">String to translate.</param>
		/// <returns>Translated string.</returns>
		public static string I2Translation(string input)
		{
			var result = I2.Loc.LocalizationManager.GetTranslation(input);
			return (result != null) ? result : input;
		}

		/// <summary>
		/// Get country code of the current locale using I2 Localization.
		/// </summary>
		/// <returns>Country code.</returns>
		public static string I2CountryCode()
		{
			return I2.Loc.LocalizationManager.CurrentLanguageCode;
		}
#endif

		static Localization()
		{
			Init();
		}

		#if UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		#endif
		static void Init()
		{
			сulture = null;
			GetCulture = DefaultGetCulture;

#if I2_LOCALIZATION_SUPPORT
			GetTranslation = I2Translation;
			GetCountryCode = I2CountryCode;
			I2.Loc.LocalizationManager.OnLocalizeEvent += LocaleChanged;
#else
			GetTranslation = NoTranslation;
			GetCountryCode = DefaultCountryCode;
#endif
		}

		/// <summary>
		/// Invoke locale changed event.
		/// </summary>
		public static void LocaleChanged()
		{
			сulture = null;
			OnLocaleChanged.Invoke();
		}

		/// <summary>
		/// Do nothing.
		/// </summary>
		static void DoNothing()
		{
		}

		/// <summary>
		/// Get default country code.
		/// Used to instantiate System.Globalization.CultureInfo.
		/// </summary>
		/// <returns>Country code.</returns>
		public static string DefaultCountryCode()
		{
			return null;
		}

		/// <summary>
		/// Return the input string without translation.
		/// </summary>
		/// <param name="input">String to translate.</param>
		/// <returns>Translated string.</returns>
		public static string NoTranslation(string input)
		{
			return input;
		}
	}
}