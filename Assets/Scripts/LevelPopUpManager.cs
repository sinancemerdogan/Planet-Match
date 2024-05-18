using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LevelPopUpManager : MonoBehaviour {

    [SerializeField] private GameObject levelPanelPrefab;
    private SortedDictionary<int, Dictionary<string, string>> levelFilesDictionary;
    [SerializeField] private GameObject loadingPanel;
    private Vector2Int screenSize;


    private void Awake() {
        levelFilesDictionary = new SortedDictionary<int, Dictionary<string, string>>();
        screenSize = new Vector2Int(Screen.width, Screen.height);
        //If there is a internet connection and all levels are not downloaded already, download all of them
        if (Application.internetReachability != NetworkReachability.NotReachable && PlayerPrefs.GetInt("All Levels Downloaded") != 1) {

            StartCoroutine(DownloadAndLoadLevelFiles());
        }
        //Else read them from file file
        else {
            ReadAllLevelFiles();
            CreateLevelPanels();
        }
    }

    private IEnumerator DownloadAndLoadLevelFiles() {

        loadingPanel.SetActive(true);
        yield return StartCoroutine(StartDownload());
        loadingPanel.SetActive(false);
        PlayerPrefs.SetInt("All Levels Downloaded", 1);
        ReadAllLevelFiles();
        CreateLevelPanels();
    }

    //Creates level panels 
    public void CreateLevelPanels() {

        //For each level key in the dictionary reads the data and writes them to appropriate text labels
        foreach (KeyValuePair<int, Dictionary<string, string>> outerPair in levelFilesDictionary) {

            Dictionary<string, string> innerDict = outerPair.Value;

            GameObject levelPanel = Instantiate(levelPanelPrefab, Vector3.zero, Quaternion.identity);
            levelPanel.transform.SetParent(transform);
            levelPanel.transform.localScale = new Vector3(1, 1, 1);

            TextMeshProUGUI levelValue = levelPanel.transform.Find("Level Value").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI moveCountValue = levelPanel.transform.Find("Move Count Value").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI highestScoreLabel = levelPanel.transform.Find("Highest Score Label").GetComponent<TextMeshProUGUI>();
            TextMeshProUGUI highestScoreValue = levelPanel.transform.Find("Highest Score Value").GetComponent<TextMeshProUGUI>();

            string levelNumber;
            if (innerDict.TryGetValue("level_number", out levelNumber)) {
                levelValue.text = levelNumber;
            }

            string moveCount;
            if (innerDict.TryGetValue("move_count", out moveCount)) {
                moveCountValue.text = "- " + moveCount;
            }

            string levelScoreKey = "Level " + levelNumber + " Score";
            if(PlayerPrefs.HasKey(levelScoreKey) && PlayerPrefs.GetInt(levelScoreKey) != 0) {
                highestScoreValue.text = PlayerPrefs.GetInt(levelScoreKey).ToString();
            }
            else {
                highestScoreLabel.text = "No Score";
                highestScoreValue.text = "";
            }


            if (int.Parse(levelNumber) == 1) {

                levelPanel.transform.Find("Play Button").GetComponent<Button>().interactable = true;
                levelPanel.transform.Find("Play Button").GetComponent<Button>().transform.Find("Play Button Label").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                levelPanel.transform.Find("Play Button").GetComponent<Button>().transform.Find("Locked Icon").GetComponent<Image>().gameObject.SetActive(false);
            }
            else {
                levelScoreKey = "Level " + (int.Parse(levelNumber) - 1) + " Score";
                if (PlayerPrefs.HasKey(levelScoreKey) && PlayerPrefs.GetInt(levelScoreKey) != 0) {
                    levelPanel.transform.Find("Play Button").GetComponent<Button>().interactable = true;
                    levelPanel.transform.Find("Play Button").GetComponent<Button>().transform.Find("Play Button Label").GetComponent<TextMeshProUGUI>().gameObject.SetActive(true);
                    levelPanel.transform.Find("Play Button").GetComponent<Button>().transform.Find("Locked Icon").GetComponent<Image>().gameObject.SetActive(false);
                }
                else {
                    highestScoreLabel.text = "Locked Level";
                    highestScoreValue.text = "";
                }
            }
        }
        float aspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio_16_9 = 9f / 16f;
        float targetAspectRatio_4_3 = 4f / 3f;
        float tolerance = 0.01f;
        ContentSizeFitter contentSizeFitter = GetComponent<ContentSizeFitter>();

        if (Mathf.Abs(aspectRatio - targetAspectRatio_16_9) < tolerance) {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        }
        else if (Mathf.Abs(aspectRatio - targetAspectRatio_4_3) < tolerance) {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else {
            Debug.LogWarning("Unsupported aspect ratio.");
        }
    }

    //Read all the level files
    public void ReadAllLevelFiles() {

        string folderPath;
        String[] filePaths;

        //If all level downloaded reads from persistent data path
        if (PlayerPrefs.HasKey("All Levels Downloaded")) {
            folderPath = Path.Combine(Application.persistentDataPath, "Levels");
        }
        //Else reads from the project data path
        else {
            folderPath = Application.dataPath + "/LevelFiles";
        }

        //If all the levels downloaded and somehow they disappear reads from from the project data path
        try {
            filePaths = Directory.GetFiles(folderPath);
        }
        catch(Exception e) {
            Debug.Log($"Failed to read from {folderPath} with exception {e}");
            folderPath = Application.dataPath + "/LevelFiles";
            filePaths = Directory.GetFiles(folderPath);
            PlayerPrefs.SetInt("All Levels Downloaded", 0);
        }

        foreach (string filePath in filePaths) {

            if (filePath.Contains("URLs.txt") || filePath.Contains(".meta")) {
                continue;
            }

            string fileContent = string.Empty;
            using (StreamReader reader = new StreamReader(filePath)) {
                string line;
                while ((line = reader.ReadLine()) != null) {
                    fileContent += line + "-";
                }
            }

            fileContent = fileContent.TrimEnd('-');
            Dictionary<string, string> tempDictionary = ParseFileContent(fileContent);

            string levelNumberString;
            int levelNumberInt;

            if (tempDictionary.TryGetValue("level_number", out levelNumberString)) {
                levelNumberInt = int.Parse(levelNumberString);
            }
            else {
                levelNumberInt = 0;
            }
            levelFilesDictionary.Add(levelNumberInt, tempDictionary);
        }
    }

    //Parses the level information that is read from the level file
    public Dictionary<string, string> ParseFileContent(string data) {


        Dictionary<string, string> levelContentsDictionary = new Dictionary<string, string>();

        string[] parts = data.Split('-');

        foreach (string part in parts) {

            string[] fields = part.Split(':');

            if (fields.Length == 2) {

                levelContentsDictionary.Add(fields[0].Trim(), fields[1].Trim());
            }
        }
        return levelContentsDictionary;
    }

    //Prints levels dictionary for debug purposes
    private void PrintLevelsDictionary() {
        foreach (KeyValuePair<int, Dictionary<string, string>> outerPair in levelFilesDictionary) {
            int outerKey = outerPair.Key;
            Dictionary<string, string> innerDict = outerPair.Value;

            Debug.Log("Outer Key: " + outerKey);

            foreach (KeyValuePair<string, string> innerPair in innerDict) {
                string innerKey = innerPair.Key;
                string value = innerPair.Value;

                Debug.Log("Inner Key: " + innerKey + ", Value: " + value);
            }
        }
    }

    //Returns level information using level number 
    public Dictionary<string, string> GetlevelFilesDictionaryByKey(int key) {

        Dictionary<string, string> fileDictionary = new Dictionary<string, string>();
        if (levelFilesDictionary.TryGetValue(key, out fileDictionary)) {
            return fileDictionary;
        }
        return null;
    }

    //Reads the URLS.txt files and downloads all the levels from the URLs
    public IEnumerator StartDownload() {
        var fullPath = Application.dataPath + "/LevelFiles/URLs.txt";

        using (StreamReader reader = new StreamReader(fullPath)) {
            string line;
            while ((line = reader.ReadLine()) != null) {
                yield return StartCoroutine(DownloadFile(line));
            }
        }
    }

    //Download a single level file and save it to persistent data path of the application in folder called "Levels"
    private IEnumerator DownloadFile(string fileURL) {
        UnityWebRequest request = UnityWebRequest.Get(fileURL);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success) {
            System.IO.Directory.CreateDirectory(Application.persistentDataPath + "/Levels");
            string filePath = Path.Combine(Application.persistentDataPath + "/Levels", Path.GetFileName(fileURL));
            File.WriteAllBytes(filePath, request.downloadHandler.data);
        }
        else {
            Debug.LogError("File download failed: " + request.error);
        }
    }
    //On screen size change calls OnScreenSizeChange
    void Update() {
        Vector2Int currentScreenSize = new Vector2Int(Screen.width, Screen.height);

        if (currentScreenSize != screenSize) {
            screenSize = currentScreenSize;
            OnScreenSizeChange();
        }
    }

    //Updates the camera 
    void OnScreenSizeChange() {

        float aspectRatio = (float)Screen.width / Screen.height;
        float targetAspectRatio_16_9 = 9f / 16f;
        float targetAspectRatio_4_3 = 4f / 3f;
        float tolerance = 0.01f;
        ContentSizeFitter contentSizeFitter = GetComponent<ContentSizeFitter>();

        if (Mathf.Abs(aspectRatio - targetAspectRatio_16_9) < tolerance) {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.MinSize;
        }
        else if (Mathf.Abs(aspectRatio - targetAspectRatio_4_3) < tolerance) {
            contentSizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
        }
        else {
            Debug.LogWarning("Unsupported aspect ratio.");
        }
    }
}
