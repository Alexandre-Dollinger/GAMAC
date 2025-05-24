using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Unity.Netcode;

public class FollowScript : NetworkBehaviour
{
    public float cameraSpeed = 7;
    public float yOffset = 3;
    private Vector3 posParent;
    private Vector3 newPos;
    private Quaternion myRotation;

    private Camera _myCamera;
    

    public override void OnNetworkSpawn()
    {
        _myCamera = GetComponent<Camera>();
        
        if (!IsOwner)
        {
            _myCamera.enabled = false;
        }
        else
        {
            myRotation = transform.rotation;
            newPos = transform.position;
        }
    }
    
    void Start()
    {
        originalPos = transform.localPosition;
    }


    void Update()
    {
        transform.rotation = myRotation;
        posParent = transform.parent.transform.position;

        newPos = Vector3.Slerp(new Vector3(newPos.x, newPos.y, -10), new Vector3(posParent.x, posParent.y, -10), cameraSpeed * Time.deltaTime);
        transform.position = new Vector3(newPos.x, newPos.y + yOffset, -10);
    }
    
    //ShakingCam
    //(https://youtu.be/iqujFS8mMcE?si=35MAj9QAxxR_2zhp)
    
    private Vector3 originalPos;
    private bool shaking = false;
    
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
