using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sets random material for enemy
public class TumbleFrameSetter : MonoBehaviour
{
    private Renderer planeRenderer;
    float ranNum;
    void Start()
    {
        // Gets plane components
        planeRenderer = GetComponent<Renderer>();

        // Selects a material which does not match any of following excluded ones
        ranNum = Random.Range(1, 12);


        // Sets material to plane
        Material mat = Resources.Load<Material>(ranNum.ToString());
        planeRenderer.material = mat;
    }
}
