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

		///<summary>
		///Provided for compatibility reasons. PLEASE don't use MD5 unless you absolutely need to. Seriously.
		///</summary>
		byte[] HashMd5(byte[] input);
		///<summary>
		///Provided for compatibility reasons. PLEASE don't use SHA1 unless you absolutely need to. Seriously.
		///</summary>
		byte[] HashSha1(byte[] input);
		byte[] HashSha256(byte[] input);
		byte[] HashSha512(byte[] input);

		///<summary>
		///Use this for password hashing only is scrypt is unavailable as an option.
		///</summary>
		byte[] Bcrypt(byte[] input, byte[] salt);
		///<summary>
		///Use this for password hashing as your first option.
		///</summary>
		byte[] Scrypt(byte[] input, byte[] salt, int cost = 262144);
		///<summary>
		///Provided for compatibility reasons. PLEASE don't use PHPass unless you absolutely need to. Seriously.
		///</summary>
		byte[] Phpass(byte[] input);

		///<summary>
		///The easiest way of encrypting/decrypting, provided as secure-by-default for lazy people.
		///</summary>
		///<param name="input">
		///Plaintext to encrypt. Can be any length.
		///</param>
		///<param name="key">
		///Key to encrypt plaintext with. Can be any length.
		///</param>
		///<returns>
		///256-bit ciphertext.
		///</returns>
		byte[] EasyEncrypt(byte[] input, byte[] key);
		byte[] EasyDecrypt(byte[] input, byte[] key);

		///<summary>
		///This is provided as secure-by-default. Use this as your first option, and try not to change mode or padding if you can avoid it.
		///</summary>
		byte[] EncryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		byte[] DecryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		///<summary>
		///This is weak and you should try to use AES if possible. Also, try not to change mode or padding if you can avoid it.
		///</summary>
		byte[] EncryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		byte[] DecryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);

		void AddRsaPublicKey(string name, byte[] publicKey);
		///<summary>
		///Use AddRsaPublicKey() to add the public key to the cache first.
		///</summary>
		byte[] EncryptRsa(string name, byte[] input, bool useLegacyPadding = false);
		byte[] DecryptRsa(byte[] input, bool useLegacyPadding = false);
		byte[] GetRsaPrivateKey();
		byte[] GetRsaPublicKey();
		///<summary>
		///A keypair is automatically generated, but you can use this if you need to.
		///</summary>
		void ImportRsaPrivateKeyPair(byte[] keyPair);
		///<summary>
		///A keypair is automatically generated, but you can use this if you need to.
		///</summary>
		void ImportRsaPrivateKeyPair(string absoluteFilePath);

		byte[] GetDHPublicKey();
		byte[] GetDHKey();
		///<summary>
		///Here's how this works:
		///  A sends B their public key from GetDHPublicKey(). B calls this function with A's public key.
		///  B sends A their public key from GetDHPublicKey(). A calls this function with B's public key.
		///  Finally, both A and B can use getDHKey() as their shared secret.
		///</summary>
		void DeriveDHKey(byte[] publicKey);

		///<summary>
		///PLEASE use this in conjunction with ciphertext. This is for validation. Just append this to the end of the byte array and check on the other side. It's a hashing algorithm, treat it as one.
		///</summary>
		byte[] Hmac256(byte[] input, byte[] key);

		///<summary>
		///Use for IVs and salts. You can also use for keys if you can figure out a way to securely implement key storage.
		///</summary>
		byte[] GetRandomBytes(int length);
		///<summary>
		///Slow, but secure. Use if you need it, but outside of crypto you probably don't.
		///</summary>
		///<returns>
		///A double between -1.0d and 1.0d, inclusive.
		///</returns>
		double GetRandomDouble();
		/// <summary>
		/// Returns a byte array of specified index and length from the input array. Think "Substring" but for byte arrays.
		/// </summary>
		byte[] GetPartial(byte[] input, int length, int index = 0);
		byte[] Combine(byte[] a, byte[] b);
		bool ByteArraysAreEqual(byte[] a, byte[] b);
	}
}
