using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteFlashFeedback : MonoBehaviour
{
    SpriteRenderer sr;
    Color original;
    Coroutine routine;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        original = sr.color;
    }

    public void Flash(Color color, float duration)
    {
        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(FlashRoutine(color, duration));
    }

    System.Collections.IEnumerator FlashRoutine(Color color, float duration)
    {
        sr.color = color;
        yield return new WaitForSeconds(duration);
        sr.color = original;
        routine = null;
    }
}
