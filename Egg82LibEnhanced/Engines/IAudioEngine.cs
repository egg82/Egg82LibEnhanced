﻿using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using System;
using System.IO;

namespace Egg82LibEnhanced.Engines {
	public interface IAudioEngine {
		//events
		event EventHandler<ExceptionEventArgs> Error;

		//functions
		Audio AddAudio(string name, AudioType type, AudioFormat format, byte[] data);
		Audio RemoveAudio(string name);
		Audio GetAudio(string name);
		int NumAudio { get; }
		string CurrentOutputDeviceName { get; }
		int CurrentOutputDevice { get; set; }
		string[] OutputDeviceNames { get; }

		void StartRecordingAudio(ref MemoryStream stream);
		void StopRecordingAudio(ref MemoryStream stream);

		string CurrentInputDeviceName { get; }
		int CurrentInputDevice { get; set; }
		string[] InputDeviceNames { get; }
	}
}
