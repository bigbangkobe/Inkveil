using Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : BaseUI
{
    /// <summary>
    /// ��������ͼƬ
    /// </summary>
    public Image mWeaponBgImg;

    /// <summary>
    /// ����ͼ��
    /// </summary>
    public Image mWeaponIconImg;

    /// <summary>
    /// ��������
    /// </summary>
    public Text mWeaponDescText;

    /// <summary>
    /// �����ı�
    /// </summary>
    public Text mWeaponText;

    public SkillInfo mSkillInfo = new SkillInfo();

    /// <summary>
    /// ����UI
    /// </summary>
    public void OnUpdateUI()
    {
        mWeaponDescText.text = mSkillInfo.skillName;
        switch (mSkillInfo.skillType)
        {
            case 1:
                mWeaponText.text = "ȫ���������ӳ�:+" + mSkillInfo.num;
                break;
            case 2:
                mWeaponText.text = "ȫ�����ټӳ�:+" + mSkillInfo.num + "%";
                break;
            case 3:
                mWeaponText.text = "ȫ��Ѫ���ӳ�:+" + mSkillInfo.num + "%";
                break;
        }
    }
}

public class SkillInfo
{
    /// <summary>
    /// ����ID
    /// </summary>
    public int skillId;

    /// <summary>
    /// ��������
    /// </summary>
    public int skillType;

    /// <summary>
    /// ��������
    /// </summary>
    public string skillName;

    /// <summary>
    /// ��������
    /// </summary>
    public string skillDesc;

    /// <summary>
    /// ��ֵ
    /// </summary>
    public int num;
}
