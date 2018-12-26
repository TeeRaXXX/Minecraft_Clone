using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Texture2D BackgroundSlot;
    [SerializeField] private Texture2D CrossHair;
    [SerializeField] private RigidbodyFirstPersonController[] Controllers;

    public Block[,] InventoryItems;
    public int[,] InventoryNum;

    public static Block[] HotbarItems;
    public static int[] HotbarNum;

    private int Width = 9;
    private int Height = 6;

    public static bool showInventory = false;
    public static int SelectedSlot;

    private Block draggingItem;
    private int draggingItemNum;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        InventoryItems = new Block[Width, Height];
        InventoryNum = new int[Width, Height];

        HotbarItems = new Block[Width];
        HotbarNum = new int[Width];

        InventoryItems[0, 0] = BlockList.GetBlock("Dirt");  
        InventoryNum[0, 0] = 12;
        InventoryItems[3, 3] = BlockList.GetBlock("Stone");
        InventoryNum[3, 3] = 64;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            showInventory = !showInventory;

            foreach (var item in Controllers)
            {
                item.enabled = !showInventory;
            }

            if (showInventory)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            if (SelectedSlot + 1 > Width)
                SelectedSlot = 0;
            else
                SelectedSlot++;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            if (SelectedSlot - 1 < 0)
                SelectedSlot = Width - 1;
            else
                SelectedSlot--;
        }
    }

    private void OnGUI()
    {
        GUI.DrawTexture(new Rect(
            Screen.width / 2 - (CrossHair.width / 2),
            Screen.height / 2 - (CrossHair.height / 2),
            CrossHair.width,
            CrossHair.height), CrossHair);


        Event e = Event.current;
        float space = 5;

        if (showInventory)
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    int inventoryWidth = BackgroundSlot.width * Width;
                    int inventoryHeigth = BackgroundSlot.height * Height;

                    Rect offset = new Rect((Screen.width / 2) - (inventoryWidth / 2), (Screen.height / 2) - (inventoryHeigth / 2),
                        inventoryWidth, inventoryHeigth);
                    Rect slotPos = new Rect(offset.x + BackgroundSlot.width * x, offset.y + BackgroundSlot.height * y,
                        BackgroundSlot.width, BackgroundSlot.height);

                    GUI.DrawTexture(slotPos, BackgroundSlot);
                    Block b = InventoryItems[x, y];
                    int n = InventoryNum[x, y];

                    if (b != null)
                    {
                        Rect blockPos = new Rect(slotPos.x + (space / 2), slotPos.y + (space / 2), slotPos.width - space, slotPos.height - space);
                        GUI.DrawTexture(blockPos, b.ItemView);

                        GUI.Label(slotPos, n.ToString());

                        if (slotPos.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0 && draggingItem == null)
                        {
                            DragItem(x, y);
                            break;
                        }
                    }
                    if (slotPos.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0 && draggingItem != null)
                    {
                        MoveItem(x, y);
                        break;
                    }
                    if (slotPos.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 1)
                    {
                        SplitItem(x, y);
                        break;
                    }
                }
            }
        }
        
        ShowHotbarItems(e, space);
        ShowDruggingItem(e, space);
    }

    private void ShowDruggingItem(Event e, float space)
    {
        if (!showInventory)
            return;
        if (draggingItem != null)
        {
            GUI.DrawTexture(new Rect(e.mousePosition.x, e.mousePosition.y,
                BackgroundSlot.width - space, BackgroundSlot.height - space), draggingItem.ItemView);
            GUI.Label(new Rect(e.mousePosition.x + (BackgroundSlot.width - space) / 2, e.mousePosition.y + (BackgroundSlot.height - space) / 2,
                BackgroundSlot.width - space, BackgroundSlot.height - space), draggingItemNum.ToString());
        }
    }
    private void ShowHotbarItems(Event e, float space)
    {
        for (int x = 0; x < Width; x++)
        {
            int inventoryWidth = BackgroundSlot.width * Width;
            int inventoryHeigth = BackgroundSlot.height;

            Rect offset = new Rect();

            if (showInventory)
                offset = new Rect((Screen.width / 2) - (inventoryWidth / 2), (Screen.height / 2) + (inventoryHeigth + 100) - (inventoryHeigth / 2),
                    inventoryWidth, inventoryHeigth);
            else
                offset = new Rect((Screen.width / 2) - (inventoryWidth / 2), (Screen.height) - (inventoryHeigth + 20) - (inventoryHeigth / 2),
                    inventoryWidth, inventoryHeigth);

            Rect slotPos = new Rect(offset.x + BackgroundSlot.width * x, offset.y + BackgroundSlot.height * 0,
                BackgroundSlot.width, BackgroundSlot.height);

            if (SelectedSlot == x)
            {
                GUI.color = Color.red;
            }
            GUI.DrawTexture(slotPos, BackgroundSlot);
            GUI.color = Color.white;
            Block b = HotbarItems[x];
            int n = HotbarNum[x];

            if (b != null)
            {
                Rect blockPos = new Rect(slotPos.x + (space / 2), slotPos.y + (space / 2), slotPos.width - space, slotPos.height - space);
                GUI.DrawTexture(blockPos, b.ItemView);

                GUI.Label(slotPos, n.ToString());

                if (slotPos.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                {
                    DragHotbarItem(x);
                }
            }
            else
            {
                if (slotPos.Contains(e.mousePosition) && e.type == EventType.MouseDown && e.button == 0)
                {
                    MoveHotbarItem(x);
                }
            }
        }
    }

    private void DragItem(int x, int y)
    {
        if (draggingItem != null)
            return;
        draggingItem = InventoryItems[x, y];
        draggingItemNum = InventoryNum[x, y];

        InventoryItems[x, y] = null;
        InventoryNum[x, y] = 0;
    }
    private void MoveItem(int x, int y)
    {
        if (draggingItem == null)
            return;

        Block block = InventoryItems[x, y];
        int blockNum =  InventoryNum[x, y];

        if (block == null)
        {
            InventoryItems[x, y] = draggingItem;
            InventoryNum[x, y] = draggingItemNum;

            draggingItem = null;
            draggingItemNum = 0;
        }
        else if (block == draggingItem)
        {
            if (blockNum + draggingItemNum > block.BlockMaxStack)
            {
                int rest = InventoryNum[x, y] + draggingItemNum - block.BlockMaxStack;
                InventoryNum[x, y] = block.BlockMaxStack;

                draggingItemNum = rest; 
            }
            else
            {
                InventoryNum[x, y] += draggingItemNum;

                draggingItem = null;
                draggingItemNum = 0;
            }
        }
    }
    private void SplitItem(int x, int y)
    {
        Block block = InventoryItems[x, y];
        int blockNum = InventoryNum[x, y];
        
        if (draggingItem != null && block == draggingItem)
        {
            if (InventoryNum[x, y] + 1 > block.BlockMaxStack)
            {

            }
            else
            {
                InventoryNum[x, y]++;
                draggingItemNum--;
            }
        }
        else if (draggingItem == null)
        {
            if (blockNum / 2 <= 0)
                return;

            draggingItem = block;
            draggingItemNum = blockNum / 2;

            InventoryNum[x, y] -= draggingItemNum;
        }
        else if (draggingItem != null)
        {
            InventoryItems[x, y] = draggingItem;
            InventoryNum[x, y]++;
            draggingItemNum--;
        }

        if (draggingItemNum <= 0)
        {
            draggingItem = null;
        }
    }

    private void DragHotbarItem(int x)
    {
        if (draggingItem != null)
            return;
        draggingItem = HotbarItems[x];
        draggingItemNum = HotbarNum[x];

        HotbarItems[x] = null;
        HotbarNum[x] = 0;
    }
    private void MoveHotbarItem(int x)
    {
        if (draggingItem == null)
            return;

        Block block = HotbarItems[x];
        int blockNum = HotbarNum[x];

        if (block == null)
        {
            HotbarItems[x] = draggingItem;
            HotbarNum[x] = draggingItemNum;

            draggingItem = null;
            draggingItemNum = 0;
        }
        else if (block == draggingItem)
        {
            if (blockNum + draggingItemNum > block.BlockMaxStack)
            {
                int rest = HotbarNum[x] + draggingItemNum - block.BlockMaxStack;
                HotbarNum[x] = block.BlockMaxStack;

                draggingItemNum = rest;
            }
            else
            {
                HotbarNum[x] += draggingItemNum;

                draggingItem = null;
                draggingItemNum = 0;
            }
        }
    }
    private void SplitHotbarItem(int x)
    {
        Block block = HotbarItems[x];
        int blockNum = HotbarNum[x];

        if (draggingItem != null && block == draggingItem)
        {
            if (HotbarNum[x] + 1 > block.BlockMaxStack)
            {

            }
            else
            {
                HotbarNum[x]++;
                draggingItemNum--;
            }
        }
        else if (draggingItem == null)
        {
            if (blockNum / 2 <= 0)
            {
                HotbarNum[x] -= draggingItemNum;
            }
        }
        else if (draggingItem != null)
        {
            HotbarItems[x] = draggingItem;
            HotbarNum[x]++;
            draggingItemNum--;
        }

        if (draggingItemNum <= 0)
        {
            draggingItem = null;
        }
    }

    public void AddItem(Block b, int num)
    {
        for (int y = 0; y < Height + 1; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (y < Height)
                {
                    if (InventoryItems[x, y] == null)
                    {
                        InventoryItems[x, y] = b;
                        if (num > b.BlockMaxStack)
                            InventoryNum[x, y] = b.BlockMaxStack;
                        else
                            InventoryNum[x, y] = num;

                        return;
                    }
                    else if (InventoryItems[x, y] == b && InventoryNum[x, y] < b.BlockMaxStack)
                    {
                        if (num > b.BlockMaxStack)
                        {
                            int rest = num - b.BlockMaxStack;
                            InventoryItems[x, y] = b;
                            InventoryNum[x, y] = b.BlockMaxStack;

                            AddItem(b, rest);
                            return;
                        }
                        else
                        {
                            if (InventoryNum[x, y] + num > b.BlockMaxStack)
                            {
                                int rest = InventoryNum[x, y] + num - b.BlockMaxStack;

                                InventoryItems[x, y] = b;
                                InventoryNum[x, y] = b.BlockMaxStack;

                                AddItem(b, rest);
                            }
                            else
                            {
                                InventoryItems[x, y] = b;
                                InventoryNum[x, y] += num;
                            }

                            return;
                        }
                    }
                }
                else
                {
                    if (HotbarItems[x] == null)
                    {
                        HotbarItems[x] = b;
                        if (num > b.BlockMaxStack)
                            HotbarNum[x] = b.BlockMaxStack;
                        else
                            HotbarNum[x] = num;

                        return;
                    }
                    else if (HotbarItems[x] == b && HotbarNum[x] < b.BlockMaxStack)
                    {
                        if (num > b.BlockMaxStack)
                        {
                            int rest = num - b.BlockMaxStack;
                            HotbarItems[x] = b;
                            HotbarNum[x] = b.BlockMaxStack;

                            AddItem(b, rest);
                            return;
                        }
                        else
                        {
                            if (HotbarNum[x] + num > b.BlockMaxStack)
                            {
                                int rest = InventoryNum[x, y] + num - b.BlockMaxStack;

                                HotbarItems[x] = b;
                                HotbarNum[x] = b.BlockMaxStack;

                                AddItem(b, rest);
                            }
                            else
                            {
                                HotbarItems[x] = b;
                                HotbarNum[x] += num;
                            }

                            return;
                        }
                    }
                }
            }
        }
    }
    public Block GetSelectedItem()
    {
        return HotbarItems[SelectedSlot];
    }
    public static void SetBlock()
    {
        if (HotbarItems[SelectedSlot] == null) return;

        HotbarNum[SelectedSlot]--;
        if (HotbarNum[SelectedSlot] == 0)
        {
            HotbarItems[SelectedSlot] = null;
        }
    }
}
