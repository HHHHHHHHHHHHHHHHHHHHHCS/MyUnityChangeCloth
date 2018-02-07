using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        var localHandler = gameObject.AddComponent<LocalHandler>();
        yield return localHandler.CheckAssetBundle();
    }
}
