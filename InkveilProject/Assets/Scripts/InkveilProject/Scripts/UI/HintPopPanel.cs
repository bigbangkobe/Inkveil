using UnityEngine;
using UnityEngine.UI;

public class HintPopPanel : MonoBehaviour
{
    [SerializeField] private Button m_BackBtn;
    [SerializeField] private Text m_ContentText;

    private void Start()
    {
        m_BackBtn.onClick.AddListener(OnBackClickHandler);
    }

    private void OnDestroy()
    {
        m_BackBtn.onClick.RemoveListener(OnBackClickHandler);
    }

    public void ShowHint(string content) 
    {
        m_ContentText.text = content;
        gameObject.SetActive(true);
    }

    private void OnBackClickHandler()
    {
        gameObject.SetActive(false);
    }
}