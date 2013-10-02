//
//  MotorRotateEventArgs.cs
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

namespace CyrusBuilt.MonoPi.Components.Motors
{
	/// <summary>
	/// Motor rotate event arguments class.
	/// </summary>
	public class MotorRotateEventArgs : EventArgs
	{
		private Int32 _steps = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.Components.Motors.MotorRotateEventArgs"/>
		/// class with the steps being taken.
		/// </summary>
		/// <param name="steps">
		/// The steps being taken. 0 steps = stopped. Greater than 0 = the number
		/// of steps forward. Less than 0 = the number of steps moving backward.
		/// </param>
		public MotorRotateEventArgs(Int32 steps)
			: base() {
			this._steps = steps;
		}

		/// <summary>
		/// Gets the steps.
		/// </summary>
		public Int32 Steps {
			get { return this._steps; }
		}
	}
}

