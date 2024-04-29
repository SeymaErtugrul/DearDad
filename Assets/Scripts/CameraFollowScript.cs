using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{

    public Transform target; // Karakterinizin transform component'i
    public float smoothing = 5f; // Kamera hareketinin ne kadar "smooth" (akıcı) olacağı

    Vector3 offset; 

    void Start()
    {
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        if (target.localScale.x > 0)
        {
            offset = new Vector3(Mathf.Abs(offset.x), offset.y, offset.z);
        }
        else if (target.localScale.x < 0)
        {
            offset = new Vector3(-Mathf.Abs(offset.x), offset.y, offset.z);
        }
        Vector3 targetCamPos = target.position + offset;

        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);
    }

}
