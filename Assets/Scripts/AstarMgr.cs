using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AstarMgr
{
    private static AstarMgr instance;

    public static AstarMgr Instance
    {
        get
        {
            if (instance == null)
                instance = new AstarMgr();
            return instance;
        }
    }
    private int mapW;
    private int mapH;
    public AstarNode[,]nodes;
    private List<AstarNode> openList = new List<AstarNode>();
    private List<AstarNode> closeList = new List<AstarNode>();

    public void InitMapInfo(int w, int h, Coord[] _dataArray)
    {
        this.mapW = w;
        this.mapH = h;
        // Debug.Log(mapW);

        nodes = new AstarNode[w, h];
        // for (int i = 0; i < w; ++i)
        // {
        //     for (int j = 0; j < h; ++j)
        //     {
        //         AstarNode node = new AstarNode(i, j, Random.Range(0, 100) < 20 ? E_Node_Type.Stop : E_Node_Type.Walk);
        //         nodes[i, j] = node;
        //     }
        // }

        for (int i = 0; i < _dataArray.Length; i++)
        {
            AstarNode node = new AstarNode(_dataArray[i].x, _dataArray[i].y, _dataArray[i].isStop ? E_Node_Type.Stop : E_Node_Type.Walk);
            nodes[_dataArray[i].x, _dataArray[i].y] = node;
            // Debug.Log(node.type + "_" + node.x + "_" + node.y);
        }
    }

    public List<AstarNode> FindPath(Vector2 startPos, Vector3 endPos)
    {
        Debug.Log(mapW);
        if (startPos.x < 0 || startPos.x >= mapW ||
            startPos.y < 0 || startPos.y >= mapH ||
            endPos.x < 0 || endPos.x >= mapW ||
            endPos.y < 0 || endPos.y >= mapH)
        {
            Debug.Log("out of range" + startPos + endPos + mapH + mapW);
            return null;
        }
        
        AstarNode start = nodes[(int)startPos.x, (int)startPos.y];
        AstarNode end = nodes[(int)endPos.x, (int)endPos.y];
        if (start.type == E_Node_Type.Stop ||
            end.type == E_Node_Type.Stop)
        {
            Debug.Log("start error");
            return null;
        }
        
        closeList.Clear();
        openList.Clear();

        start.father = null;
        start.f = 0;
        start.g = 0;
        start.h = 0;
        closeList.Add(start);
        
        while(true)
        {
            //FindNearlyNodeToOpenList(start.x - 1, start.y - 1, 1.4f, start, end);
            FindNearlyNodeToOpenList(start.x, start.y - 1, 1, start, end);
            //FindNearlyNodeToOpenList(start.x + 1, start.y - 1, 1.4f, start, end);
            FindNearlyNodeToOpenList(start.x - 1, start.y, 1, start, end);
            FindNearlyNodeToOpenList(start.x + 1, start.y, 1, start, end);
            //FindNearlyNodeToOpenList(start.x - 1, start.y + 1, 1.4f, start, end);
            FindNearlyNodeToOpenList(start.x, start.y + 1, 1, start, end);
            //FindNearlyNodeToOpenList(start.x + 1, start.y + 1, 1.4f, start, end);

            if (openList.Count == 0) 
            {
                Debug.Log("no way");
                return null;
            }

            openList.Sort(SortOpenList);

            closeList.Add(openList[0]);

            start = openList[0];
            openList.RemoveAt(0);

            if (start == end)
            {
                List<AstarNode> path = new List<AstarNode>();
                path.Add(end);
                while (end.father != null)
                {
                    path.Add(end.father);
                    end = end.father;
                }
                path.Reverse();
                
                return path;
            }
        }
    }

    private int SortOpenList(AstarNode a, AstarNode b)
    {
        if (a.f > b.f)
            return 1;
        else if (a.f == b.f)
            return 1;
        else
            return -1;
    }
    
    private void FindNearlyNodeToOpenList(int x, int y, float g, AstarNode father, AstarNode end)
    {
        if (x < 0 || x >= mapW ||
            y < 0 || y >= mapH)
            return;
        AstarNode node = nodes[x, y];
        if (node == null ||
            node.type == E_Node_Type.Stop ||
            closeList.Contains(node) ||
            openList.Contains(node) )
            return;
        node.father = father;
        node.g = father.g + g;
        node.h = Mathf.Abs(end.x - node.x) + Mathf.Abs(end.y - node.y);
        node.f = node.g + node.h;
        openList.Add(node);
    }
}

