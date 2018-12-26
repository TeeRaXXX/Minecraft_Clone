using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;
using System.IO;

public class Menu : MonoBehaviour
{

    [Header("MenuButtons")]
    [SerializeField] private Button CreateWorldButtonMenu;
    [SerializeField] private Button LoadWorldButtonMenu;
    [SerializeField] private Button SettingsButtonMenu;
    [SerializeField] private Button ExitButtonMenu;

    [Header("ActionButtons")]
    [SerializeField] private Button CreateWorldButton;
    [SerializeField] private Button CWBackToMenuButton;
    [SerializeField] private Button LWBackToMenuButton;


    [Header("InputFields")]
    [SerializeField] private InputField CWWorldName;
    [SerializeField] private InputField CWWorldSeed;

    [Header("Texts")]
    [SerializeField] private Text CWErrorText;

    [Header("Panels")]
    [SerializeField] private GameObject MainMenuPanel;
    [SerializeField] private GameObject CreateWorldPanel;
    [SerializeField] private GameObject LoadWorldPanel;
    [SerializeField] private GameObject NETPanel;

    [Header("Content")]
    [SerializeField] private GameObject Content;
    [SerializeField] private GameObject WorldUIPrefab;

    private GameObject activePanel;

    private void Start()
    {
        CWErrorText.text = "";
        CreateWorldButtonMenu.onClick.AddListener(() => GoToCreateWorld());
        LoadWorldButtonMenu.onClick.AddListener(() => GoToLoadWorld());

        ExitButtonMenu.onClick.AddListener(() => Exit());

        CreateWorldButton.onClick.AddListener(() => CreateWorld(CWWorldName.text, CWWorldSeed.text));
        CWBackToMenuButton.onClick.AddListener(() => BackToMenu());
        LWBackToMenuButton.onClick.AddListener(() => BackToMenu());
    }

    private void GoToCreateWorld()
    {
        activePanel = CreateWorldPanel;

        MainMenuPanel.SetActive(false);
        CreateWorldPanel.SetActive(true);
    }

    private void GoToLoadWorld()
    {
        activePanel = LoadWorldPanel;
    
        MainMenuPanel.SetActive(false);
        LoadWorldPanel.SetActive(true);
        
        if (!Directory.Exists(Application.dataPath + "\\Worlds"))
        {
            return;
        }
        else
            UpdateWorldList();
    }

    private void BackToMenu()
    {
        activePanel.SetActive(false);
        MainMenuPanel.SetActive(true);
    }

    private void Exit()
    {
        Application.Quit();
    }

    private void CreateWorld(string name, string seed)
    {
        if (!Directory.Exists(Application.dataPath + "\\Worlds"))
        {
            Directory.CreateDirectory(Application.dataPath + "\\Worlds");
        }
        if (Directory.Exists(Application.dataPath + "\\Worlds\\" + name))
        {
            CWErrorText.text = "World with name " + name + " already exists";
            return;
        }

        int _seed = Convert.ToInt32(seed);

        GameManager.Seed = _seed;
        GameManager.WorldName = name;
        GameManager.SetGame(true, true);

        SceneManager.LoadScene(1);
    }

    private void UpdateWorldList()
    {
        if (!Directory.Exists(Application.dataPath + "\\Worlds"))
        {
            Directory.CreateDirectory(Application.dataPath + "\\Worlds");
        }

        for (int i = 0; i < Content.transform.childCount; i++)
        {
            Destroy(Content.transform.GetChild(i).gameObject);
        }

        string[] ds = Directory.GetDirectories(Application.dataPath + "\\Worlds");

        if (ds.Length > 0)
        {
            foreach (var dir in ds)
            {
                Debug.Log(dir);
                GameObject world = Instantiate(WorldUIPrefab);
                string[] name = dir.Split('\\');
                world.GetComponent<LoadWorldUI>().WorldName = name[name.Length - 1];
                world.GetComponent<LoadWorldUI>().UpdateNameText();
                world.transform.SetParent(Content.transform);
            }
        }
    }

    private void JoinServer()
    {
        activePanel = NETPanel;

        MainMenuPanel.SetActive(false);
        NETPanel.SetActive(true);

        GameManager.SetGame(true, false);
        SceneManager.LoadScene(1);
    }   

}
