using System;
using System.Security.Cryptography;
using System.Text;

namespace Egg82LibEnhanced.Utils {
	public interface ICryptoUtil {
		/// <summary>
		/// Converts a string to bytes. Uses UTF-8 encoding.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <returns>A byte array representation of the encoded string.</returns>
		byte[] ToBytes(string input);
		/// <summary>
		/// Converts a string to bytes using the desired encoding.
		/// </summary>
		/// <param name="input">The input string.</param>
		/// <param name="encoding">The string encoding to use.</param>
		/// <returns>A byte array representation of the encoded string.</returns>
		byte[] ToBytes(string input, Encoding encoding);
		/// <summary>
		/// Converts a byte array to a string. Uses UTF-8 encoding.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <returns>The output string.</returns>
		string ToString(byte[] input);
		/// <summary>
		/// Converts a byte array to a string using the desired encoding.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="encoding">The string encoding to use.</param>
		/// <returns>The output string using the provided encoding.</returns>
		string ToString(byte[] input, Encoding encoding);

		/// <summary>
		/// Base64-encodes a byte array. Uses UTF-8 encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <returns>The Base64-encoded output bytes using the default encoding.</returns>
		byte[] Base64Encode(byte[] input);
		/// <summary>
		/// Base64-encodes a byte array using the desired encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="encoding">The encoding of the output bytes.</param>
		/// <returns>The Base64-encoded output bytes using the specified encoding.</returns>
		byte[] Base64Encode(byte[] input, Encoding encoding);
		/// <summary>
		/// Base64-decodes a byte array. Uses UTF-8 encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <returns>The plaintext output bytes using the default encoding.</returns>
		byte[] Base64Decode(byte[] input);
		/// <summary>
		/// Base64-decodes a byte array using the desired encoding. This is NOT an encryption algorithm.
		/// </summary>
		/// <param name="input">The input bytes.</param>
		/// <param name="encoding">The encoding of the input bytes.</param>
		/// <returns>The plaintext output bytes using the specified encoding.</returns>
		byte[] Base64Decode(byte[] input, Encoding encoding);

		/// <summary>
		/// MD5-hashes the specified input. PLEASE don't use MD5 unless you absolutely need to. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The MD5-hashed result.</returns>
		byte[] HashMd5(byte[] input);
		/// <summary>
		/// SHA1-hashes the specified input. PLEASE don't use SHA1 unless you absolutely need to. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The SHA1-hashed result.</returns>
		byte[] HashSha1(byte[] input);
		/// <summary>
		/// SHA256-hashes the specified input. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The SHA256-hashed result.</returns>
		byte[] HashSha256(byte[] input);
		/// <summary>
		/// SHA512-hashes the specified input. This is a ONE-WAY hash, used to efficiently compare data. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The SHA512-hashed result.</returns>
		byte[] HashSha512(byte[] input);

		/// <summary>
		/// Securely hashes the specified input with a salt using BCrypt. Use this for password hashing only if SCrypt is unavailable as an option. This is a ONE-WAY hash, used to safely store and compare passwords. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <param name="salt">The salt to append to the plaintext input before hashing.</param>
		/// <returns>The BCrypt-hashed result.</returns>
		byte[] Bcrypt(byte[] input, byte[] salt);
		/// <summary>
		/// Securely hashes the specified input with a salt using SCrypt. Use this for password hashing as your first option. This is a ONE-WAY hash, used to safely store and compare passwords. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <param name="salt">The salt to append to the plaintext input before hashing.</param>
		/// <param name="cost">(optional) The cost of the hash. More = better, but slower.</param>
		/// <returns>The SCrypt-hashed result.</returns>
		byte[] Scrypt(byte[] input, byte[] salt, int cost = 262144);
		/// <summary>
		/// Hashes the specified input with a salt using PHPass. PLEASE don't use PHPass unless you absolutely need to. This is a ONE-WAY hash, used to store and compare passwords. THERE IS NO WAY TO REVERSE THIS.
		/// </summary>
		/// <param name="input">The input bytes to hash.</param>
		/// <returns>The PHPass-hashed result.</returns>
		byte[] Phpass(byte[] input);

