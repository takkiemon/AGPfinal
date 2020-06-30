using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetBehavior : MonoBehaviour
{
    public GameObject core;
    public GameObject clouds;
    public float planetRotationSpeed;
    public float cloudRotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotatePlanet();
    }

    public void RotatePlanet()
    {
        core.transform.Rotate(new Vector3(0, planetRotationSpeed * Time.deltaTime, 0));
        clouds.transform.Rotate(new Vector3(0, cloudRotationSpeed * Time.deltaTime, 0));
    }
}
