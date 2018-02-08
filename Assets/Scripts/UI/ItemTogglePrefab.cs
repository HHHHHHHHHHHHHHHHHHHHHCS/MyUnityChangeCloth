using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemTogglePrefab : MonoBehaviour
{
    private string part,index;

    public void Init(string _part,string _index)
    {
        part = _part;
        index = _index;
        GetComponent<Toggle>().onValueChanged.AddListener(ChangeCloth);
    }

    private void ChangeCloth(bool tf)
    {
        if(tf)
        {
            MainGameManager.Instance.ChangeCurrentPart(part, index);
        }
    }
}
