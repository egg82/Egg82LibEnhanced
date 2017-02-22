﻿using CryptSharp;
using CryptSharp.Utility;
using Org.BouncyCastle.Bcpg.OpenPgp;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Egg82LibEnhanced.Utils {
	public class CryptoUtil : ICryptoUtil {
		//vars
		private MD5 md5 = MD5.Create();
		private SHA1 sha1 = SHA1.Create();
		private SHA256 sha256 = SHA256.Create();
		private SHA512 sha512 = SHA512.Create();

		private object aesLock = new object();
		private AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
		private object triDesLock = new object();
		private TripleDES tripleDes = TripleDES.Create();

		private ECDiffieHellmanCng dh = new ECDiffieHellmanCng(521);
		private byte[] dhKey = null;

		private RSACryptoServiceProvider rsa = new RSACryptoServiceProvider(4096);
		private byte[] rsaPrivateKey = null;
		private byte[] rsaPublicKey = null;

		private RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
		private SecureRandom pgpRng = new SecureRandom();

		private Dictionary<string, RSACryptoServiceProvider> rsaCache = new Dictionary<string, RSACryptoServiceProvider>();
		private Dictionary<string, PgpPublicKey> pgpCache = new Dictionary<string, PgpPublicKey>();
		private PgpPublicKey pgpPublicKey = null;
		private PgpPrivateKey pgpPrivateKey = null;

		//constructor
		public CryptoUtil() {
			DsaKeyPairGenerator gen = new DsaKeyPairGenerator();
			DsaParametersGenerator pGen = new DsaParametersGenerator();
			pgpRng.SetSeed(GetRandomBytes(512));
			pGen.Init(1024, 100, pgpRng);
			DsaKeyGenerationParameters p = new DsaKeyGenerationParameters(pgpRng, pGen.GenerateParameters());
			gen.Init(p);
			AsymmetricCipherKeyPair keys = gen.GenerateKeyPair();
			pgpPublicKey = new PgpPublicKey(Org.BouncyCastle.Bcpg.PublicKeyAlgorithmTag.Dsa, keys.Public, DateTime.Now);
			pgpPrivateKey = new PgpPrivateKey(pgpPublicKey.KeyId, pgpPublicKey.PublicKeyPacket, keys.Private);
		}

		//public
		public byte[] ToBytes(string input) {
			return ToBytes(input, Encoding.UTF8);
		}
		public byte[] ToBytes(string input, Encoding encoding) {
			return encoding.GetBytes(input);
		}

		public string ToString(byte[] input) {
			return ToString(input, Encoding.UTF8);
		}
		public string ToString(byte[] input, Encoding encoding) {
			return encoding.GetString(input);
		}

		public byte[] Base64Encode(byte[] input) {
			return Base64Encode(input, Encoding.UTF8);
		}
		public byte[] Base64Encode(byte[] input, Encoding encoding) {
			return encoding.GetBytes(Convert.ToBase64String(input));
		}

		public byte[] Base64Decode(byte[] input) {
			return Base64Decode(input, Encoding.UTF8);
		}
		public byte[] Base64Decode(byte[] input, Encoding encoding) {
			return Convert.FromBase64String(encoding.GetString(input));
		}

		///<summary>
		///Provided for compatibility reasons. PLEASE don't use MD5 unless you absolutely need to. Seriously.
		///</summary>
		public byte[] HashMd5(byte[] input) {
			return md5.ComputeHash(input);
		}
		///<summary>
		///Provided for compatibility reasons. PLEASE don't use SHA1 unless you absolutely need to. Seriously.
		///</summary>
		public byte[] HashSha1(byte[] input) {
			return sha1.ComputeHash(input);
		}
		public byte[] HashSha256(byte[] input) {
			return sha256.ComputeHash(input);
		}
		public byte[] HashSha512(byte[] input) {
			return sha512.ComputeHash(input);
		}

		///<summary>
		///Use this for password hashing only is scrypt is unavailable as an option.
		///</summary>
		public byte[] Bcrypt(byte[] input, byte[] salt) {
			return ToBytes(Crypter.Blowfish.Crypt(input, ToString(salt)));
		}
		///<summary>
		///Use this for password hashing as your first option.
		///</summary>
		public byte[] Scrypt(byte[] input, byte[] salt, int cost = 262144) {
			return CryptSharp.Utility.SCrypt.ComputeDerivedKey(input, salt, cost, 8, 1, null, 128);
		}

		///<summary>
		///Provided for compatibility reasons. PLEASE don't use PHPass unless you absolutely need to. Seriously.
		///</summary>
		public byte[] Phpass(byte[] input) {
			return ToBytes(Crypter.Phpass.Crypt(input));
		}

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
		public byte[] EasyEncrypt(byte[] input, byte[] key) {
			key = HashSha256(key);
			byte[] iv = GetRandomBytes(16);
			byte[] encrypted = EncryptAes(input, key, iv);
			byte[] combined = Combine(iv, encrypted);
			byte[] hmac = Hmac256(combined, key);
			return Combine(hmac, combined);
		}
		public byte[] EasyDecrypt(byte[] input, byte[] key) {
			byte[] newInput = GetPartial(input, input.Length - 48, 48);
			key = HashSha256(key);
			byte[] iv = GetPartial(input, 16, 32);
			byte[] hmac = GetPartial(input, 32);
			byte[] combined = GetPartial(input, input.Length - 32, 32);

			if (!ByteArraysAreEqual(Hmac256(combined, key), hmac)) {
				throw new Exception("HMAC validation failed.");
			}
			
			return DecryptAes(newInput, key, iv);
		}

		///<summary>
		///This is provided as secure-by-default. Use this as your first option, and try not to change mode or padding if you can avoid it.
		///</summary>
		public byte[] EncryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7) {
			lock (aesLock) {
				aes.Mode = mode;
				aes.Padding = padding;
				ICryptoTransform cryptor = aes.CreateEncryptor(key, iv);
				byte[] retVal = cryptor.TransformFinalBlock(input, 0, input.Length);
				cryptor.Dispose();
				return retVal;
			}
		}
		public byte[] DecryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7) {
			lock (aesLock) {
				aes.Mode = mode;
				aes.Padding = padding;
				ICryptoTransform cryptor = aes.CreateDecryptor(key, iv);
				byte[] retVal = cryptor.TransformFinalBlock(input, 0, input.Length);
				cryptor.Dispose();
				return retVal;
			}
		}

		///<summary>
		///This is weak and you should try to use AES if possible. Also, try not to change mode or padding if you can avoid it.
		///</summary>
		public byte[] EncryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7) {
			lock (triDesLock) {
				tripleDes.Mode = mode;
				tripleDes.Padding = padding;
				ICryptoTransform cryptor = tripleDes.CreateEncryptor(key, iv);
				byte[] retVal = cryptor.TransformFinalBlock(input, 0, input.Length);
				cryptor.Dispose();
				return retVal;
			}
		}
		public byte[] DecryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7) {
			lock (triDesLock) {
				tripleDes.Mode = mode;
				tripleDes.Padding = padding;
				ICryptoTransform cryptor = tripleDes.CreateDecryptor(key, iv);
				byte[] retVal = cryptor.TransformFinalBlock(input, 0, input.Length);
				cryptor.Dispose();
				return retVal;
			}
		}

		public void AddPgpPublicKey(string name, byte[] publicKey) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (publicKey == null) {
				throw new ArgumentNullException("publicKey");
			}

			using (MemoryStream stream = new MemoryStream(publicKey)) {
				using (Stream decodedStream = PgpUtilities.GetDecoderStream(stream)) {
					PgpPublicKeyRingBundle keys = new PgpPublicKeyRingBundle(decodedStream);
					foreach (PgpPublicKeyRing ring in keys.GetKeyRings()) {
						foreach (PgpPublicKey key in ring.GetPublicKeys()) {
							if (key.IsEncryptionKey) {
								if (pgpCache.ContainsKey(name)) {
									pgpCache[name] = key;
								} else {
									pgpCache.Add(name, key);
								}
								return;
							}
						}
					}
				}
			}

			throw new Exception("publicKey is not a valid PGP public key.");
		}
		/*///<summary>
		///Use AddPgpPublicKey() to add the public key to the cache first.
		///</summary>
		public byte[] EncryptPgp(string name, byte[] input) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (input == null) {
				throw new ArgumentNullException("input");
			}

			PgpPublicKey publicKey = null;
			if (pgpCache.TryGetValue(name, out publicKey)) {
				
			}
			return null;
		}
		///<summary>
		///Use AddPgpPublicKey() to add the public key to the cache first.
		///</summary>
		public bool VerifyPgp(string name, byte[] input, byte[] signature) {

		}
		public byte[] DecryptPgp(byte[] input) {

		}
		public byte[] SignPgp(byte[] input) {

		}

		public byte[] GetPgpPrivateKey() {
			
		}
		public byte[] GetPgpPublicKey() {
			
		}
		///<summary>
		///A keypair is automatically generated, but you can use this if you need to.
		///</summary>
		public void ImportPgpPrivateKeyPair(byte[] keyPair) {
			
		}
		///<summary>
		///A keypair is automatically generated, but you can use this if you need to.
		///</summary>
		public void ImportPgpPrivateKeyPair(string absoluteFilePath) {
			
		}*/

		public void AddRsaPublicKey(string name, byte[] publicKey) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (publicKey == null) {
				throw new ArgumentNullException("publicKey");
			}

			RSACryptoServiceProvider provider = (RSACryptoServiceProvider) new X509Certificate2(Base64Decode(publicKey)).PublicKey.Key;

			if (rsaCache.ContainsKey(name)) {
				rsaCache[name] = provider;
			} else {
				rsaCache.Add(name, provider);
			}
		}
		///<summary>
		///Use AddRsaPublicKey() to add the public key to the cache first.
		///</summary>
		public byte[] EncryptRsa(string name, byte[] input, bool useLegacyPadding = false) {
			if (name == null) {
				throw new ArgumentNullException("name");
			}
			if (input == null) {
				throw new ArgumentNullException("input");
			}

			RSACryptoServiceProvider provider = null;
			if (rsaCache.TryGetValue(name, out provider)) {
				return provider.Encrypt(input, !useLegacyPadding);
			}
			return null;
		}
		public byte[] DecryptRsa(byte[] input, bool useLegacyPadding = false) {
			return rsa.Decrypt(input, !useLegacyPadding);
		}

		public byte[] GetRsaPrivateKey() {
			if (rsaPrivateKey == null) {
				rsaPrivateKey = encodeRsaPrivateKey(rsa.ExportParameters(true));
			}
			return (byte[]) rsaPrivateKey.Clone();
		}
		public byte[] GetRsaPublicKey() {
			if (rsaPublicKey == null) {
				rsaPublicKey = encodeRsaPublicKey(rsa.ExportParameters(false));
			}
			return (byte[]) rsaPublicKey.Clone();
		}
		///<summary>
		///A keypair is automatically generated, but you can use this if you need to.
		///</summary>
		public void ImportRsaPrivateKeyPair(byte[] keyPair) {
			rsa = (RSACryptoServiceProvider) new X509Certificate2(keyPair).PrivateKey;
			rsaPrivateKey = encodeRsaPrivateKey(rsa.ExportParameters(true));
			rsaPublicKey = encodeRsaPublicKey(rsa.ExportParameters(false));
		}
		///<summary>
		///A keypair is automatically generated, but you can use this if you need to.
		///</summary>
		public void ImportRsaPrivateKeyPair(string absoluteFilePath) {
			rsa = (RSACryptoServiceProvider) new X509Certificate2(absoluteFilePath).PrivateKey;
			rsaPrivateKey = encodeRsaPrivateKey(rsa.ExportParameters(true));
			rsaPublicKey = encodeRsaPublicKey(rsa.ExportParameters(false));
		}

		public byte[] GetDHPublicKey() {
			return dh.PublicKey.ToByteArray();
		}
		public byte[] GetDHKey() {
			if (dhKey == null) {
				return null;
			}
			return (byte[]) dhKey.Clone();
		}
		///<summary>
		///Here's how this works:
		///  A sends B their public key from GetDHPublicKey(). B calls this function with A's public key.
		///  B sends A their public key from GetDHPublicKey(). A calls this function with B's public key.
		///  Finally, both A and B can use getDHKey() as their shared secret.
		///</summary>
		public void DeriveDHKey(byte[] publicKey) {
			dhKey = dh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob));
		}

		///<summary>
		///PLEASE use this in conjunction with ciphertext. This is for validation. Just append this to the end of the byte array and check on the other side. It's a hashing algorithm, treat it as one.
		///</summary>
		public byte[] Hmac256(byte[] input, byte[] key) {
			HMACSHA256 hmac = new HMACSHA256(key);
			byte[] retVal = hmac.ComputeHash(input);
			hmac.Dispose();
			return retVal;
		}

		///<summary>
		///Use for IVs and salts. You can also use for keys if you can figure out a way to securely implement key storage.
		///</summary>
		public byte[] GetRandomBytes(int length) {
			byte[] retVal = new byte[length];
			rng.GetBytes(retVal);
			return retVal;
		}
		///<summary>
		///Slow, but secure. Use if you need it, but outside of crypto you probably don't.
		///</summary>
		///<returns>
		///A double between -1.0d and 1.0d, inclusive.
		///</returns>
		public double GetRandomDouble() {
			byte[] retVal = new byte[8];
			rng.GetBytes(retVal);
			return BitConverter.ToDouble(retVal, 0) / double.MaxValue;
		}
		/// <summary>
		/// Returns a byte array of specified index and length from the input array. Think "Substring" but for byte arrays.
		/// </summary>
		public byte[] GetPartial(byte[] input, int length, int index = 0) {
			byte[] retVal = new byte[length];
			Array.Copy(input, index, retVal, 0, length);
			return retVal;
		}
		public byte[] Combine(byte[] a, byte[] b) {
			byte[] retVal = new byte[a.Length + b.Length];
			Array.Copy(a, 0, retVal, 0, a.Length);
			Array.Copy(b, 0, retVal, a.Length, b.Length);
			return retVal;
		}

		public bool ByteArraysAreEqual(byte[] a, byte[] b) {
			if (a.Length != b.Length) {
				return false;
			}

			for (int i = 0; i < a.Length; i++) {
				if (a[i] != b[i]) {
					return false;
				}
			}

			return true;
		}

		//private
		private byte[] encodeRsaPrivateKey(RSAParameters p) {
			byte[] retVal = null;

			using (MemoryStream stream = new MemoryStream()) {
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write((byte) 0x30);
				using (MemoryStream innerStream = new MemoryStream()) {
					BinaryWriter innerWriter = new BinaryWriter(innerStream);
					//encodeIntegerToBigEndian(innerWriter, new byte[] { 0x00 });
					encodeIntegerToBigEndian(innerWriter, p.Modulus);
					encodeIntegerToBigEndian(innerWriter, p.Exponent);
					encodeIntegerToBigEndian(innerWriter, p.D);
					encodeIntegerToBigEndian(innerWriter, p.P);
					encodeIntegerToBigEndian(innerWriter, p.Q);
					encodeIntegerToBigEndian(innerWriter, p.DP);
					encodeIntegerToBigEndian(innerWriter, p.DQ);
					encodeIntegerToBigEndian(innerWriter, p.InverseQ);
					int length = (int) innerStream.Length;
					encodeLength(writer, length);
					writer.Write(innerStream.ToArray(), 0, length);
				}
				retVal = Base64Encode(stream.ToArray());
			}

			return retVal;
		}
		private byte[] encodeRsaPublicKey(RSAParameters p) {
			byte[] retVal = null;

			using (MemoryStream stream = new MemoryStream()) {
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write((byte) 0x30);
				using (MemoryStream innerStream = new MemoryStream()) {
					BinaryWriter innerWriter = new BinaryWriter(innerStream);
					innerWriter.Write((byte) 0x30);
					encodeLength(innerWriter, 13);
					innerWriter.Write((byte) 0x06);
					byte[] rsaEncryptionOid = new byte[] { 0x2a, 0x86, 0x48, 0x86, 0xf7, 0x0d, 0x01, 0x01, 0x01 };
					encodeLength(innerWriter, rsaEncryptionOid.Length);
					innerWriter.Write(rsaEncryptionOid);
					innerWriter.Write((byte) 0x05);
					encodeLength(innerWriter, 0);
					innerWriter.Write((byte) 0x03);
					using (MemoryStream bitStringStream = new MemoryStream()) {
						BinaryWriter bitStringWriter = new BinaryWriter(bitStringStream);
						bitStringWriter.Write((byte) 0x00);
						bitStringWriter.Write((byte) 0x30);
						using (MemoryStream paramsStream = new MemoryStream()) {
							BinaryWriter paramsWriter = new BinaryWriter(paramsStream);
							encodeIntegerToBigEndian(paramsWriter, p.Modulus);
							encodeIntegerToBigEndian(paramsWriter, p.Exponent);
							int paramsLength = (int) paramsStream.Length;
							encodeLength(bitStringWriter, paramsLength);
							bitStringWriter.Write(paramsStream.ToArray(), 0, paramsLength);
						}
						int bitStringLength = (int) bitStringStream.Length;
						encodeLength(innerWriter, bitStringLength);
						innerWriter.Write(bitStringStream.ToArray(), 0, bitStringLength);
					}
					int length = (int) innerStream.Length;
					encodeLength(writer, length);
					writer.Write(innerStream.ToArray(), 0, length);
				}
				retVal = Base64Encode(stream.ToArray());
			}

			return retVal;
		}
		private void encodeIntegerToBigEndian(BinaryWriter stream, byte[] value, bool forceUnsigned = true) {
			stream.Write((byte) 0x02);
			int prefixZeroes = 0;

			for (int i = 0; i < value.Length; i++) {
				if (value[i] != 0) {
					break;
				}
				prefixZeroes++;
			}
			if (value.Length - prefixZeroes == 0) {
				encodeLength(stream, 1);
				stream.Write((byte) 0);
			} else {
				if (forceUnsigned && value[prefixZeroes] > 0x7F) {
					encodeLength(stream, value.Length - prefixZeroes + 1);
					stream.Write((byte) 0);
				} else {
					encodeLength(stream, value.Length - prefixZeroes);
				}
				for (int i = prefixZeroes; i < value.Length; i++) {
					stream.Write(value[i]);
				}
			}
		}
		private void encodeLength(BinaryWriter stream, int length) {
			if (length < 0) {
				return;
			}
			if (length < 0x80) {
				stream.Write((byte) length);
			} else {
				int temp = length;
				int bytesRequred = 0;
				while (temp > 0) {
					temp >>= 8;
					bytesRequred++;
				}
				stream.Write((byte) (bytesRequred | 0x80));
				for (int i = bytesRequred - 1; i >= 0; i--) {
					stream.Write((byte) (length >> (8 * i) & 0xFF));
				}
			}
		}
	}
}
