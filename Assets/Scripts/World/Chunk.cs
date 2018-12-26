using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using LibNoise;
using System.Threading;
using System.IO;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
[RequireComponent(typeof(MeshRenderer))]
public class Chunk : MonoBehaviour
{
    public Block[,,] map;
    public static int Width = 16, Height = 16;

    public static Dictionary<Vector3, Chunk> Chuncks = new Dictionary<Vector3, Chunk>();
    public static Dictionary<Vector3, Block> AdditionalBLocks = new Dictionary<Vector3, Block>();

    Chunk left;
    Chunk right;
    Chunk back;
    Chunk front;
    Chunk bottom;
    Chunk bottom2;
    Chunk top;
    
    private LibNoise.Generator.Perlin noise = new LibNoise.Generator.Perlin(1f, 1f, 1f, 8, GameManager.Seed, QualityMode.High);
    private LibNoise.Generator.Perlin biome = new LibNoise.Generator.Perlin(0.1f, 2f, 0.5f, 1, GameManager.Seed, QualityMode.High);

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    List<Vector3> noCollVertices = new List<Vector3>();
    List<int> noCollTriangles = new List<int>();
    List<Vector2> noCollUvs = new List<Vector2>();
    List<Color> noCollColors = new List<Color>();

    public Color shadowColors;
    public GameObject MeshNoColl;

    float TextureOffset = 1F / 24F;
    Mesh mesh;
    Mesh meshNoColl;

    public static bool working = false;
    public static bool Blockworking = false;
    public static bool SpawningChunks = false;
    public static bool SpawningChunksS = false;

    public bool ready = false;
    public bool generatedMap = false;
    public bool localWorking = false;
    public  bool localSpawningChunksS = false;

    public Vector3 pos;

    bool once = true;

    public GameObject Player;

    public static Thread tmap;

    void Start()
    {
        MeshNoColl = new GameObject("Mesh No Collision");
        MeshNoColl.transform.SetParent(transform);
        MeshNoColl.transform.localPosition = Vector3.zero;

        pos = transform.position;
        if (Chuncks.ContainsKey(pos))
        {
            Destroy(gameObject);
            return;
        }
        Chuncks.Add(pos, this);

        if (Time.time < 1)
        {
            StartFunction();
        }
    }
    void Update()
    {
        if (!ready && generatedMap)
        {
            if (left != null && right != null && front != null && back != null && bottom != null && top != null)
            {
                if (left.generatedMap && right.generatedMap && front.generatedMap && back.generatedMap && bottom.generatedMap && top.generatedMap)
                StartCoroutine(CalculateMesh());
            }
        }
        if (Player == null)
        {
            Player = GameObject.FindGameObjectWithTag("Player");
        }
        if (ready)
        {
            if (Player != null)
            {
                if (Vector3.Distance(this.transform.position, Player.transform.position) > PlayerController.viewRange)
                {
                    SaveToFile(this);
                    Chuncks.Remove(transform.position);
                    if (localWorking)
                        working = false;
                    if (localSpawningChunksS)
                        SpawningChunksS = false;
                    Destroy(this.gameObject);
                }
            }
        }
        if (SpawningChunksS || SpawningChunks || Time.time < 1f) return;

        if (left == null || right == null || front == null || back == null || bottom == null || top == null)
        {
            if (left == null)
            {
                int x = Mathf.FloorToInt((transform.position.x - Width) / Width) * Width;
                int y = Mathf.FloorToInt((transform.position.y) / Height) * Height;
                int z = Mathf.FloorToInt((transform.position.z) / Width) * Width;

                if (Vector3.Distance(new Vector3(x, y, z), Player.transform.position) <= PlayerController.viewRange)
                {
                    if (IsChunkExists(new Vector3(x, y, z)))
                    {
                        left = GetChunk(new Vector3(x, y, z));
                    }
                }
            }
            if (right == null)
            {
                int x = Mathf.FloorToInt((transform.position.x + Width) / Width) * Width;
                int y = Mathf.FloorToInt((transform.position.y) / Height) * Height;
                int z = Mathf.FloorToInt((transform.position.z) / Width) * Width;

                if (Vector3.Distance(new Vector3(x, y, z), Player.transform.position) <= PlayerController.viewRange)
                {
                    if (IsChunkExists(new Vector3(x, y, z)))
                    {
                        right = GetChunk(new Vector3(x, y, z));
                    }
                }
            }
            if (top == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Width) * Width;
                int y = Mathf.FloorToInt((transform.position.y + Height) / Height) * Height;
                int z = Mathf.FloorToInt((transform.position.z) / Width) * Width;

                if (Vector3.Distance(new Vector3(x, y, z), Player.transform.position) <= PlayerController.viewRange)
                {
                    if (IsChunkExists(new Vector3(x, y, z)))
                    {
                        top = GetChunk(new Vector3(x, y, z));
                    }
                }
            }
            if (bottom == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Width) * Width;
                int y = Mathf.FloorToInt((transform.position.y - Height) / Height) * Height;
                int z = Mathf.FloorToInt((transform.position.z) / Width) * Width;

                if (Vector3.Distance(new Vector3(x, y, z), Player.transform.position) <= PlayerController.viewRange)
                {
                    if (IsChunkExists(new Vector3(x, y, z)))
                    {
                        bottom = GetChunk(new Vector3(x, y, z));
                    }
                }
            }
            if (back == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Width) * Width;
                int y = Mathf.FloorToInt((transform.position.y) / Height) * Height;
                int z = Mathf.FloorToInt((transform.position.z - Width) / Width) * Width;

