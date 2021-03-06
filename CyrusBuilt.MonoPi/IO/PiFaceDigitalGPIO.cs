//
//  PiFaceDigitalGPIO.cs
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
using System.Diagnostics;

namespace CyrusBuilt.MonoPi.IO
{
	/// <summary>
	/// PiFace GPIO pin implementing DMA.
	/// </summary>
	public class PiFaceDigitalGPIO : PiFaceGpioBase
	{
		#region Fields
		private PinState _lastState = PinState.Low;
		private static Boolean _initialized = false;
		#endregion

		#region Constructors and Destructors
		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>
		/// class with the PiFace pin to control.
		/// </summary>
		/// <param name="pin">
		/// The PiFace pin to control.
		/// </param>
		public PiFaceDigitalGPIO(PiFacePins pin)
			: base(pin) {
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>
		/// class with the PiFace pin to control and the initial value to write.
		/// </summary>
		/// <param name="pin">
		/// The PiFace pin to control.
		/// </param>
		/// <param name="initialValue">
		/// The initial value to write.
		/// </param>
		public PiFaceDigitalGPIO(PiFacePins pin, PinState initialValue)
			: base(pin, initialValue) {
		}

		/// <summary>
		/// Releases all resource used by the <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>
		/// object.
		/// </summary>
		/// <remarks>
		/// Call <see cref="Dispose"/> when you are finished using the
		/// <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>. The <see cref="Dispose"/>
		/// method leaves the <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/> in an unusable
		/// state. After calling <see cref="Dispose"/>, you must release all references to the
		/// <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/> so the garbage collector can
		/// reclaim the memory that the <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>
		/// was occupying.
		/// </remarks>
		public override void Dispose() {
			if (base.IsDisposed) {
				return;
			}
			base.Write(PinState.Low);
			UnexportPin(base.InnerPin);
			Destroy();
			base.Dispose();
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets a value indicating whether or not the GPIO is initialized.
		/// </summary>
		public static Boolean Initialized {
			get { return _initialized; }
		}
		#endregion

		#region Static Methods
		/// <summary>
		/// Initialize the memory access to the GPIO.
		/// </summary>
		/// <returns>
		/// true if initialized; Otherwise, false.
		/// </returns>
		public static Boolean Initialize() {
			Int32 ret = 1;
			if (!_initialized) {
				// Init the mapped memory.
				ret = UnsafeNativeMethods.bcm2835_init();
				_initialized = true;
			}
			return (ret == 0) ? false : true;
		}

		/// <summary>
		/// Export the GPIO setting the direction.
		/// </summary>
		/// <param name="pin">
		/// The pin to export.
		/// </param>
		/// <param name="mode">
		/// The I/0 mode of the pin.
		/// </param>
		private static void internal_ExportPin(Int32 pin, PinMode mode) {
			Initialize();

			// If the pin is already exported, check it's in the proper direction.
			if (ExportedPins.ContainsKey(pin)) {
				// If the direction matches, return out of the function. If not,
				// change the direction.
				if (ExportedPins[pin] == mode) {
					return;
				}
			}

			// Set the direction on the pin and update the exported list.
			// BCM2835_GPIO_FSEL_INPT = 0
			// BCM2835_GPIO_FSEL_OUTP = 1
			uint pindir = (mode == PinMode.IN) ? (uint)0 : (uint)1;
			UnsafeNativeMethods.bcm2835_gpio_fsel((uint)pin, pindir);
			if (mode == PinMode.IN) {
				// BCM2835_GPIO_PUD_OFF = 0b00 = 0
				// BCM2835_GPIO_PUD_DOWN = 0b01 = 1
				// BCM2835_GPIO_PUD_UP = 0b10 = 2
				UnsafeNativeMethods.bcm2835_gpio_set_pud((uint)pin, 0);
			}
			ExportedPins[pin] = mode;
		}

		/// <summary>
		/// Exports the pin setting the direction.
		/// </summary>
		/// <param name="pin">
		/// The pin on the PiFace to export.
		/// </param>
		/// <param name="mode">
		/// The I/0 mode of the pin.
		/// </param>
		private static void ExportPin(PiFacePins pin, PinMode mode) {
			internal_ExportPin((Int32)pin, mode);
		}

		/// <summary>
		/// Unexports an exported pin.
		/// </summary>
		/// <param name="pin">
		/// The pin to unexport.
		/// </param>
		private static void internal_UnexportPin(Int32 pin) {
			Debug.WriteLine("Unexporting pin " + pin.ToString());
			// TODO Somehow reverse what we did in internal_ExportPin? Is there
			// a way to "free" the pin in the libbcm2835 library?
			UnsafeNativeMethods.bcm2835_gpio_write((uint)pin, (uint)PinState.Low);
			UnsafeNativeMethods.bcm2835_gpio_fsel((uint)pin, (uint)PinMode.TRI);
			ExportedPins.Remove(pin);
		}

		/// <summary>
		/// Unexport the GPIO.
		/// </summary>
		/// <param name="pin">
		/// The pin to unexport.
		/// </param>
		private static void UnexportPin(PiFacePins pin) {
			internal_UnexportPin((Int32)pin);
		}

		/// <summary>
		/// Write the value to the specified pin.
		/// </summary>
		/// <param name="pin">
		/// The pin to write to.
		/// </param>
		/// <param name="value">
		/// The value to write to the pin.
		/// </param>
		/// <param name="pinname">
		/// The name of the GPIO associated with the pin.
		/// </param>
		private static void internal_Write(Int32 pin, PinState value, String pinname) {
			if (pin == (Int32)PiFacePins.None) {
				return;
			}
				
			internal_ExportPin(pin, PinMode.OUT);
			UnsafeNativeMethods.bcm2835_gpio_write((uint)pin, (uint)value);
			Debug.WriteLine("Output to pin " + pinname + "/gpio" + pin.ToString() + ", value was " + ((uint)value).ToString());
		}

		/// <summary>
		/// Write the specified pin and value.
		/// </summary>
		/// <param name="pin">
		/// The pin to write to.
		/// </param>
		/// <param name="value">
		/// The value to write.
		/// </param>
		public static void Write(PiFacePins pin, PinState value) {
			String name = Enum.GetName(typeof(PiFacePins), pin);
			internal_Write((Int32)pin, value, name);
		}

		/// <summary>
		/// Read the value of the specified pin.
		/// </summary>
		/// <param name="pin">
		/// The pin to read.
		/// </param>
		/// <param name="pinname">
		/// The name of the PiFace associated with the pin.
		/// </param>
		/// <returns>
		/// The value read from the pin.
		/// </returns>
		private static PinState internal_Read(Int32 pin, String pinname) {
			internal_ExportPin(pin, PinMode.IN);
			uint value = UnsafeNativeMethods.bcm2835_gpio_lev((uint)pin);
			PinState returnValue = (value == 0) ? PinState.High : PinState.Low;
			Debug.WriteLine("Input from pin " + pinname + "/gpio" + pin.ToString() + ", value was " + ((uint)returnValue).ToString());
			return returnValue;
		}

		/// <summary>
		/// Read the specified Revision 1.0 pin.
		/// </summary>
		/// <param name="pin">
		/// The pin to read.
		/// </param>
		/// <returns>
		/// The value read from the pin.
		/// </returns>			
		public static PinState Read(PiFacePins pin) {
			String name = Enum.GetName(typeof(PiFacePins), pin);
			return internal_Read((Int32)pin, name);
		}

		/// <summary>
		/// Destroy this GPIO factory.
		/// </summary>
		public static void Destroy() {
			if (ExportedPins != null) {
				if (ExportedPins.Count > 0) {
					foreach (Int32 pin in ExportedPins.Keys) {
						internal_UnexportPin(pin);
					}
					ExportedPins.Clear();
				}
			}
			UnsafeNativeMethods.bcm2835_close();
		}
		#endregion

		#region Instance Methods
		/// <summary>
		/// Provisions the pin (initialize to specified mode and make active).
		/// </summary>
		public override void Provision() {
			ExportPin(base.InnerPin, base.Mode);
			Write(base.InnerPin, base._initValue);
		}

		/// <summary>
		/// Write the specified value to the pin.
		/// </summary>
		/// <param name="value">
		/// The value to write to the pin.
		/// </param>
		public override void Write(PinState value) {
			Write(base.InnerPin, value);
			base.Write(value);
			if (this._lastState != value) {
				this.OnStateChanged(new PinStateChangeEventArgs(this._lastState, value));
			}
		}

		/// <summary>
		/// Pulse the pin output for the specified number of milliseconds.
		/// </summary>
		/// <param name="millis">
		/// The number of milliseconds to wait between states.
		/// </param>
		/// <exception cref="InvalidOperationException">
		/// An attempt was made to pulse an input pin.
		/// </exception>
		public override void Pulse(Int32 millis) {
			if (base.Mode == PinMode.IN) {
				throw new InvalidOperationException("You cannot pulse a pin set as an input.");
			}
			Write(base.InnerPin, PinState.High);
			this.OnStateChanged(new PinStateChangeEventArgs(base.State, PinState.High));
			base.Pulse(millis);
			Write(base.InnerPin, PinState.Low);
			this.OnStateChanged(new PinStateChangeEventArgs(base.State, PinState.Low));
		}

		/// <summary>
		/// Pulses the pin for 500ms.
		/// </summary>
		public void Pulse() {
			this.Pulse(500);
		}

		/// <summary>
		/// Read a value from the pin.
		/// </summary>
		/// <returns>
		/// The value read from the pin.
		/// </returns>
		public override PinState Read() {
			PinState newState = Read(base.InnerPin);
			if (this._lastState != newState) {
				this.OnStateChanged(new PinStateChangeEventArgs(this._lastState, newState));
			}
			return newState;
		}

		/// <summary>
		/// Returns a <see cref="String"/> that represents the current
		/// <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>.
		/// </summary>
		/// <returns>
		/// A <see cref="String"/> that represents the current
		/// <see cref="CyrusBuilt.MonoPi.IO.PiFaceDigitalGPIO"/>.
		/// </returns>
		public override String ToString() {
			return base.Name;
		}
		#endregion
	}
}

