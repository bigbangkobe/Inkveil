using System;

// 敌人类型枚举
public enum EnemyType
{
    Minion,      // 小怪
    Elite,       // 精英怪
    General,     // 妖将
    DemonLord,   // 妖王
    GreatDemon   // 大妖
}

// 敌人技能类型标记
[Flags]
public enum EnemyAbility
{
    None = 0,
    Summon = 1,         // 召唤能力
    DamageImmunity = 2, // 伤害免疫
    AuraEnhance = 4     // 光环增强
}

public class EnemyInfo
{
    public int enemyID;              // 唯一ID
    public string enemyName;         // 敌人名称
    public string enemyDes;         // 敌人名称
    public int enemyType;           // 敌人类型
    public string prefabPath;        // 预制体路径

    // 基础数值
    public int hpBase;             // 基础生命值（对应文档中的初始值）
    public double hpGrowthPerLevel;   // 每关血量成长系数（如0.1表示10%）
    public int attackDamage;         // 对护盾伤害值
    public float moveSpeed;          // 移动速度

    // 特殊能力
    public int abilities;
    public double immunityProbability; // 免疫概率（0-1）
    public double injuryRelief; 
    public double godInjuryRelief; 
    public double rewardProbability; 
    public int reward; 
}