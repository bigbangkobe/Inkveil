using Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TBButton : BaseUI
{
    [SerializeField]
    private Button m_Btn;

    [SerializeField]
    private TextMeshProUGUI m_TMP;

    /// <summary>
    /// ��ʾ����
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        m_Btn.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// ���ؽ���
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        m_Btn.onClick.RemoveListener(OnButtonClick);
    }

    /// <summary>
    /// �����ť�¼�
    /// </summary>
    private void OnButtonClick()
    {
        print("�����ť�¼�");
    }
}
