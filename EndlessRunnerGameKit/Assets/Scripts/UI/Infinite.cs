using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;

public class Infinite : MonoBehaviour
{
    GameObject mainCam;
    int[] width = {165, 160, 185};
    private void Start()
    {
        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
    }

    private void Update()
    {
        Vector3 floor_position = transform.position;
        Vector3 Plane1 = GameObject.Find("Plane1").transform.position;
        Vector3 Plane2 = GameObject.Find("Plane2").transform.position;
        Vector3 Plane3 = GameObject.Find("Plane3").transform.position;
        int current = 0;
        if(floor_position.x == Plane1.x)
        {
            current = 1;
        }
        else if(floor_position.x == Plane2.x)
        {
            current = 2;
        }
        else if(floor_position.x == Plane3.x)
        {
            current = 3;
        }

    
        if(mainCam.transform.position.x <= floor_position.x + 100 &&
           mainCam.transform.position.x >= floor_position.x + 70)
        {
            if(Plane1.x <= floor_position.x && Plane2.x <= floor_position.x 
            && Plane3.x <= floor_position.x)
            {
                int randomNumber = Random.Range(1, 4);
                while(randomNumber == current)
                {
                    randomNumber = Random.Range(1, 4);
                }
                Debug.Log(randomNumber);
                if(randomNumber == 1)
                {
                    Plane1.x = floor_position.x + width[current - 1];   
                }
                else if(randomNumber == 2)
                {
                    Plane2.x = floor_position.x + width[current - 1];  
                }
                else
                {
                    Plane3.x = floor_position.x + width[current - 1];  
                }
            }
        }

        GameObject.Find("Plane1").transform.position = Plane1;
        GameObject.Find("Plane2").transform.position = Plane2;
        GameObject.Find("Plane3").transform.position = Plane3;  

    }
}
