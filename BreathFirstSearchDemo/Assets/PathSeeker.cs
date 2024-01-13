using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathSeeker : MonoBehaviour
{
    GameObject childObj = null;
    SphereCollider sphereCollider = null;

    // Start is called before the first frame update
    void Start()
    {
        childObj = transform.GetChild(0).gameObject;
        sphereCollider = childObj.GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        //childObj.transform.Translate((Vector3.up * 0.001f));
    }
}
