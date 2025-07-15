using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
	// 异步对象
	AsyncOperation op = null;

	void Start()
	{
		op = SceneManager.LoadSceneAsync(1);
		op.allowSceneActivation = false;
	}

	public void AllowSceneActivation()
	{
		op.allowSceneActivation = true;
	}
}