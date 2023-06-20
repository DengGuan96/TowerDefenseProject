using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAstar : MonoBehaviour
{

    private Vector2 beginPos = Vector2.right * -1;
    private Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();
    List<AstarNode> list;
    public Material red;
    public Material yellow;
    public Material green;
    public Material normal;
    public GameObject path;
    // void Start ()
    // {
    //     AstarMgr.Instance.InitMapInfo(mapW, mapH);

    //     for (int i = 0; i < mapW; ++i)
    //     {
    //         for (int j = 0; j < mapH; ++j)
    //         {
    //             GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //             obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0);
    //             obj.name = i + "_" + j;

    //             cubes.Add(obj.name, obj);

    //             AstarNode node = AstarMgr.Instance.nodes[i, j];
    //             if (node.type == E_Node_Type.Stop)
    //             {
    //                 obj.GetComponent<MeshRenderer>().material = red;
    //             }
    //         }
    //     }
    // }

    void Update() 
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
                        GameObject[] pathlist = GameObject.FindGameObjectsWithTag("path");
                        for (int i = 0; i < list.Count; ++i)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = normal;
                            GameObject.Destroy(GameObject.FindWithTag("enemy"));
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().path[i] = null;
                            // GameObject.Destroy(pathlist[i]);
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().startpointIsCreated = false;
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().i = 0;
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
                            // GameObject pathInstance = Instantiate(path);
                            //pathInstance.transform.parent = cubes[list[i].x + "_" + list[i].y].transform;
                            // pathInstance.transform.position = cubes[list[i].x + "_" + list[i].y].transform.position - new Vector3(0, 0, 1f);
                            // GameObject.FindGameObjectWithTag("point").GetComponent<EnemyMove>().path[i] = pathInstance;
                        }
                    }
                    
                    
                    beginPos = Vector2.right * -1;
                }
            }
        }
    }
}
