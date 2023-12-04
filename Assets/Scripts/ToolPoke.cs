using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolPoke : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Use(Ray r)
    {
        if(Physics.SphereCast(r, 2f,out RaycastHit hit, 10f))
        {
            if (hit.collider.CompareTag("Alien"))
            {
                hit.collider.SendMessage("React");
            }
        }

    }
}
