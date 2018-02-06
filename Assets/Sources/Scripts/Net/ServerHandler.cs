using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.Collections.Generic;

public class ServerHandler : MonoBehaviour
{
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
        using (UnityWebRequest request = UnityWebRequest.GetAssetBundle(filePath))
        {
            yield return request.SendWebRequest();
            //var a = (DownloadHandlerFile)request.downloadHandler;
            Debug.Log(request.downloadProgress);
            cacheMessage = string.IsNullOrEmpty(request.error) ? DownloadHandlerAssetBundle.GetContent(request) : null;
        
        }
    }

    public static IEnumerator DownLoadAssetBundles(Queue<string> updateBundles)
    {
        string filePath;
        while (updateBundles.Count <= 0)
        {//这里使用单线程下载
            filePath = string.Format("{0}/{1}", URL.server_dir, updateBundles.Dequeue());
            UnityWebRequest request = UnityWebRequest.GetAssetBundle(filePath);
            yield return request.SendWebRequest();
            while (!request.isDone)
            {

            }
            if (string.IsNullOrEmpty(request.error))
            {

            }
        }
    }
}
