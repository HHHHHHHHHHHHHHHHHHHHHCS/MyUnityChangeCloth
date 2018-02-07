using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AvatorSystem : MonoBehaviour
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
    /// 人物的骨骼信息
    /// </summary>
    protected Transform[] peopleHips;

    /// <summary>
    /// 换装骨骼身上的SkinnedMeshRenderer信息
    /// </summary>
    protected Dictionary<string, SkinnedMeshRenderer> peopleSmr;



    protected virtual void Awake()
    {
        var source = InitSource(null);
        var target = InitTarget(null);
        InitData(source, target);

        InitAvator();
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ChangeMesh("top", (Random.Range(1, 7)).ToString());
        }
    }

    /// <summary>
    /// 初始化正确人物资源
    /// </summary>
    protected virtual Transform InitSource(string peopleModelStr)
    {
        if(string.IsNullOrEmpty(peopleModelStr))
        {
            return null;
        }
        var peopleSource = Instantiate(Resources.Load<GameObject>(peopleModelStr));
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
        var peopleTarget = Instantiate(Resources.Load<GameObject>(peopleModelTargetStr)
            , new Vector3(x, y, z), Quaternion.identity);
        peopleHips = peopleTarget.GetComponentsInChildren<Transform>();
        return peopleTarget.transform;
    }

    /// <summary>
    /// 初始化人物默认的装备部件
    /// </summary>
    protected virtual void InitAvator()
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
    protected virtual void InitData(Transform source,Transform target)
    {
        if (!(source||target))
        {
            return;
        }
        //初始化字典
        peopleData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();
        peopleSmr = new Dictionary<string, SkinnedMeshRenderer>();
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
                partGo.transform.parent = target;

                //把骨骼target身上的skm信息存储
                peopleSmr.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>());

                peopleData.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            peopleData[names[0]].Add(names[1], part);
        }
    }

    /// <summary>
    /// 换装
    /// </summary>
    /// <param name="part">部位类型</param>
    /// <param name="num">换装的编号</param>
    protected virtual void ChangeMesh(string part, string num)
    {
        //找到部位
        SkinnedMeshRenderer skm = peopleData[part][num];

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


}
