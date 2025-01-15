using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace NosAssistant2.Helpers;

public static class CryptoHelper
{
	public static string EncryptString(string plainText, byte[] key, byte[] iv)
	{
		Aes aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Key = key;
		aes.IV = iv;
		MemoryStream memoryStream = new MemoryStream();
		ICryptoTransform transform = aes.CreateEncryptor();
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		byte[] bytes = Encoding.UTF8.GetBytes(plainText);
		cryptoStream.Write(bytes, 0, bytes.Length);
		cryptoStream.FlushFinalBlock();
		byte[] array = memoryStream.ToArray();
		memoryStream.Close();
		cryptoStream.Close();
		return Convert.ToBase64String(array, 0, array.Length);
	}

	public static string DecryptString(string cipherText, byte[] key, byte[] iv)
	{
		Aes aes = Aes.Create();
		aes.Mode = CipherMode.CBC;
		aes.Key = key;
		aes.IV = iv;
		MemoryStream memoryStream = new MemoryStream();
		ICryptoTransform transform = aes.CreateDecryptor();
		bool flag = false;
		CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		string empty = string.Empty;
		try
		{
			byte[] array = Convert.FromBase64String(cipherText);
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.FlushFinalBlock();
			byte[] array2 = memoryStream.ToArray();
			return Encoding.UTF8.GetString(array2, 0, array2.Length);
		}
		catch
		{
			flag = true;
			return "";
		}
		finally
		{
			if (!flag)
			{
				memoryStream.Close();
				cryptoStream.Close();
			}
		}
	}

	public static string EncryptedString(string plainText)
	{
		string s = "XKrvmEXYrI9EYeBZbZXeXKZBXemXBv9KrN99Ibv99Ib";
		byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(s));
		byte[] iv = new byte[16];
		return EncryptString(plainText, key, iv);
	}

	public static string DecryptedString(string encryptedText)
	{
		string s = "XKrvmEXYrI9EYeBZbZXeXKZBXemXBv9KrN99Ibv99Ib";
		byte[] key = SHA256.Create().ComputeHash(Encoding.UTF8.GetBytes(s));
		byte[] iv = new byte[16];
		return DecryptString(encryptedText, key, iv);
	}
}
