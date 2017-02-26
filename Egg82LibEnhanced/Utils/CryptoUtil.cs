using CryptSharp;
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
		/// <summary>
		/// A utility for easily using cryptographic functions. Automatically generates PGP and RSA keypairs.
		/// </summary>
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
			
			rsaPublicKey = encodeRsaPublicKey(rsa.ExportParameters(false));
			rsaPrivateKey = encodeRsaPrivateKey(rsa.ExportParameters(true));
		}

		//public
		/// <summary>
		/// Converts a string to bytes. Uses UTF-8 encoding.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>A byte array representation of the encoded string.</returns>
		public byte[] ToBytes(string input) {
			return ToBytes(input, Encoding.UTF8);
		}
		/// <summary>
		/// Converts a string to bytes using the desired encoding.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="encoding">The string encoding to use.</param>
		/// <returns>A byte array representation of the encoded string.</returns>
		public byte[] ToBytes(string input, Encoding encoding) {
			return encoding.GetBytes(input);
		}
		/// <summary>
		/// Converts a byte array to a string. Uses UTF-8 encoding.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <returns>The output string.</returns>
		public string ToString(byte[] input) {
			return ToString(input, Encoding.UTF8);
		}
		/// <summary>
		/// Converts a byte array to a string using the desired encoding.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="encoding">The string encoding to use.</param>
		/// <returns>The output string using the provided encoding.</returns>
		public string ToString(byte[] input, Encoding encoding) {
			return encoding.GetString(input);
		}

		/// <summary>
		/// Base64-encodes a byte array. Uses UTF-8 encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <returns>The Base64-encoded output bytes using the default encoding.</returns>
		public byte[] Base64Encode(byte[] input) {
			return Base64Encode(input, Encoding.UTF8);
		}
		/// <summary>
		/// Base64-encodes a byte array using the desired encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="encoding">The encoding of the output bytes.</param>
		/// <returns>The Base64-encoded output bytes using the specified encoding.</returns>
		public byte[] Base64Encode(byte[] input, Encoding encoding) {
			return encoding.GetBytes(Convert.ToBase64String(input));
		}
		/// <summary>
		/// Base64-decodes a byte array. Uses UTF-8 encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <returns>The plaintext output bytes using the default encoding.</returns>
		public byte[] Base64Decode(byte[] input) {
			return Base64Decode(input, Encoding.UTF8);
		}
		/// <summary>
		/// Base64-decodes a byte array using the desired encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="encoding">The encoding of the input bytes.</param>
		/// <returns>The plaintext output bytes using the specified encoding.</returns>
		public byte[] Base64Decode(byte[] input, Encoding encoding) {
			return Convert.FromBase64String(encoding.GetString(input));
		}

		/// <summary>
		/// MD5-hashes the specified input. PLEASE don't use MD5 unless you absolutely need to. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The MD5-hashed result.</returns>
		public byte[] HashMd5(byte[] input) {
			return md5.ComputeHash(input);
		}
		/// <summary>
		/// SHA1-hashes the specified input. PLEASE don't use SHA1 unless you absolutely need to. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The SHA1-hashed result.</returns>
		public byte[] HashSha1(byte[] input) {
			return sha1.ComputeHash(input);
		}
		/// <summary>
		/// SHA256-hashes the specified input. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The SHA256-hashed result.</returns>
		public byte[] HashSha256(byte[] input) {
			return sha256.ComputeHash(input);
		}
		/// <summary>
		/// SHA512-hashes the specified input. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The SHA512-hashed result.</returns>
		public byte[] HashSha512(byte[] input) {
			return sha512.ComputeHash(input);
		}

		/// <summary>
		/// Securely hashes the specified input with a salt using BCrypt. Use this for password hashing only if SCrypt is unavailable as an option. This is a ONE-WAY hash, used to safely store and compare passwords. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <param name="salt">The salt to append to the plaintext input before hashing.</param>
		/// <returns>The BCrypt-hashed result.</returns>
		public byte[] Bcrypt(byte[] input, byte[] salt) {
			return ToBytes(Crypter.Blowfish.Crypt(input, ToString(salt)));
		}
		/// <summary>
		/// Securely hashes the specified input with a salt using SCrypt. Use this for password hashing as your first option. This is a ONE-WAY hash, used to safely store and compare passwords. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <param name="salt">The salt to append to the plaintext input before hashing.</param>
		/// <param name="cost">(optional) The cost of the hash. More = better, but slower.</param>
		/// <returns>The SCrypt-hashed result.</returns>
		public byte[] Scrypt(byte[] input, byte[] salt, int cost = 262144) {
			return CryptSharp.Utility.SCrypt.ComputeDerivedKey(input, salt, cost, 8, 1, null, 128);
		}
		/// <summary>
		/// Hashes the specified input with a salt using PHPass. PLEASE don't use PHPass unless you absolutely need to. This is a ONE-WAY hash, used to store and compare passwords. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The PHPass-hashed result.</returns>
		public byte[] Phpass(byte[] input) {
			return ToBytes(Crypter.Phpass.Crypt(input));
		}

		/// <summary>
		/// The easiest way of encrypting/decrypting, provided as secure-by-default. Use EasyDecrypt() to decrypt ciphertext.
		/// </summary>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="key">Key to encrypt plaintext with. Can be any length.</param>
		/// <returns>256-bit AES encrypted ciphertext.</returns>
		public byte[] EasyEncrypt(byte[] input, byte[] key) {
			key = HashSha256(key);
			byte[] iv = GetRandomBytes(16);
			byte[] encrypted = EncryptAes(input, key, iv);
			byte[] combined = Combine(iv, encrypted);
			byte[] hmac = Hmac256(combined, key);
			return Combine(hmac, combined);
		}
		/// <summary>
		/// Decrypts input povided by EasyEncrypt().
		/// </summary>
		/// <param name="input">Ciphertext to decrypt.</param>
		/// <param name="key">The key that was used to encrypt the ciphertext. Can be any length.</param>
		/// <returns>Decrypted plaintext.</returns>
		public byte[] EasyDecrypt(byte[] input, byte[] key) {
			byte[] newInput = GetPartial(input, input.Length - 48, 48);
			key = HashSha256(key);
			byte[] iv = GetPartial(input, 16, 32);
			byte[] hmac = GetPartial(input, 32);
			byte[] combined = GetPartial(input, input.Length - 32, 32);

			if (!ByteArraysAreEqual(Hmac256(combined, key), hmac)) {
				throw new CryptographicException("HMAC validation failed.");
			}
			
			return DecryptAes(newInput, key, iv, CipherMode.CFB, PaddingMode.PKCS7);
		}

		/// <summary>
		/// Encrypts input using the Advanced Encryption Standard. This is provided as secure-by-default. Use this as your first option, and try not to change the mode or padding if you can avoid it.
		/// </summary>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="key">Key to encrypt plaintext with. Use standard lengths.</param>
		/// <param name="iv">Initialization vector for the algorithm. Use standard lengths.</param>
		/// <param name="mode">(optional) Cipher mode to use. Please stick with the default if possible.</param>
		/// <param name="padding">(optional) Padding mode to use. Please stick with the default if possible.</param>
		/// <returns>AES-encrypted ciphertext.</returns>
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
		/// <summary>
		/// Decrypts input provided by the Advanced Encryption Standard.
		/// </summary>
		/// <param name="input">Ciphertext to decrypt.</param>
		/// <param name="key">The key that was used to encrypt the ciphertext.</param>
		/// <param name="iv">The IV that was used to encrypt the ciphertext.</param>
		/// <param name="mode">The cipher mode that was used to encrypt the ciphertext.</param>
		/// <param name="padding">The padding mode that was used to encrypt the ciphertext.</param>
		/// <returns>Decrypted plaintext.</returns>
		public byte[] DecryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding) {
			lock (aesLock) {
				aes.Mode = mode;
				aes.Padding = padding;
				ICryptoTransform cryptor = aes.CreateDecryptor(key, iv);
				byte[] retVal = cryptor.TransformFinalBlock(input, 0, input.Length);
				cryptor.Dispose();
				return retVal;
			}
		}

		/// <summary>
		/// Use this for encryption only if AES is unavailable as an option. Try not to change the mode or padding if you can avoid it.
		/// </summary>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="key">Key to encrypt plaintext with. Use standard lengths.</param>
		/// <param name="iv">Initialization vector for the algorithm. Use standard lengths.</param>
		/// <param name="mode">(optional) Cipher mode to use. Please stick with the default if possible.</param>
		/// <param name="padding">(optional) Padding mode to use. Please stick with the default if possible.</param>
		/// <returns>3DES-encrypted ciphertext.</returns>
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
		/// <summary>
		/// Decrypts input provided by 3DES.
		/// </summary>
		/// <param name="input">Ciphertext to decrypt.</param>
		/// <param name="key">The key that was used to encrypt the ciphertext.</param>
		/// <param name="iv">The IV that was used to encrypt the ciphertext.</param>
		/// <param name="mode">The cipher mode that was used to encrypt the ciphertext.</param>
		/// <param name="padding">The padding mode that was used to encrypt the ciphertext.</param>
		/// <returns>Decrypted plaintext.</returns>
		public byte[] DecryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding) {
			lock (triDesLock) {
				tripleDes.Mode = mode;
				tripleDes.Padding = padding;
				ICryptoTransform cryptor = tripleDes.CreateDecryptor(key, iv);
				byte[] retVal = cryptor.TransformFinalBlock(input, 0, input.Length);
				cryptor.Dispose();
				return retVal;
			}
		}

		/// <summary>
		/// Adds a PGP public key to the keyring for later encryption and signing verification.
		/// </summary>
		/// <param name="name">The name to give the PGP public key.</param>
		/// <param name="publicKey">The PGP public key.</param>
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

		/// <summary>
		/// Adds an RSA public key to the keyring for later encryption.
		/// </summary>
		/// <param name="name">The name to give the RSA public key.</param>
		/// <param name="publicKey">The RSA public key.</param>
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
		/// <summary>
		/// Encrypts the input using the RSA public key added by AddRsaPublicKey() earlier.
		/// </summary>
		/// <param name="name">The name of the RSA public key to use.</param>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="useLegacyPadding">(optional) Enable is legacy padding is needed (Windows XP or earlier).</param>
		/// <returns>RSA-encrypted ciphertext.</returns>
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
		/// <summary>
		/// Decrypts the input using the RSA private key in the keyring.
		/// </summary>
		/// <param name="input">The ciphertext to decrypt.</param>
		/// <param name="useLegacyPadding">(optional) Enable is legacy padding is needed (Windows XP or earlier).</param>
		/// <returns>Decrypted plaintext.</returns>
		public byte[] DecryptRsa(byte[] input, bool useLegacyPadding = false) {
			if (input == null) {
				throw new ArgumentNullException("input");
			}

			return rsa.Decrypt(input, !useLegacyPadding);
		}

		/// <summary>
		/// The current RSA private key in the keyring.
		/// </summary>
		public byte[] RsaPrivateKey {
			get {
				return (byte[]) rsaPrivateKey.Clone();
			}
		}
		/// <summary>
		/// The current RSA public key in the keyring.
		/// </summary>
		public byte[] RsaPublicKey {
			get {
				return (byte[]) rsaPublicKey.Clone();
			}
		}
		/// <summary>
		/// Imports an RSA keypair for encryption and decryption. A keypair is automaticaly generated, but this may be useful for long-term RSA keys.
		/// </summary>
		/// <param name="keyPair">The keypair file to import.</param>
		public void ImportRsaPrivateKeyPair(byte[] keyPair) {
			rsa = (RSACryptoServiceProvider) new X509Certificate2(keyPair).PrivateKey;
			rsaPrivateKey = encodeRsaPrivateKey(rsa.ExportParameters(true));
			rsaPublicKey = encodeRsaPublicKey(rsa.ExportParameters(false));
		}
		/// <summary>
		/// Imports an RSA keypair for encryption and decryption. A keypair is automaticaly generated, but this may be useful for long-term RSA keys.
		/// </summary>
		/// <param name="absoluteFilePath">The absolute path of the keypair file.</param>
		public void ImportRsaPrivateKeyPair(string absoluteFilePath) {
			rsa = (RSACryptoServiceProvider) new X509Certificate2(absoluteFilePath).PrivateKey;
			rsaPrivateKey = encodeRsaPrivateKey(rsa.ExportParameters(true));
			rsaPublicKey = encodeRsaPublicKey(rsa.ExportParameters(false));
		}

		/// <summary>
		/// The current DH public key in the keyring.
		/// </summary>
		public byte[] DHPublicKey {
			get {
				return dh.PublicKey.ToByteArray();
			}
		}
		/// <summary>
		/// The current DH shared secret, used as a key for encryption and decryption.
		/// </summary>
		public byte[] DHSharedSecret {
			get {
				if (dhKey == null) {
					return null;
				}
				return (byte[]) dhKey.Clone();
			}
		}
		/// <summary>
		/// Here's how this works:
		///   1. A sends B their public key from DHPublicKey. B calls this function with A's public key.
		///   2. B sends A their public key from DHPublicKey. A calls this function with B's public key.
		///   3. Finally, both A and B can use DHSharedSecret as their encryption key to eachother.
		/// </summary>
		/// <param name="publicKey">The other party's public DH key.</param>
		public void DeriveDHKey(byte[] publicKey) {
			dhKey = dh.DeriveKeyMaterial(CngKey.Import(publicKey, CngKeyBlobFormat.EccPublicBlob));
		}

		/// <summary>
		/// Securely hashes the specified input with the key. PLEASE use this in conjunction with ciphertext. This is for validation- just append this to the end of the byte array and check on the other side. It's a hashing algorithm, treat it as one.
		/// </summary>
		/// <param name="input">The input to hash.</param>
		/// <param name="key">The key to hash the input with. Can be any length.</param>
		/// <returns>HMAC-256 hashed result.</returns>
		public byte[] Hmac256(byte[] input, byte[] key) {
			HMACSHA256 hmac = new HMACSHA256(HashSha256(key));
			byte[] retVal = hmac.ComputeHash(input);
			hmac.Dispose();
			return retVal;
		}

		/// <summary>
		/// Creates a byte array using a cryptographically secure random number generator. Used for keys, IVs, and salts.
		/// </summary>
		/// <param name="length">The number of bytes to generate.</param>
		/// <returns>A securely-random array.</returns>
		public byte[] GetRandomBytes(int length) {
			byte[] retVal = new byte[length];
			rng.GetBytes(retVal);
			return retVal;
		}
		/// <summary>
		/// Generates a secure random double between -1 and 1, inclusive.
		/// </summary>
		/// <returns>A securely-random double.</returns>
		public double GetRandomDouble() {
			byte[] retVal = new byte[8];
			rng.GetBytes(retVal);
			return BitConverter.ToDouble(retVal, 0) / double.MaxValue;
		}

		/// <summary>
		/// Creates a new byte array of specified length starting from specified index from the input array. Think "Substring" for byte arrays.
		/// </summary>
		/// <param name="input">The input array to aplice.</param>
		/// <param name="length">The number of bytes in the returned array.</param>
		/// <param name="index">(optional) The zero-based starting byte position.</param>
		/// <returns>The partial/"Substringed" array.</returns>
		public byte[] GetPartial(byte[] input, int length, int index = 0) {
			byte[] retVal = new byte[length];
			Array.Copy(input, index, retVal, 0, length);
			return retVal;
		}
		/// <summary>
		/// Combines two byte arrays, starting with a and ending with b.
		/// </summary>
		/// <param name="a">The first byte array in the returned array.</param>
		/// <param name="b">The second byte array in the returned array.</param>
		/// <returns>A combined array containing inputs a, then b.</returns>
		public byte[] Combine(byte[] a, byte[] b) {
			byte[] retVal = new byte[a.Length + b.Length];
			Array.Copy(a, 0, retVal, 0, a.Length);
			Array.Copy(b, 0, retVal, a.Length, b.Length);
			return retVal;
		}
		/// <summary>
		/// Determines wther two byte array are equal.
		/// </summary>
		/// <param name="a">The first array.</param>
		/// <param name="b">The second array.</param>
		/// <returns>True if equal, false if not.</returns>
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
