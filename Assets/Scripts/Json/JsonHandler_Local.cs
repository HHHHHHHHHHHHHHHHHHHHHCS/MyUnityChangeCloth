using UnityEngine;

public class JsonHandler_Local: JsonHandler_Base
{
    public static void CreateJson(string[] strArray)
    {
        CreateJson(strArray, URL.local_dir, URL.version_file);
    }

    public static string ReadVersionMD5()
    {
        return ReadVersionMD5(URL.local_dir, URL.version_file);
    }
}
