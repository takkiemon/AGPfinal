using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManager : MonoBehaviour
{
    private const int MAX_NUMBER_OF_ARRAYS = 1023; // each array has a maximum of 1023 matrices

    public int numberOfAsteroids;
    [SerializeField]public AsteroidCube[] asteroids;
    private List<Matrix4x4[]> matrixArrayList = new List<Matrix4x4[]>(); // each array has a maximum of 1023 matrices
    private Matrix4x4[] asteroidMatrix;
    public GameObject cameraObject;

    public Material mat;
    public Mesh mesh;

    //helpvariables
    public float range;
    public Vector3 minRange, maxRange, minSizeAstr, maxSizeAstr;
    public float minRotSpd, maxRotSpd;

    [System.Serializable]
    public struct AsteroidCube
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 rotationVelocity;
        public Vector3 dimensions;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitAsteroids();
        initMatrixArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMatrixArrayList();
        UpdateAsteroids();
        DrawAllAsteroids();
    }

    void initMatrixArrayList()
    {
        int numberOfLists = (int)Mathf.Ceil(numberOfAsteroids / MAX_NUMBER_OF_ARRAYS);
        for (int i = 0; i < numberOfLists; i++)
        {
            if (i == numberOfLists - 1)
                matrixArrayList.Add(new Matrix4x4[numberOfAsteroids % MAX_NUMBER_OF_ARRAYS]); // the last array has less than or equals MAX_NUMBER_OF_ARRAYS (1023)
            else
                matrixArrayList.Add(new Matrix4x4[MAX_NUMBER_OF_ARRAYS]);
        }
        UpdateMatrixArrayList();
    }

    void UpdateMatrixArrayList()
    {
        matrixArrayList.Clear();
        for (int i = 0, list = 0; i < asteroids.Length; list++)
        {
            if (i + MAX_NUMBER_OF_ARRAYS >= asteroids.Length)
            {
                asteroidMatrix = new Matrix4x4[asteroids.Length % MAX_NUMBER_OF_ARRAYS]; // the last array has less than or equals MAX_NUMBER_OF_ARRAYS (1023)
            }
            else
            {
                asteroidMatrix = new Matrix4x4[MAX_NUMBER_OF_ARRAYS];
            }

            for (int j = 0; j < asteroidMatrix.Length; j++, i++)
            {
                asteroidMatrix[j].SetTRS(asteroids[i].position, asteroids[i].rotation, asteroids[i].dimensions);
            }

            matrixArrayList.Add(asteroidMatrix);
        }
    }

    public void InitAsteroids()
    {
        asteroids = new AsteroidCube[numberOfAsteroids];

        minRange = new Vector3(cameraObject.transform.position.x - range * Mathf.Sqrt((float)numberOfAsteroids), cameraObject.transform.position.y - range * Mathf.Sqrt((float)numberOfAsteroids), cameraObject.transform.position.z - range * Mathf.Sqrt((float)numberOfAsteroids));
        maxRange = new Vector3(cameraObject.transform.position.x + range * Mathf.Sqrt((float)numberOfAsteroids), cameraObject.transform.position.y + range * Mathf.Sqrt((float)numberOfAsteroids), cameraObject.transform.position.z + range * Mathf.Sqrt((float)numberOfAsteroids));

        for (int i = 0; i < numberOfAsteroids; i++)
        {
            asteroids[i] = new AsteroidCube();
            asteroids[i].dimensions = new Vector3(Random.Range(minSizeAstr.x, maxSizeAstr.x), Random.Range(minSizeAstr.y, maxSizeAstr.y), Random.Range(minSizeAstr.z, maxSizeAstr.z));
            asteroids[i].rotation = Random.rotation;
            asteroids[i].rotationVelocity = new Vector3(
                Random.Range(minRotSpd, maxRotSpd), 
                Random.Range(minRotSpd, maxRotSpd),
                Random.Range(minRotSpd, maxRotSpd)
                );
            asteroids[i].position = new Vector3(Random.Range(minRange.x, maxRange.x), 0f, Random.Range(minRange.z, maxRange.z));
        }
    }

    public void UpdateAsteroids()
    {
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            asteroids[i].rotation.eulerAngles += new Vector3(
                asteroids[i].rotationVelocity.x * Time.deltaTime, 
                asteroids[i].rotationVelocity.y * Time.deltaTime, 
                asteroids[i].rotationVelocity.z * Time.deltaTime
                );
            //asteroids[i].position = new Vector3(Random.Range(minRange.x, maxRange.x), 0f, Random.Range(minRange.z, maxRange.z));
        }
    }

    public void DrawAllAsteroids()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        foreach (Matrix4x4[] matrixArray in matrixArrayList)
        {
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrixArray, matrixArray.Length, block, UnityEngine.Rendering.ShadowCastingMode.Off);
        }
    }
}