		/// <summary>
		/// The easiest way of encrypting/decrypting, provided as secure-by-default. Use EasyDecrypt() to decrypt ciphertext.
		/// </summary>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="key">Key to encrypt plaintext with. Can be any length.</param>
		/// <returns>256-bit AES encrypted ciphertext.</returns>
		byte[] EasyEncrypt(byte[] input, byte[] key);
		/// <summary>
		/// Decrypts input povided by EasyEncrypt().
		/// </summary>
		/// <param name="input">Ciphertext to decrypt.</param>
		/// <param name="key">The key that was used to encrypt the ciphertext. Can be any length.</param>
		/// <returns>Decrypted plaintext.</returns>
		byte[] EasyDecrypt(byte[] input, byte[] key);

		/// <summary>
		/// Encrypts input using the Advanced Encryption Standard. This is provided as secure-by-default. Use this as your first option, and try not to change the mode or padding if you can avoid it.
		/// </summary>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="key">Key to encrypt plaintext with. Use standard lengths.</param>
		/// <param name="iv">Initialization vector for the algorithm. Use standard lengths.</param>
		/// <param name="mode">(optional) Cipher mode to use. Please stick with the default if possible.</param>
		/// <param name="padding">(optional) Padding mode to use. Please stick with the default if possible.</param>
		/// <returns>AES-encrypted ciphertext.</returns>
		byte[] EncryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		/// <summary>
		/// Decrypts input provided by the Advanced Encryption Standard.
		/// </summary>
		/// <param name="input">Ciphertext to decrypt.</param>
		/// <param name="key">The key that was used to encrypt the ciphertext.</param>
		/// <param name="iv">The IV that was used to encrypt the ciphertext.</param>
		/// <param name="mode">The cipher mode that was used to encrypt the ciphertext.</param>
		/// <param name="padding">The padding mode that was used to encrypt the ciphertext.</param>
		/// <returns>Decrypted plaintext.</returns>
		byte[] DecryptAes(byte[] input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding);

		/// <summary>
		/// Use this for encryption only if AES is unavailable as an option. Try not to change the mode or padding if you can avoid it.
		/// </summary>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="key">Key to encrypt plaintext with. Use standard lengths.</param>
		/// <param name="iv">Initialization vector for the algorithm. Use standard lengths.</param>
		/// <param name="mode">(optional) Cipher mode to use. Please stick with the default if possible.</param>
		/// <param name="padding">(optional) Padding mode to use. Please stick with the default if possible.</param>
		/// <returns>3DES-encrypted ciphertext.</returns>
		byte[] EncryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode = CipherMode.CFB, PaddingMode padding = PaddingMode.PKCS7);
		/// <summary>
		/// Decrypts input provided by 3DES.
		/// </summary>
		/// <param name="input">Ciphertext to decrypt.</param>
		/// <param name="key">The key that was used to encrypt the ciphertext.</param>
		/// <param name="iv">The IV that was used to encrypt the ciphertext.</param>
		/// <param name="mode">The cipher mode that was used to encrypt the ciphertext.</param>
		/// <param name="padding">The padding mode that was used to encrypt the ciphertext.</param>
		/// <returns>Decrypted plaintext.</returns>
		byte[] DecryptTripleDes(byte[] input, byte[] key, byte[] iv, CipherMode mode, PaddingMode padding);

		/// <summary>
		/// Adds a PGP public key to the keyring for later encryption and signing verification.
		/// </summary>
		/// <param name="name">The name to give the PGP public key.</param>
		/// <param name="publicKey">The PGP public key.</param>
		void AddPgpPublicKey(string name, byte[] publicKey);

