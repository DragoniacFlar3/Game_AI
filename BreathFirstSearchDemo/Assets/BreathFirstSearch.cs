using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BreathFirstSearch : MonoBehaviour
{
    MapManager mapMgr = null;
    Stack<(int, int)> pathStack = new Stack<(int, int)>();
    [SerializeField] GameControls gameCtrl = null;

    [HideInInspector]
    public bool isPause = false;
    [HideInInspector]
    public bool isFirstRun = true;

    // Start is called before the first frame update
    void Start()
    {
        mapMgr = GetComponent<MapManager>();
        if (mapMgr == null)
            Debug.LogError("BreathFirstSearch: cannot find map manager script");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConductBFS()
    {
        if (isFirstRun)
        {
            isFirstRun = false;
            StartCoroutine(SlowBFS());
        }
    }

    IEnumerator SlowBFS()
    {
        Queue<(int, int)> toVisitStack = new Queue<(int, int)>();                   //1st int -> parent, 2nd int -> child
        Dictionary<int, int> visitedToParentMap = new Dictionary<int, int>();       //key -> pos on map, val -> parent pos on map
        Queue<int> thisLoopVisitedStack = new Queue<int>();
        int w = mapMgr.NumTilesX;
        int h = mapMgr.NumTilesZ;
        int sX = mapMgr.StartX;
        int sZ = mapMgr.StartZ;

        pathStack.Clear();

        //Start process
        int t = sZ * w + sX;
        toVisitStack.Enqueue((-1, t));

        int endIdx = -1;
        while (toVisitStack.Count != 0)
        {
            if (isPause)
            {
                yield return new WaitForSeconds(2.0f);
                continue;
            }

            //Take 1 from queue, if not already checked, remember its parent
            (int, int) idx = toVisitStack.Dequeue();
            if (visitedToParentMap.ContainsKey(idx.Item2))
                continue;
            visitedToParentMap[idx.Item2] = idx.Item1;

            //Calculate its pos on a 1d array index
            int y = idx.Item2 / w;
            int x = idx.Item2 % w;

            //Check what is on this pos
            if (mapMgr.MapVal[y][x] == '1')
                continue;
            if (mapMgr.MapVal[y][x] == 'X')
            {
                endIdx = idx.Item2;
                break;
            }

            //Only make for non-walls, non-endpoint
            thisLoopVisitedStack.Enqueue(idx.Item2);

            //Considering neighbours
            for (int i = -1; i < 2; i += 2)
            {
                int n1 = y + i;
                int n11 = n1 * w + x;
                int n2 = x + i;
                int n22 = y * w + n2;

                if (n1 >= 0 && n1 < h && !visitedToParentMap.ContainsKey(n11))
                {
                    toVisitStack.Enqueue((idx.Item2, n11));
                }
                if (n2 >= 0 && n2 < w && !visitedToParentMap.ContainsKey(n22))
                {
                    toVisitStack.Enqueue((idx.Item2, n22));
                }
            }

            mapMgr.CreateBFSProgress(thisLoopVisitedStack); //Call map manager to make visuals
            yield return new WaitForSeconds(0.5f);
        }

        //If never found the end point
        if (endIdx == -1)
        {
            gameCtrl.PathNotFound();
            yield break;
        }

        //Calculate length of path found
        int len = 0;
        while (visitedToParentMap[endIdx] != -1)
        {
            len++;
            endIdx = visitedToParentMap[endIdx];

            int y = endIdx / w;
            int x = endIdx % w;

            pathStack.Push((x, y));
        }
        mapMgr.FinishBFS(pathStack);
        gameCtrl.PathFound();
    }
}