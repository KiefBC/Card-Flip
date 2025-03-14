using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 0.1f;
    
    private Vector3 initialPosition;
    private bool isShaking = false;
    
    private void Awake()
    {
        initialPosition = transform.localPosition;
    }
    
    public void ShakeCamera()
    {
        if (!isShaking)
        {
            StartCoroutine(Shake());
        }
    }
    
    private IEnumerator Shake()
    {
        isShaking = true;
        float elapsedTime = 0f;
        
        while (elapsedTime < shakeDuration)
        {
            Vector3 randomOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.localPosition = initialPosition + randomOffset;
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = initialPosition;
        isShaking = false;
    }
}