using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AvatarSystem : MonoBehaviour
{
    /// <summary>
    /// 初始化人物信息加载
    /// </summary>
    protected string[,] peopleStartStr = new string[,]
    { { "eyes","1"},{ "hair","1"},{ "face","1"},{ "top","1"},{ "pants","1"},{ "shoes","1"} };

    /// <summary>
    /// 人物所有的装备部件信息
    /// </summary>
    protected Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> peopleData;

    /// <summary>
    /// 人物换装的UI信息
    /// </summary>
    protected Dictionary<string, List<Sprite>> peopleUIData;


    /// <summary>
    /// 人物的骨骼信息
    /// </summary>
    protected Transform[] peopleHips;

    /// <summary>
    /// 换装骨骼身上的SkinnedMeshRenderer信息
    /// </summary>
    protected Dictionary<string, SkinnedMeshRenderer> peopleSmr;

    /// <summary>
    /// 换装信息的部件保存
    /// </summary>
    protected Dictionary<string, string> peopleNowData;

    /// <summary>
    /// 展示出的人物
    /// </summary>
    protected GameObject peopleTarget;

    /// <summary>
    /// 任务的资源包
    /// </summary>
    protected AssetBundle assetBundle;

    /// <summary>
    /// 人物的动画组件
    /// </summary>
    protected Animation peopleAnim;

    protected virtual void Init(AssetBundle ab)
    {
        if (ab)
        {
            //assetBundle = ab;
            //var source = InitSource(null);
            //InitTarget(null);
            //InitData(source);

            InitJson();
            InitAvatar();
            ShowHideAvatar(false);
            Debug.Log(ExportJson());
        }
    }

    //protected virtual void Update()
    //{
    //    if (Input.GetMouseButtonDown(0))
    //    {
    //        ChangeMesh("top", (Random.Range(1, 7)).ToString());
    //    }
    //}

    /// <summary>
    /// 初始化正确人物资源
    /// </summary>
    protected virtual Transform InitSource(string peopleModelStr)
    {
        if (string.IsNullOrEmpty(peopleModelStr))
        {
            return null;
        }
        var peopleSource = Instantiate(assetBundle.LoadAsset<GameObject>(peopleModelStr));
        peopleSource.SetActive(false);
        return peopleSource.transform;
    }


    /// <summary>
    /// 初始化要被换装的人物
    /// </summary>
    protected virtual Transform InitTarget(string peopleModelTargetStr, float x = 0, float y = 0, float z = 0)
    {
        if (string.IsNullOrEmpty(peopleModelTargetStr))
        {
            return null;
        }
        peopleTarget = Instantiate(assetBundle.LoadAsset<GameObject>(peopleModelTargetStr)
            , new Vector3(x, y, z), Quaternion.identity);
        peopleHips = peopleTarget.GetComponentsInChildren<Transform>();
        peopleAnim = peopleTarget.GetComponent<Animation>();
        return peopleTarget.transform;
    }

    /// <summary>
    /// 初始化人物默认的装备部件
    /// </summary>
    protected virtual void InitAvatar()
    {
        int length = peopleStartStr.GetLength(0);//获取行数

        for (int i = 0; i < length; i++)
        {
            ChangeMesh(peopleStartStr[i, 0], peopleStartStr[i, 1]);
        }
    }


    /// <summary>
    /// 保存 换装的东西的信息
    /// </summary>
    protected virtual void InitData(Transform source)
    {
        if (!(source || peopleTarget))
        {
            return;
        }
        //初始化字典
        peopleData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
        peopleSmr = new Dictionary<string, SkinnedMeshRenderer>();
        peopleNowData = new Dictionary<string, string>();
        //遍历所有子物体有SkinnedMeshRenderer,进行存储
        SkinnedMeshRenderer[] parts = source.GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (var part in parts)
        {
            string[] names = part.name.Split('-');

            //如果部位的物体没有生成过
            if (!peopleData.ContainsKey(names[0]))
            {
                //生成对应的部位，且只生成一个
                GameObject partGo = new GameObject(names[0]);
                partGo.transform.parent = peopleTarget.transform;

                //把骨骼target身上的skm信息存储
                peopleSmr.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>());

                peopleData.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            peopleData[names[0]].Add(names[1], part);
        }
    }

    /// <summary>
    /// 初始化Json信息
    /// </summary>
    protected virtual void InitJson()
    {
        foreach (var item in assetBundle.GetAllAssetNames())
        {
            if (item.IndexOf(AssetBundleNames.json) > 0)
            {
                peopleUIData = new Dictionary<string, List<Sprite>>();
                TextAsset ta = assetBundle.LoadAsset<TextAsset>(item);
                JObject jo = JObject.Parse(ta.text);
                foreach (var i in jo)
                {
                    List<string> pathList = new List<string>();
                    foreach (var j in i.Value)
                    {
                        pathList.Add(j.ToString());
                    }

                    var allAssetNames = assetBundle.GetAllAssetNames();
                    List<Sprite> spriteList = new List<Sprite>();
                    string path;
                    foreach (var _path in pathList)
                    {
                        path = string.Format("/{0}.", _path);
                        foreach (var currentName in allAssetNames)
                        {
                            if (currentName.IndexOf(path) > 0)
                            {
                                spriteList.Add(assetBundle.LoadAsset<Sprite>(currentName));
                            }
                        }
                    }

                    peopleUIData.Add(i.Key, spriteList);
                }
                break;
            }
        }
    }

    /// <summary>
    /// 换装
    /// </summary>
    /// <param name="part">部位类型</param>
    /// <param name="num">换装的编号</param>
    public virtual void ChangeMesh(string part, string num)
    {
        Dictionary<string, SkinnedMeshRenderer> dic;
        SkinnedMeshRenderer skm;
        //判断异常，顺便找到部位
        if (peopleData.TryGetValue(part, out dic)
            && dic.TryGetValue(num, out skm))
        {
            //骨骼匹配   即 皮上的信息 跟 骨骼的信息想互相匹配
            List<Transform> bones = new List<Transform>();
            foreach (var trans in skm.bones)
            {
                foreach (var bone in peopleHips)
                {
                    if (trans.name == bone.name)
                    {
                        bones.Add(bone);
                        break;
                    }
                }
            }

            //换装实现
            var body = peopleSmr[part];
            body.bones = bones.ToArray();//骨骼重新绑定
            body.materials = skm.materials;//材质重新绑定
            body.sharedMesh = skm.sharedMesh;//蒙皮重新绑定
        }
        peopleNowData[part] = num;
        PlayAnim(part);
    }

    /// <summary>
    /// 播放换装的动画
    /// </summary>
    public void PlayAnim(string part, string animStr = null)
    {
        if (string.IsNullOrEmpty(animStr))
        {
            switch (part)
            {
                case AssetBundleNames.top:
                    peopleAnim.Play("item_shirt");
                    break;
                case AssetBundleNames.pants:
                    peopleAnim.Play("item_pants");
                    break;
                case AssetBundleNames.shoes:
                    peopleAnim.Play("item_boots");
                    break;
                default:
                    break;
            }
        }
        else
        {
            peopleAnim.Play(animStr);
        }
    }

    public void ShowHideAvatar(bool isShow = false)
    {
        if (peopleTarget)
        {
            peopleTarget.SetActive(isShow);
        }
    }

    public List<Sprite> GetSpritesByPart(string part)
    {
        if (peopleUIData == null)
        {
            return null;
        }
        List<Sprite> spriteList = null;

        if (!peopleUIData.TryGetValue(part, out spriteList))
        {
            return null;
        }

        return spriteList;
    }

    public string ExportJson()
    {
        string json = string.Empty;
        foreach (var item in peopleNowData)
        {
            json = JsonConvert.SerializeObject(peopleNowData);
        }
        return json;
    }
}
