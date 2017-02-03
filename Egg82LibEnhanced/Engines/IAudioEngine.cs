using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using System;
using System.IO;

namespace Egg82LibEnhanced.Engines {
	public interface IAudioEngine {
		//functions
		event EventHandler<ExceptionEventArgs> Error;

		void AddAudio(string name, AudioType type, AudioFormat format, byte[] data);
		void RemoveAudio(string name);
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
