using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Reflection;
using TMPro;

public class SongEnd : MonoBehaviour
{
    [Header("Song")]
    [SerializeField] private AudioSource musicSource;
    
    [Header("UI Elements")]
    [SerializeField] private GameObject endPanel;
    [SerializeField] private Button nextButton;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text accuracyText;
    [SerializeField] private TMP_Text maxComboText;
    [SerializeField] private TMP_Text perfectCountText;
    [SerializeField] private TMP_Text goodCountText;
    [SerializeField] private TMP_Text missCountText;

    [Header("Scene")]
    [Tooltip("Leave -1 to lead next build index")]
    [SerializeField] private int nextSceneBuildIndex = -1;
    
    private SongStatManager songStatManager;
    private bool _shown;

    private void Awake()
    {
        if (!songStatManager) songStatManager = FindFirstObjectByType<SongStatManager>();
        if (endPanel) endPanel.SetActive(false);
        if (nextButton) nextButton.onClick.AddListener(LoadNextScene);
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
        SongStats songStats = songStatManager.getData();

        scoreText.text = songStats.score.ToString();
        accuracyText.text = songStats.accuracy.ToString();
        maxComboText.text = songStats.maxCombo.ToString();
        perfectCountText.text = songStats.perfectCount.ToString();
        goodCountText.text = songStats.goodCount.ToString();
        missCountText.text = songStats.missCount.ToString();

        if (endPanel) endPanel.SetActive(true);
        if (nextButton) nextButton.Select();

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
