using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class MD5Handler
{
    public static string MD5Encrypt(string[] strArray)
    {
        string str = string.Empty;
        foreach (var item in strArray)
        {
            str += item;
        }
        return MD5Encrypt(str);
    }

    public static string MD5Encrypt(string strText)
    {
        MD5 md5 = new MD5CryptoServiceProvider();
        byte[] result = md5.ComputeHash(Encoding.Default.GetBytes(strText));
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < result.Length; i++)
        {
            sb.Append(result[i].ToString("x2"));

        }
        return sb.ToString();
    }

    public static bool EqualMD5(string localMD5, string serverMD5)
    {
        return localMD5.Equals(serverMD5);
    }
}
