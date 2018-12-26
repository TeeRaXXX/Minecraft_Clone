using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    public GameObject chunkPrefab;
    public Material grassMat;

    public static GameObject ChunkPrefab;
    public static int viewRange = Chunk.Width * 5;
    public static Material GrassMat;

    public GameObject BlockHighlight;

    public LayerMask lm;
    int gridSize = 2;

    bool once = true;

    private PlayerInventory pi;

    private void Awake()
    {
        GrassMat = grassMat;
        Physics.IgnoreLayerCollision(9, 10);
        ChunkPrefab = chunkPrefab;

        for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunk.Width)
        {
            for (float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunk.Height)
            {
                for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunk.Width)
                {
                    int xx = Mathf.FloorToInt(x / Chunk.Width) * Chunk.Width;
                    int zz = Mathf.FloorToInt(z / Chunk.Width) * Chunk.Width;
                    int yy = Mathf.FloorToInt(y / Chunk.Height) * Chunk.Height;

                    if (!Chunk.IsChunkExists(new Vector3(xx, yy, zz)))
                    {
                        Instantiate(chunkPrefab, new Vector3(xx, yy, zz), Quaternion.identity);
                    }
                }
            }
        }
    }

    void Update()
    {
        for (float x = transform.position.x - viewRange; x < transform.position.x + viewRange; x += Chunk.Width)
        {
            for (float y = transform.position.y - viewRange; y < transform.position.y + viewRange; y += Chunk.Height)
            {
                for (float z = transform.position.z - viewRange; z < transform.position.z + viewRange; z += Chunk.Width)
                {
                    int xx = Mathf.FloorToInt(x / Chunk.Width) * Chunk.Width;
                    int zz = Mathf.FloorToInt(z / Chunk.Width) * Chunk.Width;
                    int yy = Mathf.FloorToInt(y / Chunk.Height) * Chunk.Height;
        
                    if (!Chunk.IsChunkExists(new Vector3(xx, yy, zz)))
                    {
                        Instantiate(chunkPrefab, new Vector3(xx, yy, zz), Quaternion.identity);
                    }
                }
            }
        }

        BlockController();
        ChunkSpawn();
    }

    void ChunkSpawn()
    {
        if (Chunk.working)
            return;

        float lastDistance = 9999999f;
        Chunk c = null;

        Dictionary<Vector3, Chunk> copyC = Chunk.Chuncks;

        foreach (var dc in copyC)
        {
            float dis = Vector3.Distance(transform.position, dc.Value.pos);

            if (dis < lastDistance)
            {
                Chunk cc = dc.Value.gameObject.GetComponent<Chunk>();

                if (!cc.generatedMap)
                {
                    lastDistance = dis;
                    c = cc;
                }
            }
        }

        if (c != null)
        {
            c.StartFunction();
        }

    }

    void BlockController()
    {
        if (Chunk.Blockworking || PlayerInventory.showInventory) return;

        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 7f, lm))
        {
            Vector3 p = hit.point - hit.normal / 2;
            BlockHighlight.transform.position = new Vector3(Mathf.Floor(p.x) + 0.5f, Mathf.Floor(p.y) + 0.5f, Mathf.Floor(p.z) + 0.5f);

            if (Input.GetMouseButtonDown(0))
            {
                SetBlock(p, null);
            }
            if (Input.GetMouseButtonDown(1))
            {
                Block b = PlayerInventory.HotbarItems[PlayerInventory.SelectedSlot];
                if (b != null)
                {
                    p = hit.point + hit.normal / 2;
                    SetBlock(p, b);
                    PlayerInventory.SetBlock();
                }
            }
        }
        else
        {
            BlockHighlight.transform.position = new Vector3(0, -1000, 0);
        }
    }

    void SetBlock(Vector3 p, Block b)
    {
        Chunk chunck = Chunk.GetChunk(new Vector3(Mathf.FloorToInt(p.x), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z)));
        Vector3 localPos = chunck.transform.position - p;

        if ((Mathf.FloorToInt(localPos.x) * -1) == (Chunk.Width))
        {
            Chunk c = Chunk.GetChunk(new Vector3(Mathf.FloorToInt(p.x + 5), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z)));
            if (c == null)
                return;

            c.SetBlock(p + new Vector3(+1, 0, 0), b);
        }
        else
        {
            Chunk c = Chunk.GetChunk(new Vector3(Mathf.FloorToInt(p.x - 5), Mathf.FloorToInt(p.y), Mathf.FloorToInt(p.z)));
            if (c == null)
                return;

            c.SetBlock(p + new Vector3(+1, 0, 0), b);
        }

        chunck.SetBlock(p + new Vector3(+1, 0, 0), b);
    }
}
