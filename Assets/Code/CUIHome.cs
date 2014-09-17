using UnityEngine;
using System.Collections;
using DG.Tweening;
public class CUIHome : CUIController
{
    UIButton StartBtn;
    public UILabel TitleLabel;

    public override void OnInit()
    {
        base.OnInit();

        StartBtn = this.GetControl<UIButton>("Button");
        StartBtn.onClick.Add(new EventDelegate(OnClickStartBtn));

        TitleLabel = GetControl<UILabel>("Title");


    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);

        Transform titleTrans = TitleLabel.transform;
        Transform startBtnTrans = StartBtn.transform;
        //StartBtn.gameObject.SetActive(false);
        titleTrans.DOLocalMoveYFrom(titleTrans.localPosition.y + 300, 1f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            //StartBtn.gameObject.SetActive(true);
            
        });
        startBtnTrans.DOLocalMoveYFrom(startBtnTrans.localPosition.y - 500, 1f).SetEase(Ease.OutBounce);
        //TitleLabel.transform.DOMoveFrom()

    }
    void OnClickStartBtn()
    {
        CUIManager.Instance.OpenWindow<CUIMathGame>();
        CloseWindow();
    }
}
