﻿using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using NAudio.Wave;
using System;
using System.IO;

namespace Egg82LibEnhanced.Engines.Nulls {
	public class NullAudioEngine : IAudioEngine {
		//vars
#pragma warning disable 0414
		public event EventHandler<ExceptionEventArgs> Error = null;
#pragma warning restore 0414

		//constructor
		public NullAudioEngine() {

		}

		//public
		public Audio AddAudio(string name, AudioType type, AudioFormat format, byte[] data) {
			return null;
		}
		public Audio RemoveAudio(string name) {
			return null;
		}
		public Audio GetAudio(string name) {
			return null;
		}
		public int NumAudio {
			get {
				return 0;
			}
		}
		public string CurrentOutputDeviceName {
			get {
				return WaveOut.GetCapabilities(0).ProductName;
			}
		}
		public int CurrentOutputDevice { get; set; }
		public string[] OutputDeviceNames {
			get {
				string[] retVal = new string[WaveOut.DeviceCount];
				for (int i = 0; i < WaveOut.DeviceCount; i++) {
					retVal[i] = WaveOut.GetCapabilities(i).ProductName;
				}
				return retVal;
			}
		}

		public void StartRecordingAudio(ref MemoryStream stream) {

		}
		public void StopRecordingAudio(ref MemoryStream stream) {

		}

		public string CurrentInputDeviceName {
			get {
				return WaveIn.GetCapabilities(0).ProductName;
			}
		}
		public int CurrentInputDevice { get; set; }
		public string[] InputDeviceNames {
			get {
				string[] retVal = new string[WaveIn.DeviceCount];
				for (int i = 0; i < WaveIn.DeviceCount; i++) {
					retVal[i] = WaveIn.GetCapabilities(i).ProductName;
				}
				return retVal;
			}
		}

		//private

	}
}
