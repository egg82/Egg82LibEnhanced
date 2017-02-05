using SFML.Graphics;
using System;
using System.Collections.Generic;

namespace Egg82LibEnhanced.Graphics {
	public class MovieClip : DisplayObject {
		//vars
		private TextureAtlas atlas = null;
		private int _currentFrame = 0;
		private List<Texture> frames = new List<Texture>();

		private double _frameTime = 0.0d;
		private double cumulativeTime = 0.0d;

		//constructor
		public MovieClip(ref TextureAtlas atlas, double frameTime) {
			this.atlas = atlas;
			_frameTime = frameTime;
		}

		//public
		public double FrameTime {
			get {
				return _frameTime;
			}
			set {
				if (double.IsNaN(value) || double.IsInfinity(value)) {
					return;
				}
				_frameTime = value;
			}
		}

		public void AddFrame(string bitmapName) {
			AddFrame(bitmapName, frames.Count);
		}
		public void AddFrame(string bitmapName, int index) {
			if (index < 0 || index > frames.Count) {
				throw new IndexOutOfRangeException();
			}
			if (bitmapName == null || !atlas.HasValue(bitmapName)) {
				frames.Insert(index, null);
			} else {
				frames.Insert(index, atlas.GetTexture(bitmapName));
			}
		}
		public void RemoveFrame(int frame) {
			if (frame < 0 || frame >= frames.Count) {
				throw new IndexOutOfRangeException();
			}
			frames.RemoveAt(frame);
		}
		public int CurrentFrame {
			get {
				return _currentFrame;
			}
			set {
				if (value < 0 || value >= frames.Count) {
					throw new IndexOutOfRangeException();
				}
				swapTexture(value);
			}
		}
		public int NumFrames {
			get {
				return frames.Count;
			}
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			int steps = 0;

			cumulativeTime += deltaTime;
			while (cumulativeTime >= _frameTime) {
				cumulativeTime -= _frameTime;
				steps++;
			}

			swapTexture(steps);
		}

		private void swapTexture(int frame) {
			if (frame == _currentFrame) {
				return;
			}
			_currentFrame = frame;

			if (Texture != null) {
				Texture.Dispose();
			}

			Texture = (frames[frame] == null) ? null : frames[frame];
		}
	}
}
