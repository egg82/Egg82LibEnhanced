using System;
using System.Security.Cryptography;
using System.Text;

namespace Egg82LibEnhanced.Utils {
	public interface ICryptoUtil {
		//functions
		byte[] ToBytes(string input);
		byte[] ToBytes(string input, Encoding encoding);
		string ToString(byte[] input);
		string ToString(byte[] input, Encoding encoding);
		byte[] Base64Encode(byte[] input);
		byte[] Base64Encode(byte[] input, Encoding encoding);
		byte[] Base64Decode(byte[] input);
		byte[] Base64Decode(byte[] input, Encoding encoding);

		/**
		 * Provided for compatibility reasons. PLEASE don't use MD5 unless you absolutely need to. Seriously.
		 */
		byte[] HashMd5(byte[] input);
		/**
		 * Provided for compatibility reasons. PLEASE don't use SHA1 unless you absolutely need to. Seriously.
		 */
		byte[] HashSha1(byte[] input);
		byte[] HashSha256(byte[] input);
		byte[] HashSha512(byte[] input);

		/**
		 * Use this for password hashing only is scrypt is unavailable as an option.
		 */
		byte[] Bcrypt(byte[] input, byte[] salt);
		/**
		 * Use this for password hashing as your first option.
		 */
		byte[] Scrypt(byte[] input, byte[] salt, int cost = 262144);
		/**
		 * Provided for compatibility reasons. PLEASE don't use PHPass unless you absolutely need to. Seriously.
		 */
		byte[] Phpass(byte[] input);

		/**
		 * This is provided as secure-by-default. Use this as your first option, and try not to change mode or padding if you can avoid it.
		 */
		byte[] EncryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		byte[] DecryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		/**
		 * This is weak and you should try to use AES if possible. Also, try not to change mode or padding if you can avoid it.
		 */
		byte[] EncryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		byte[] DecryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);

		void AddRsaPublicKey(string name, byte[] publicKey);
		/**
		 * Use addRsaPublicKey() to add the public key to the cache first.
		 */
		byte[] EncryptRsa(string name, byte[] input, bool useLegacyPadding = false);
		byte[] DecryptRsa(byte[] input, bool useLegacyPadding = false);
		byte[] GetRsaPrivateKey();
		byte[] GetRsaPublicKey();
		/**
		 * A keypair is automatically generated, but you can use this if you need to.
		 */
		void ImportRsaPrivateKeyPair(byte[] keyPair);
		/**
		 * A keypair is automatically generated, but you can use this if you need to.
		 */
		void ImportRsaPrivateKeyPair(string absoluteFilePath);

		/**
		 * PLEASE use this in conjunction with ciphertext. This is for validation. Just append this to the end of the byte array and check on the other side. It's a hashing algorithm, treat it as one.
		 */
		byte[] Hmac256(byte[] input, byte[] key);

		byte[] GetDHPublicKey();
		byte[] GetDHKey();
		/**
		 * Here's how this works:
		 *   A sends B their public key. B calls this function with A's public key.
		 *   B sends A their public key. A calls this function with B's public key.
		 *   Finally, both A and B can use getDHKey() as their shared secret.
		 */
		void DeriveDHKey(byte[] publicKey);

		/**
		 * Use for IVs, salts, and possibly keys.
		 */
		byte[] GetRandomBytes(int length);
		double GetRandomDouble();
	}
}
