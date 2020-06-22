using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour
{
    [SerializeField]
    private float tumble;
    private Vector3 currentUnitsphere;

    void Start()
    {
        currentUnitsphere = Random.insideUnitSphere;
        GetComponent<Rigidbody>().angularVelocity = currentUnitsphere * tumble;
    }

    private void Update()
    {
        GetComponent<Rigidbody>().angularVelocity = currentUnitsphere * tumble;
    }
}