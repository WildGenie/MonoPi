//
//  CaptureDoneEventArgs.cs
//
//  Author:
//       Chris Brunner <cyrusbuilt at gmail dot com>
//
//  Copyright (c) 2014 Copyright (c) 2013 CyrusBuilt
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

namespace CyrusBuilt.MonoPi.Devices.PiCamera
{
	/// <summary>
	/// Capture done event arguments class.
	/// </summary>
	public class CaptureDoneEventArgs : EventArgs
	{
		private Int32 _exitCode = 0;

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.Devices.PiCamera.CaptureDoneEventArgs"/>
		/// class with the exit code returned by the capture process.
		/// </summary>
		/// <param name="exitCode">
		/// The exit code returned by the capture process.
		/// </param>
		public CaptureDoneEventArgs(Int32 exitCode)
			: base() {
			this._exitCode = exitCode;
		}

		/// <summary>
		/// Gets the exit code returned by the capture process.
		/// </summary>
		public Int32 ExitCode {
			get { return this._exitCode; }
		}
	}
}

