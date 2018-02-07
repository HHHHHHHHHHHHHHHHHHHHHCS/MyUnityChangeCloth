using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class AssetBundleHelper
{
    [MenuItem("AssetBundle/AllPack")]
    public static void BuildAllAB()
    {
        string dir = URL.pack_Dir;
        if (!Directory.Exists(dir))
        {//如果文件夹不存在则创建
            Directory.CreateDirectory(dir);
        }
        else
        {//否则删了重新打包
            Directory.Delete(dir, true);
            Directory.CreateDirectory(dir);
        }
        AssetBundleManifest abm = BuildPipeline.BuildAssetBundles(dir, BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows64);
        SetVersionMD5(abm);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("打包完毕！！！");
    }

    private static void SetVersionMD5(AssetBundleManifest abm)
    {
        var bundles = abm.GetAllAssetBundles();
        string[] strArray = new string[bundles.Length];
        for (int i=0;i<bundles.Length;i++)
        {
            strArray[i]=abm.GetAssetBundleHash(bundles[i]).ToString();
        }
        JsonHandler_Editor.CreateJson(strArray);
    }
}
