public class JsonHandler_Editor : JsonHandler_Base
{
#if UNITY_EDITOR
    public static void CreateJson(string[] strArray)
    {
        CreateJson(strArray, URL.server_dir, URL.version_file);
    }


    public static string ReadVersionMD5()
    {
        return ReadVersionMD5(URL.server_dir, URL.version_file);
    }
#endif
}
