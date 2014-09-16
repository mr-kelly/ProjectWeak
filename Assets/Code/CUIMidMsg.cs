using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CUIMidMsg : CUIController
{
	private Stack<XUIMidMsg_Animator> m_MsgTempllatePool;  // 内存池
	public List<XUIMidMsg_Animator> m_WaitingMsgList = new List<XUIMidMsg_Animator>();  // waiting List... wait for 1-5 seconds

	public Transform MsgTemplate;

	public const int MSG_LIMIT = 3;
	public const float MSG_TIME = 3f; // 持续3秒
	public const float FADE_TIME = 0.5f; // 渐变效果时间
	public const float MSG_HEIGHT = 50f;

	// 静态方法， 快速弹信息
	public static void QuickMsg(string szMsg, params object[] format)
	{
		CUIManager.Instance.OpenWindow<CUIMidMsg>();
		CUIManager.Instance.CallUI<CUIMidMsg>(
			(_ui, _arg) => _ui.ShowMsg((string)_arg[0]),
			string.Format(szMsg, format));
	}

	public override void OnInit()
	{
		base.OnInit();

		m_MsgTempllatePool = new Stack<XUIMidMsg_Animator>();

		MsgTemplate = GetControl<Transform>("MsgTemplate");
		MsgTemplate.gameObject.SetActive(false); // hide

		CBase.Assert(MsgTemplate);
	}

	public override void OnOpen(params object[] args)
	{
		//base.OnOpen(args);
	}

	public void ShowMsg(string msgStr)
	{
		CBase.Assert(MsgTemplate);

		if (m_WaitingMsgList.Count == MSG_LIMIT)  // 超过限制了，隐藏第一个，并从等待列表中移除
		{
			XUIMidMsg_Animator msgSave = m_WaitingMsgList[0];
			m_WaitingMsgList.RemoveAt(0);
			msgSave.StopAnimate(); // 先停止其显示协程
			msgSave.StartCoroutine(msgSave.WaitMsgDelete());  // 让其渐变淡出
		}

		// 将之前的，向上移位，并缩放效果
		m_WaitingMsgList.ForEach((msgSave) =>
		{
			TweenPosition tweenPos = msgSave.GetComponent<TweenPosition>();
			Vector3 startPos = (tweenPos != null && tweenPos.enabled) ? tweenPos.to : msgSave.transform.localPosition;  // 如果正在移位中...那么取当前移位结果位置进行下一步位移
			TweenPosition.Begin(msgSave.gameObject, FADE_TIME, startPos + new Vector3(0, MSG_HEIGHT, 0));

			TweenScale tweenScale = msgSave.GetComponent<TweenScale>();
			Vector3 startScale = (tweenScale != null && tweenScale.enabled) ? tweenScale.to : msgSave.transform.localScale;  // 如果正在缩放中...那么取当前缩放结果位置进行下一步缩放
			TweenScale.Begin(msgSave.gameObject, FADE_TIME, startScale * 0.95f);
		});

		XUIMidMsg_Animator msgInstance = PoolGet();

		// 开始执行动画~
		m_WaitingMsgList.Add(msgInstance);
		msgInstance.StartAnimate(msgStr);
	}

	public void PoolDelete(XUIMidMsg_Animator msgInstance)
	{
		msgInstance.gameObject.SetActive(false);
		m_MsgTempllatePool.Push(msgInstance);
	}

	public XUIMidMsg_Animator PoolGet()
	{
		XUIMidMsg_Animator msgInstance = null;
		if (m_MsgTempllatePool.Count > 0)
			msgInstance = m_MsgTempllatePool.Pop();

		if (msgInstance == null)
		{
			GameObject newGameObj = GameObject.Instantiate(MsgTemplate.gameObject) as GameObject;
			msgInstance = newGameObj.AddComponent<XUIMidMsg_Animator>();
			msgInstance.transform.parent = this.transform;
			msgInstance.UICtrler = this;
		}

		ClearLocalTransform(msgInstance.transform);

		msgInstance.gameObject.SetActive(true);

		return msgInstance;
	}
}

public class XUIMidMsg_Animator : MonoBehaviour
{
	public CUIMidMsg UICtrler;

	public void StartAnimate(string msgStr)
	{
		UILabel msgLabel = UICtrler.GetControl<UILabel>("Label", transform);
		UIColorQuad msgBackground = UICtrler.GetControl<UIColorQuad>("Background", transform);
		CBase.Assert(msgLabel);
		CBase.Assert(msgBackground);

		msgLabel.text = msgStr;

		StartCoroutine(MsgCoroutine());
	}

	public void StopAnimate()
	{
		StopAllCoroutines();

		// 清理残留动画
		foreach (UITweener tween in gameObject.GetComponents<UITweener>())
			tween.enabled = false;  // 要用这个来暂停动画！ 坑！
	}

	// 传入需要淡入淡出的控件
	IEnumerator MsgCoroutine()
	{
		foreach (UIWidget widget in this.GetComponentsInChildren<UIWidget>())  // 淡入
		{
			widget.alpha = 0;
			TweenAlpha.Begin(widget.gameObject, CUIMidMsg.FADE_TIME, 0.8f);
		}
		yield return new WaitForSeconds(CUIMidMsg.FADE_TIME);  // 等待淡入动画

		yield return new WaitForSeconds(CUIMidMsg.MSG_TIME);   // 等待显示时间

		UICtrler.m_WaitingMsgList.Remove(this);   //已经结束显示了

		yield return StartCoroutine(WaitMsgDelete());
	}

	public IEnumerator WaitMsgDelete()
	{
		foreach (UIWidget widget in this.GetComponentsInChildren<UIWidget>())  // 淡出
			TweenAlpha.Begin(widget.gameObject, CUIMidMsg.FADE_TIME, 0f);
		TweenPosition.Begin(this.gameObject, CUIMidMsg.FADE_TIME, this.transform.localPosition + new Vector3(0, CUIMidMsg.MSG_HEIGHT, 0));  // 淡出向上位移
		yield return new WaitForSeconds(CUIMidMsg.FADE_TIME);  // 等待淡出动画

		StopAnimate();
		UICtrler.PoolDelete(this);
	}


}