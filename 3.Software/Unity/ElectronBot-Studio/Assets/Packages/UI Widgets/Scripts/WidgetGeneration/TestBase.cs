namespace UIWidgets.WidgetGeneration
{
	using System.Collections.Generic;
	using UIWidgets;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Base class for the test script for the DataItem.
	/// </summary>
	/// <typeparam name="T">Type of the test item.</typeparam>
	public abstract class TestBase<T> : MonoBehaviour
	{
		/// <summary>
		/// Generate item for specified index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Item.</returns>
		protected abstract T GenerateItem(int index);

		/// <summary>
		/// Generate item for specified index with specified name.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <param name="index">Index.</param>
		/// <returns>Item.</returns>
		protected abstract T GenerateItem(string name, int index);

		/// <summary>
		/// Sprites.
		/// </summary>
		[SerializeField]
		public List<Sprite> Sprites;

		/// <summary>
		/// Textures.
		/// </summary>
		[SerializeField]
		public List<Texture2D> Textures;

		/// <summary>
		/// Root canvas.
		/// </summary>
		[SerializeField]
		public GameObject RootCanvas;

		/// <summary>
		/// Default style.
		/// </summary>
		[SerializeField]
		public Style StyleDefault;

		/// <summary>
		/// Blue style.
		/// </summary>
		[SerializeField]
		public Style StyleBlue;

		/// <summary>
		/// Is items can be generated?
		/// </summary>
		protected virtual bool CanGenerateItems
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Set default style.
		/// </summary>
		public void SetDefaultStyle()
		{
			StyleDefault.ApplyTo(RootCanvas);
		}

		/// <summary>
		/// Set blue style.
		/// </summary>
		public void SetBlueStyle()
		{
			StyleBlue.ApplyTo(RootCanvas);
		}

		/// <summary>
		/// Get random sprite.
		/// </summary>
		/// <returns>Sprite.</returns>
		protected Sprite GetSprite()
		{
			if (Sprites.Count == 0)
			{
				return null;
			}

			return Sprites[Random.Range(0, Sprites.Count - 1)];
		}

		/// <summary>
		/// Get random texture.
		/// </summary>
		/// <returns>Texture.</returns>
		protected Texture2D GetTexture()
		{
			if (Textures.Count == 0)
			{
				return null;
			}

			return Textures[Random.Range(0, Textures.Count - 1)];
		}

		/// <summary>
		/// Generate items list.
		/// </summary>
		/// <param name="count">Items count.</param>
		/// <returns>List.</returns>
		protected ObservableList<T> GenerateList(int count)
		{
			var result = new ObservableList<T>();
			if (!CanGenerateItems)
			{
				return result;
			}

			result.BeginUpdate();

			for (int i = 0; i < count; i++)
			{
				result.Add(GenerateItem(i + 1));
			}

			result.EndUpdate();

			return result;
		}

		/// <summary>
		/// Generate nodes.
		/// </summary>
		/// <param name="config">List of required nodes amount per level.</param>
		/// <param name="namePrefix">Name prefix.</param>
		/// <param name="start">Start index in the configuration.</param>
		/// <returns>Nodes.</returns>
		protected ObservableList<TreeNode<T>> GenerateNodes(List<int> config, string namePrefix = "Node ", int start = 0)
		{
			if (!CanGenerateItems)
			{
				return new ObservableList<TreeNode<T>>();
			}

			var count = config[start];
			var result = new ObservableList<TreeNode<T>>(true, count);

			result.BeginUpdate();

			for (int i = 1; i <= count; i++)
			{
				result.Add(GenerateNode(i, config, namePrefix, start));
			}

			result.EndUpdate();

			return result;
		}

		/// <summary>
		/// Generate node.
		/// </summary>
		/// <param name="index">Node index.</param>
		/// <param name="config">List of required nodes amount per level.</param>
		/// <param name="namePrefix">Node name prefix.</param>
		/// <param name="start">Start index in the configuration.</param>
		/// <returns>Node.</returns>
		protected TreeNode<T> GenerateNode(int index, List<int> config, string namePrefix, int start)
		{
			var item_name = string.Format("{0}{1}", namePrefix, index.ToString());
			var item = GenerateItem(item_name, index);

			var nodes = config.Count > (start + 1)
				? GenerateNodes(config, string.Format("{0} - ", item_name), start + 1)
				: null;

			return new TreeNode<T>(item, nodes, true);
		}

		/// <summary>
		/// Get random color.
		/// </summary>
		/// <returns>Color.</returns>
		protected static Color RandomColor()
		{
			return new Color(Random.value, Random.value, Random.value, Random.value);
		}
	}
}