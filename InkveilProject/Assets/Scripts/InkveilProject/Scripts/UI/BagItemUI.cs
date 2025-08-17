using UnityEngine;
using UnityEngine.UI;

public class BagItemUI : MonoBehaviour
{
    public Image m_My;     // 背景框（根据等级变色）
    public Image m_Icon;   // 物品图标
    public Image m_Lock;   // 锁定图标
    public Text m_Title;   // 物品名称
    public Text m_Number;  // 物品数量

    public BagItemInfo bagItem;

    // 等级颜色配置
    public Sprite[] gradeColors;

    // 初始化UI元素
    public async void OnInit(BagItemInfo itemInfo)
    {
        bagItem = itemInfo;

        m_Icon.gameObject.SetActive(true);
        m_Lock.gameObject.SetActive(true);
        m_Title.gameObject.SetActive(true);
        m_Number.gameObject.SetActive(true);

        // 设置背景框颜色
        SetBackgroundColor(itemInfo.propertyInfo.propertyGrade);

        // 设置物品图标（假设使用Resources加载）
        if (!string.IsNullOrEmpty(itemInfo.propertyInfo.imagePath))
        {
            Sprite iconSprite = await ResourceService.LoadAsync<Sprite>(itemInfo.propertyInfo.imagePath);
            if (iconSprite != null)
            {
                m_Icon.sprite = iconSprite;

                m_Title.gameObject.SetActive(false);
            }
            else
            {
                m_Title.gameObject.SetActive(true);
                m_Icon.gameObject.SetActive(false);

            }
        }
        else 
        {
            m_Title.gameObject.SetActive(true);
            m_Icon.gameObject.SetActive(false);
        }

        // 设置锁定状态
        m_Lock.gameObject.SetActive(itemInfo.isLock);

        // 设置物品名称
        m_Title.text = itemInfo.propertyInfo.propertyDes;

        // 设置物品数量（只在数量大于1时显示）
        if (itemInfo.propertyInfo.number > 1)
        {
            m_Number.text = $"X{itemInfo.propertyInfo.number}";
            m_Number.gameObject.SetActive(true);
        }
        else
        {
            m_Number.gameObject.SetActive(false);
        }
    }

    public void UnInit() 
    {
        m_Icon.gameObject.SetActive(false);
        m_Lock.gameObject.SetActive(false);
        m_Title.gameObject.SetActive(false);
        m_Number.gameObject.SetActive(false);
        m_My.sprite = gradeColors[3];
    }

    // 根据等级设置背景颜色
    private void SetBackgroundColor(int grade)
    {
        // 确保等级在有效范围内（1-3）
        int colorIndex = Mathf.Clamp(grade - 1, 0, gradeColors.Length - 1);

        // 设置背景颜色
        if (m_My != null)
        {
            m_My.sprite = gradeColors[colorIndex];
        }
    }

    // 更新物品数量显示
    public void UpdateNumber(int newNumber)
    {
        if (newNumber > 1)
        {
            m_Number.text = $"x{newNumber}";
            m_Number.gameObject.SetActive(true);
        }
        else
        {
            m_Number.gameObject.SetActive(false);
        }
    }

    // 更新锁定状态
    public void UpdateLockState(bool isLocked)
    {
        if (bagItem != null) 
        {
            BagManager.instance.UpdateItemState(bagItem.propertyInfo.propertyID,true);
        }
        m_Lock.gameObject.SetActive(isLocked);
    }
}