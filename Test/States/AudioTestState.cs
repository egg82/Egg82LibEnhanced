using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.IO;

namespace Test.States {
	public class AudioTestState : State {
		//vars
		private IAudioEngine audioEngine = ServiceLocator.GetService<IAudioEngine>();
		private string ambientPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + ".."  + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Assets" + Path.DirectorySeparatorChar + "Audio" + Path.DirectorySeparatorChar + "725191_Subwoofer-Lullaby.mp3";

		//constructor
		public AudioTestState() {

		}

		//public

		//private
		protected override void OnEnter() {
			FileUtil.Open(ambientPath);
			audioEngine.AddAudio("ambient1", AudioType.Ambient, AudioFormat.Mp3, FileUtil.Read(ambientPath, 0, (int) FileUtil.GetTotalBytes(ambientPath)));
			FileUtil.Close(ambientPath);
			Audio a = audioEngine.GetAudio("ambient1");
			a.Play(true);
		}
		protected override void OnExit() {

		}
		protected override void OnUpdate(double deltaTime) {
			
		}
	}
}
