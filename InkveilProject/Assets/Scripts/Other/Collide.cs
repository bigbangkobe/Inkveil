using System;
using UnityEngine;

public class Collide : MonoBehaviour
{
    /// <summary>
    /// 进入碰撞体回调2D
    /// </summary>
    [HideInInspector]
    public Action<Collision2D> EnterCollision2DCallback = null;

    /// <summary>
    /// 静止在碰撞体Collision2D事件
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (EnterCollision2DCallback != null)
            EnterCollision2DCallback.Invoke(collision);
    }

    /// <summary>
    /// 进入碰撞体回调
    /// </summary>
    [HideInInspector]
    public Action<Collision> EnterCollisionCallback = null;

    /// <summary>
    /// 静止在碰撞体Collision事件
    /// </summary>
    /// <param name="collision"></param>
    void OnCollisionEnter(Collision collision)
    {
        if (EnterCollisionCallback != null)
            EnterCollisionCallback.Invoke(collision);
    }

    /// <summary>
    /// 处于触发器回调2D
    /// </summary>
    [HideInInspector]
    public Action<Collider2D> StayCallback2D = null;

    /// <summary>
    /// 进入触发器回调2D
    /// </summary>
    [HideInInspector]
    public Action<Collider2D> EnterCallback2D = null;

    /// <summary>
    /// 离开触发器回调2D
    /// </summary>
    [HideInInspector]
    public Action<Collider2D> ExitCallback2D = null;

    /// <summary>
    /// 处于触发器事件2D
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerStay2D(Collider2D collision)
    {
        if (StayCallback2D != null)
            StayCallback2D.Invoke(collision);
    }

    /// <summary>
    /// 进入触发器事件2D
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (EnterCallback2D != null)
            EnterCallback2D.Invoke(collision);
    }

    /// <summary>
    /// 离开触发器事件2D
    /// </summary>
    /// <param name="collision"></param>
    void OnTriggerExit2D(Collider2D collision)
    {
        if (ExitCallback2D != null)
            ExitCallback2D.Invoke(collision);
    }

    /// <summary>
    /// 处于触发器回调
    /// </summary>
    [HideInInspector]
    public Action<Collider> StayCallback = null;

    /// <summary>
    /// 进入触发器回调
    /// </summary>
    [HideInInspector]
    public Action<Collider> EnterCallback = null;

    /// <summary>
    /// 离开触发器回调
    /// </summary>
    [HideInInspector]
    public Action<Collider> ExitCallback = null;

    /// <summary>
    /// 处于触发器事件
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerStay(Collider other)
    {
        if (StayCallback != null)
            StayCallback.Invoke(other);
    }

    /// <summary>
    /// 进入触发器事件
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerEnter(Collider other)
    {
        if (EnterCallback != null)
            EnterCallback.Invoke(other);
    }

    /// <summary>
    /// 离开触发器事件
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (ExitCallback != null)
            ExitCallback.Invoke(other);
    }
}
