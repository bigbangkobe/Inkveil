using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : BaseUI
{
    /// <summary>
    /// 武器背景图片
    /// </summary>
    public Image mWeaponBgImg;

    /// <summary>
    /// 武器图标
    /// </summary>
    public Image mWeaponIconImg;

    /// <summary>
    /// 武器描述
    /// </summary>
    public Text mWeaponDescText;

    /// <summary>
    /// 武器文本
    /// </summary>
    public Text mWeaponText;

    public SkillInfo mSkillInfo = new SkillInfo();

    /// <summary>
    /// 更新UI
    /// </summary>
    public void OnUpdateUI()
    {
        mWeaponDescText.text = mSkillInfo.skillName;
        switch (mSkillInfo.skillType)
        {
            case 1:
                mWeaponText.text = "全部攻击力加成:+" + mSkillInfo.num;
                break;
            case 2:
                mWeaponText.text = "全部攻速加成:+" + mSkillInfo.num + "%";
                break;
            case 3:
                mWeaponText.text = "全部血量加成:+" + mSkillInfo.num + "%";
                break;
        }
    }
}

public class SkillInfo
{
    /// <summary>
    /// 技能ID
    /// </summary>
    public int skillId;

    /// <summary>
    /// 技能类型
    /// </summary>
    public int skillType;

    /// <summary>
    /// 武器名称
    /// </summary>
    public string skillName;

    /// <summary>
    /// 武器描述
    /// </summary>
    public string skillDesc;

    /// <summary>
    /// 数值
    /// </summary>
    public int num;
}
