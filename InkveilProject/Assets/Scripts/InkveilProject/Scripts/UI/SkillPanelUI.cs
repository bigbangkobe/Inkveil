using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameConfig;

public class SkillPanelUI : BaseUI
{
    /// <summary>
    /// ����1
    /// </summary>
    public SkillUI mSkill1;

    /// <summary>
    /// ����2
    /// </summary>
    public SkillUI mSkill2;

    /// <summary>
    /// ����3
    /// </summary>
    public SkillUI mSkill3;


    /// <summary>
    /// ��ʼ��
    /// </summary>
    protected override void OnAwake()
    {
        base.OnAwake();
        OnUpdateUI();
    }

    /// <summary>
    /// չʾUI
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        OnUpdateUI();
    }

    /// <summary>
    /// ����UI
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
    }

    /// <summary>
    /// ����UI
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
    /// ���ü��ܼӳ����ݸ���
    /// </summary>
    /// <param name="skillInfo">���ܼӳ����ݶ���</param>
    /// <param name="skillArray">���õļ�������</param>
    /// <param name="weaponInfo">�������ݶ���</param>
    public void SetSkillUI(SkillInfo skillInfo, int[] skillArray, WeaponInfo weaponInfo)
    {
        skillInfo.skillId = skillArray[0];
        skillInfo.skillName = weaponInfo.weaponName;
        skillInfo.skillDesc = weaponInfo.weaponDes;
        skillInfo.skillType = skillArray[1];
        skillInfo.num = skillArray[2];
    }


    /// <summary>
    /// �������1��ť�¼�
    /// </summary>
    public void OnSkill1ButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;

    }

    /// <summary>
    /// �������2��ť�¼�
    /// </summary>
    public void OnSkill2ButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;
    }

    /// <summary>
    /// �������3��ť�¼�
    /// </summary>
    public void OnSkill3ButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;
    }

    /// <summary>
    /// ˢ��
    /// </summary>
    public void OnRefleshButtonClick()
    {
        OnUpdateUI();

    }

    /// <summary>
    /// ȫ����ȡ
    /// </summary>
    public void OnGetAllButtonClick()
    {
        Hide();
        GameManager.instance.GameStateEnum = GameState.State.Play;
    }
}
