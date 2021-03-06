//
//  GpioMemLcdTransferProvider.cs
//
//  Author:
//       chris brunner <cyrusbuilt at gmail dot com>
//
//  Copyright (c) 2013 CyrusBuilt
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
using CyrusBuilt.MonoPi.IO;

namespace CyrusBuilt.MonoPi.LCD
{
	/// <summary>
	/// Raspberry Pi GPIO (via memory) provider for the Micro Liquid Crystal library.
	/// </summary>
	public class GpioMemLcdTransferProvider : ILcdTransferProvider
	{
		#region Fields
		private GpioMem _registerSelectPort = null;
		private GpioMem _readWritePort = null;
		private GpioMem _enablePort = null;
		private GpioMem[] _dataPorts = { };
		private Boolean _fourBitMode = false;
		private Boolean _isDisposed = false;
		#endregion

		#region Constructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.LCD.GpioMemLcdTransferProvider"/>
		/// class with the mode flag, register select pin, read/write pin,
		/// enable pin, and data pins.
		/// </summary>
		/// <param name="fourBitMode">
		/// If set to true, switch to four bit mode instead of 8 bit mode.
		/// </param>
		/// <param name="rs">
		/// The number of the CPU pin that is connected to the RS (Register Select)
		/// pin on the LCD.
		/// </param>
		/// <param name="rw">
		/// The number of the CPU pin that is connected to the RW (Read/Write)
		/// pin on the LCD.
		/// </param>
		/// <param name="enable">
		/// The number of the CPU pin that is connected to the enable pin
		/// on the LCD.
		/// </param>
		/// <param name="d0">
		/// Data line 0.
		/// </param>
		/// <param name="d1">
		/// Data line 1.
		/// </param>
		/// <param name="d2">
		/// Data line 2.
		/// </param>
		/// <param name="d3">
		/// Data line 3.
		/// </param>
		/// <param name="d4">
		/// Data line 4.
		/// </param>
		/// <param name="d5">
		/// Data line 5.
		/// </param>
		/// <param name="d6">
		/// Data line 6.
		/// </param>
		/// <param name="d7">
		/// Data line 7.
		/// </param>
		/// <remarks>
		/// The display can be controlled using 4 or 8 data lines. If the former,
		/// omit the pin numbers for d0 to d3 and leave those lines disconnected.
		/// The RW pin can be tied to ground instead of connected to a pin on the
		/// Arduino; If so, omit it from this constructor's parameters.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="rs"/> and <paramref name="enable"/> cannot be set to
		/// <see cref="GpioPins.GPIO_NONE"/>.
		/// </exception>
		public GpioMemLcdTransferProvider(Boolean fourBitMode, GpioPins rs, GpioPins rw, GpioPins enable,
		                                  GpioPins d0, GpioPins d1, GpioPins d2, GpioPins d3, GpioPins d4,
		                                  GpioPins d5, GpioPins d6, GpioPins d7) {
			this._fourBitMode = fourBitMode;
			if (rs == GpioPins.GPIO_NONE) {
				throw new ArgumentException("rs");
			}

			this._registerSelectPort = new GpioMem(rs, PinMode.OUT);
			this._registerSelectPort.Provision();

			// We can save a pin by not using RW. Indicate by passing GpioPins.GPIO_NONE
			// instead of pin # (RW is optional).
			if (rw != GpioPins.GPIO_NONE) {
				this._readWritePort = new GpioMem(rw, PinMode.OUT);
				this._readWritePort.Provision();
			}

			if (enable == GpioPins.GPIO_NONE) {
				throw new ArgumentException("enable");
			}

			this._enablePort = new GpioMem(enable, PinMode.OUT);
			this._enablePort.Provision();

			GpioPins[] dataPins = { d0, d1, d2, d3, d4, d5, d6, d7 };
			this._dataPorts = new GpioMem[8];
			for (Int32 i = 0; i < 8; i++) {
				if (dataPins[i] == GpioPins.GPIO_NONE) {
					this._dataPorts[i] = new GpioMem(dataPins[i], PinMode.OUT);
					this._dataPorts[i].Provision();
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.LCD.GpioMemLcdTransferProvider"/>
		/// class with the mode flag, register select pin, read/write pin,
		/// enable pin, and data pins.
		/// </summary>
		/// <param name="rs">
		/// The number of the CPU pin that is connected to the RS (Register Select)
		/// pin on the LCD.
		/// </param>
		/// <param name="rw">
		/// The number of the CPU pin that is connected to the RW (Read/Write)
		/// pin on the LCD.
		/// </param>
		/// <param name="enable">
		/// The number of the CPU pin that is connected to the enable pin
		/// on the LCD.
		/// </param>
		/// <param name="d0">
		/// Data line 0.
		/// </param>
		/// <param name="d1">
		/// Data line 1.
		/// </param>
		/// <param name="d2">
		/// Data line 2.
		/// </param>
		/// <param name="d3">
		/// Data line 3.
		/// </param>
		/// <param name="d4">
		/// Data line 4.
		/// </param>
		/// <param name="d5">
		/// Data line 5.
		/// </param>
		/// <param name="d6">
		/// Data line 6.
		/// </param>
		/// <param name="d7">
		/// Data line 7.
		/// </param>
		/// <remarks>
		/// The display can be controlled using 4 or 8 data lines. If the former,
		/// omit the pin numbers for d0 to d3 and leave those lines disconnected.
		/// The RW pin can be tied to ground instead of connected to a pin on the
		/// Arduino; If so, omit it from this constructor's parameters.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="rs"/> and <paramref name="enable"/> cannot be set to
		/// <see cref="GpioPins.GPIO_NONE"/>.
		/// </exception>
		public GpioMemLcdTransferProvider(GpioPins rs, GpioPins rw, GpioPins enable, GpioPins d0, GpioPins d1,
		                                  GpioPins d2, GpioPins d3, GpioPins d4, GpioPins d5, GpioPins d6, GpioPins d7)
			: this(false, rs, rw, enable, d0, d1, d2, d3, d4, d5, d6, d7) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.LCD.GpioMemLcdTransferProvider"/>
		/// class with the mode flag, register select pin, read/write pin,
		/// enable pin, and data pins.
		/// </summary>
		/// <param name="rs">
		/// The number of the CPU pin that is connected to the RS (Register Select)
		/// pin on the LCD.
		/// </param>
		/// <param name="enable">
		/// The number of the CPU pin that is connected to the enable pin
		/// on the LCD.
		/// </param>
		/// <param name="d0">
		/// Data line 0.
		/// </param>
		/// <param name="d1">
		/// Data line 1.
		/// </param>
		/// <param name="d2">
		/// Data line 2.
		/// </param>
		/// <param name="d3">
		/// Data line 3.
		/// </param>
		/// <param name="d4">
		/// Data line 4.
		/// </param>
		/// <param name="d5">
		/// Data line 5.
		/// </param>
		/// <param name="d6">
		/// Data line 6.
		/// </param>
		/// <param name="d7">
		/// Data line 7.
		/// </param>
		/// <remarks>
		/// The display can be controlled using 4 or 8 data lines. If the former,
		/// omit the pin numbers for d0 to d3 and leave those lines disconnected.
		/// The RW pin can be tied to ground instead of connected to a pin on the
		/// Arduino; If so, omit it from this constructor's parameters.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="rs"/> and <paramref name="enable"/> cannot be set to
		/// <see cref="GpioPins.GPIO_NONE"/>.
		/// </exception>
		public GpioMemLcdTransferProvider(GpioPins rs, GpioPins enable, GpioPins d0, GpioPins d1, GpioPins d2,
		                                  GpioPins d3, GpioPins d4, GpioPins d5, GpioPins d6, GpioPins d7)
			: this(rs, GpioPins.GPIO_NONE, enable, d0, d1, d2, d3, d4, d5, d6, d7) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.LCD.GpioMemLcdTransferProvider"/>
		/// class with the mode flag, register select pin, read/write pin,
		/// enable pin, and data pins.
		/// </summary>
		/// <param name="rs">
		/// The number of the CPU pin that is connected to the RS (Register Select)
		/// pin on the LCD.
		/// </param>
		/// <param name="rw">
		/// The number of the CPU pin that is connected to the RW (Read/Write)
		/// pin on the LCD.
		/// </param>
		/// <param name="enable">
		/// The number of the CPU pin that is connected to the enable pin
		/// on the LCD.
		/// </param>
		/// <param name="d4">
		/// Data line 4.
		/// </param>
		/// <param name="d5">
		/// Data line 5.
		/// </param>
		/// <param name="d6">
		/// Data line 6.
		/// </param>
		/// <param name="d7">
		/// Data line 7.
		/// </param>
		/// <remarks>
		/// The display can be controlled using 4 or 8 data lines. If the former,
		/// omit the pin numbers for d0 to d3 and leave those lines disconnected.
		/// The RW pin can be tied to ground instead of connected to a pin on the
		/// Arduino; If so, omit it from this constructor's parameters.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="rs"/> and <paramref name="enable"/> cannot be set to
		/// <see cref="GpioPins.GPIO_NONE"/>.
		/// </exception>
		public GpioMemLcdTransferProvider(GpioPins rs, GpioPins rw, GpioPins enable, GpioPins d4, GpioPins d5, GpioPins d6, GpioPins d7)
			: this(true, rs, rw, enable, GpioPins.GPIO_NONE, GpioPins.GPIO_NONE, GpioPins.GPIO_NONE, GpioPins.GPIO_NONE,
			      d4, d5, d6, d7) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.LCD.GpioMemLcdTransferProvider"/>
		/// class with the mode flag, register select pin, read/write pin,
		/// enable pin, and data pins.
		/// </summary>
		/// <param name="rs">
		/// The number of the CPU pin that is connected to the RS (Register Select)
		/// pin on the LCD.
		/// </param>
		/// <param name="enable">
		/// The number of the CPU pin that is connected to the enable pin
		/// on the LCD.
		/// </param>
		/// <param name="d4">
		/// Data line 4.
		/// </param>
		/// <param name="d5">
		/// Data line 5.
		/// </param>
		/// <param name="d6">
		/// Data line 6.
		/// </param>
		/// <param name="d7">
		/// Data line 7.
		/// </param>
		/// <remarks>
		/// The display can be controlled using 4 or 8 data lines. If the former,
		/// omit the pin numbers for d0 to d3 and leave those lines disconnected.
		/// The RW pin can be tied to ground instead of connected to a pin on the
		/// Arduino; If so, omit it from this constructor's parameters.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// <paramref name="rs"/> and <paramref name="enable"/> cannot be set to
		/// <see cref="GpioPins.GPIO_NONE"/>.
		/// </exception>
		public GpioMemLcdTransferProvider(GpioPins rs, GpioPins enable, GpioPins d4, GpioPins d5, GpioPins d6, GpioPins d7)
			: this(true, rs, GpioPins.GPIO_NONE, enable, GpioPins.GPIO_NONE, GpioPins.GPIO_NONE, GpioPins.GPIO_NONE,
			       GpioPins.GPIO_NONE, d4, d5, d6, d7) {
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether this instance is disposed.
		/// </summary>
		public Boolean IsDisposed {
			get { return this._isDisposed; }
		}
		
		/// <summary>
		/// Gets a value indicating whether this instance is in 4-bit mode.
		/// </summary>
		public Boolean FourBitMode {
			get { return this._fourBitMode; }
		}
		#endregion

		#region Methods
		/// <summary>
		/// Pulses the enable pin.
		/// </summary>
		private void PulseEnable() {
			this._enablePort.Write(PinState.Low);
			this._enablePort.Write(PinState.High);
			this._enablePort.Write(PinState.Low);
		}

		/// <summary>
		/// Writes the command or data in 4bit mode (the last 4 data lines).
		/// </summary>
		/// <param name="value">
		/// The command or data to write.
		/// </param>
		private void Write4Bits(Byte value) {
			for (Int32 i = 0; i < 4; i++) {
				this._dataPorts[i + 4].Write((((value >> i) & 0x01) == 0x01) ? PinState.High : PinState.Low);
			}
			this.PulseEnable();
		}
		
		/// <summary>
		/// Writes the command or data in 8bit mode (all 8 data lines).
		/// </summary>
		/// <param name="value">
		/// The command or data to write.
		/// </param>
		private void Write8Bits(Byte value) {
			for (Int32 i = 0; i < 8; i++) {
				this._dataPorts[i].Write((((value >> i) & 0x01) == 0x01) ? PinState.High : PinState.Low);
			}
			this.PulseEnable();
		}

		/// <summary>
		/// Write either command or data, with automatic 4/8-bit selection.
		/// </summary>
		/// <param name="value">
		/// The data or command to send.
		/// </param>
		/// <param name="mode">
		/// Mode for register-select pin (PinState.High = on, PinState.Low = off).
		/// </param>
		/// <param name="backlight">
		/// Turns the backlight on or off.
		/// </param>
		public void Send(Byte value, PinState mode, Boolean backlight) {
			if (this._isDisposed) {
				throw new ObjectDisposedException("GpioMemLcdTransferProvider");
			}

			// TODO set backlight.

			this._registerSelectPort.Write(mode);

			// If there is a RW pin indicated, set it low to write.
			if (this._readWritePort != null) {
				this._readWritePort.Write(PinState.Low);
			}

			if (this._fourBitMode) {
				this.Write4Bits((Byte)(value >> 4));
				this.Write4Bits(value);
			}
			else {
				this.Write8Bits(value);
			}
		}
		#endregion

		#region Destructors
		/// <summary>
		/// Dispose this instance and release managed resources.
		/// </summary>
		/// <param name="disposing">
		/// If set to <c>true</c> disposing and not finalizing.
		/// </param>
		private void Dispose(Boolean disposing) {
			if (this._isDisposed) {
				return;
			}
			
			if (this._registerSelectPort != null) {
				this._registerSelectPort.Dispose();
				this._registerSelectPort = null;
			}
			
			if (this._readWritePort != null) {
				this._readWritePort.Dispose();
				this._readWritePort = null;
			}
			
			if (this._enablePort != null) {
				this._enablePort.Dispose();
				this._enablePort = null;
			}
			
			if ((this._dataPorts != null) && (this._dataPorts.Length > 0)) {
				for (Int32 i = 0; i < 8; i++) {
					if (this._dataPorts[i] != null) {
						this._dataPorts[i].Dispose();
					}
				}
				Array.Clear(this._dataPorts, 0, this._dataPorts.Length);
			}
			
			if (disposing) {
				GC.SuppressFinalize(this);
			}
			this._isDisposed = true;
		}
		
		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations
		/// before the <see cref="CyrusBuilt.MonoPi.LCD.GpioFileLcdTransferProvider"/>
		/// is reclaimed by garbage collection.
		/// </summary>
		~GpioMemLcdTransferProvider() {
			this.Dispose(false);
		}

		#pragma warning disable 419
		/// <summary>
		/// Releases all resource used by the <see cref="CyrusBuilt.MonoPi.LCD.GpioFileLcdTransferProvider"/>
		/// object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the
		/// <see cref="CyrusBuilt.MonoPi.LCD.GpioFileLcdTransferProvider"/>.
		/// The <see cref="Dispose"/> method leaves the
		/// <see cref="CyrusBuilt.MonoPi.LCD.GpioFileLcdTransferProvider"/> in
		/// an unusable state. After calling <see cref="Dispose"/>, you must
		/// release all references to the <see cref="CyrusBuilt.MonoPi.LCD.GpioFileLcdTransferProvider"/>
		/// so the garbage collector can reclaim the memory that the
		/// <see cref="CyrusBuilt.MonoPi.LCD.GpioFileLcdTransferProvider"/> was occupying.
		/// </remarks>
		public void Dispose() {
			this.Dispose(true);
		}
		#pragma warning restore 419
		#endregion
	}
}

