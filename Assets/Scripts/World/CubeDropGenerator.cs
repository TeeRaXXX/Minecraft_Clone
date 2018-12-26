using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Rigidbody))]
public class CubeDropGenerator : MonoBehaviour
{

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangulos = new List<int>();
    List<Vector2> uvs = new List<Vector2>();
    List<Color> colors = new List<Color>();

    float TextureOffset = 1F / 24F;
    Mesh mesh;
    GameObject player;
    Vector3 takeCubeOffset = new Vector3(0, 1, 0);
    Block block = null;

    public void StartCube(Block b)
    {
        block = b;
        StartCoroutine(GenerateCube(b));
    }

    private IEnumerator GenerateCube(Block b)
    {
        AddCubeFront(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeBack(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeTop(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeBottom(0, -0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeRight(0, 0, 0, b);
        yield return new WaitForEndOfFrame();
        AddCubeLeft(0, 0, 0, b);

        mesh = new Mesh();
        yield return new WaitForEndOfFrame();
        mesh.vertices = vertices.ToArray();
        yield return new WaitForEndOfFrame();
        mesh.SetTriangles(triangulos.ToArray(), 0);
        mesh.uv = uvs.ToArray();
        yield return new WaitForEndOfFrame();
        mesh.colors = colors.ToArray();

        yield return new WaitForEndOfFrame();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<BoxCollider>().center = new Vector3(0.5f, 0.5f, 0.5f);
    }
    private void Update()
    {
        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player");

        if (Vector3.Distance(transform.position, player.transform.position - takeCubeOffset) < 1f)
        {
            player.GetComponent<PlayerInventory>().AddItem(block, 1);
            Destroy(gameObject);
        }
    }

    public void AddCubeFront(int x, int y, int z, Block b)
    {
        //x = x /*+ Mathf.FloorToInt(transform.position.x)*/;
        //y = y /*+ Mathf.FloorToInt(transform.position.y)*/;
        //z = z /*+ Mathf.FloorToInt(transform.position.z*/);

        z++;

        int offset = 1;
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));

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
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        
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
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureX, TextureOffset * b.TextureY));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, TextureOffset * b.TextureY));
        uvs.Add(new Vector2((TextureOffset * b.TextureX) + TextureOffset, (TextureOffset * b.TextureY) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureX, (TextureOffset * b.TextureY) + TextureOffset));
        
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
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, TextureOffset * b.TextureYBottom));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, TextureOffset * b.TextureYBottom));
        uvs.Add(new Vector2((TextureOffset * b.TextureXBottom) + TextureOffset, (TextureOffset * b.TextureYBottom) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXBottom, (TextureOffset * b.TextureYBottom) + TextureOffset));
        
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
        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(2 - offset + vertices.Count);

        triangulos.Add(4 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        
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
        triangulos.Add(2 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(1 - offset + vertices.Count);

        triangulos.Add(1 - offset + vertices.Count);
        triangulos.Add(3 - offset + vertices.Count);
        triangulos.Add(4 - offset + vertices.Count);

        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, TextureOffset * b.TextureYSide));
        uvs.Add(new Vector2((TextureOffset * b.TextureXSide) + TextureOffset, (TextureOffset * b.TextureYSide) + TextureOffset));
        uvs.Add(new Vector2(TextureOffset * b.TextureXSide, (TextureOffset * b.TextureYSide) + TextureOffset));
        
        vertices.Add(new Vector3(x + 0, y + 0, z + 0)); // 1
        vertices.Add(new Vector3(x - 0, y + 0, z + 1)); // 2
        vertices.Add(new Vector3(x - 0, y + 1, z + 1)); // 3
        vertices.Add(new Vector3(x + 0, y + 1, z + 0)); // 4
    }
}
