using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;

namespace Egg82LibEnhanced.Engines {
	public class AudioEngine : IAudioEngine {
		//vars
		public event EventHandler<ExceptionEventArgs> Error = null;

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
			Audio audio = new Audio(type, format, 1.0d, data, _currentOutputDevice);
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
		public Audio GetAudio(string name) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}

			Audio audio = null;
			if (sounds.TryGetValue(name, out audio)) {
				return audio;
			}
			return null;
		}
		public int NumAudio {
			get {
				return sounds.Count;
			}
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

		//private
		private void onError(object sender, ExceptionEventArgs e) {
			Error?.Invoke(this, new ExceptionEventArgs(e.Exception));
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
	}
}
