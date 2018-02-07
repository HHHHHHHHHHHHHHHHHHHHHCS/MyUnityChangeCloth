public class URL
{

#if UNITY_EDITOR
    /// <summary>
    /// 打包的位置
    /// </summary>
    public const string pack_Dir = @"Pack/AssetBundles";
#endif

    /// <summary>
    /// 服务器位置
    /// </summary>
    public const string server_dir = @"G:/MyUnity/MyUnityChangeCloth/Pack/AssetBundles";


    /// <summary>
    /// 本地位置
    /// </summary>
    public static readonly string local_dir = UnityEngine.Application.dataPath + @"/AssetBundles";


    public const string version_file = "AssetBundlesVersion";
    public const string assetBundles_file = "AssetBundles";
}
