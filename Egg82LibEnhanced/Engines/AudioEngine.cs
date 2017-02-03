using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace Egg82LibEnhanced.Engines {
	public class AudioEngine : IAudioEngine {
		//vars
		public event EventHandler<ExceptionEventArgs> Error = null;

		private double _masterVolume = 1.0d;
		private double _ambientVolume = 1.0d;
		private double _musicVolume = 1.0d;
		private double _sfxVolume = 1.0d;
		private double _uiVolume = 1.0d;
		private double _voiceVolume = 1.0d;

		private Dictionary<string, Audio> sounds = new Dictionary<string, Audio>();
		private WaveInEvent inputDevice = new WaveInEvent();
		private int _currentOutputDevice = 0;
		private List<MemoryStream> recordingStreams = new List<MemoryStream>();

		//constructor
		public AudioEngine() {
			inputDevice.DataAvailable += onRecordData;
		}
		~AudioEngine() {
			try {
				inputDevice.StopRecording();
			} catch (Exception) {

			}
			inputDevice.Dispose();
		}

		//public
		public void AddAudio(string name, AudioType type, AudioFormat format, byte[] data) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (data == null) {
				throw new ArgumentNullException("data");
			}

			if (sounds.ContainsKey(name)) {
				sounds[name].Dispose();
				sounds.Remove(name);
			}
			Audio audio = new Audio(type, format, getVolume(type), data, _currentOutputDevice);
			audio.Error += onError;
			sounds.Add(name, audio);
		}
		public void RemoveAudio(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				audio.Error -= onError;
				audio.Dispose();
				sounds.Remove(name);
			}
		}
		public void PlayAudio(string name, bool repeat = false) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				audio.Repeating = repeat;
				audio.Play();
			}
		}
		public void PauseAudio(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				audio.Pause();
			}
		}
		public int NumAudio {
			get {
				return sounds.Count;
			}
		}

		public int GetPositionInBytes(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				return audio.PositionInBytes;
			}
			return -1;
		}
		public void SetPositionInBytes(string name, int positionInBytes) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				audio.PositionInBytes = (int) MathUtil.Clamp(0.0d, (double) audio.Length, (double) positionInBytes);
			}
		}
		public TimeSpan GetPositionInTime(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				return audio.PositionInTime;
			}
			return new TimeSpan();
		}
		public void SetPositionInTime(string name, TimeSpan positionInTime) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (positionInTime == null) {
				throw new ArgumentNullException("positionInTime");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				audio.PositionInTime = positionInTime;
			}
		}
		public double GetPan(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				return audio.Pan;
			}
			return 0.0d;
		}
		public void SetPan(string name, double pan) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				audio.Pan = pan;
			}
		}
		public int LengthInBytes(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				return audio.Length;
			}
			return -1;
		}

		public string CurrentOutputDeviceName {
			get {
				return WaveOut.GetCapabilities(_currentOutputDevice).ProductName;
			}
		}
		public int CurrentOutputDevice {
			get {
				return _currentOutputDevice;
			}
			set {
				if (value < 0 || value > WaveOut.DeviceCount) {
					throw new IndexOutOfRangeException();
				}
				if (value == _currentOutputDevice) {
					return;
				}

				Dictionary<string, bool> playing = new Dictionary<string, bool>();
				Dictionary<string, int> position = new Dictionary<string, int>();
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					playing.Add(kvp.Key, kvp.Value.Playing);
					position.Add(kvp.Key, kvp.Value.PositionInBytes);
					kvp.Value.Dispose();
				}

				_currentOutputDevice = value;

				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					kvp.Value.Initialize(_currentOutputDevice);
					kvp.Value.PositionInBytes = position[kvp.Key];
					if (playing[kvp.Key]) {
						kvp.Value.Play();
					}
				}
			}
		}
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
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}

			if (!recordingStreams.Contains(stream)) {
				recordingStreams.Add(stream);
			}
			inputDevice.StartRecording();
		}
		public void StopRecordingAudio(ref MemoryStream stream) {
			if (stream == null) {
				throw new ArgumentNullException("stream");
			}
			
			recordingStreams.Remove(stream);
			if (recordingStreams.Count == 0) {
				inputDevice.StopRecording();
			}
		}

		public string CurrentInputDeviceName {
			get {
				return WaveIn.GetCapabilities(inputDevice.DeviceNumber).ProductName;
			}
		}
		public int CurrentInputDevice {
			get {
				return inputDevice.DeviceNumber;
			}
			set {
				if (value < 0 || value > WaveOut.DeviceCount) {
					throw new IndexOutOfRangeException();
				}
				if (value == inputDevice.DeviceNumber) {
					return;
				}

				inputDevice.StopRecording();
				inputDevice.DeviceNumber = value;
				if (recordingStreams.Count > 0) {
					inputDevice.StartRecording();
				}
			}
		}
		public string[] InputDeviceNames {
			get {
				string[] retVal = new string[WaveIn.DeviceCount];
				for (int i = 0; i < WaveIn.DeviceCount; i++) {
					retVal[i] = WaveIn.GetCapabilities(i).ProductName;
				}
				return retVal;
			}
		}

		public double MasterVolume {
			get {
				return _masterVolume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _masterVolume) {
					return;
				}
				_masterVolume = value;
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					kvp.Value.Volume = getVolume(kvp.Value.Type);
				}
			}
		}
		public double AmbientVolume {
			get {
				return _ambientVolume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _ambientVolume) {
					return;
				}
				_ambientVolume = value;
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					if (kvp.Value.Type == AudioType.Ambient) {
						kvp.Value.Volume = getVolume(kvp.Value.Type);
					}
				}
			}
		}
		public double MusicVolume {
			get {
				return _musicVolume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _musicVolume) {
					return;
				}
				_musicVolume = value;
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					if (kvp.Value.Type == AudioType.Music) {
						kvp.Value.Volume = getVolume(kvp.Value.Type);
					}
				}
			}
		}
		public double SfxVolume {
			get {
				return _sfxVolume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _sfxVolume) {
					return;
				}
				_sfxVolume = value;
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					if (kvp.Value.Type == AudioType.Sfx) {
						kvp.Value.Volume = getVolume(kvp.Value.Type);
					}
				}
			}
		}
		public double UiVolume {
			get {
				return _uiVolume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _uiVolume) {
					return;
				}
				_uiVolume = value;
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					if (kvp.Value.Type == AudioType.Ui) {
						kvp.Value.Volume = getVolume(kvp.Value.Type);
					}
				}
			}
		}
		public double VoiceVolume {
			get {
				return _voiceVolume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _voiceVolume) {
					return;
				}
				_voiceVolume = value;
				foreach (KeyValuePair<string, Audio> kvp in sounds) {
					if (kvp.Value.Type == AudioType.Voice) {
						kvp.Value.Volume = getVolume(kvp.Value.Type);
					}
				}
			}
		}

		//private
		private void onError(object sender, ExceptionEventArgs e) {
			if (Error != null) {
				Error.Invoke(this, new ExceptionEventArgs(e.Exception));
			}
		}
		private void onRecordData(object sender, WaveInEventArgs e) {
			List<int> unwritables = new List<int>();
			for (int i = 0; i < recordingStreams.Count; i++) {
				if (recordingStreams[i] != null && recordingStreams[i].CanWrite) {
					recordingStreams[i].Write(e.Buffer, 0, e.BytesRecorded);
				} else {
					unwritables.Add(i);
				}
			}

			if (unwritables.Count > 0) {
				for (int i = unwritables.Count - 1; i >= 0; i--) {
					recordingStreams.RemoveAt(unwritables[i]);
				}
				if (recordingStreams.Count == 0) {
					inputDevice.StopRecording();
				}
			}
		}
		
		private double getVolume(AudioType type) {
			double volume = 1.0d;
			if (type == AudioType.Ambient) {
				volume = _ambientVolume;
			} else if (type == AudioType.Music) {
				volume = _musicVolume;
			} else if (type == AudioType.Sfx) {
				volume = _sfxVolume;
			} else if (type == AudioType.Ui) {
				volume = _uiVolume;
			} else if (type == AudioType.Voice) {
				volume = _voiceVolume;
			}
			volume *= _masterVolume;
			return volume;
		}
	}
}
