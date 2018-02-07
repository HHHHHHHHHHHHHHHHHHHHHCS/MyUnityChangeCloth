using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

public class ServerHandler : MonoBehaviour
{
    /// <summary>
    /// 记录下载的几种状态
    /// </summary>
    public enum DownloadEnum
    {
        UnDownLoad,//还没有下载
        BreakDownload,//下载中途出错
        AllDownload//全部下载完成
    }

    private static object cacheMessage;

    public static T GetCacheMessage<T>()
    {
        return (T)cacheMessage;
    }


    public static IEnumerator GetServerVersion()
    {
        var filePath = string.Format("{0}/{1}", URL.server_dir, URL.version_file);
        using (UnityWebRequest request = UnityWebRequest.Get(filePath))
        {
            yield return request.SendWebRequest();
            cacheMessage = string.IsNullOrEmpty(request.error) ? System.Text.Encoding.UTF8.GetString(request.downloadHandler.data) : null;
        }
    }

    public static IEnumerator DownLoadAssetBundle(string filePath)
    {
        using (WWW www = new WWW(filePath))
        {
            yield return www;
            cacheMessage = string.IsNullOrEmpty(www.error) ? www.bytes : null;
        }
    }

    public static IEnumerator DownLoadAssetBundles(Queue<string> updateBundles)
    {
        string fileName, fileServerPath,fileLocalPath;
        var downloadState = DownloadEnum.UnDownLoad;
        while (updateBundles.Count > 0)
        {//这里使用单线程下载,如果中途下载出错 则要全部重新下载
            fileName = updateBundles.Dequeue();
            fileServerPath = string.Format("{0}/{1}", URL.server_dir, fileName);
            WWW www = new WWW(fileServerPath);
            yield return www;
            while (!www.isDone)
            {

            }

            if (string.IsNullOrEmpty(www.error))
            {
                fileLocalPath = string.Format("{0}/{1}", URL.local_dir, fileName);
                LocalHandler.SaveBundleToLocal(fileLocalPath, www.bytes);
            }
            else
            {
                downloadState = DownloadEnum.BreakDownload;
                break;
            }
        }
        if (updateBundles.Count <= 0 && downloadState != DownloadEnum.BreakDownload)
        {
            cacheMessage = DownloadEnum.AllDownload;
        }
    }
}
