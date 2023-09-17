using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
public class MySceneManager
{
    private Define.Scene _curSceneType = Define.Scene.Login;
    public float LoadingPercent = 0;
    public BaseScene CurrentScene = null;   

    public void Init()
    {
        CurrentScene = GameObject.Find("@Scene").GetComponent<BaseScene>();
    }

    public void ChangeScene(Define.Scene type, bool delay = false, float delayTime =3)
    {
        LoadSceneAsync(Enum.GetName(typeof(Define.Scene), type) + "Scene", delay, delayTime);
        _curSceneType = type;

    }

    private async void LoadSceneAsync(string sceneName, bool delay = false, float delayTIme = 3)
    {
        LoadingPercent = 0;
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        var loading = Managers.UI.ShowPopupUI<UI_LoadingPopup>();
        op.allowSceneActivation = false;
        if (delay)
        {
            float timer = delayTIme;
            while (timer >= 0)
            {
                LoadingPercent = 1 - (timer / delayTIme);
                timer -= Time.deltaTime;
                await Task.Yield();
            }
        }
        else
        {
            while (!op.isDone)
            {
                if (op.progress >= 0.9f)
                    break;
                LoadingPercent = op.progress;
                await Task.Yield();
            }
        }

        await Task.Delay(1000);
        
        LoadingPercent = 1;
        Managers.UI.ClosePopupUI(loading);
        op.allowSceneActivation = true;
    }



}
