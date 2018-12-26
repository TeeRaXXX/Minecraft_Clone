using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockList : MonoBehaviour
{

    public static List<Block> Blocks = new List<Block>();

    private void Awake()
    {
        //Dirt Block
        Block dirt = new Block("Dirt", false, 2, 23);
        dirt.SetMaxStack(64);
        Blocks.Add(dirt);

        //Grass Block
        Block grass = new Block("Grass", false, 0, 23, 3, 23, 2, 23);
        grass.SetMaxStack(64);
        Blocks.Add(grass);

        //Tall Grass Block
        Block tallGrass = new Block("TallGrass", false, 8, 21);
        tallGrass.SetMaxStack(64);
        tallGrass.IsCollider = false;
        Blocks.Add(tallGrass);

        //Stone Block
        Block stone = new Block("Stone", false, 1, 23);
        stone.SetMaxStack(64);
        Blocks.Add(stone);

        //Sand Block
        Block sand = new Block("Sand", false, 2, 22);
        sand.SetMaxStack(64);
        Blocks.Add(sand);

        //Log Block
        Block log = new Block("Log", false, 5, 22, 4, 22, 5, 22);
        log.SetMaxStack(64);
        Blocks.Add(log);

        //Log Block
        Block leaves = new Block("Leaves", false, 5, 20);
        leaves.SetMaxStack(64);
        Blocks.Add(leaves);

        //Log Cactus
        Block cactus = new Block("Cactus", false, 5, 19, 6, 19, 7, 19);
        cactus.SetMaxStack(64);
        Blocks.Add(cactus);
    }

    public static Block GetBlock(string name)
    {
        foreach (Block b in Blocks)
        {
            if (b.BlockName == name)
                return b;
        }

        return null;
    }

}
