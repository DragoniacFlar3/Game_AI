using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameControls : MonoBehaviour
{
    [SerializeField] BreathFirstSearch bfsAlgo = null;
    [SerializeField] TextMeshProUGUI textCompo = null;
    [SerializeField] Transform cameraTransform = null;
    [SerializeField] float cameraRotSpeed = 100.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Space))
        { 
            bfsAlgo.isPause = !bfsAlgo.isPause;

            if (bfsAlgo.isPause)
                textCompo.text = "Simulation paused!";
            else
            {
                textCompo.text = "Simulation running!";
                bfsAlgo.ConductBFS();
            }
        }

        if (Input.GetKey(KeyCode.A))
        {
            cameraTransform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime * cameraRotSpeed);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            cameraTransform.RotateAround(Vector3.zero, Vector3.up, -Time.deltaTime * cameraRotSpeed);
        }
    }

    public void PathFound()
    {
        textCompo.text = "Path has been found!";
    }

    public void PathNotFound()
    {
        textCompo.text = "Path cannot be found!";
    }
}
