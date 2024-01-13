using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MapManager : MonoBehaviour
{
    [SerializeField] GameObject wallPrefab = null;
    [SerializeField] GameObject seekerPrefab = null;
    [SerializeField] GameObject endpointPrefab = null;
    [SerializeField] GameObject visitedSpotPrefab = null;
    [SerializeField] GameObject wallParent = null;
    [SerializeField] GameObject seekerParent = null;
    [SerializeField] GameObject endpointParent = null;
    [SerializeField] GameObject visitedSpotParent = null;
    [SerializeField] int mapScriptIdx = 0;
    [SerializeField] List<MapScriptObj> mapSOLst;
    [SerializeField] Material pathMat = null;
    List<char[]> mapVal = null;
    Dictionary<(int, int), GameObject> idx2InstanceDic = new Dictionary<(int, int), GameObject>();

    const int tilePerScale = 10;
    int numTilesX;
    int numTilesZ;
    int totalTiles = 0;

    int startX;
    int startZ;

    public List<char[]> MapVal
    { get { return mapVal; } }
    public int NumTilesX
    { get { return numTilesX; } }
    public int NumTilesZ
    { get { return numTilesZ; } }
    public int StartX
    { get { return startX; } }
    public int StartZ
    { get { return startZ; } }

    // Start is called before the first frame update
    void Start()
    {
        //Read a map from somewhere
        ParseMapArray();

        //Set the scale of plane, scale 1 = 10 squares
        numTilesX = mapVal[0].Length;
        numTilesZ = mapVal.Count;
        totalTiles = numTilesX * numTilesZ;
        transform.localScale = new Vector3(mapVal[0].Length / (float)tilePerScale, 1.0f, mapVal.Count / (float)tilePerScale);
        idx2InstanceDic.Clear();

        //Generate the necessary cube walls
        Vector3 originOffset = new Vector3(-numTilesX / 2, 0f, -numTilesZ / 2);
        int countIdx = 1;
        for (int j = 0; j < numTilesZ; ++j)
        {
            for (int i = 0; i < numTilesX; ++i)
            {
                switch (mapVal[j][i])
                {
                    //Walls
                    case '1':
                        {
                            GameObject instance = Instantiate(wallPrefab, wallParent.transform);
                            instance.name = wallPrefab.name + countIdx++;
                            instance.transform.position = originOffset;
                            instance.transform.Translate(i, 0f, j);         //Translate to match index of wall
                            instance.transform.Translate(0.5f, 0f, 0.5f);   //Translate to offset wall depth
                        }
                        break;

                    //Start point - pathseeker
                    case 'S':
                        {
                            GameObject instance = Instantiate(seekerPrefab, seekerParent.transform);
                            instance.name = seekerPrefab.name;
                            instance.transform.position = originOffset;
                            instance.transform.Translate(i, 0f, j);         //Translate to match index of wall
                            instance.transform.Translate(0.5f, 0f, 0.5f);   //Translate to offset wall depth
                            startX = i;
                            startZ = j;
                        }
                        break;

                    //End point
                    case 'X':
                        {
                            GameObject instance = Instantiate(endpointPrefab, endpointParent.transform);
                            instance.name = endpointPrefab.name;
                            instance.transform.position = originOffset;
                            instance.transform.Translate(i, 0f, j);         //Translate to match index of wall
                            instance.transform.Translate(0.5f, 0f, 0.5f);   //Translate to offset wall depth
                        }
                        break;

                    //Nothing & undefined
                    case '0':
                    default:
                        break;
                }

            }
        }

        //Generate pathseeker on the plane
        //Make a button to process BFS
        //-- make processed tiles blue
        //-- when done, make path red
        //Make 2nd button to move pathseeker


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ParseMapArray()
    {
        if (mapScriptIdx < 0 || mapScriptIdx > mapSOLst.Count)
        {
            Debug.Log("mapScriptIdx is out of bounds");
            return;
        }
        List<string> mapContent = mapSOLst[mapScriptIdx].mapVal;
        if (mapContent.Count <= 0)
        {
            Debug.Log($"map with index {mapScriptIdx} is an empty map!");
            return;
        }

        //Get dimension of this map, and create our char array representing this map
        int numRow = mapContent.Count;
        int numCol = mapContent[0].Length;
        mapVal = new List<char[]>();

        //Convert each string row into char array and assign to our char arr map
        for (int i = 0; i < numRow; ++i)
        {
            mapVal.Add(mapContent[i].ToCharArray());
        }
    }

    //Responsible for creating visual cues on the map
    public void CreateBFSProgress(Queue<int> thisLoopVisitedStack)
    {
        int childCount = visitedSpotParent.transform.childCount;
        int i = childCount + 1;
        Vector3 originOffset = new Vector3(-numTilesX / 2, 0f, -numTilesZ / 2);
        

        while (thisLoopVisitedStack.Count != 0)
        { 
            int idx = thisLoopVisitedStack.Dequeue();
            int z = idx / numTilesX;
            int x = idx % numTilesX;
            float tz = (float)(z) + 0.5f; //Translate to offset
            float tx = (float)(x) + 0.5f;

            //Create fake floor at spot
            GameObject instance = Instantiate(visitedSpotPrefab, visitedSpotParent.transform);
            instance.name = visitedSpotPrefab.name + i++;
            instance.transform.position = originOffset;
            instance.transform.Translate(tx, 0.1f, tz);         //Translate to match index

            idx2InstanceDic[new(x, z)] = instance;
        }
    }

    public void FinishBFS(Stack<(int,int)> pathStack)
    {
        Vector3 originOffset = new Vector3(-numTilesX / 2, 0f, -numTilesZ / 2);

        while (pathStack.Count != 0)
        { 
            (int, int)tPath = pathStack.Pop();

            //Pick the instance at this pos
            GameObject instance = idx2InstanceDic[tPath];
            if (instance == null)
                Debug.LogError("Found path, but path hasn't been considered during BFS search");

            //Assign a different material to it
            Renderer re = instance.GetComponentInChildren<Renderer>();
            re.material = pathMat;
        }
    }
}
