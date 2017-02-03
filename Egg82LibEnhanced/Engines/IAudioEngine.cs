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
		void PlayAudio(string name, bool repeat = false);
		void PauseAudio(string name);
		int NumAudio { get; }
		string CurrentOutputDeviceName { get; }
		int CurrentOutputDevice { get; set; }
		string[] OutputDeviceNames { get; }

		int GetPositionInBytes(string name);
		void SetPositionInBytes(string name, int positionInBytes);
		TimeSpan GetPositionInTime(string name);
		void SetPositionInTime(string name, TimeSpan positionInTime);
		double GetPan(string name);
		void SetPan(string name, double pan);
		int LengthInBytes(string name);

		void StartRecordingAudio(ref MemoryStream stream);
		void StopRecordingAudio(ref MemoryStream stream);

		string CurrentInputDeviceName { get; }
		int CurrentInputDevice { get; set; }
		string[] InputDeviceNames { get; }

		double MasterVolume { get; set; }
		double AmbientVolume { get; set; }
		double MusicVolume { get; set; }
		double SfxVolume { get; set; }
		double UiVolume { get; set; }
		double VoiceVolume { get; set; }
	}
}
