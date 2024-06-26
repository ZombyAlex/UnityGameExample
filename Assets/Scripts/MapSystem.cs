using System.Collections;
using System.Collections.Generic;
using SWFServer.Data;
using SWFServer.Data.Entities;
using SWFServer.Data.Net;
using UnityEngine;
using VContainer.Unity;

public class MapObject
{
    public ushort id;
    public GameObject obj;
}


public class MapSystem : IInitializable
{
    private Map map;
    public bool IsMap => map != null;
    public Vector2w Size => map.Size;
    
    public Map Map => map;
    
    private Dictionary<Vector2w, List<MapObject>> objects = new Dictionary<Vector2w, List<MapObject>>();

    private List<Vector2w> viewGrid = new List<Vector2w>();


    private Dictionary<ushort, List<GameObject>> pool = new Dictionary<ushort, List<GameObject>>();

    private SceneData sceneData;
    private GameConfig config;
    private GameContent content;

    private GameObject gameObject;

    public MapSystem(SceneData sceneData, GameConfig config, GameContent content)
    {
        this.sceneData = sceneData;
        this.config = config;
        this.content = content;
    }

    public void Initialize()
    {
        map = new Map();
        gameObject = new GameObject("Map");
    }


    public bool IsMapPos(Vector2w p)
    {
        return map.IsMap(p);
    }

    public void ClearAll()
    {
        
        foreach (var obj in objects)
        {
            foreach (var mapObject in obj.Value)
            {
                GameObject.Destroy(mapObject.obj);
            }
        }

        map.SetFree(Vector2w.Zero, map.Size, 0, 0);

        objects.Clear();
    }

    public void UpdateMap(List<MapSector> sectors, Vector2w unitPos)
    {
        sceneData.StartCoroutine(DrawMap(sectors));

        for (int i = 0; i < viewGrid.Count; i++)
        {
            if (viewGrid[i].GetR(unitPos) > 2)
            {
                RemoveSector(viewGrid[i]);
                viewGrid.RemoveAt(i);
                i--;
            }
        }
    }

    
    private IEnumerator DrawMap(List<MapSector> sectors)
    {
        foreach (var s in sectors)
        {
            viewGrid.Add(s.GridPos);
        }
            
        foreach (var s in sectors)
        {
            var p = SWFServer.Data.Util.GridToMapPos(s.GridPos);
            for (int x = 0; x < s.Size.x; x++)
            {
                for (int y = 0; y < s.Size.y; y++)
                {
                    var pos = p + new Vector2w(x, y);
                    var cell = s.Cells[x, y];
                    map.SetCell(pos, cell);
                    map.UpdateFree(pos);
                    if (cell.Block != null)
                        map.SetFree(pos, Info.EntityInfo[cell.Block.Id].size, cell.Block.Id, 
                            cell.Block.Rotate?.Value ?? (byte)0);
                    DrawCell(pos);
                }
            }

            yield return null;
        }
    }
    

    private void RemoveSector(Vector2w posSector)
    {
        var p = SWFServer.Data.Util.GridToMapPos(posSector);

        for (int x = 0; x < GameConst.mapGrid; x++)
        {
            for (int y = 0; y < GameConst.mapGrid; y++)
            {
                var pos = p + new Vector2w(x, y);
                RemoveCell(pos);
                var cell = map[pos];
                if (cell != null)
                {
                    if (cell.Block != null)
                        map.SetFree(pos, Info.EntityInfo[cell.Block.Id].size, 0, cell.Block.Rotate?.Value ?? (byte)0);
                    map.SetCell(pos, null);
                    map.UpdateFree(pos);
                }
            }
        }
    }

    private void DrawCell(Vector2w p)
    {
        RemoveCell(p);

        var cell = map[p];
        if (cell != null)
        {
            if (cell.Floor == null)
            {
                AddObject(p, GetObject(cell.Ground.Id, ConvPos(p, Info.EntityInfo[cell.Ground.Id].size, 0), cell.Ground.Rotate), 
                    cell.Ground.Id);
            }
            if (cell.Block != null)
            {
                AddObject(p, GetObject(cell.Block.Id, ConvPos(p, Info.EntityInfo[cell.Block.Id].size, cell.Block.Rotate.Value), cell.Block.Rotate), 
                    cell.Block.Id);
                //if (cell.Block.UserId != null && cell.Block.UserId.Value == Data.instance.UserId)
                //    UIGame.instance.ShowRespawn(ConvPos(p));
            }
            if (cell.Floor != null)
            {
                AddObject(p, GetObject(cell.Floor.Id, ConvPos(p, Info.EntityInfo[cell.Floor.Id].size,0), cell.Floor.Rotate),
                    cell.Floor.Id);
            }
        }
    }

    private GameObject GetObject(ushort id, Vector3 pos, ComponentByte rotate)
    {
        Quaternion r = rotate == null ? Quaternion.identity : Quaternion.Euler(0, 90f * rotate.Value, 0);
        if (pool.ContainsKey(id) && pool[id].Count > 0)
        {
            var go = pool[id][0];
            pool[id].RemoveAt(0);
            go.transform.position = pos;
            go.transform.rotation = r;
            go.SetActive(true);
            return go;
        }
        else
        {
            var prefab = content.Block(id);
            var go = GameObject.Instantiate(prefab, pos, r, gameObject.transform);
            return go;
        }
    }

    private void RemoveCell(Vector2w p)
    {
        if (objects.ContainsKey(p))
        {
            var obj = objects[p];
            for (int i = 0; i < obj.Count; i++)
            {
                if (!pool.ContainsKey(obj[i].id))
                {
                    pool.Add(obj[i].id, new List<GameObject>());
                    
                }
                obj[i].obj.SetActive(false);
                pool[obj[i].id].Add(obj[i].obj);
            }

            objects.Remove(p);
        }
    }

    private void AddObject(Vector2w pos, GameObject go, ushort id)
    {
        if(!objects.ContainsKey(pos))
            objects.Add(pos, new List<MapObject>());

        objects[pos].Add(new MapObject(){id = id, obj = go});
    }

    public Vector3 ConvPos(Vector2w pos)
    {
        return new Vector3(pos.x * config.CellSize + config.CellSize / 2, 0, pos.y * config.CellSize + config.CellSize / 2);
    }

    public Vector3 ConvPos(Vector2w pos, Vector2w size, byte rotate)
    {
        if(rotate == 1 ||  rotate == 3)
            size.Swap();
        Vector2 offs = new Vector2(size.x / 2f, size.y / 2f);
        return new Vector3(pos.x * config.CellSize + config.CellSize * offs.x, 0, pos.y * config.CellSize + config.CellSize * offs.y);
    }


    public void UpdateMapCell(MapCell cell, Vector2w pos)
    {
        map.SetCell(pos, cell);
        map.UpdateFree(pos);
        DrawCell(pos);
    }

    public void UpdateMapCellLayer(Entity entity, Vector2w pos, EntityMapLayer layer)
    {
        map.SetEntity(pos, entity);
        DrawCell(pos);
    }

    public bool IsFloor(Vector2w pos)
    {
        if (!IsMapPos(pos)) return false;

        if(map[pos] == null) return false;

        return map[pos].Floor != null;
    }

    public bool IsMapRect(Vector2w pos)
    {
        for (int i = 0; i < viewGrid.Count; i++)
        {
            var p = SWFServer.Data.Util.GridToMapPos(viewGrid[i]);
            WRect rect = new WRect(p.x, p.y, GameConst.mapGrid, GameConst.mapGrid);
            if (rect.Contains(pos))
                return true;
        }

        return false;
    }

    
}
