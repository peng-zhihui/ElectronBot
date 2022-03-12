namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test Date widgets.
	/// </summary>
	public class TestDate : MonoBehaviour
	{
		/// <summary>
		/// Calendar.
		/// </summary>
		[SerializeField]
		protected UIWidgets.DateBase DateBase;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			LocaleEn();
		}

		/// <summary>
		/// Set en-US culture.
		/// </summary>
		public void LocaleEn()
		{
			DateBase.Culture = new System.Globalization.CultureInfo("en-US");
		}

		/// <summary>
		/// Set ja-JP culture.
		/// </summary>
		public void LocaleJp()
		{
			DateBase.Culture = new System.Globalization.CultureInfo("ja-JP");
		}

		/// <summary>
		/// Set fr-FR culture.
		/// </summary>
		public void LocaleFr()
		{
			DateBase.Culture = new System.Globalization.CultureInfo("fr-FR");
		}

		/// <summary>
		/// Set de-DE culture.
		/// </summary>
		public void LocaleDe()
		{
			DateBase.Culture = new System.Globalization.CultureInfo("de-DE");
		}

		/// <summary>
		/// Set zh-CN culture.
		/// </summary>
		public void LocaleCh()
		{
			DateBase.Culture = new System.Globalization.CultureInfo("zh-CN");
		}

		/// <summary>
		/// Set ru-RU culture.
		/// </summary>
		public void LocaleRu()
		{
			DateBase.Culture = new System.Globalization.CultureInfo("ru-RU");
		}

		void SetCalendar(string cultureCode, System.Globalization.Calendar calendar)
		{
			var culture = new System.Globalization.CultureInfo(cultureCode);

			if (System.Array.IndexOf(culture.OptionalCalendars, calendar) == -1)
			{
				Debug.Log(string.Format("Calendar {0} not supported by culture {1}", calendar.GetType().Name, cultureCode));
				return;
			}

			culture.DateTimeFormat.Calendar = calendar;
			DateBase.Culture = culture;

			DateBase.UpdateCalendar();
		}

		/// <summary>
		/// Set gregorian calendar.
		/// </summary>
		public void GregorianCalendar()
		{
			SetCalendar("en-US", new System.Globalization.GregorianCalendar());
		}

		/// <summary>
		/// Set hebrew calendar.
		/// </summary>
		public void HebrewCalendar()
		{
			SetCalendar("he-HE", new System.Globalization.HebrewCalendar());
		}

		/// <summary>
		/// Set korean calendar.
		/// </summary>
		public void KoreanCalendar()
		{
			SetCalendar("ko-KO", new System.Globalization.KoreanCalendar());
		}

		/// <summary>
		/// Set japanese calendar.
		/// </summary>
		public void JapaneseCalendar()
		{
			SetCalendar("ja-JP", new System.Globalization.JapaneseCalendar());
		}

		/// <summary>
		/// Set hijri calendar.
		/// </summary>
		public void HijriCalendar()
		{
			SetCalendar("ar-EG", new System.Globalization.HijriCalendar());
		}

		/// <summary>
		/// Set julian calendar.
		/// </summary>
		public void JulianCalendar()
		{
			SetCalendar("en-US", new System.Globalization.JulianCalendar());
		}

		/// <summary>
		/// Set persian calendar.
		/// </summary>
		public void PersianCalendar()
		{
			SetCalendar("ar-EG", new System.Globalization.PersianCalendar());
		}

		/// <summary>
		/// Set taiwan calendar.
		/// </summary>
		public void TaiwanCalendar()
		{
			SetCalendar("zh-TW", new System.Globalization.TaiwanCalendar());
		}
	}
}