using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatorSystem : MonoBehaviour
{
    /// <summary>
    /// 资源Model
    /// </summary>
    private GameObject girlSource;

    /// <summary>
    /// 女孩的资源骨骼
    /// </summary>
    private Transform girlSourceTrans;

    /// <summary>
    /// 骨架物体，换装的人
    /// </summary>
    private GameObject girlTarget;


    /// <summary>
    /// 小女孩所有的资源信息
    /// </summary>
    private Dictionary<string, Dictionary<string, SkinnedMeshRenderer>> girlData = new Dictionary<string, Dictionary<string, SkinnedMeshRenderer>>();

    /// <summary>
    /// 小女孩的骨骼信息
    /// </summary>
    private Transform[] girlHips;
    /// <summary>
    /// 换装骨骼身上的SkinnedMeshRenderer信息
    /// </summary>
    private Dictionary<string, SkinnedMeshRenderer> gilrSmr = new Dictionary<string, SkinnedMeshRenderer>();

    /// <summary>
    /// 初始女孩加载
    /// </summary>
    private readonly string[,] girlStr = new string[,]
    { { "eyes","1"},{ "hair","1"},{ "top","1"},{ "pants","1"},{ "shoes","1"},{ "face","1"} };

    private void Awake()
    {
        InitSource();
        InitTarget();
        SaveData();
        InitAvator();
    }

    private void InitSource()
    {
        GameObject grilSource = Instantiate(Resources.Load<GameObject>("Prefabs/FemaleModel"));
        girlSourceTrans = grilSource.transform;
        grilSource.SetActive(false);
    }

    private void InitTarget()
    {
        girlTarget = Instantiate(Resources.Load<GameObject>("Prefabs/FemaleModelTarget"));
        girlHips = girlTarget.GetComponentsInChildren<Transform>();
    }

    private void SaveData()
    {
        if (!girlSourceTrans)
        {
            return;
        }

        //遍历所有子物体有SkinnedMeshRenderer,进行存储
        SkinnedMeshRenderer[] parts = girlSourceTrans.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var part in parts)
        {
            string[] names = part.name.Split('-');

            //如果物体没有生成过
            if (!girlData.ContainsKey(names[0]))
            {
                //生成对应的部位，且只生成一个
                GameObject partGo = new GameObject(names[0]);
                partGo.transform.parent = girlTarget.transform;

                //把骨骼target身上的skm信息存储
                gilrSmr.Add(names[0], partGo.AddComponent<SkinnedMeshRenderer>());

                girlData.Add(names[0], new Dictionary<string, SkinnedMeshRenderer>());
            }
            girlData[names[0]].Add(names[1], part);
        }
    }


    private void ChangMesh(string part, string num)
    {
        //找到部位
        SkinnedMeshRenderer skm = girlData[part][num];

        //骨骼匹配   即 皮上的信息 跟 骨骼的信息想互相匹配
        List<Transform> bones = new List<Transform>();
        foreach (var trans in skm.bones)
        {
            foreach (var bone in girlHips)
            {
                if (trans.name == bone.name)
                {
                    bones.Add(bone);
                    break;
                }
            }
        }

        //换装实现
        var body = gilrSmr[part];
        body.bones = bones.ToArray();//骨骼重新绑定
        body.materials = skm.materials;//材质重新绑定
        body.sharedMesh = skm.sharedMesh;//蒙皮重新绑定
    }

    /// <summary>
    /// 初始化骨骼框架 让他有mesh 材质 骨骼信息
    /// </summary>
    private void InitAvator()
    {
        int length = girlStr.GetLength(0);//获取行数

        for(int i =0;i<length;i++)
        {
            ChangMesh(girlStr[i, 0], girlStr[i, 1]);
        }
    }
}
