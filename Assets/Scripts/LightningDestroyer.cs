using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

[System.Serializable]
public class LightningDestroyer : MonoBehaviour
{
    [SerializeField]
    private float countdown = 12.0f;

    void Update()
    {
        if (countdown <= 0.0f)
        {
            Destroy(gameObject);
            countdown = 12.0f;
        }
        countdown -= Time.deltaTime;

    }
}
