using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CUIMathGame : CUIController
{
    public UIColorQuad Bg;

    public UILabel QuestionLabel;
    public UIButton SelectBtn1;
    public UIButton SelectBtn2;
    public UIButton SelectBtn3;
    public UILabel ScoreLabel;
    public UILabel ScoreEffectTemplate; // 飛出特效

    public List<UIButton> SelectBtns;
    AudioSource TimeCounterSound;
    UILabel TimeCounterLabel;

    public override void OnInit()
    {
        base.OnInit();
        TimeCounterSound = GetControl<AudioSource>("TimeCounter");
        TimeCounterLabel = GetControl<UILabel>("TimeCounter");
        ScoreLabel = GetControl<UILabel>("ScoreLabel");
        ScoreEffectTemplate = GetControl<UILabel>("ScoreEffectTemplate");

        Bg = GetControl<UIColorQuad>("Bg");

        QuestionLabel = GetControl<UILabel>("Question");
        SelectBtn1 = GetControl<UIButton>("SelectBtnsContainer/SelectBtn1");
        SelectBtn2 = GetControl<UIButton>("SelectBtnsContainer/SelectBtn2");
        SelectBtn3 = GetControl<UIButton>("SelectBtnsContainer/SelectBtn3");

        SelectBtns = new List<UIButton>(){
            SelectBtn1, 
            SelectBtn2,
            SelectBtn3
        };

        foreach (UIButton btn in SelectBtns)
        {
            btn.onClick.Add(new EventDelegate(OnClickSelectBtn));
        }
    }

    public override void OnOpen(params object[] args)
    {
        base.OnOpen(args);
        NewLevel();
        
    }
    IEnumerator TimerCounter()
    {
        float time = 0;
        while (true)
        {
            time += Time.deltaTime;
            TimeCounterLabel.text = string.Format("{0:F2}s", time);
            yield return null;
        }
        
    }

    void NewLevel()
    {
        NewQuestion();
        ResetAllTweens();
        StartCoroutine(TimerCounter());
    }
    void OnClickSelectBtn()
    {
        UIButton clickBtn = UIButton.current;
        string btnName = clickBtn.gameObject.name;
        int selectIndex = int.Parse(btnName[btnName.Length - 1].ToString()) - 1;
        if (selectIndex == RightAnswerIndex)
        {
            CBase.Log("Right");
        }
        else
            CBase.Log("Wrong");
        AnswerQuestion();
    }

    void AnswerQuestion()
    {
        NewQuestion();
    }

    int RightAnswerIndex = 0;

    void NewQuestion()
    {
        int rightAnswer = GenerateQuestion();
        GenerateSelections(rightAnswer);

        ResetAllTweens();
    }

    int GenerateQuestion()
    {
        int aNum = Random.Range(0, 101);
        int bNum = Random.Range(0, 101);
        int symbol = Random.Range(0, 2);

        // 交换两个变量, 防止负数
        if (bNum > aNum)
        {
            aNum ^= bNum;
            bNum ^= aNum;
            aNum ^= bNum;
        }

        int rightAnswer;
        string symbolStr;
        if (symbol == 0) // +
        {
            rightAnswer = aNum + bNum;
            symbolStr = "+";
        }
        else // -
        {
            rightAnswer = aNum - bNum;
            symbolStr = "-";
        }

        string QuestionText = string.Format("{0} {1} {2} = ?", aNum, symbolStr, bNum);
        QuestionLabel.text = QuestionText;
        return rightAnswer;
    }

    void GenerateSelections(int rightAnswer)
    {
        List<UIButton> shuffleBtns = this.SelectBtns;

        RightAnswerIndex = Random.Range(0, 3);

        int offsetFactor = Random.Range(1, 4);
        for (int idx = 0; idx < shuffleBtns.Count; idx++)
        {
            int offset = (RightAnswerIndex - idx) * offsetFactor;
            GetControl<UILabel>("Label", shuffleBtns[idx].transform).text = (rightAnswer + offset).ToString();
        }

    }

    void ResetAllTweens()
    {
        foreach (UITweener tweener in this.gameObject.GetComponentsInChildren<UITweener>())
        {
            tweener.ResetToBeginning();
            tweener.PlayForward();
        }

        foreach (UIButton btn in SelectBtns)
        {
            btn.gameObject.transform.localScale = Vector3.zero;
            TweenScale.Begin(btn.gameObject, 0.5f, Vector3.one);
        }
    }

    /// <summary>
    /// 受傷，屏幕變紅
    /// </summary>
    void ScreenHurt()
    {
        // 生命條減少, 無數字
    }

    /// <summary>
    /// 答對題目，出現的效果
    /// </summary>
    void ScreenRight()
    {
        // 數字+1 彈過去
    }

    // TODO: no use
    static public List<T> ShufflePro<T>(ref List<T> list)
    {
        byte[] randomBytes = new System.Byte[4];
        var rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
        for (int i = 0; i < list.Count; ++i)
        {
            rng.GetNonZeroBytes(randomBytes);
            int randomSeed = (randomBytes[0] << 24) | (randomBytes[1] << 16) | (randomBytes[2] << 8) | randomBytes[3];
            int var = randomSeed % list.Count;
            //var = System.Math.Abs(var);
            if (var < 0) var *= -1;
            T temp = list[i];
            list[i] = list[var];
            list[var] = temp;
        }
        return list;
    }
}
