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
    public GameObject planet;
    public float planetSize;
    public GameObject spaceShip;
    public Vector2 beltDistanceToPlanet;
    public Vector3 beltRange;
    public float beltHeight;
    public float minBeltDistance;
    public float maxBeltDistance;
    public Vector3 beltSpeed;
    public Vector3 beltTilt;
    public Vector3 planetTilt;

    public Material[] mat;
    public Mesh mesh;

    //helpvariables
    public float range;
    public Vector3 minRange, maxRange, minSizeAstr, maxSizeAstr;
    public Vector3 tempPosition;
    public Vector3 tempPosition2;
    public Vector3 tempAngle;
    public float minRotSpd, maxRotSpd;
    public float tempFloat;
    public GameObject randomSphere;

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
        tempPosition = new Vector3(0, 0, 0);
        tempPosition2 = new Vector3(0, 0, 0);
        planet.transform.localScale = new Vector3(planetSize, planetSize, planetSize);
        planet.transform.rotation = Quaternion.Euler(planetTilt);
        spaceShip.transform.position = new Vector3(-(maxBeltDistance + 40f), maxBeltDistance * .333f, maxBeltDistance * .333f);
        InitAsteroids();
        initMatrixArrayList();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMatrixArrayList();
        UpdateAsteroids();
        DrawAllAsteroids();
        UpdateSphericalMask();
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
        range = range * Mathf.Sqrt((float)numberOfAsteroids) + Mathf.Sqrt(.5f) * planetSize /* * .5f */+ Mathf.Sqrt(.5f) * beltDistanceToPlanet.x;
        minRange = new Vector3(planet.transform.position.x - range, (planet.transform.position.y - range) * beltHeight, planet.transform.position.z - range);
        maxRange = new Vector3(planet.transform.position.x + range, (planet.transform.position.y + range) * beltDistanceToPlanet.y, planet.transform.position.z + range);

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

            tempPosition = new Vector3(0, 0, Random.Range(minBeltDistance, maxBeltDistance));
            tempAngle = new Vector3(Random.Range(-180f * beltRange.x, 180f * beltRange.x), Random.Range(-180f * beltRange.y, 180f * beltRange.y), Random.Range(-180f * beltRange.z, 180f * beltRange.z));
            asteroids[i].position = RotatePointAroundPivot(tempPosition, planet.transform.position, tempAngle);
            //tempPosition2 = new Vector3(asteroids[i].position.x * beltTilt.x, asteroids[i].position.y * beltTilt.y, asteroids[i].position.z * beltTilt.z);
            //tempPosition2 = new Vector3(0, 0, asteroids[i].position.z);
            //asteroids[i].position = RotatePointAroundPivot(tempPosition, tempPosition2, beltTilt);
        }
    }

    public void UpdateAsteroids()
    {
        for (int i = 0; i < numberOfAsteroids; i++)
        {
            asteroids[i].rotation.eulerAngles += asteroids[i].rotationVelocity * Time.deltaTime;
            asteroids[i].position = RotatePointAroundPivot(asteroids[i].position, planet.transform.position, beltSpeed * Time.deltaTime);
        }
    }

    public void DrawAllAsteroids()
    {
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        foreach (Matrix4x4[] matrixArray in matrixArrayList)
        {
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                Graphics.DrawMeshInstanced(mesh, i, mat[i], matrixArray, matrixArray.Length, block, UnityEngine.Rendering.ShadowCastingMode.Off);
            }
        }
    }

    public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angles)
    {
        return Quaternion.Euler(angles) * (point - pivot) + pivot;
    }

    public void UpdateSphericalMask()
    {
        Shader.SetGlobalFloat("SphericalMask_Radius", 20f);
        Shader.SetGlobalFloat("SphericalMask_Softness", 0f);
        tempPosition = (spaceShip.transform.position - planet.transform.position).normalized * planetSize * .5f;
        Shader.SetGlobalVector("SphericalMask_Position", tempPosition);
        Debug.Log(tempPosition);
        randomSphere.transform.position = tempPosition;
    }
}
