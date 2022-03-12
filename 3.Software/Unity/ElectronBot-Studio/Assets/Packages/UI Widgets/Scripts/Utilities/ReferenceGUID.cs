#if UNITY_EDITOR
namespace UIWidgets
{
	/// <summary>
	/// GUID list.
	/// </summary>
	public static class ReferenceGUID
	{
		/// <summary>
		/// Assemblies required TMPro reference if support enabled.
		/// </summary>
		public static readonly string[] TMProRequiredAssemblies = new string[]
		{
			"9b890130495c8904b9dcfaf28273b376", // New UI Widgets/Editor/UIWidgets.Editor.asmdef
			"6db589689499ad14c8b33ef4478bb29d", // New UI Widgets/Examples/Editor/UIWidgets.Examples.Editor.asmdef
			"ce661a99e9e51a34eb04bd42897d8e4c", // New UI Widgets/Examples/UIWidgets.Examples.asmdef
			"8158a7b0409ed1b49a27393c5d96ca43", // New UI Widgets/Scripts/Style/Editor/UIWidgets.Styles.Editor.asmdef
			"77b640ebe03a1194598db26ced37dd6e", // New UI Widgets/Scripts/UIWidgets.asmdef
		};

		/// <summary>
		/// File with TMPro support recompile status.
		/// </summary>
		public static readonly string TMProStatus = "8b1fbe80f7518be41872eadbd10d7067";

		/// <summary>
		/// Directories and files to recompile when TMPro support enabled.
		/// </summary>
		public static readonly string[] TMProSupport = new string[]
		{
			"91c23f731032c0649b92539754ca1c35", // Scripts\Converter\*
			"cd38b3043ce53d54b8f534f4de095a75", // Scripts\ThirdPartySupport\TMProSupport\*

			"92953f60ab285fd429ee7c0a7944c4b7", // Scripts\InputField\InputFieldAdapter.cs
			"98d84927b0690ab40afffcb089ec81a8", // Scripts\InputField\InputFieldExtendedAdapter.cs
			"84efaef737bf5184ba72224e49593615", // Scripts\InputField\InputFieldTMProProxy.cs
			"fef7da31ec25ead469faea1d77addefa", // Scripts\Spinner\SpinnerBase.cs
			"e6068bbda50527f419fe2c3a5af4d62b", // Scripts\Style\Style.cs
			"2483f16ccad27d64f8474c0f973c6a3c", // Scripts\Style\Fast\StyleFast.cs
			"6ce2adbf95e74fb46b112b2a9f122158", // Scripts\Style\Unity\StyleInputField.cs
			"1fa840be3ac537041af432e36e1da4b2", // Scripts\Style\Unity\StyleText.cs
			"52e6b3c9e0434ff43bb4f3722a0b0a57", // Scripts\Text\TextAdapter.cs
			"1752ed17982fef34c979ad689810dd7a", // Scripts\Text\TextTMPro.cs
			"0627250e80fa5bc4180d15dab4780e95", // Scripts\WidgetGeneration\Editor\ClassInfo.cs
			"91cef37d757e4b347aa34861c46f395b", // Scripts\WidgetGeneration\Editor\PrefabGenerator.cs
		};

		/// <summary>
		/// Foldes with DataBind support files.
		/// </summary>
		public static readonly string DataBindFolder = "8c23c22a14c225149bd9bf7d5b69ae29";

		/// <summary>
		/// File with DataBind support recompile status.
		/// </summary>
		public static readonly string DataBindStatus = "3bcbe5da50ac7f24db9b4db908dcc110";

		/// <summary>
		/// Directories and files to recompile when DataBind support enabled.
		/// </summary>
		public static readonly string[] DataBindSupport = new string[]
		{
			"8c23c22a14c225149bd9bf7d5b69ae29", // Scripts\ThirdPartySupport\DataBindSupport\*
			"18f783f02c29bd3448684141f9d2ff3d", // Scripts\WidgetGeneration\DataBindSupport\*
			"91cef37d757e4b347aa34861c46f395b", // Scripts\WidgetGeneration\Editor\PrefabGenerator.cs
		};

		/// <summary>
		/// File with I2 Localization support recompile status.
		/// </summary>
		public static readonly string I2LocalizationStatus = "8be3e66b21a4bf8489e41fda2bda8781";

		/// <summary>
		/// Directories and files to recompile when I2 Localization support enabled.
		/// </summary>
		public static readonly string[] I2LocalizationSupport = new string[]
		{
			"8d1e8758d46c0db419781b23554c9e48", // Scripts\Localization\Localization.cs
		};

		/// <summary>
		/// Scripts folder.
		/// </summary>
		public static readonly string ScriptsFolder = "1a0ae82418872774d8240b5bc4df7d06";

		/// <summary>
		/// Asset with references to the prefabs used by menu.
		/// </summary>
		public static readonly string PrefabsMenu = "6a9100cba93b8194b974a6bf0e54197a";

		/// <summary>
		/// Asset with references to the prefabs templates used by widget generation.
		/// </summary>
		public static readonly string PrefabsTemplates = "6e48cc01edc8c384c8986c32d6343834";

		/// <summary>
		/// Asset with references to the scripts templates used by widget generation.
		/// </summary>
		public static readonly string ScriptsTemplates = "87c41e4b0b1f0b24aacf776a72df2626";

		/// <summary>
		/// Asset with references to the DataBind scripts templates used by DataBind support.
		/// </summary>
		public static readonly string DataBindTemplates = "48d6be4465afc704fad81633397bf474";
	}
}
#endif