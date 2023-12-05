using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    Color SkinColour;
    float exampleTimer;
    Material skinMat;
    MeshRenderer meshRenderer;
    // Start is called before the first frame update
    void Start()
    {
        exampleTimer = 0; 
        
        meshRenderer = GetComponent<MeshRenderer>();
        skinMat = meshRenderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        // switch cases for level of the emtion
        if (calmness > 0.9)
        {
            //do smth
        } else if (calmness > 0.7)
        {
            //do smth else
        }



        skinMat.SetColor("_Color",SkinColour);
        exampleTimer -= Time.deltaTime;
        if (exampleTimer > 0)
        {
            SkinColour = new Color(255, 0, 0);

        }
        else
        {
            SkinColour = new Color(255, 255, 255);
        }
    }

    void React()
    {
        exampleTimer = 1.5f;
    }
}
