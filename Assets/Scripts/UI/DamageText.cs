using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageText : MonoBehaviour
{
    public GameObject damageTextCanvasPrefab;
    [Range(0.1f, 2.0f)]
    public float animationDuration = 0.5f;
    [Range(0.1f, 2.0f)]
    public float afterAnimationDuration = 0.5f;
    [Range(0.1f, 2.0f)]
    public float fadeOutDuration = 0.2f;
    [Range(0.1f, 2.0f)]
    public float yDisplacement = 1.0f;

    private GameObject[] damageTextInstances;
    private bool[] instanceAvailability;
    private const int numOfInstances = 5;
    private Text[] damageTexts;
    private Color[] damageColors;

    void Start()
    {
        InitializeInstances();
    }

    public void ActivateDamageText(int damage)
    {
        for (int i = 0; i < numOfInstances; i++)
        {
            if (instanceAvailability[i])
            {
                damageTexts[i].text = damage.ToString();
                StartCoroutine(AnimateDamageText(i));
                
                return;
            }
        }

        Debug.LogError("[!] No damage text instance available to animate");
    }

    IEnumerator AnimateDamageText(int instanceIndex)
    {
        instanceAvailability[instanceIndex] = false;
        float timeElapsed = 0.0f;

        while (timeElapsed < animationDuration)
        {
            timeElapsed += Time.deltaTime;

            damageColors[instanceIndex].a = Mathf.Lerp(0.0f, 1.0f, timeElapsed / animationDuration);
            damageTextInstances[instanceIndex].transform.localPosition = Vector3.up * Mathf.Lerp(0.0f, yDisplacement, timeElapsed / animationDuration);
            damageTexts[instanceIndex].color = damageColors[instanceIndex];

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < afterAnimationDuration)
        {
            timeElapsed += Time.deltaTime;

            yield return null;
        }

        timeElapsed = 0.0f;

        while (timeElapsed < fadeOutDuration)
        {
            timeElapsed += Time.deltaTime;

            damageColors[instanceIndex].a = 1.0f - Mathf.Lerp(0.0f, 1.0f, timeElapsed / fadeOutDuration);
            damageTexts[instanceIndex].color = damageColors[instanceIndex];

            yield return null;
        }

        damageTextInstances[instanceIndex].transform.localPosition = Vector3.zero;
        instanceAvailability[instanceIndex] = true;
    }

    void InitializeInstances()
    {
        damageTextInstances = new GameObject[numOfInstances];
        instanceAvailability = new bool[numOfInstances];
        damageTexts = new Text[numOfInstances];
        damageColors = new Color[numOfInstances];

        for (int i = 0; i < numOfInstances; i++)
        {
            damageTextInstances[i] = Instantiate(damageTextCanvasPrefab, transform);
            instanceAvailability[i] = true;
            damageTexts[i] = damageTextInstances[i].GetComponentInChildren<Text>();
            damageColors[i] = damageTexts[i].color;
            damageColors[i].a = 0.0f;
            damageTexts[i].color = damageColors[i];
        }
    }
}
