//
//  SwitchBase.cs
//
//  Author:
//       Chris Brunner <cyrusbuilt at gmail dot com>
//
//  Copyright (c) 2013 Copyright (c) 2013 CyrusBuilt
//
//  This program is free software; you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation; either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
//
using System;

namespace CyrusBuilt.MonoPi.Components.Switches
{
	/// <summary>
	/// Base class for switch component abstractions.
	/// </summary>
	public abstract class SwitchBase : ComponentBase, ISwitch
	{
		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.Components.Switches.SwitchBase"/>
		/// class. This is the default constructor.
		/// </summary>
		protected SwitchBase()
			: base() {
		}
		#endregion

		#region Events
		/// <summary>
		/// Occurs when state changed.
		/// </summary>
		public event SwitchStateChangeEventHandler StateChanged;
		#endregion

		#region Properties
		/// <summary>
		/// Gets the state.
		/// </summary>
		/// <value>The state of the switch.</value>
		public abstract SwitchState State { get; }

		/// <summary>
		/// Gets a value indicating whether this instance is on.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is on; otherwise, <c>false</c>.
		/// </value>
		public Boolean IsOn {
			get { return (this.State == SwitchState.On); }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is off.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance is off; otherwise, <c>false</c>.
		/// </value>
		public Boolean IsOff {
			get { return (this.State == SwitchState.Off); }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Raises the switch state changed event.
		/// </summary>
		/// <param name="e">
		/// The event arguments.
		/// </param>
		protected virtual void OnSwitchStateChanged(SwitchStateChangeEventArgs e) {
			if (this.StateChanged != null) {
				this.StateChanged(this, e);
			}
		}

		/// <summary>
		/// Determines whether the state of this instance is the specified state.
		/// </summary>
		/// <returns>
		/// <c>true</c> if this instance is state the specified state; otherwise, <c>false</c>.
		/// </returns>
		/// <param name="state">
		/// The state to compare to.
		/// </param>
		public Boolean IsState(SwitchState state) {
			return (this.State == state);
		}
		#endregion
	}
}

