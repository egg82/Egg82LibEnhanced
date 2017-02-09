using Egg82LibEnhanced.Base;
using Egg82LibEnhanced.Core;
using Egg82LibEnhanced.Engines;
using Egg82LibEnhanced.Enums;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;
using System.IO;

namespace Test.States {
	public class AudioTestState : BaseState {
		//vars
		private IAudioEngine audioEngine = ServiceLocator.GetService(typeof(IAudioEngine));
		private string ambientPath = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + ".."  + Path.DirectorySeparatorChar + ".." + Path.DirectorySeparatorChar + "Asstes" + Path.DirectorySeparatorChar + "Audio" + Path.DirectorySeparatorChar + "725191_Subwoofer-Lullaby.mp3";

		//constructor
		public AudioTestState() {

		}

		//public
		public override void OnEnter() {
			FileUtil.Open(ambientPath);
			audioEngine.AddAudio("ambient1", AudioType.Ambient, AudioFormat.Mp3, FileUtil.Read(ambientPath, 0, (int) FileUtil.GetTotalBytes(ambientPath)));
			FileUtil.Close(ambientPath);
			Audio a = audioEngine.GetAudio("ambient1");
			a.Play(true);
		}
		public override void OnExit() {
			
		}

		//private
		protected override void OnUpdate(double deltaTime) {
			
		}
	}
}
