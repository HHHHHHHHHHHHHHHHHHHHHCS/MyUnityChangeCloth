using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class LocalHandler : MonoBehaviour
{
    /// <summary>
    /// 记录版本号的几种状态
    /// </summary>
    public enum VersionEnum
    {
        Same,//相同的
        Different,//不相同的
        ServerDown//服务器炸了,或者获取不到服务器
    }

    private void Awake()
    {
        StartCoroutine(CheckAssetBundle());
    }

    private IEnumerator CheckAssetBundle()
    {
        yield return ServerHandler.GetServerVersion();
        var serverJson = ServerHandler.GetCacheMessage<string>();
        var state = CheckVersionMD5(serverJson);

        switch (state)
        {
            case VersionEnum.Same:
                //相同版本号忽略
                Debug.Log("Same");
                break;
            case VersionEnum.Different:
                //不同版本号下载
                Debug.Log("Diff");
                StartCoroutine( DownloadByDifferent());
                break;
            case VersionEnum.ServerDown:
                //服务器炸了，本地没有网，离线模式
                Debug.Log("Server BOOOM");
                break;
            default:
                break;
        }
    }

    private IEnumerator DownloadByDifferent()
    {
        var filePath= string.Format("{0}/{1}", URL.server_dir, URL.assetBundles_file);
        yield return ServerHandler.DownLoadAssetBundle(filePath);
        var serverbundles = ServerHandler.GetCacheMessage<AssetBundle>();
        //SaveBundleToLocal(filePath, ((DownloadHandlerAssetBundle)serverbundles.downloadHandler).GetData());

        //var serverbundles = DownloadHandlerAssetBundle.GetContent(serverdownload);

        Dictionary<string, string> serverDic=null, localDic=null;
        MakeDictionaryByBundles(serverbundles, out serverDic);

        var localFilePath= string.Format("{0}/{1}", URL.local_dir, URL.version_file);
        if (File.Exists(localFilePath))
        {
            AssetBundle localBundles = AssetBundle.LoadFromFile(localFilePath);
            MakeDictionaryByBundles(localBundles,out localDic);
        }


        Queue<string> updateBundles = new Queue<string>();
        if(localDic!=null)
        {
            string value;
            foreach (var item in localDic)
            {
                if(serverDic.TryGetValue(item.Key,out value))
                {
                    if(value!=item.Value)
                    {
                        updateBundles.Enqueue(item.Key);
                    }
                }
            }
        }
        else
        {
            foreach(var item in serverDic.Keys)
            {
                updateBundles.Enqueue(item);
            }
        }

        ServerHandler.DownLoadAssetBundles(updateBundles);
    }

    private void MakeDictionaryByBundles(AssetBundle bundles, out Dictionary<string, string> dic)
    {
        dic = new Dictionary<string, string>();
        var manifest = bundles.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        var serverBundles = manifest.GetAllAssetBundles();

        for (int i = 0; i < serverBundles.Length; i++)
        {
            dic.Add(serverBundles[i], manifest.GetAssetBundleHash(serverBundles[i]).ToString());
        }
    }


    public VersionEnum CheckVersionMD5(string serverJson)
    {
        string localVersionMD5, serverVersionMD5;
        localVersionMD5 = JsonHandler_Local.ReadVersionMD5();
        serverVersionMD5 = JsonHandler_Server.ReadVersionMD5(serverJson);
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
        return VersionEnum.ServerDown;
    }


    void SaveBundleToLocal(string fileName,byte[] data)
    {
        using (StreamWriter sw = File.CreateText(fileName))
        {
            sw.Write(data);
            sw.Flush();
            sw.Close();
            sw.Dispose();
        }
    }

}
