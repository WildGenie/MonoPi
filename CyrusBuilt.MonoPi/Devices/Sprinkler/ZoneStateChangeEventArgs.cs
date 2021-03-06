//
//  ZoneStateChangeEventArgs.cs
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

namespace CyrusBuilt.MonoPi.Devices.Sprinkler
{
	/// <summary>
	/// Zone state change event arguments class.
	/// </summary>
	public class ZoneStateChangeEventArgs : EventArgs
	{
		#region Fields
		private Boolean _oldState = false;
		private Boolean _newState = false;
		private Int32 _zone = 0;
		#endregion	

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.Devices.Sprinkler.ZoneStateChangeEventArgs"/>
		/// class with the old and new states as well as the zone that changed state.
		/// </summary>
		/// <param name="oldState">
		/// The previous state of the zone.
		/// </param>
		/// <param name="newState">
		/// The current state of the zone.
		/// </param>
		/// <param name="zone">
		/// The zone that changed state.
		/// </param>
		public ZoneStateChangeEventArgs(Boolean oldState, Boolean newState, Int32 zone)
			: base() {
			this._oldState = oldState;
			this._newState = newState;
			this._zone = zone;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets the previous state of the zone.
		/// </summary>
		/// <value>
		/// true if the zone was on; Otherwise, false.
		/// </value>
		public Boolean OldState {
			get { return this._oldState; }
		}

		/// <summary>
		/// Gets the current state of the zone.
		/// </summary>
		/// <value>
		/// true if the zone is on; Otherwise, false.
		/// </value>
		public Boolean NewState {
			get { return this._newState; }
		}

		/// <summary>
		/// Gets the zone that changed state.
		/// </summary>
		public Int32 Zone {
			get { return this._zone; }
		}
		#endregion
	}
}

