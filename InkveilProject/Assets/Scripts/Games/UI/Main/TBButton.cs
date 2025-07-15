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
    /// 显示界面
    /// </summary>
    protected override void OnShowEnable()
    {
        base.OnShowEnable();
        m_Btn.onClick.AddListener(OnButtonClick);
    }

    /// <summary>
    /// 隐藏界面
    /// </summary>
    protected override void OnHideDisable()
    {
        base.OnHideDisable();
        m_Btn.onClick.RemoveListener(OnButtonClick);
    }

    /// <summary>
    /// 点击按钮事件
    /// </summary>
    private void OnButtonClick()
    {
        print("点击按钮事件");
    }
}
