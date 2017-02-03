using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.IO;

namespace Test.States {
	public class AudioTestState : BaseState {
		//vars
		private IAudioEngine audioEngine = ServiceLocator.GetService(typeof(IAudioEngine));
		private string ambientPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "Audio" + Path.DirectorySeparatorChar + "725191_Subwoofer-Lullaby.mp3";

		//constructor
		public AudioTestState() {

		}

		//public
		public override void OnEnter() {
			FileUtil.Open(ambientPath);
			audioEngine.AddAudio("ambient1", Egg82LibEnhanced.Enums.AudioType.Ambient, Egg82LibEnhanced.Enums.AudioFormat.Mp3, FileUtil.Read(ambientPath, 0, (int) FileUtil.GetTotalBytes(ambientPath)));
			FileUtil.Close(ambientPath);
			audioEngine.PlayAudio("ambient1", true);
		}
		public override void OnExit() {
			
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			
		}
	}
}
