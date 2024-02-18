using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class semafore : MonoBehaviour
{
    [SerializeField] private MeshRenderer redRenderer;
    [SerializeField] private MeshRenderer yellowRenderer;
    [SerializeField] private MeshRenderer greenRenderer;

    private void Start()
    {
        StartCoroutine(Animation());
    }

    IEnumerator Animation()
    {
        while (true)
        {
            yield return StartCoroutine(EmissionColorTo(redRenderer, Color.red, 0.2f));

            yield return new WaitForSeconds(3);

            yield return StartCoroutine(EmissionColorTo(redRenderer, Color.white, 0.2f));

            yield return StartCoroutine(EmissionColorTo(yellowRenderer, Color.yellow, 0.2f));

            yield return new WaitForSeconds(0.5f);

            yield return StartCoroutine(EmissionColorTo(yellowRenderer, Color.white, 0.2f));

            yield return StartCoroutine(EmissionColorTo(greenRenderer, Color.green, 0.2f));

            yield return new WaitForSeconds(3);

            yield return StartCoroutine(EmissionColorTo(greenRenderer, Color.white, 0.2f));
        }
    }

    IEnumerator EmissionColorTo(Renderer renderer, Color color, float duration)
    {
        Color emissionColor = renderer.materials[0].GetColor("_Color");
        Color startColor = emissionColor;
        float time = 0.0f;

        while (time < duration)
        {
            time += Time.deltaTime;

            emissionColor = Color.Lerp(startColor, color, time / duration);
            renderer.materials[0].SetColor("_Color", emissionColor);
            yield return null;
        }
    }
}
