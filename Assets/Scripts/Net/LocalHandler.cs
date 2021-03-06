﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LocalHandler
{
    /// <summary>
    /// 记录版本号的几种状态
    /// </summary>
    public enum VersionEnum
    {
        Same,//相同的
        Different,//不相同的
        WebDown//服务器炸了，本地没有网，离线模式
    }

    /// <summary>
    /// 检查包的状态
    /// </summary>
    /// <returns></returns>
    public static IEnumerator CheckAssetBundle()
    {
        yield return ServerHandler.GetServerVersion();
        var serverJson = ServerHandler.GetCacheMessage<string>();
        var state = CheckVersionMD5(serverJson);

        switch (state)
        {
            case VersionEnum.Same:
                //相同版本号忽略
                //Debug.Log("Same Version");
                break;
            case VersionEnum.Different:
                //不同版本号下载
                //Debug.Log("Different");
                yield return DownloadByDifferent();
                break;
            case VersionEnum.WebDown:
                //服务器炸了，本地没有网，离线模式
                //Debug.Log("Web BOOOM");
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// 下载不同的东西，前提是玩家没有自己删东西
    /// </summary>
    /// <returns></returns>
    private static IEnumerator DownloadByDifferent()
    {
        //从服务器读取包
        var filePath = string.Format("{0}/{1}", URL.server_dir, URL.assetBundles_file);
        yield return ServerHandler.DownLoadAssetBundle(filePath);
        var bs = ServerHandler.GetCacheMessage<byte[]>();
        if (bs != null)
        {
            var serverbundles = AssetBundle.LoadFromMemory(bs);

            //创建字典
            Dictionary<string, string> serverDic = null, localDic = null;
            MakeDictionaryByBundles(serverbundles, out serverDic);
            serverbundles.Unload(true);

            //从本地读取包
            var localFilePath = string.Format("{0}/{1}", URL.local_dir, URL.assetBundles_file);
            if (File.Exists(localFilePath))
            {
                AssetBundle localBundles = AssetBundle.LoadFromFile(localFilePath);
                MakeDictionaryByBundles(localBundles, out localDic);
                localBundles.Unload(true);
            }

            //更新字典
            Queue<string> updateBundles = new Queue<string>();
            if (localDic != null)
            {
                string value;
                foreach (var item in serverDic)
                {
                    if (localDic.TryGetValue(item.Key, out value))
                    {
                        if (value != item.Value)
                        {
                            updateBundles.Enqueue(item.Key);
                        }
                    }
                }
            }
            else
            {
                foreach (var item in serverDic.Keys)
                {
                    updateBundles.Enqueue(item);
                }
            }

            yield return ServerHandler.DownLoadAssetBundles(updateBundles);

            Resources.UnloadUnusedAssets();//释放无效资源为下面的存储做准备

            var downloadState = ServerHandler.GetCacheMessage<ServerHandler.DownloadEnum>();
            if (downloadState == ServerHandler.DownloadEnum.AllDownload)
            {
                filePath = string.Format("{0}/{1}", URL.local_dir, URL.assetBundles_file);
                SaveBundleToLocal(filePath, bs);
                SetVersionMD5ByDic(serverDic);
            }
        }
        Resources.UnloadUnusedAssets();//释放无效资源
    }

    /// <summary>
    /// 创建字典根据包
    /// </summary>
    /// <param name="bundles"></param>
    /// <param name="dic"></param>
    private static void MakeDictionaryByBundles(AssetBundle bundles, out Dictionary<string, string> dic)
    {
        dic = new Dictionary<string, string>();
        if (bundles != null)
        {
            var manifest = bundles.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            var allBundles = manifest.GetAllAssetBundles();

            for (int i = 0; i < allBundles.Length; i++)
            {
                dic.Add(allBundles[i], manifest.GetAssetBundleHash(allBundles[i]).ToString());
            }
        }
    }

    /// <summary>
    /// 检查包的MD5
    /// </summary>
    /// <param name="serverJson"></param>
    /// <returns></returns>
    public static VersionEnum CheckVersionMD5(string serverJson)
    {
        string localVersionMD5, serverVersionMD5;
        localVersionMD5 = JsonHandler_Local.ReadVersionMD5();
        serverVersionMD5 = JsonHandler_Server.ReadVersionMD5(serverJson);
        //Debug.Log(localVersionMD5);
        //Debug.Log(serverVersionMD5);
        if (!string.IsNullOrEmpty(serverVersionMD5))
        {
            if (!string.IsNullOrEmpty(localVersionMD5))
            {
                if (localVersionMD5.Equals(serverVersionMD5))
                {
                    return VersionEnum.Same;
                }
            }
            return VersionEnum.Different;
        }
        return VersionEnum.WebDown;
    }

    /// <summary>
    /// 保存包在本地
    /// </summary>
    /// <param name="filePath"></param>
    /// <param name="data"></param>
    public static void SaveBundleToLocal(string filePath, byte[] data)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        using (FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate))
        {
            fs.Write(data, 0, data.Length);
            fs.Flush();
            fs.Close();
            fs.Dispose();
        }
    }

    /// <summary>
    /// 根据服务器包的MD5保存在本地
    /// </summary>
    /// <param name="dic"></param>
    private static void SetVersionMD5ByDic(Dictionary<string, string> dic)
    {
        int i = 0;
        string[] strArray = new string[dic.Count];
        foreach (var item in dic.Values)
        {
            strArray[i++] = item;
        }

        JsonHandler_Local.CreateJson(strArray);
    }

    /// <summary>
    /// 加载全部的包
    /// </summary>
    public static Dictionary<string, AssetBundle> LoadAllAssetBundles()
    {
        string allBundlesStr = string.Format("{0}/{1}", URL.local_dir, URL.assetBundles_file);
        Dictionary<string, AssetBundle> dic = new Dictionary<string, AssetBundle>();
        if (File.Exists(allBundlesStr))
        {
            AssetBundle allBundles = AssetBundle.LoadFromFile(allBundlesStr);
            AssetBundleManifest abm = allBundles.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
            string bundleStr;
            foreach (var filePath in abm.GetAllAssetBundles())
            {
                bundleStr = string.Format("{0}/{1}", URL.local_dir, filePath);
                if (File.Exists(bundleStr))
                {
                    if (filePath.IndexOf(AssetBundleNames.female_file) > 0)
                    {
                        dic.Add(AssetBundleNames.female_dic, AssetBundle.LoadFromFile(bundleStr));
                    }
                    else if (filePath.IndexOf(AssetBundleNames.male_file) > 0)
                    {
                        dic.Add(AssetBundleNames.male_dic, AssetBundle.LoadFromFile(bundleStr));
                    }
                }
            }
        }
        return dic;
    }
}
