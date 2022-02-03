using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UINotificationsManager : MonoBehaviour
{
    public static UINotificationsManager instance;

    [Header("Level Up")]

    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float fadeDuration = 0.2f;
    [SerializeField]
    [Range(0.1f, 3.0f)]
    private float visibleDuration = 1.5f;
    [SerializeField]
    private GameObject levelUpPanel;
    [SerializeField]
    private Text levelUpText;
    [SerializeField]
    private Text levelUpShadow;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlayNotification(UINotificationType type, params int[] parameters)
    {
        switch(type)
        {
            case UINotificationType.levelUp:
                StartCoroutine(LevelUpAnimation(parameters[0]));
                break;
            default:
                Debug.Log("Uknown UI notification type...");
                break;
        }

    }

    private IEnumerator LevelUpAnimation(int newLevel)
    {
        float timer = 0.0f;
        levelUpText.text = "Level Up!\n" + newLevel.ToString();
        levelUpShadow.text = levelUpText.text;

        Color tempColorText = levelUpText.color;
        Color tempColorShadow = levelUpShadow.color;

        levelUpPanel.SetActive(true);

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0.0f, 1.0f, timer / fadeDuration);
            tempColorText.a = alpha;
            tempColorShadow.a = alpha;
            levelUpText.color = tempColorText;
            levelUpShadow.color = tempColorShadow;

            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < visibleDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        timer = 0.0f;

        while (timer < fadeDuration)
        {
            float alpha = Mathf.Lerp(0.0f, 1.0f, (fadeDuration - timer) / fadeDuration);
            tempColorText.a = alpha;
            tempColorShadow.a = alpha;
            levelUpText.color = tempColorText;
            levelUpShadow.color = tempColorShadow;

            timer += Time.deltaTime;
            yield return null;
        }

        levelUpPanel.SetActive(false);
    }
}

public enum UINotificationType
{
    levelUp
}