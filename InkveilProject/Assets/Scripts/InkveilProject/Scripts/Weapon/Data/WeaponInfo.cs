using System;
using UnityEngine;

public class WeaponInfo
{
    // 技能触发类型
    public enum SkillTriggerType
    {
        OnAttack,       // 攻击时触发
        OnCrit,         // 暴击时触发
        Continuous,     // 持续生效
        OnKill,         // 击杀时触发
        OnLowHealth     // 低血量时触发
    }

    public enum WeaponType
    {
        Sword,      // 剑
        Blade,      // 刀
        Spear,      // 枪
        Staff,      // 棒
        Hammer,     // 双锤
        Halberd,    // 戟
        Divine      // 神兵（金箍棒等）
    }

    // 基础信息
    public int weaponID;                 // 唯一ID（如3001）
    public string weaponName;            // 武器名称（多语言键）
    public string weaponDes;            // 武器名称（多语言键）
    public int weaponType;        // 武器类型
    public string prefabPath;            // 预制体路径
    public string iconPath;              // 图标路径

    // 核心数值
    public int baseDamage;             // 基础伤害（对应文档中的武器伤害序列）
    public double attackSpeed = 1.0f;     // 攻击速度（次/秒）
    public double growUpValue;
    public double critRate;               // 暴击率（0-1）
    public double critMultiplier = 2.0f;  // 暴击伤害倍率

    // 成长系统
    public int maxLevel = 10;            // 最大等级
    public int currentLevel = 1;         // 当前等级

    // 技能系统
    public int triggerType; // 技能触发类型
    public double skillRadius = 3f;       // 技能作用范围
    public double skillDamageMulti = 1f;  // 技能伤害系数
    public int projectileCount = 1;      // 投射物数量（剑的飞剑数量等）
    public int penetrationCount;         // 穿透次数（枪的穿刺效果）

    // 升级精炼
    public int upgradeSoulInk = 50;      // 每次升级需要魂墨
    public string refineMaterialIds;      // 精炼所需材料ID数组
    public int maxRefineLevel = 5;       // 最大精炼等级

    // 特殊属性
    public int isDivineWeapon;          // 是否神兵
    public int linkedGodId = -1;         // 关联神明ID（-1表示无）

    // 资源系统
    public string attackSound;           // 攻击音效
    public string skillSound;            // 技能音效
    public string hitSound;              // 击中音效
    public string trailEffect;           // 拖尾特效路径
    public string hitEffect;             // 受到攻击特效

    /// <summary>
    /// 获取当前等级的实际伤害
    /// </summary>
    public int GetCurrentDamage()
    {
        return (int)(baseDamage * (currentLevel - 1) * growUpValue + baseDamage);
    }

    /// <summary>
    /// 检查是否可以升级
    /// </summary>
    public bool CanUpgrade()
    {
        return false;
    }

    /// <summary>
    /// 根据类型获取特征参数（适配文档中的武器特性）
    /// </summary>
    public double GetCharacteristicValue()
    {
        return (WeaponType)weaponType switch
        {
            WeaponType.Sword => projectileCount * 0.3f,      // 飞剑数量加成
            WeaponType.Blade => currentLevel * 0.1f,         // 连击加成
            WeaponType.Spear => penetrationCount * 0.5f,     // 穿透加成
            WeaponType.Hammer => critRate * 2f,              // 眩晕概率
            _ => 0
        };
    }
}