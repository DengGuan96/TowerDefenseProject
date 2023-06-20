using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MapGenerator : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject enemy;
    public Vector2 mapSize;
    public Transform mapHolder;
    [Range(0, 1)]public float outlinePercent;

    public GameObject obsPrefab;
    public List<Coord> allTilesCoord = new List<Coord>();

    private Queue<Coord> shuffledQueue;
    private int num = 0;

    private Vector2 beginPos = Vector2.right * -1;
    private Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();
    List<AstarNode> list;
    public Material red;
    public Material yellow;
    public Material green;
    public Material normal;
    public GameObject path;
    public GameObject[] way;
    public GameObject start;

    [Header("Map Fully Acessible")]
    [Range(0,1)] public float obsPercent;
    private Coord mapCenter;
    bool[,] mapObstacles;
    bool pathIsCreated;

    private void Start()
    {
        GenerateMap();
    }
    private void Update()
    {
        if (!pathIsCreated)
            FindPath();
        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("111");
            GameObject spawnTile = Instantiate(enemy, way[0].transform.position, Quaternion.identity);
        }
    }
    private void GenerateMap()
    {
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + i, 0, -mapSize.y / 2 + 0.5f + j);
                GameObject spawnTile = Instantiate(tilePrefab, newPos, Quaternion.Euler(90, 0, 0));
                spawnTile.name = i + "_" + j;
                cubes.Add(spawnTile.name, spawnTile);
                spawnTile.transform.SetParent(mapHolder);
                spawnTile.transform.localScale *= (1 - outlinePercent);

                allTilesCoord.Add(new Coord(num, i, j, false));
                num++;
            }
        }

        // for (int i = 0; i < obsCount; i++)
        // {
        //     Coord randomCoord = allTilesCoord[UnityEngine.Random.Range(0, allTilesCoord.Count)];

        //     Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + randomCoord.x, 0, -mapSize.y / 2 + 0.5f + randomCoord.y);
        //     GameObject spawnObs = Instantiate(obsPrefab, newPos, Quaternion.identity);
        //     spawnObs.transform.SetParent(mapHolder);
        //     spawnObs.transform.localScale *= (1 - outlinePercent);
        // }

        shuffledQueue = new Queue<Coord>(Utilities.ShuffleCoords(allTilesCoord.ToArray()));

        int obsCount = (int)(mapSize.x * mapSize.y * obsPercent);
        mapCenter = new Coord(num / 2, (int)mapSize.x / 2, (int)mapSize.y / 2, false);
        mapObstacles = new bool[(int)mapSize.x, (int)mapSize.y];

        int currentObsCount = 0;

        for (int i = 0; i < obsCount; i++)
        {
            Coord randomCoord = GetRandomCoord();

            mapObstacles[randomCoord.x, randomCoord.y] = true;
            currentObsCount++;

            if (randomCoord != mapCenter && MapIsFullyAccessible(mapObstacles, currentObsCount))
            {
                Vector3 newPos = new Vector3(-mapSize.x / 2 + 0.5f + randomCoord.x, 0.05f, -mapSize.y / 2 + 0.5f + randomCoord.y);
                GameObject spawnObs = Instantiate(obsPrefab, newPos, Quaternion.identity);
                spawnObs.transform.SetParent(mapHolder);
                // spawnObs.transform.localScale *= (1 - outlinePercent);
                spawnObs.transform.localScale = new Vector3(1 - outlinePercent, 0.1f, 1-outlinePercent);
                allTilesCoord[randomCoord.num] = new Coord(randomCoord.num, randomCoord.x, randomCoord.y, true);
            }
            else
            {
                mapObstacles[randomCoord.x, randomCoord.y] = false;
                currentObsCount--;
            }
        }
        AstarMgr.Instance.InitMapInfo((int)mapSize.x, (int)mapSize.y, allTilesCoord.ToArray());
    }

    private Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledQueue.Dequeue();
        shuffledQueue.Enqueue(randomCoord);
        return randomCoord;
    }

    private bool MapIsFullyAccessible(bool[,] _mapObstacles, int _currentObsCount)
    {
        bool[,] mapFlags = new bool[_mapObstacles.GetLength(0), _mapObstacles.GetLength(1)];//标记是否已被【检查】
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(mapCenter);
        mapFlags[mapCenter.x, mapCenter.y] = true;//中心点被标记为【检查】

        int accessibleTileCount = 1;//不含邮障碍物的瓦片的数量，一开始为1因为中心点肯定不是障碍物

        while(queue.Count > 0)
        {
            Coord currentTile = queue.Dequeue();

            for(int x = -1; x <= 1; x++)//遍历currentTile周边的四个相邻瓦片
            {
                for(int y = -1; y <= 1; y++)
                {
                    int neightbourX = currentTile.x + x;//这一步会遍历周围的八个
                    int neightbourY = currentTile.y + y;//这一步会遍历周围的八个

                    if(x == 0 || y == 0)//排除对角线
                    {
                        if(neightbourX >= 0 && neightbourX < _mapObstacles.GetLength(0) && neightbourY >= 0 && neightbourY < _mapObstacles.GetLength(1))//不出现在地图之外
                        {
                            //检查这个Tile是否我们已经检测过了 && 相邻的这个瓦片并没有obstacle瓦片
                            if(mapFlags[neightbourX, neightbourY] == false && _mapObstacles[neightbourX, neightbourY] == false)
                            {
                                mapFlags[neightbourX, neightbourY] = true;
                                queue.Enqueue(new Coord(accessibleTileCount, neightbourX, neightbourY, false));

                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - _currentObsCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    

    private void FindPath()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit info;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out info, 1000))
            {

                if (beginPos == Vector2.right * -1)
                {
                    
                    if (list != null)
                    {
                        // GameObject[] pathlist = GameObject.FindGameObjectsWithTag("path");
                        for (int i = 0; i < list.Count; ++i)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = normal;
                            // GameObject.Destroy(GameObject.FindWithTag("enemy"));
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().path[i] = null;
                            // GameObject.Destroy(pathlist[i]);
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().startpointIsCreated = false;
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().i = 0;
                            way[i] = null;
                        }
                    }

                    string[] strs = info.collider.gameObject.name.Split('_');
                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = yellow;
                }
                else
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    // if (beginPos.x <= endPos.x)
                    //     GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().isRight = true;
                    // else
                    //     GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().isRight = false;

                    list = AstarMgr.Instance.FindPath(beginPos, endPos);
                    cubes[(int)beginPos.x + "_" + (int)beginPos.y].GetComponent<MeshRenderer>().material = normal;
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; ++i)
                        {
                            //Debug.Log(list[i].x + "_" + list[i].y);
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = green;
                            GameObject pathInstance = Instantiate(path);
                            //pathInstance.transform.parent = cubes[list[i].x + "_" + list[i].y].transform;
                            pathInstance.transform.position = cubes[list[i].x + "_" + list[i].y].transform.position + new Vector3(0, 0.5f, 0);
                            way[i] = pathInstance;
                        }
                    }
                    
                    
                    beginPos = Vector2.right * -1;
                    GameObject startPoint = Instantiate(start, way[0].transform.position, Quaternion.identity);
                    GameObject.Find("GameManager").GetComponent<EnemySpawner>().START = startPoint.transform;
                    pathIsCreated = true;
                }
            }
        }
    }
}


[System.Serializable]
public struct Coord
{
    public int num;
    public int x;
    public int y;
    public bool isStop;
    public Coord(int _num, int _x, int _y, bool _isStop)
    {
        this.num = _num;
        this.x = _x;
        this.y = _y;
        this.isStop = _isStop;
    }

    public static bool operator == (Coord _c1, Coord _c2)
    {
        return (_c1.x == _c2.x) && (_c1.y == _c2.y);
    }

    public static bool operator !=(Coord _c1, Coord _c2)
    {
        return !(_c1 == _c2);
    }

}