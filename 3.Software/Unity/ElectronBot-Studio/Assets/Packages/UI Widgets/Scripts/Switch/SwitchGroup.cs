namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Works like ToogleGroup for Switches.
	/// A Switch Group is not a visible UI control but rather a way to modify the behavior of a set of Switches.
	/// Switches that belong to the same group are constrained so that only one of them can switched on at a time - pressing one of them to switch it on automatically switches the others off.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Switch Group")]
	public class SwitchGroup : MonoBehaviour
	{
		/// <summary>
		/// Is it allowed that no switch is switched on?
		/// </summary>
		[SerializeField]
		public bool AllowSwitchOff;

		/// <summary>
		/// Switches.
		/// </summary>
		[NonSerialized]
		protected HashSet<Switch> Switches = new HashSet<Switch>();

		/// <summary>
		/// Check if switch is in group.
		/// </summary>
		/// <param name="currentSwitch">Current switch.</param>
		protected void ValidateSwitchIsInGroup(Switch currentSwitch)
		{
			if (currentSwitch == null || !Switches.Contains(currentSwitch))
			{
				throw new ArgumentException(string.Format("Switch {0} is not part of SwitchGroup {1}", currentSwitch, this));
			}
		}

		/// <summary>
		/// Notifies the switch on.
		/// </summary>
		/// <param name="currentSwitch">Current switch.</param>
		/// <param name="raiseEvent">Raise event.</param>
		/// <param name="animate">Set state with animation.</param>
		public void NotifySwitchOn(Switch currentSwitch, bool raiseEvent = true, bool animate = true)
		{
			ValidateSwitchIsInGroup(currentSwitch);

			foreach (var s in Switches)
			{
				if (s.GetInstanceID() != currentSwitch.GetInstanceID())
				{
					if (raiseEvent)
					{
						s.IsOn = false;
					}
					else
					{
						s.SetStatus(false, animate);
					}
				}
			}
		}

		/// <summary>
		/// Unregisters the switch.
		/// </summary>
		/// <param name="currentSwitch">Current switch.</param>
		public void UnregisterSwitch(Switch currentSwitch)
		{
			Switches.Remove(currentSwitch);
		}

		/// <summary>
		/// Registers the switch.
		/// </summary>
		/// <param name="toggle">Toggle.</param>
		public void RegisterSwitch(Switch toggle)
		{
			Switches.Add(toggle);
		}

		/// <summary>
		/// Is any switch on?
		/// </summary>
		/// <returns><c>true</c>, if any switch is on, <c>false</c> otherwise.</returns>
		public bool AnySwitchesOn()
		{
			foreach (var sw in Switches)
			{
				if (sw.IsOn)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Get active the switches.
		/// </summary>
		/// <returns>The switches.</returns>
		public IEnumerable<Switch> ActiveSwitches()
		{
			foreach (var sw in Switches)
			{
				if (sw.IsOn)
				{
					yield return sw;
				}
			}
		}

		/// <summary>
		/// Sets all switches off.
		/// </summary>
		public void SetAllSwitchesOff()
		{
			bool oldAllowSwitchOff = AllowSwitchOff;

			AllowSwitchOff = true;

			foreach (var s in Switches)
			{
				s.IsOn = false;
			}

			AllowSwitchOff = oldAllowSwitchOff;
		}
	}
}