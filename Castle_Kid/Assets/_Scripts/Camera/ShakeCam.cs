using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeCam : MonoBehaviour
{
    private Vector3 originalPos;
    private bool shaking = false;
    
    // Start before first update
    void Start()
    {
        originalPos = transform.localPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShakeCamera(0.5f, 0.05f, true, true);
        }
    }

    public void ShakeCamera(float duration, float strength, bool horizontal, bool vertical)
    {
        if (shaking)
        {
            return;
        }
        StartCoroutine(Shake(duration, strength, horizontal, vertical));
    }

    private IEnumerator Shake(float duration, float strength, bool horizontal, bool vertical)
    {
        originalPos = transform.localPosition;
        shaking = true;

        while (duration > 0)
        {
            Vector3 ShakeOffset = Vector3.zero;

            if (horizontal)
            {
                ShakeOffset.x = Random.Range(-1f, 1f) * strength;
            }

            if (vertical)
            {
                ShakeOffset.y = Random.Range(-1f, 1f) * strength;
            }
            
            transform.localPosition = originalPos + ShakeOffset;
            duration -= Time.deltaTime;
            yield return null;
        }
        
        transform.localPosition = originalPos;
        shaking = false;
    }
}
