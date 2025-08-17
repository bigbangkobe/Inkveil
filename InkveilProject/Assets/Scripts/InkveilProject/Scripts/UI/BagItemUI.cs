using UnityEngine;
using UnityEngine.UI;

public class BagItemUI : MonoBehaviour
{
    public Image m_My;     // �����򣨸��ݵȼ���ɫ��
    public Image m_Icon;   // ��Ʒͼ��
    public Image m_Lock;   // ����ͼ��
    public Text m_Title;   // ��Ʒ����
    public Text m_Number;  // ��Ʒ����

    public BagItemInfo bagItem;

    // �ȼ���ɫ����
    public Sprite[] gradeColors;

    // ��ʼ��UIԪ��
    public async void OnInit(BagItemInfo itemInfo)
    {
        bagItem = itemInfo;

        m_Icon.gameObject.SetActive(true);
        m_Lock.gameObject.SetActive(true);
        m_Title.gameObject.SetActive(true);
        m_Number.gameObject.SetActive(true);

        // ���ñ�������ɫ
        SetBackgroundColor(itemInfo.propertyInfo.propertyGrade);

        // ������Ʒͼ�꣨����ʹ��Resources���أ�
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

        // ��������״̬
        m_Lock.gameObject.SetActive(itemInfo.isLock);

        // ������Ʒ����
        m_Title.text = itemInfo.propertyInfo.propertyDes;

        // ������Ʒ������ֻ����������1ʱ��ʾ��
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

    // ���ݵȼ����ñ�����ɫ
    private void SetBackgroundColor(int grade)
    {
        // ȷ���ȼ�����Ч��Χ�ڣ�1-3��
        int colorIndex = Mathf.Clamp(grade - 1, 0, gradeColors.Length - 1);

        // ���ñ�����ɫ
        if (m_My != null)
        {
            m_My.sprite = gradeColors[colorIndex];
        }
    }

    // ������Ʒ������ʾ
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

    // ��������״̬
    public void UpdateLockState(bool isLocked)
    {
        if (bagItem != null) 
        {
            BagManager.instance.UpdateItemState(bagItem.propertyInfo.propertyID,true);
        }
        m_Lock.gameObject.SetActive(isLocked);
    }
}