#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	/// <summary>
	/// Converter functions to replace component with another component.
	/// </summary>
	public partial class ConverterTMPro
	{
		/// <summary>
		/// Message class.
		/// </summary>
		public class Message
		{
			/// <summary>
			/// Message text.
			/// </summary>
			public readonly string Info;

			/// <summary>
			/// Target.
			/// </summary>
			public readonly UnityEngine.Object Target;

			/// <summary>
			/// Initializes a new instance of the <see cref="UIWidgets.ConverterTMPro.Message"/> class.
			/// </summary>
			/// <param name="info">Message info.</param>
			public Message(string info)
			{
				Info = info;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="UIWidgets.ConverterTMPro.Message"/> class.
			/// </summary>
			/// <param name="info">Message info.</param>
			/// <param name="target">Target.</param>
			public Message(string info, UnityEngine.Object target)
			{
				Info = info;
				Target = target;
			}
		}
	}
}
#endif