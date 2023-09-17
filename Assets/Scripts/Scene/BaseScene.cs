using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public class BaseScene : MonoBehaviour
{
    public Define.Scene SceneType = Define.Scene.Base;
    protected  bool _init = false;

    private void Start()
    {
        Init();
    }

    protected virtual bool Init()
    {
        if (_init)
            return false;

        _init = true;
        GameObject go = GameObject.Find("EventSystem");
        if (go == null)
        {
            Managers.Resource.Instantiate("UI/EventSystem").name = "@EventSystem";
        }

        return true;
    }

    public virtual void Clear()
    {

    }

}