		/// <summary>
		/// Adds an RSA public key to the keyring for later encryption.
		/// </summary>
		/// <param name="name">The name to give the RSA public key.</param>
		/// <param name="publicKey">The RSA public key.</param>
		void AddRsaPublicKey(string name, byte[] publicKey);
		/// <summary>
		/// Encrypts the input using the RSA public key added by AddRsaPublicKey() earlier.
		/// </summary>
		/// <param name="name">The name of the RSA public key to use.</param>
		/// <param name="input">Plaintext to encrypt. Can be any length.</param>
		/// <param name="useLegacyPadding">(optional) Enable is legacy padding is needed (Windows XP or earlier).</param>
		/// <returns>RSA-encrypted ciphertext.</returns>
		byte[] EncryptRsa(string name, byte[] input, bool useLegacyPadding = false);
		/// <summary>
		/// Decrypts the input using the RSA private key in the keyring.
		/// </summary>
		/// <param name="input">The ciphertext to decrypt.</param>
		/// <param name="useLegacyPadding">(optional) Enable is legacy padding is needed (Windows XP or earlier).</param>
		/// <returns>Decrypted plaintext.</returns>
		byte[] DecryptRsa(byte[] input, bool useLegacyPadding = false);
		/// <summary>
		/// The current RSA private key in the keyring.
		/// </summary>
		byte[] RsaPrivateKey { get; }
		/// <summary>
		/// The current RSA public key in the keyring.
		/// </summary>
		byte[] RsaPublicKey { get; }
		/// <summary>
		/// Imports an RSA keypair for encryption and decryption. A keypair is automaticaly generated, but this may be useful for long-term RSA keys.
		/// </summary>
		/// <param name="keyPair">The keypair file to import.</param>
		void ImportRsaPrivateKeyPair(byte[] keyPair);
		/// <summary>
		/// Imports an RSA keypair for encryption and decryption. A keypair is automaticaly generated, but this may be useful for long-term RSA keys.
		/// </summary>
		/// <param name="absoluteFilePath">The absolute path of the keypair file.</param>
		void ImportRsaPrivateKeyPair(string absoluteFilePath);

		/// <summary>
		/// The current DH public key in the keyring.
		/// </summary>
		byte[] DHPublicKey { get; }
		/// <summary>
		/// The current DH shared secret, used as a key for encryption and decryption.
		/// </summary>
		byte[] DHSharedSecret { get; }
		/// <summary>
		/// Here's how this works:
		///   1. A sends B their public key from DHPublicKey. B calls this function with A's public key.
		///   2. B sends A their public key from DHPublicKey. A calls this function with B's public key.
		///   3. Finally, both A and B can use DHSharedSecret as their encryption key to eachother.
		/// </summary>
		/// <param name="publicKey">The other party's public DH key.</param>
		void DeriveDHKey(byte[] publicKey);

		/// <summary>
		/// Securely hashes the specified input with the key. PLEASE use this in conjunction with ciphertext. This is for validation- just append this to the end of the byte array and check on the other side. It's a hashing algorithm, treat it as one.
		/// </summary>
		/// <param name="input">The input to hash.</param>
		/// <param name="key">The key to hash the input with. Can be any length.</param>
		/// <returns>HMAC-256 hashed result.</returns>
		byte[] Hmac256(byte[] input, byte[] key);

		/// <summary>
		/// Creates a byte array using a cryptographically secure random number generator. Used for keys, IVs, and salts.
		/// </summary>
		/// <param name="length">The number of bytes to generate.</param>
		/// <returns>A securely-random array.</returns>
		byte[] GetRandomBytes(int length);
		/// <summary>
		/// Generates a secure random double between -1 and 1, inclusive.
		/// </summary>
		/// <returns>A securely-random double.</returns>
		double GetRandomDouble();

		/// <summary>
		/// Creates a new byte array of specified length starting from specified index from the input array. Think "Substring" for byte arrays.
		/// </summary>
		/// <param name="input">The input array to aplice.</param>
		/// <param name="length">The number of bytes in the returned array.</param>
		/// <param name="index">(optional) The zero-based starting byte position.</param>
		/// <returns>The partial/"Substringed" array.</returns>
		byte[] GetPartial(byte[] input, int length, int index = 0);
		/// <summary>
		/// Combines two byte arrays, starting with a and ending with b.
		/// </summary>
		/// <param name="a">The first byte array in the returned array.</param>
		/// <param name="b">The second byte array in the returned array.</param>
		/// <returns>A combined array containing inputs a, then b.</returns>
		byte[] Combine(byte[] a, byte[] b);
		/// <summary>
		/// Determines wther two byte array are equal.
		/// </summary>
		/// <param name="a">The first array.</param>
		/// <param name="b">The second array.</param>
		/// <returns>True if equal, false if not.</returns>
		bool ByteArraysAreEqual(byte[] a, byte[] b);
	}
}
