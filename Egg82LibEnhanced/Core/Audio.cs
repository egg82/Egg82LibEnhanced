using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Events;
using Egg82LibEnhanced.Utils;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.IO;

namespace Egg82LibEnhanced.Core {
	public class Audio : IDisposable {
		//vars
		public event EventHandler<ExceptionEventArgs> Error = null;
		public bool Repeating = false;

		private AudioType _type = AudioType.None;
		private AudioFormat _format = AudioFormat.None;
		private byte[] _data = null;
		private double _volume = 0.0d;
		private bool _playing = false;

		private double _pan = 0.0d;

		private MemoryStream byteStream = null;
		private WaveStream waveStream = null;
		private VolumeSampleProvider volumeProvider = null;
		private PanningSampleProvider panningProvider = null;
		private WaveOutEvent waveOut = null;

		//constructor
		public Audio(AudioType type, AudioFormat format, double volume, byte[] data, int audioDevice) {
			_type = type;
			_format = format;
			_data = data;
			_volume = volume;

			if (data == null) {
				return;
			}

			Initialize(audioDevice);
		}
		~Audio() {
			Dispose();
		}

		//public
		public AudioType Type {
			get {
				return _type;
			}
		}
		public AudioFormat Format {
			get {
				return _format;
			}
		}
		public double Volume {
			get {
				return _volume;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value) || value == _volume) {
					return;
				}
				_volume = value;
				volumeProvider.Volume = (float) _volume;
			}
		}
		public byte[] CloneData() {
			return (byte[]) _data.Clone();
		}

		public bool Playing {
			get {
				return _playing;
			}
		}

		public void Play(bool repeat = false) {
			Repeating = repeat;

			if (_playing) {
				return;
			}
			_playing = true;

			waveOut.Play();
		}
		public void Pause() {
			if (!_playing) {
				return;
			}
			_playing = false;

			waveOut.Pause();
		}

		public int PositionInBytes {
			get {
				return (int) waveStream.Position;
			}
			set {
				waveStream.Position = (int) MathUtil.Clamp(0.0d, (double) _data.Length, (double) value);
			}
		}
		public TimeSpan PositionInTime {
			get {
				return waveStream.CurrentTime;
			}
			set {
				waveStream.CurrentTime = value;
			}
		}
		public int Length {
			get {
				return _data.Length;
			}
		}

		public double Pan {
			get {
				return _pan;
			}
			set {
				if (value == _pan || double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				if (panningProvider == null) {
					throw new Exception("Sound must be mono in order to use panning.");
				}
				_pan = value;
				panningProvider.Pan = (float) MathUtil.Clamp(-1.0d, 1.0d, _pan);
			}
		}

		public void Initialize(int audioDevice) {
			Dispose();

			waveOut = new WaveOutEvent();
			waveOut.DeviceNumber = audioDevice;
			waveOut.PlaybackStopped += onPlaybackComplete;

			byteStream = new MemoryStream(_data);
			if (_format == AudioFormat.Aiff) {
				waveStream = new AiffFileReader(byteStream);
			} else if (_format == AudioFormat.Wav) {
				waveStream = new WaveFileReader(byteStream);
			} else if (_format == AudioFormat.Mp3) {
				waveStream = new Mp3FileReader(byteStream);
			} else if (_format == AudioFormat.Vorbis) {
				waveStream = new VorbisWaveReader(byteStream);
			}

			volumeProvider = new VolumeSampleProvider(waveStream.ToSampleProvider());
			volumeProvider.Volume = (float) _volume;
			if (waveStream.WaveFormat.Channels == 1) {
				panningProvider = new PanningSampleProvider(volumeProvider);
				panningProvider.Pan = (float) _pan;
				waveOut.Init(panningProvider);
			} else {
				waveOut.Init(volumeProvider);
			}
		}

		public void Dispose() {
			if (waveStream == null) {
				return;
			}

			waveOut.Stop();
			waveOut.PlaybackStopped -= onPlaybackComplete;
			waveOut.Dispose();
			waveStream.Dispose();
			byteStream.Dispose();
			waveOut = null;
			panningProvider = null;
			volumeProvider = null;
			waveStream = null;
			byteStream = null;
		}

		//private
		private void onPlaybackComplete(object sender, StoppedEventArgs e) {
			if (e != null) {
				if (Error != null) {
					Error.Invoke(this, new ExceptionEventArgs(e.Exception));
				}
			} else {
				if (Repeating) {
					waveOut.Play();
				}
			}
		}
	}
}
