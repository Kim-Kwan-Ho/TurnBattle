using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LoadingPopup : UI_Popup
{
    enum Images
    {
        LoadingFill
    }


    private MySceneManager _scene;

    public override bool Init()
    {
        if (base.Init() == false)
            return false;

        _scene = Managers.Scene;
        BindImage(typeof(Images));
        GetImage((int)Images.LoadingFill).fillAmount = 0;
        return true;
    }

    private void Update()
    {
        RefreshLoading();
    }
    public void RefreshLoading()
    {
        GetImage((int)Images.LoadingFill).fillAmount = _scene.LoadingPercent;
    }


}
