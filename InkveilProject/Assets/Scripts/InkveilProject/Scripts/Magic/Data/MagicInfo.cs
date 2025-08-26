// 法宝类型枚举
using System;
using UnityEngine;
[Serializable]
public class MagicInfo
{
    public enum MagicType
    {
        Attack,     // 攻击型（如九龙神火罩）
        Defense,    // 防御型（如混天绫）
        Summon,     // 召唤型（如金毫毛）
        Control     // 控制型（如芭蕉扇）
    }

    // 触发方式枚举
    public enum TriggerType
    {
        AutoCycle,      // 自动周期触发
        OnKill,         // 击杀敌人时
        OnHit,          // 受击时
        OnLowHealth,    // 低血量时
        Passive         // 被动常驻
    }

    // 基础信息
    public int magicID;                  // 唯一ID
    public string magicName;             // 法宝名称
    public MagicType magicType;          // 法宝类型
    public string prefabPath;            // 预制体路径
    public string iconPath;              // 图标路径

    // 效果系统
    public TriggerType triggerType;      // 触发方式
    public float baseEffectValue;        // 基础效果值（伤害/治疗量等）
    public float effectRadius = 3f;      // 作用范围
    public float duration;               // 持续时间（0表示瞬时）

    // 冷却系统
    public float baseCooldown = 10f;     // 基础冷却时间
    public float minCooldown = 3f;       // 最小冷却时间

    // 成长系统
    public int maxLevel = 10;            // 最大等级
    public int currentLevel = 1;         // 当前等级
    public AnimationCurve valueCurve;    // 数值成长曲线
    public AnimationCurve cdCurve;       // 冷却缩减曲线

    // 升级消耗
    public int upgradeSoulInk = 50;      // 每次升级需要魂墨
    public int upgradeMaterialID;        // 专属材料ID

    // 资源系统
    public string activateSound;         // 触发音效
    public string loopSound;             // 持续音效
    public string effectParticle;        // 特效资源

    /// <summary>
    /// 获取当前等级的实际效果值
    /// </summary>
    public float GetCurrentValue()
    {
        return baseEffectValue * valueCurve.Evaluate((float)currentLevel / maxLevel);
    }

    /// <summary>
    /// 获取当前冷却时间
    /// </summary>
    public float GetCurrentCooldown()
    {
        return Mathf.Lerp(minCooldown, baseCooldown,
            cdCurve.Evaluate((float)currentLevel / maxLevel));
    }

    /// <summary>
    /// 检查是否可以升级
    /// </summary>
    public bool CanUpgrade()
    {
        return false;
    }
}