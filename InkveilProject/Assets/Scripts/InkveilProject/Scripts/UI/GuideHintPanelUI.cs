using UnityEngine;
using UnityEngine.UI;

public class GuideHintPanelUI : MonoBehaviour
{
    public GameObject m_Hint;
    public Image m_Image;

    public RectTransform my;
    public Transform vector3;

    private void Awake()
    {
        my = GetComponent<RectTransform>();
        m_Image = GetComponent<Image>();
    }

    private void OnEnable()
    {
       

        if (GameManager.instance.GameStateEnum == GameConfig.GameState.State.Play)
        {
            GameManager.instance.GameStateEnum = GameConfig.GameState.State.Pause;
        }
    }

    private void OnDisable()
    {
        if (GameManager.instance.GameStateEnum == GameConfig.GameState.State.Pause)
        {
            GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play;
        }
    }

    public void SetPoint(Transform vector3)
    {
        if (!GuideDispositionManager.instance.isGuide)
        {
            gameObject.SetActive(true);
            transform.position = vector3.position;
            m_Image.material.SetVector("_Center", new Vector4(transform.localPosition.x, transform.localPosition.y, 0, 0));
        }
    }
}