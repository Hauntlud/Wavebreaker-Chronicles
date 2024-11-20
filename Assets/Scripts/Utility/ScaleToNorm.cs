using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleToNorm : MonoBehaviour
{
    public float minScale;
    public float maxScale;

    public void ScaleToNormFunction(float normScale)
    {
        float newScale = Mathf.Lerp(minScale, maxScale, normScale);
        gameObject.transform.localScale = new Vector3(newScale,newScale,newScale);
    }
}
