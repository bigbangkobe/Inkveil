using UnityEngine;
using UnityEngine.UI;

public class RevivePanelUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Button back;
    [SerializeField] private GameObject failPanel;
    [SerializeField] private Text text;

    int count = 3;

    private void Awake()
    {
        button.onClick.AddListener(OnClick);
        back.onClick.AddListener(OnBackClick);
    }

    private void OnEnable()
    {
        if (count == 0) 
        {
            failPanel.SetActive(true);
            gameObject.SetActive(false);
        }
        text.text = $"¸´»î£¨{count}/3£©";
    }

    private void OnBackClick()
    {

        //SceneManager.LoadSceneAsync("Main");
        failPanel.SetActive(true);
    }

    private void OnClick()
    {
        PlayerController.instance.InitializePlaierInfo();
        GameManager.instance.GameStateEnum = GameConfig.GameState.State.Play;
        gameObject.SetActive(false);

        count--;
        EnemyManager.instance.OnJiTui();
    }
}
