using UnityEngine;
using System.Collections;

public class CUIHome : CUIController
{
    UIButton StartBtn;
    public override void OnInit()
    {
        base.OnInit();

        StartBtn = this.GetControl<UIButton>("Button");
        StartBtn.onClick.Add(new EventDelegate(OnClickStartBtn));
    }

    void OnClickStartBtn()
    {
        CUIManager.Instance.OpenWindow<CUIMathGame>();
        CloseWindow();
    }
}
