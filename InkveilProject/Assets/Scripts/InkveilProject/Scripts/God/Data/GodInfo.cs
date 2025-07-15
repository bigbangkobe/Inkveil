public class GodInfo
{
    // 基础属性
    public int godID { get; set; }              // 角色唯一ID
    public string godName { get; set; }         // 角色名称
    public int godType { get; set; }            // 角色类型
    public string prefabPath { get; set; }      // 预制体资源路径
    public string iconPath { get; set; }        // 图标资源路径

    // 成长属性
    public int level { get; set; }              // 角色等级
    public int baseDuration { get; set; }     // 基础登场时间(秒)
    public int maxDuration { get; set; }      // 最大登场时间(秒)
    public int baseCooldown { get; set; }     // 基础冷却时间(秒)
    public int fragRequire { get; set; }        // 合成所需碎片

    // 技能属性
    public string skillDescription { get; set; } // 技能描述
    public double attackSpeed { get; set; } // 攻速
    public int baseAttack { get; set; } // 基础伤害
    public double skillDamageMulti { get; set; } // 伤害系数
    public int attach { get; set; }             // 附加伤害
    public double skillRadius { get; set; }      // 作用范围
    public int skillEnergy { get; set; }        // 技能所需能量

    public string basicAttackEffect { get; set; } // 普攻特效路径
    public string basicAttackSound { get; set; }  // 普攻音效路径
    public string skillEffect { get; set; }      // 技能特效路径
    public string skillSound { get; set; }       // 技能音效路径
    public string summonSound { get; set; }
}