using DG.Tweening;
using Framework;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainButtomToggle : BaseUI
{
    [SerializeField]
    private GameObject m_Panel;

    [SerializeField]
    private Toggle m_Toggle;

    [SerializeField]
    private Image m_Image;

    [SerializeField]
    private Image m_Text;

    /// <summary>
    /// ��ʼ������
    /// </summary>
    protected override void OnInit()
    {
        base.OnInit();
    }

    /// <summary>
    /// ��ʾ���溯��
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        m_Toggle.onValueChanged.AddListener(OnToogleValueChanged);
    }

    /// <summary>
    /// ���ؽ��溯��
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        m_Toggle.onValueChanged.RemoveListener(OnToogleValueChanged);
    }

    /// <summary>
    /// ���Toogle�¼�
    /// </summary>
    /// <param name="isOn"></param>
    private void OnToogleValueChanged(bool isOn)
    {
        //if (isOn)
        //{
        //    m_Image.rectTransform.DOAnchorPosY(40, 0.2f);
        //    m_Text.rectTransform.DOAnchorPosY(40, 0.2f);
        //    if (m_Panel)
        //    {
        //        m_Panel.SetActive(true);
        //    }
        //}
        //else
        //{
        //    m_Image.rectTransform.DOAnchorPosY(0, 0.2f);
        //    m_Text.rectTransform.DOAnchorPosY(0, 0.2f);
        //    if (m_Panel)
        //    {
        //        m_Panel.SetActive(false);
        //    }
        //}
    }
}
