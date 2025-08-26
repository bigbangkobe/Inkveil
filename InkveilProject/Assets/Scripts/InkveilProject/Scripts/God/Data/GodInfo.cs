using System;

[Serializable]
public class GodInfo
{
    // 基础属性
    public int godID;            // 角色唯一ID
    public string godName;         // 角色名称
    public int godType;         // 角色类型
    public string prefabPath;     // 预制体资源路径
    public string iconPath;       // 图标资源路径

    public int propertyID;      // 商品ID

    // 成长属性
    public int level;             // 角色等级
    public int baseDuration;     // 基础登场时间(秒)
    public int maxDuration;      // 最大登场时间(秒)
    public int baseCooldown;    // 基础冷却时间(秒)
    public int fragRequire;      // 合成所需碎片

    // 技能属性
    public string skillDescription; // 技能描述
    public double attackSpeed; // 攻速
    public int baseAttack; // 基础伤害
    public double skillDamageMulti; // 伤害系数
    public int attach;            // 附加伤害
    public double skillRadius;     // 作用范围
    public int skillEnergy;        // 技能所需能量

    public string basicAttackEffect; // 普攻特效路径
    public string basicAttackSound; // 普攻音效路径
    public string skillEffect;     // 技能特效路径
    public string skillSound;       // 技能音效路径
    public string summonSound;
}