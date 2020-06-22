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
    public Vector3 asteroidSize;
    public GameObject cameraObject;

    public Material mat;
    public Mesh mesh;

    public int counter = 0;

    //helpvariables
    public float range;
    public Vector3 minRange, maxRange;

    [System.Serializable]
    public struct AsteroidCube
    {
        public Vector3 position;
        public Quaternion rotation;
        public float scale;
    }

    // Start is called before the first frame update
    void Start()
    {
        asteroidSize = new Vector3(4f, 4f, 4f);
        InitAsteroids();
        initMatrixArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMatrixArrayList();
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
                asteroidMatrix[j].SetTRS(asteroids[i].position, cameraObject.transform.rotation, asteroidSize);
            }

            matrixArrayList.Add(asteroidMatrix);
        }
    }

    public void InitAsteroids()
    {
        asteroids = new AsteroidCube[numberOfAsteroids];

        minRange = new Vector3(cameraObject.transform.position.x - range, cameraObject.transform.position.y - range, cameraObject.transform.position.z - range);
        maxRange = new Vector3(cameraObject.transform.position.x + range, cameraObject.transform.position.y + range, cameraObject.transform.position.z + range);

        for (int i = 0; i < numberOfAsteroids; i++)
        {
            asteroids[i] = new AsteroidCube();
            asteroids[i].scale = asteroidSize.x;
            asteroids[i].position = new Vector3(Random.Range(minRange.x, maxRange.x), Random.Range(minRange.y, maxRange.y), Random.Range(minRange.z, maxRange.z));
            //Debug.Log(asteroids[i].position);
        }
    }

    public void DrawAllAsteroids()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        foreach (Matrix4x4[] matrixArray in matrixArrayList)
        {
            Debug.Log(matrixArray.Length);
            Graphics.DrawMeshInstanced(mesh, 0, mat, matrixArray, matrixArray.Length, block, UnityEngine.Rendering.ShadowCastingMode.Off);
        }
    }
}
