using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

public class JsonHandler_Base
{
    protected static void CreateJson(string[] strArray, string dir, string file)
    {
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        JObject o = new JObject();
        o["version_md5"] = MD5Handler.MD5Encrypt(strArray);
        string filePath = string.Format("{0}/{1}", dir, file);
        using (StreamWriter sw = File.CreateText(filePath))
        {
            sw.Write(o.ToString());
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }

    protected static string ReadJson(string dir, string file)
    {
        string result = string.Empty;
        if (File.Exists(dir))
        {
            string filePath = string.Format("{0}/{1}", dir, file);
            using (StreamReader sr = File.OpenText(filePath))
            {
                result = sr.ReadToEnd();
            }
        }
        return result;
    }

    protected static string ReadVersionMD5(string dir, string file)
    {
        var str = ReadJson(dir, file);
        if(string.IsNullOrEmpty(str))
        {
            JObject jo = JObject.Parse(str);
            return jo["version_md5"].ToString();
        }
        else
        {
            return null;
        }
    }
}
