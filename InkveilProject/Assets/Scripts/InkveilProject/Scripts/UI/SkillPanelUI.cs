using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

public class SkillPanelUI : BaseUI
{
    /// <summary>
    /// 增益1
    /// </summary>
    public SkillUI mSkill1;

    /// <summary>
    /// 增益2
    /// </summary>
    public SkillUI mSkill2;

    /// <summary>
    /// 增益3
    /// </summary>
    public SkillUI mSkill3;


    /// <summary>
    /// 初始化
    /// </summary>
    protected override void OnAwake()
    {
        base.OnAwake();
        OnUpdateUI();
    }

    /// <summary>
    /// 展示UI
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        OnUpdateUI();
    }

    /// <summary>
    /// 隐藏UI
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }

    /// <summary>
    /// 更新UI
    /// </summary>
    public void OnUpdateUI()
    {
        LevelInfo levelInfo = LevelManager.instance.m_CurLevelInfo;
        int[] skill1Array = levelInfo.GetSkillGroup()[0];
        int[] skill2Array = levelInfo.GetSkillGroup()[1];
        int[] skill3Array = levelInfo.GetSkillGroup()[2];
        WeaponInfo weaponInfo1 = WeaponDispositionManager.instance.GetWeaponById(skill1Array[0]);
        WeaponInfo weaponInfo2 = WeaponDispositionManager.instance.GetWeaponById(skill2Array[0]);
        WeaponInfo weaponInfo3 = WeaponDispositionManager.instance.GetWeaponById(skill3Array[0]);

        SetSkillUI(mSkill1.mSkillInfo, skill1Array, weaponInfo1);
        SetSkillUI(mSkill2.mSkillInfo, skill2Array, weaponInfo2);
        SetSkillUI(mSkill3.mSkillInfo, skill3Array, weaponInfo3);

        mSkill1.OnUpdateUI();
        mSkill2.OnUpdateUI();
        mSkill3.OnUpdateUI();

    }

    /// <summary>
    /// 设置技能加成数据更新
    /// </summary>
    /// <param name="skillInfo">技能加成数据对象</param>
    /// <param name="skillArray">配置的技能数组</param>
    /// <param name="weaponInfo">武器数据对象</param>
    public void SetSkillUI(SkillInfo skillInfo, int[] skillArray, WeaponInfo weaponInfo)
    {
        skillInfo.skillId = skillArray[0];
        skillInfo.skillName = weaponInfo.weaponName;
        skillInfo.skillDesc = weaponInfo.weaponDes;
        skillInfo.skillType = skillArray[1];
        skillInfo.num = skillArray[2];
    }


    /// <summary>
    /// 点击技能1按钮事件
    /// </summary>
    public void OnSkill1ButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;

    }

    /// <summary>
    /// 点击技能2按钮事件
    /// </summary>
    public void OnSkill2ButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;
    }

    /// <summary>
    /// 点击技能3按钮事件
    /// </summary>
    public void OnSkill3ButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;
    }

    /// <summary>
    /// 刷新
    /// </summary>
    public void OnRefleshButtonClick()
    {
        OnUpdateUI();

    }

    /// <summary>
    /// 全部领取
    /// </summary>
    public void OnGetAllButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;
    }
}
