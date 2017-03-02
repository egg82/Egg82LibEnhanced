using Egg82LibEnhanced.Display;
using Egg82LibEnhanced.Patterns;
using Egg82LibEnhanced.Utils;
using System;

namespace Test.States {
	class CryptoTestState : State {
		//vars
		private ICryptoUtil cryptoUtil = ServiceLocator.GetService(typeof(ICryptoUtil));

		//constructor
		public CryptoTestState() {

		}

		//public

		//private
		protected override void OnEnter() {
			string plaintext = "Hello, world!";
			byte[] plainBytes = cryptoUtil.ToBytes(plaintext);
			byte[] key = cryptoUtil.GetRandomBytes(32);

			Console.WriteLine("Plaintext: " + plaintext);
			Console.WriteLine("Base64 Key: " + cryptoUtil.ToString(cryptoUtil.Base64Encode(key)));
			Console.WriteLine("Encrypted result #1: " + cryptoUtil.ToString(cryptoUtil.Base64Encode(cryptoUtil.EasyEncrypt(plainBytes, key))));
			Console.WriteLine("Encrypted result #2: " + cryptoUtil.ToString(cryptoUtil.Base64Encode(cryptoUtil.EasyEncrypt(plainBytes, key))));
			Console.WriteLine("Encrypted result #3: " + cryptoUtil.ToString(cryptoUtil.Base64Encode(cryptoUtil.EasyEncrypt(plainBytes, key))));
			byte[] encrypted = cryptoUtil.EasyEncrypt(plainBytes, key);
			byte[] decrypted = cryptoUtil.EasyDecrypt(encrypted, key);
			Console.WriteLine("Decrypted result: " + cryptoUtil.ToString(decrypted));
		}
		protected override void OnExit() {

		}
		protected override void OnUpdate(double deltaTime) {
			
		}
	}
}
