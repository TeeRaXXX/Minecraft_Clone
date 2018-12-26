using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public string BlockName;
    public int BlockID;
    public bool Transparent = false;
    public bool IsCollider = true;
    public int TextureX;
    public int TextureY;

    public Texture ItemView;

    public int TextureXSide;
    public int TextureYSide;

    public int TextureXBottom;
    public int TextureYBottom;

    public int BlockMaxStack;

    public bool BlockGlow;
    public Color BlockColor = Color.white;

    public Block()
    {
        BlockID = -1;
        Transparent = true;
    }

    public Block(string name, bool transparent, int tX, int tY)
    {
        BlockName = name;
        Transparent = transparent;
        BlockID = BlockList.Blocks.Count;
        TextureX = tX;
        TextureY = tY;
        TextureXSide = tX;
        TextureYSide = tY;
        TextureXBottom = tX;
        TextureYBottom = tY;
        ItemView = Resources.Load<Texture>(name) as Texture;
    }
    public Block(string name, bool transparent, int tX, int tY, int sX, int sY)
    {
        BlockName = name;
        Transparent = transparent;
        BlockID = BlockList.Blocks.Count;
        TextureX = tX;
        TextureY = tY;
        TextureXSide = sX;
        TextureYSide = sY;
        TextureXBottom = tX;
        TextureYBottom = tY;
        ItemView = Resources.Load<Texture>(name) as Texture;
    }
    public Block(string name, bool transparent, int tX, int tY, int sX, int sY, int bX, int bY)
    {
        BlockName = name;
        Transparent = transparent;
        BlockID = BlockList.Blocks.Count;
        TextureX = tX;
        TextureY = tY;
        TextureXSide = sX;
        TextureYSide = sY;
        TextureXBottom = bX;
        TextureYBottom = bY;
        ItemView = Resources.Load<Texture>(name) as Texture;
    }

    public void SetColor(Color color, bool Glow)
    {
        BlockColor = color;
        BlockGlow = Glow;
    }
    public void SetTextureView(Texture texture)
    {
        ItemView = texture;
    }
    public void SetMaxStack(int maxStack)
    {
        BlockMaxStack = maxStack;
    }
    public static Block GetBlock(string name)
    {
        foreach (Block b in BlockList.Blocks)
        {
            if (b.BlockName == name)
                return b;
        }

        return new Block();
    }
    public static Block GetBlock(int id)
    {
        foreach (Block b in BlockList.Blocks)
        {
            if (b.BlockID == id)
                return b;
        }

        return new Block();
    }
}