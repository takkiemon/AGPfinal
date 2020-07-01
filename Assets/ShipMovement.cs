using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipMovement : MonoBehaviour
{
    public float shipSpeed;
    public RoundedCube roundedCubeScript;
    public BloomEffect bloom;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) // forward
        {
            gameObject.transform.Translate(Vector3.forward * Time.deltaTime * shipSpeed);
        }
        if (Input.GetKey(KeyCode.S)) // backward
        {
            gameObject.transform.Translate(Vector3.back * Time.deltaTime * shipSpeed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                gameObject.transform.Translate(Vector3.left * Time.deltaTime * shipSpeed);
            }
            else
            {
                gameObject.transform.Rotate(Vector3.down * Time.deltaTime * shipSpeed); // turn left
            }
        }
        if (Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                gameObject.transform.Translate(Vector3.right * Time.deltaTime * shipSpeed);
            }
            else
            {
                gameObject.transform.Rotate(Vector3.up * Time.deltaTime * shipSpeed); // turn right
            }
        }
        if (Input.GetKey(KeyCode.E)) // up
        {
            gameObject.transform.Translate(Vector3.up * Time.deltaTime * shipSpeed);
        }
        if (Input.GetKey(KeyCode.Q)) // down
        {
            gameObject.transform.Translate(Vector3.down * Time.deltaTime * shipSpeed);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            roundedCubeScript.IncreaseRoundness(1);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            roundedCubeScript.IncreaseRoundness(-1);
        }

        if (Input.GetKeyDown(KeyCode.PageUp)) // bloomier
        {
            bloom.iterations++;
        }
        if (Input.GetKeyDown(KeyCode.PageDown)) // less bloomier
        {
            bloom.iterations--;
        }
    }
}
