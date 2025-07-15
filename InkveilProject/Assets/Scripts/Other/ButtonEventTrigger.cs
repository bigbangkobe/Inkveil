using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Framework;
using System;

public class ButtonEventTrigger : MonoBehaviour ,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler
{
    /// <summary>
    /// 缩放对象
    /// </summary>
    private Transform tweenTransform;

    /// <summary>
    /// 按下动画
    /// </summary>
    private Tweener pointDownTweener = null;

    /// <summary>
    /// 弹起动画
    /// </summary>
    private Tweener pointUpTweener = null;

    /// <summary>
    /// 是否点击
    /// </summary>
    private bool isClick = false;

	/// <summary>
	/// 初始大小
	/// </summary>
	private Vector3 preSize;

	/// <summary>
	/// 缩放倍数
	/// </summary>
	private readonly float sizeValue = 0.9f;

	/// <summary>
	/// 按下回调
	/// </summary>
	public Action PointerDownCallBack = null;

	/// <summary>
	/// 初始化
	/// </summary>
	/// <param name="button"></param>
	void Start()
    {
        tweenTransform = this.transform;
		preSize = tweenTransform.localScale;
	}

	public void Init(Transform target)
	{

	}

    /// <summary>
    /// 按下
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnPointerDown(PointerEventData pointerEventData)
    {
		if(PointerDownCallBack!=null)
		{
			PointerDownCallBack.Invoke();
			return;
		}
        OnKillTweener();
        pointDownTweener = tweenTransform.DOScale(sizeValue, 0.05f).SetEase(Ease.InSine).SetAutoKill();
        isClick = false;
    }

    /// <summary>
    /// 弹起
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnPointerUp(PointerEventData pointerEventData)
    {
		if (PointerDownCallBack != null)
			return;

		if (isClick)
            return;
        TimerObject timerObject = null;
        timerObject = TimerSystem.Start(delegate (object obj) 
        {
            tweenTransform.localScale = preSize;
            timerObject.Stop();
        }, false, 0.2f);
        
    }

    /// <summary>
    /// 点击
    /// </summary>
    /// <param name="pointerEventData"></param>
    public void OnPointerClick(PointerEventData pointerEventData)
    {
		if (PointerDownCallBack != null)
			return;
		OnKillTweener();
        tweenTransform.localScale = Vector2.one * sizeValue;
        pointUpTweener = tweenTransform.DOScale(preSize, 0.5f).SetEase(Ease.OutElastic).SetAutoKill();
        isClick = true;
    }

    /// <summary>
    /// 杀掉动画
    /// </summary>
    private void OnKillTweener()
    {
        if (pointDownTweener != null)
            pointDownTweener.Kill();
        if (pointUpTweener != null)
            pointUpTweener.Kill();
    }
}
