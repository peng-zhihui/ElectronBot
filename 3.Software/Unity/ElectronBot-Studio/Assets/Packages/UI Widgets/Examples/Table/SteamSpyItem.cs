namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// SteamSpy item.
	/// </summary>
	[Serializable]
	public class SteamSpyItem
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// ScoreRank.
		/// </summary>
		[SerializeField]
		public int ScoreRank;

		/// <summary>
		/// Owners.
		/// </summary>
		[SerializeField]
		public int Owners;

		/// <summary>
		/// OwnersVariance.
		/// </summary>
		[SerializeField]
		public int OwnersVariance;

		/// <summary>
		/// Players.
		/// </summary>
		[SerializeField]
		public int Players;

		/// <summary>
		/// PlayersVariance.
		/// </summary>
		[SerializeField]
		public int PlayersVariance;

		/// <summary>
		/// PlayersIn2Week.
		/// </summary>
		[SerializeField]
		public int PlayersIn2Week;

		/// <summary>
		/// PlayersIn2WeekVariance.
		/// </summary>
		[SerializeField]
		public int PlayersIn2WeekVariance;

		/// <summary>
		/// AverageTimeIn2Weeks.
		/// </summary>
		[SerializeField]
		public int AverageTimeIn2Weeks;

		/// <summary>
		/// MedianTimeIn2Weeks.
		/// </summary>
		[SerializeField]
		public int MedianTimeIn2Weeks;

		/// <inheritdoc/>
		public override string ToString()
		{
			return Name;
		}
	}
}