namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// ChatView.
	/// </summary>
	public class ChatView : ListViewCustomHeight<ChatLineComponent, ChatLine>
	{
		/// <summary>
		/// Template selector.
		/// </summary>
		protected class Selector : IListViewTemplateSelector<ChatLineComponent, ChatLine>
		{
			/// <summary>
			/// Incoming template.
			/// </summary>
			public ChatLineComponent IncomingTemplate;

			/// <summary>
			/// Outgoing template.
			/// </summary>
			public ChatLineComponent OutgoingTemplate;

			/// <inheritdoc/>
			public ChatLineComponent[] AllTemplates()
			{
				return new[] { IncomingTemplate, OutgoingTemplate };
			}

			/// <inheritdoc/>
			public ChatLineComponent Select(int index, ChatLine item)
			{
				if (item.Type == ChatLineType.Incoming)
				{
					return IncomingTemplate;
				}

				return OutgoingTemplate;
			}
		}

		/// <summary>
		/// Chat event.
		/// </summary>
		[SerializeField]
		public UnityEvent MyEvent;

		#region DataSource wrapper and Filter

		ObservableList<ChatLine> fullDataSource;

		/// <summary>
		/// All messages.
		/// </summary>
		public ObservableList<ChatLine> FullDataSource
		{
			get
			{
				return fullDataSource;
			}

			set
			{
				if (fullDataSource != null)
				{
					// unsubscribe update event
					fullDataSource.OnChange -= UpdateDataSource;
				}

				fullDataSource = value;

				if (fullDataSource != null)
				{
					// subscribe update event
					fullDataSource.OnChange += UpdateDataSource;
				}

				UpdateDataSource();
			}
		}

		Func<ChatLine, bool> filter;

		/// <summary>
		/// Messages filter.
		/// </summary>
		public Func<ChatLine, bool> Filter
		{
			get
			{
				return filter;
			}

			set
			{
				filter = value;
				UpdateDataSource();
			}
		}

		void UpdateDataSource()
		{
			DataSource.BeginUpdate();
			DataSource.Clear();
			if (filter != null)
			{
				foreach (var item in FullDataSource)
				{
					if (Filter(item))
					{
						DataSource.Add(item);
					}
				}
			}
			else
			{
				DataSource.AddRange(FullDataSource);
			}

			DataSource.EndUpdate();
		}

		/// <summary>
		/// IncomingTemplate.
		/// </summary>
		[SerializeField]
		protected ChatLineComponent IncomingTemplate;

		/// <summary>
		/// OutgoingTemplate.
		/// </summary>
		[SerializeField]
		protected ChatLineComponent OutgoingTemplate;

		Selector ChatTemplateSelector;

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			ChatTemplateSelector = new Selector()
			{
				IncomingTemplate = IncomingTemplate,
				OutgoingTemplate = OutgoingTemplate,
			};

			TemplateSelector = ChatTemplateSelector;

			base.Init();

			if (fullDataSource == null)
			{
				fullDataSource = new ObservableList<ChatLine>();
				fullDataSource.AddRange(DataSource);
				fullDataSource.OnChange += UpdateDataSource;

				UpdateDataSource();
			}
		}
		#endregion

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (fullDataSource != null)
			{
				fullDataSource.OnChange -= UpdateDataSource;
			}

			base.OnDestroy();
		}
	}
}