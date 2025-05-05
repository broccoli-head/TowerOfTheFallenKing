using UnityEngine;

public class BackgroundShake : MonoBehaviour
{
    public float shakeStrength;
    public float shakeSpeed;
    private Vector3 initialPosition;

    void Start()
    {
        //oryginalna pozycja
        initialPosition = transform.localPosition;
    }

    void Update()
    {
        //perlin noice aby efekt by³ p³ynny
        float x = (Mathf.PerlinNoise(Time.time * shakeSpeed, 0f) - 0.5f) * 2f;
        float y = (Mathf.PerlinNoise(0f, Time.time * shakeSpeed) - 0.5f) * 2f;

        Vector3 shakeOffset = new Vector3(x, y, 0f) * shakeStrength;
        transform.localPosition = initialPosition + shakeOffset;
    }
}