                if (Vector3.Distance(new Vector3(x, y, z), Player.transform.position) <= PlayerController.viewRange)
                {
                    if (IsChunkExists(new Vector3(x, y, z)))
                    {
                        back = GetChunk(new Vector3(x, y, z));
                    }
                }
            }
            if (front == null)
            {
                int x = Mathf.FloorToInt((transform.position.x) / Width) * Width;
                int y = Mathf.FloorToInt((transform.position.y) / Height) * Height;
                int z = Mathf.FloorToInt((transform.position.z + Width) / Width) * Width;

                if (Vector3.Distance(new Vector3(x, y, z), Player.transform.position) <= PlayerController.viewRange)
                {
                    if (IsChunkExists(new Vector3(x, y, z)))
                    {
                        front = GetChunk(new Vector3(x, y, z));
                    }
                }
            }
        }

    }

    public void StartFunction()
    {
        mesh = new Mesh();

        StartCoroutine(CalculateMap());
    }
    public void StartFunction(Block[,,] _map)
    {
        mesh = new Mesh();
        map = _map;
        StartCoroutine(CalculateMesh());
    }

    public void SaveToFile(Chunk c)
    {
        string[] blocks = File.ReadAllLines(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + c.pos.ToString());
        int i = 0;
        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (c.map[x, y, z] == null)
                        blocks[i] = "-1";
                    else
                        blocks[i] = c.map[x, y, z].BlockID.ToString();
                    i++;
                }
            }
        }
        File.WriteAllLines(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + c.pos.ToString(), blocks);
    }

    public IEnumerator SpawnChunk(Vector3 pos)
    {
        SpawningChunks = true;

        Instantiate(PlayerController.ChunkPrefab, pos, Quaternion.identity);

        yield return 0;

        SpawningChunks = false;
    }

    public IEnumerator CalculateMap()
    {
        while (working || localWorking)
        {
            yield return 0;
        }

        if (!Directory.Exists(Application.dataPath + "\\Worlds"))
        {
            Directory.CreateDirectory(Application.dataPath + "\\Worlds");
        }
        else
        {

        }
        if (tmap != null && tmap.IsAlive)
        {
        }
        else
        {
            bool createFile = true;
            if (File.Exists(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + pos.ToString()))
            {
                createFile = false;
            }
            else
            {

            }

            working = true;
            localWorking = true;

            if (!createFile)
            {
                map = new Block[Width, Height, Width];

                string[] lines = File.ReadAllLines(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + pos.ToString());

                int i = 0;
                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Height; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            int blockID = -1;
                            Block b = null;

                            blockID = int.Parse(lines[i]);
                            if (blockID == -1)
                                map[x, y, z] = b;
                            else
                                map[x, y, z] = Block.GetBlock(blockID);

                            i++;
                        }
                    }
                }
            }

            else if (createFile)
            {
                map = new Block[Width, Height, Width];
                
                while (tmap != null && tmap.IsAlive)
                {
                    yield return 0;
                }

                tmap = new Thread(CMap);
                tmap.Start();

                while (tmap.IsAlive)
                {
                    yield return 0;
                }

                tmap.Abort();

                if (!Directory.Exists(Application.dataPath + "\\Worlds\\" + GameManager.WorldName))
                {
                    Directory.CreateDirectory(Application.dataPath + "\\Worlds\\" + GameManager.WorldName);
                }

                if (!File.Exists(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + "seed"))
                {
                    StreamWriter sw = File.CreateText(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + "seed");
                    sw.Write(GameManager.Seed);
                    sw.Close();
                }

                File.Create(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + pos.ToString()).Close();
                List<string> lines = new List<string>();

                TextWriter tw = new StreamWriter(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + pos.ToString());

                for (int z = 0; z < Width; z++)
                {
                    for (int x = 0; x < Height; x++)
                    {
                        for (int y = 0; y < Height; y++)
                        {
                            if (map[x, y, z] == null)
                                tw.WriteLine("-1");
                            else
                                tw.WriteLine(map[x, y, z].BlockID);
                        }
                    }
                }

                tw.Close();
            }

            generatedMap = true;

            yield return 0;

            working = false;
            localWorking = false;
        }
    }
    public void CMap()
    {
        System.Random r = new System.Random();
        working = true;

        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                for (int z = 0; z < Width; z++)
                {
                    Block bloco = GetTheoreticalBlock(pos + new Vector3(x, y, z));
                    map[x, y, z] = bloco;

                    if (map[x, y, z] == Block.GetBlock("Dirt") && GetTheoreticalBlock(pos + new Vector3(x, y + 1, z)) == null)
                    {
                        map[x, y, z] = BlockList.GetBlock("Grass");
                        if (r.Next(0, 200) == 1)
                        {
                            CreateTree(pos + new Vector3(x, y + 1, z));
                        }
                        else if (r.Next(0, 50) == 1)
                        {
                            if (!AdditionalBLocks.ContainsKey(pos + new Vector3(x, y + 1, z)))
                                AdditionalBLocks.Add(pos + new Vector3(x, y + 1, z), Block.GetBlock("TallGrass"));
                        }
                    }
                    else if (map[x, y, z] == Block.GetBlock("Sand") && GetTheoreticalBlock(pos + new Vector3(x, y + 1, z)) == null)
                    {
                        if (r.Next(0, 400) == 1)
                        {
                            CreateCactus(pos + new Vector3(x, y + 1, z));
                        }
                    }
                }
            }
        }
    }

    public void CreateTree(Vector3 key)
    {
        System.Random r = new System.Random();
        int trunk = r.Next(5, 7);
        int leavesWidth = 6;

        for (int i = 0; i < trunk; i++)
        {
            Vector3 keyPos = key + new Vector3(0, i, 0);

            if (AdditionalBLocks.ContainsKey(keyPos))
                AdditionalBLocks[keyPos] = Block.GetBlock("Log");
            else
                AdditionalBLocks.Add(keyPos, Block.GetBlock("Log"));
        }

        for (int z = -(leavesWidth / 2); z < (leavesWidth / 2); z++)
        {
            for (int x = -(leavesWidth / 2); x < (leavesWidth / 2); x++)
            {
                for (int y = -(leavesWidth / 2); y < (leavesWidth / 2); y++)
                {
                    Vector3 keyPos = key + new Vector3(x, trunk + y, z);

                    if (Vector3.Distance(key + new Vector3(0, trunk, 0), keyPos) < leavesWidth / 2f)
                    {
                        if (AdditionalBLocks.ContainsKey(keyPos))
                            AdditionalBLocks[keyPos] = Block.GetBlock("Leaves");
                        else
                            AdditionalBLocks.Add(keyPos, Block.GetBlock("Leaves"));
                    }
                }
            }
        }
    }
    public void CreateCactus(Vector3 key)
    {
        System.Random r = new System.Random();
        int trunk = r.Next(2, 3);

        for (int i = 0; i < trunk; i++)
        {
            Vector3 keyPos = key + new Vector3(0, i, 0);
            if (AdditionalBLocks.ContainsKey(keyPos))
                AdditionalBLocks[keyPos] = Block.GetBlock("Cactus");
            else
                AdditionalBLocks.Add(keyPos, Block.GetBlock("Cactus"));
        }
    }

    public IEnumerator CalculateMesh()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {   
                for (int z = 0; z < Width; z++)
                {
                    if (AdditionalBLocks.ContainsKey(pos + new Vector3(x, y, z)))
                    {
                        map[x, y, z] = AdditionalBLocks[pos + new Vector3(x, y, z)];
                    }
                }
            }
        }
        
        SaveToFile(this);

        working = true;
        localWorking = true;

        mesh = new Mesh();
        meshNoColl = new Mesh();

        CMesh();

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.colors = colors.ToArray();
        mesh.uv = uvs.ToArray();

        meshNoColl.vertices = noCollVertices.ToArray();
        meshNoColl.triangles = noCollTriangles.ToArray();
        meshNoColl.colors = noCollColors.ToArray();
        meshNoColl.RecalculateNormals();
        meshNoColl.uv = noCollUvs.ToArray();
        
        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = mesh;

        MeshNoColl.AddComponent<MeshRenderer>();
        MeshNoColl.AddComponent<MeshFilter>();

        MeshNoColl.GetComponent<MeshFilter>().sharedMesh = meshNoColl;
        MeshNoColl.GetComponent<MeshRenderer>().material = PlayerController.GrassMat;

        MeshNoColl.AddComponent<MeshCollider>();
        MeshNoColl.layer = 11;

        Physics.IgnoreLayerCollision(8, 11);

        yield return new WaitForEndOfFrame();

        ready = true;
        working = false;
        localWorking = false;
    }
    public void CMesh()
    {
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        uvs.Clear();

        noCollVertices.Clear();
        noCollTriangles.Clear();
        noCollColors.Clear();
        noCollUvs.Clear();

        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[x, y, z] != null)
                    {
                        if (!map[x, y, z].IsCollider)
                        {
                            AddGrass(x, y, z, map[x, y, z]);
                        }
                        else
                        {
                            if (isBlockTransparent(x, y, z + 1))
                                AddCubeFront(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x, y, z - 1))
                                AddCubeBack(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x, y + 1, z))
                                AddCubeTop(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x, y - 1, z))
                                AddCubeBottom(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x + 1, y, z))
                                AddCubeRight(x, y, z, map[x, y, z]);
                            if (isBlockTransparent(x - 1, y, z))
                                AddCubeLeft(x, y, z, map[x, y, z]);
                        }
                    }
                }
            }
        }
    }

    public IEnumerator RecalculateMesh()
    {
        Blockworking = true;
        ready = true;

        mesh = new Mesh();
        vertices.Clear();
        triangles.Clear();
        colors.Clear();
        uvs.Clear();

        CMesh();

        meshNoColl = new Mesh();

        for (int z = 0; z < Width; z++)
        {
            for (int x = 0; x < Height; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (map[x, y, z] != null)
                    {
                        if (isBlockTransparent(x, y, z + 1))
                            AddCubeFront(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x, y, z - 1))
                            AddCubeBack(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x, y + 1, z))
                            AddCubeTop(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x, y - 1, z))
                            AddCubeBottom(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x + 1, y, z))
                            AddCubeRight(x, y, z, map[x, y, z]);
                        if (isBlockTransparent(x - 1, y, z))
                            AddCubeLeft(x, y, z, map[x, y, z]);
                    }
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.SetTriangles(triangles.ToArray(), 0);
        mesh.colors = colors.ToArray();
        mesh.uv = uvs.ToArray();

        meshNoColl.vertices = noCollVertices.ToArray();
        meshNoColl.triangles = noCollTriangles.ToArray();
        meshNoColl.colors = noCollColors.ToArray();
        meshNoColl.RecalculateNormals();
        meshNoColl.uv = noCollUvs.ToArray();

        MeshNoColl.AddComponent<MeshRenderer>();
        MeshNoColl.AddComponent<MeshFilter>();

        MeshNoColl.GetComponent<MeshFilter>().sharedMesh = meshNoColl;
        MeshNoColl.GetComponent<MeshRenderer>().material = PlayerController.GrassMat;

        MeshNoColl.AddComponent<MeshCollider>();
        MeshNoColl.layer = 11;

        Physics.IgnoreLayerCollision(8, 11);

        GetComponent<MeshCollider>().sharedMesh = mesh;
        GetComponent<MeshFilter>().sharedMesh = mesh;

        yield return 0;

        Blockworking = false;
        yield return 0;
    }

    public Block GetTheoreticalBlock(Vector3 pos)
    {
        System.Random r = new System.Random(GameManager.Seed);

        Vector3 offset = new Vector3((float)r.NextDouble() * 100000, (float)r.NextDouble() * 100000, (float)r.NextDouble() * 100000);

        double noiseX = Mathf.Abs((float)(pos.x + offset.x) / 20);
        double noiseY = Mathf.Abs((float)(pos.y + offset.y) / 20);
        double noiseZ = Mathf.Abs((float)(pos.z + offset.z) / 20);

        double noiseValue = noise.GetValue(noiseX, noiseY, noiseZ);
        double biomeValue = biome.GetValue(noiseX, 50, noiseZ);

        noiseValue += (200 - (float)pos.y) / 18f;
        noiseValue /= (float)pos.y / 8f;

        if (noiseValue > 0.5f)
        {
            if (noiseValue > 0.6f)
                return Block.GetBlock("Stone");
            if (biomeValue > 0.1f)
                return Block.GetBlock("Sand");
            else
                return Block.GetBlock("Dirt");
        }

        return null;
    }

    public void AddCubeFront(int x, int y, int z, Block b)
    {
        //x = x /*+ Mathf.FloorToInt(transform.position.x)*/;
        //y = y /*+ Mathf.FloorToInt(transform.position.y)*/;
        //z = z /*+ Mathf.FloorToInt(transform.position.z*/);

        z++;

        int offset = 1;
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(2 - offset + vertices.Count);
        triangles.Add(1 - offset + vertices.Count);

        triangles.Add(4 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

        CalculateLightFront(x, y, z, b);

        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeBack(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangles.Add(1 - offset + vertices.Count);
        triangles.Add(2 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);

        triangles.Add(1 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        CalculateLightBack(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x + -1, y + 0, z + 0)); // 2
        vertices.Add(new Vector3(x + -1, y + 1, z + 0)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeTop(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangles.Add(1 - offset + vertices.Count);
        triangles.Add(2 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);

        triangles.Add(1 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureX, TextureOffset * b.TextureY));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, TextureOffset * b.TextureY));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, (TextureOffset * b.TextureY) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureX, (TextureOffset * b.TextureY) + TextureOffset));
        CalculateLightTop(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }
    public void AddCubeBottom(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        y--;

        int offset = 1;
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(2 - offset + vertices.Count);
        triangles.Add(1 - offset + vertices.Count);

        triangles.Add(4 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, TextureOffset * b.TextureYBottom));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, TextureOffset * b.TextureYBottom));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, (TextureOffset * b.TextureYBottom) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, (TextureOffset * b.TextureYBottom) + TextureOffset));
        CalculateLightTop(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 1
        vertices.Add(new Vector3(x - 1, y + 1, z + 0)); // 2
        vertices.Add(new Vector3(x - 1, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 4
    }
    public void AddCubeRight(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        //y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        int offset = 1;
        triangles.Add(1 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(2 - offset + vertices.Count);

        triangles.Add(4 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        CalculateLightRight(x, y, z, b);
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
    public void AddCubeLeft(int x, int y, int z, Block b)
    {
        //x = x + Mathf.FloorToInt(transform.position.x);
        // y = y + Mathf.FloorToInt(transform.position.y);
        //z = z + Mathf.FloorToInt(transform.position.z);

        x--;

        int offset = 1;

        triangles.Add(2 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(1 - offset + vertices.Count);

        triangles.Add(1 - offset + vertices.Count);
        triangles.Add(3 - offset + vertices.Count);
        triangles.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

        CalculateLightLeft(x, y, z, b);

        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }

    void CalculateLightTop(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        {
            if (!InitialisBlockTransparent(x - 1, y + 1, z))
            {
                colors[index + 2] = shadowColors; ;
                colors[index + 1] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z))
            {
                colors[index + 0] = shadowColors; ;
                colors[index + 3] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors; ;
                colors[index + 0] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors; ;
                colors[index + 3] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z + 1))
            {
                colors[index + 3] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x - 1, y + 1, z - 1))
            {
                colors[index + 1] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x - 1, y + 1, z + 1))
            {
                colors[index + 2] = shadowColors; ;
            }

            if (!InitialisBlockTransparent(x + 1, y + 1, z - 1))
            {
                colors[index + 0] = shadowColors; ;
            }
        }
    }
    void CalculateLightRight(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x + 1, y - 1, z) && InitialisBlockTransparent(x + 1, y, z))
            {
                colors[index + 0] = shadowColors; ;
                colors[index + 1] = shadowColors; ;
            }
        }
    }
    void CalculateLightLeft(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x, y - 1, z) && InitialisBlockTransparent(x, y, z))
            {
                colors[index + 0] = shadowColors; ;
                colors[index + 1] = shadowColors; ;
            }
        }
    }
    void CalculateLightBack(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x, y - 1, z - 1) && InitialisBlockTransparent(x, y, z - 1))
            {
                colors[index + 0] = shadowColors; ;
                colors[index + 1] = shadowColors; ;
            }
        }
    }
    void CalculateLightFront(int x, int y, int z, Block b)
    {
        int index = colors.Count;

        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);
        colors.Add(b.BlockColor);

        //SideShadows
        {
            if (!InitialisBlockTransparent(x, y - 1, z) && InitialisBlockTransparent(x, y, z))
            {
                colors[index + 0] = shadowColors; ;
                colors[index + 1] = shadowColors; ;
            }
        }
    }

    public void AddGrass(int x, int y, int z, Block b)
    {
        x--;

        noCollTriangles.Add(1  + noCollVertices.Count);
        noCollTriangles.Add(2  + noCollVertices.Count);
        noCollTriangles.Add(0  + noCollVertices.Count);

        noCollTriangles.Add(0 + noCollVertices.Count);
        noCollTriangles.Add(2 + noCollVertices.Count);
        noCollTriangles.Add(3 + noCollVertices.Count);

        noCollTriangles.Add(0 + noCollVertices.Count);
        noCollTriangles.Add(2 + noCollVertices.Count);
        noCollTriangles.Add(1 + noCollVertices.Count);

        noCollTriangles.Add(3 + noCollVertices.Count);
        noCollTriangles.Add(2 + noCollVertices.Count);
        noCollTriangles.Add(0 + noCollVertices.Count);

        noCollTriangles.Add(4 + 0 + noCollVertices.Count);
        noCollTriangles.Add(4 + 2 + noCollVertices.Count);
        noCollTriangles.Add(4 + 1 + noCollVertices.Count);

        noCollTriangles.Add(4 + 3 + noCollVertices.Count);
        noCollTriangles.Add(4 + 2 + noCollVertices.Count);
        noCollTriangles.Add(4 + 0 + noCollVertices.Count);

        noCollTriangles.Add(4 + 1 + noCollVertices.Count);
        noCollTriangles.Add(4 + 2 + noCollVertices.Count);
        noCollTriangles.Add(4 + 0 + noCollVertices.Count);

        noCollTriangles.Add(4 + 0 + noCollVertices.Count);
        noCollTriangles.Add(4 + 2 + noCollVertices.Count);
        noCollTriangles.Add(4 + 3 + noCollVertices.Count);

        noCollUvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        noCollUvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        noCollUvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        noCollUvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

        noCollUvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        noCollUvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        noCollUvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        noCollUvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);
        noCollColors.Add(b.BlockColor);

        noCollVertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        noCollVertices.Add(new Vector3(x + 1, y + 0, z + 1)); // 2
        noCollVertices.Add(new Vector3(x + 1, y + 1, z + 1)); // 3
        noCollVertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4

        noCollVertices.Add(new Vector3(x + 1, y + 0, z + 0)); // 1
        noCollVertices.Add(new Vector3(x + 0, y + 0, z + 1)); // 2
        noCollVertices.Add(new Vector3(x + 0, y + 1, z + 1)); // 3
        noCollVertices.Add(new Vector3(x + 1, y + 1, z + 0)); // 4
    }

    bool isBlockTransparent(int x, int y, int z)
    {
        if (x >= Width || y >= Height || z >= Width || x < 0 || y < 0 || z < 0)
        {
            Vector3 chunk = pos + new Vector3(x, y, z);
            Chunk c = Chunk.GetChunk(chunk);

            Vector3 localPos = chunk - c.pos;

            if (c == null || c.map == null) return false;

            if (c.map[(int)localPos.x, (int)localPos.y, (int)localPos.z] == null || !c.map[(int)localPos.x, (int)localPos.y, (int)localPos.z].IsCollider)
                return true;
            return c.map[(int)localPos.x, (int)localPos.y, (int)localPos.z] == null;
        }
        else
        {
            if (map[x, y, z] == null || !map[x, y, z].IsCollider)
                return true;
            return map[x, y, z] == null;
        }
    }
    bool InitialisBlockTransparent(int x, int y, int z)
    {
        if (x >= Width || y >= Height || z >= Width
            || x < 0 || y < 0 || z < 0)
        {
            if (GetTheoreticalBlock(pos + new Vector3(x, y, z)) == null)
                return true;
            else
                return false;
        }

        if (map[x, y, z] == null)
            return true;

        if (map[x, y, z].Transparent)
            return true;

        return false;
    }

    public static Chunk GetChunk(Vector3 _pos)
    {
        int x = Mathf.FloorToInt(_pos.x / Width) * Width;
        int z = Mathf.FloorToInt(_pos.z / Width) * Width;
        int y = Mathf.FloorToInt(_pos.y / Height) * Height;

        if (Chuncks.ContainsKey(new Vector3(x, y, z)))
            return Chuncks[new Vector3(x, y, z)];
        return null;
    }
    public static bool IsChunkExists(Vector3 _pos)
    {
        int x = Mathf.FloorToInt(_pos.x / Width) * Width;
        int z = Mathf.FloorToInt(_pos.z / Width) * Width;
        int y = Mathf.FloorToInt(_pos.y / Height) * Height;

        if (Chuncks.ContainsKey(new Vector3(x, y, z)))
            return true;
        return false;
    }
    public void SetBlock(Vector3 worldPos, Block b)
    {
        Vector3 localPos;
        localPos = worldPos - transform.position;

        if (localPos.x > (Width))
        {
            return;
        }

        if (Mathf.FloorToInt(localPos.x) >= Width || Mathf.FloorToInt(localPos.y) >= Height || Mathf.FloorToInt(localPos.z) >= Width
            || Mathf.FloorToInt(localPos.x) < 0 || Mathf.FloorToInt(localPos.y) < 0 || Mathf.FloorToInt(localPos.z) < 0)
        {
        }
        else
        {
            if (b == null)
            {
                Block bm = map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
                if (bm != null)
                {
                    GameObject g = new GameObject("DropedI" + bm.BlockName) as GameObject;
                    CubeDropGenerator cdg = g.gameObject.AddComponent<CubeDropGenerator>();
                    cdg.StartCube(bm);

                    cdg.GetComponent<Renderer>().material = this.gameObject.GetComponent<Renderer>().material;
                    cdg.transform.position = worldPos - new Vector3(1, 0, 0);
                    cdg.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
                    cdg.gameObject.layer = 9;
                }
            }

            map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)] = b;

            string[] blocksSave = File.ReadAllLines(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + pos.ToString());
            int i = 0;
            for (int z = 0; z < Width; z++)
            {
                for (int x = 0; x < Height; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (x == Mathf.FloorToInt(localPos.x) && y == Mathf.FloorToInt(localPos.y) && z == Mathf.FloorToInt(localPos.z))
                        {
                            if (b == null)
                                blocksSave[i] = "-1";
                            else
                                blocksSave[i] = b.BlockID.ToString();
                        }
                        i++;
                    }
                }
            }
            File.WriteAllLines(Application.dataPath + "\\Worlds\\" + GameManager.WorldName + "\\" + pos.ToString(), blocksSave);
        }

        StartCoroutine(RecalculateMesh());

        if (b != null) return;
        if (Mathf.FloorToInt(localPos.x) >= Width - 1)
        {
            if (right == null)
            {
                right = GetChunk(new Vector3(Mathf.FloorToInt(worldPos.x + 1), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z)));
            }
            StartCoroutine(right.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.x) <= 1)
        {
            if (left == null)
            {
                left = GetChunk(new Vector3(Mathf.FloorToInt(worldPos.x - 1), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z)));
            }
            StartCoroutine(left.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.z) >= Width - 1)
        {
            if (front == null)
            {
                front = GetChunk(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z + 1)));
            }
            StartCoroutine(front.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.z) <= 1)
        {
            if (back == null)
            {
                back = GetChunk(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y), Mathf.FloorToInt(worldPos.z - 1)));
            }
            StartCoroutine(back.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.y) >= Height - 1)
        {
            if (top == null)
            {
                top = GetChunk(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y + 1), Mathf.FloorToInt(worldPos.z - 1)));
            }
            StartCoroutine(top.RecalculateMesh());
        }
        if (Mathf.FloorToInt(localPos.y) <= 1)
        {
            if (bottom == null)
            {
                bottom = GetChunk(new Vector3(Mathf.FloorToInt(worldPos.x), Mathf.FloorToInt(worldPos.y - 1), Mathf.FloorToInt(worldPos.z - 1)));
            }
            StartCoroutine(bottom.RecalculateMesh());
        }
    }
    public Block GetBlock(Vector3 worldPos)
    {
        Vector3 localPos = worldPos - transform.position;
        return map[Mathf.FloorToInt(localPos.x), Mathf.FloorToInt(localPos.y), Mathf.FloorToInt(localPos.z)];
    }
}   