using Newtonsoft.Json.Linq;

public class JsonHandler_Server : JsonHandler_Base
{
    public static string ReadVersionMD5(string str)
    {
        if (!string.IsNullOrEmpty(str))
        {
            JObject jo = JObject.Parse(str);
            return jo["version_md5"].ToString();
        }
        return null;
    }
}
