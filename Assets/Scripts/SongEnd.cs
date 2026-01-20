using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SongEnd : MonoBehaviour
{
    [Header("Song")]
    [SerializeField] private AudioSource musicSource;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button nextButton;

    [Header("Scene")]
    [Tooltip("Leave -1 to lead next build index")]
    [SerializeField] private int nextSceneBuildIndex = -1;

    private bool _shown;

    private void Awake()
    {
        if (endPanel) endPanel.SetActive(false);

        if (nextButton)
            nextButton.onClick.AddListener(LoadNextScene);
    }

    private void Update()
    {
        if (_shown || !musicSource) return;

        // Song was playing, but now is done
        if (!musicSource.isPlaying && musicSource.time > 0f)
        {
            ShowEndPanel();
        }
    }

    private void ShowEndPanel()
    {
        _shown = true;

        if (endPanel)
            endPanel.SetActive(true);

        if (nextButton)
            nextButton.Select();
    }

    public void LoadNextScene()
    {
        int current = SceneManager.GetActiveScene().buildIndex;
        int target = (nextSceneBuildIndex >= 0) ? nextSceneBuildIndex : current + 1;

        // Clamp to valid build settings range
        if (target >= SceneManager.sceneCountInBuildSettings)
            target = 0;

        SceneManager.LoadScene(target);
    }
}
