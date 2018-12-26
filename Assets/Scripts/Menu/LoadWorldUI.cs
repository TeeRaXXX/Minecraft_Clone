using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadWorldUI : MonoBehaviour
{

    [SerializeField] private Button LoadWorldButton;
    [SerializeField] private Button DeleteWorldButton;
    [SerializeField] private Text WorldNameText;

    public string WorldName;

    private void Awake()
    {
        LoadWorldButton.onClick.AddListener(() => LoadWorld());
        DeleteWorldButton.onClick.AddListener(() => DeleteWorld());
    }

    private void LoadWorld()
    {
        GameManager.WorldName = WorldName;
        GameManager.Seed = int.Parse(File.ReadAllText(Application.dataPath + "\\Worlds\\" + WorldName + "\\seed"));

        SceneManager.LoadScene(1);
    }

    private void DeleteWorld()
    {
        string path = Application.dataPath + "\\Worlds\\" + WorldName;
        foreach (var file in Directory.GetFiles(path))
        {
            File.Delete(file);
        }
        Directory.Delete(path);
        File.Delete(Application.dataPath + "\\Worlds\\" + WorldName + ".meta");

        Destroy(gameObject);
    }

    public void UpdateNameText()
    {
        WorldNameText.text = "Load\n" + WorldName;
    }
}
