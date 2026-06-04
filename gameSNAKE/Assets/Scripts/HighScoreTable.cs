using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

/*
 * HighScoreTable.cs
 * Tác dụng: Quản lý bảng xếp hạng điểm cao.
 * - Đọc điểm cao từ PlayerPrefs.
 * - Sắp xếp điểm từ cao xuống thấp.
 * - Tạo các dòng UI hiển thị rank, score, name.
 * - Thêm/cập nhật điểm mới.
 * - Xóa danh sách điểm cao.
 */
public class HighScoreTable : MonoBehaviour
{
    private const string HighscorePrefsKey = "highscoreTable";

    // Object cha chứa các dòng điểm cao.
    private Transform entryContainer;

    // Dòng mẫu để nhân bản ra nhiều dòng điểm.
    private Transform entryTemplate;

    // Danh sách các dòng UI đã tạo.
    private List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        entryContainer = transform.Find("highscoreEntryContainer");
        if (entryContainer == null)
        {
            Debug.LogWarning("Không tìm thấy highscoreEntryContainer trong bảng xếp hạng.");
            return;
        }

        entryTemplate = entryContainer.Find("HSET");
        if (entryTemplate == null)
        {
            Debug.LogWarning("Không tìm thấy template HSET trong highscoreEntryContainer.");
            return;
        }

        entryTemplate.gameObject.SetActive(false);

        Highscores highscores = LoadHighscores();
        highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

        highscoreEntryTransformList = new List<Transform>();

        // Hiển thị đúng Top 10.
        int maxEntries = Mathf.Min(10, highscores.highscoreEntryList.Count);
        for (int i = 0; i < maxEntries; i++)
        {
            CreateHighscoreEntryTransform(highscores.highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
        }
    }

    private static Highscores LoadHighscores()
    {
        string jsonString = PlayerPrefs.GetString(HighscorePrefsKey, string.Empty);

        if (string.IsNullOrEmpty(jsonString))
        {
            return new Highscores();
        }

        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        if (highscores == null)
        {
            highscores = new Highscores();
        }

        if (highscores.highscoreEntryList == null)
        {
            highscores.highscoreEntryList = new List<HighScoreEntry>();
        }

        return highscores;
    }

    private static void SaveHighscores(Highscores highscores)
    {
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString(HighscorePrefsKey, json);
        PlayerPrefs.Save();
    }

    // Thêm một điểm mới vào bảng xếp hạng.
    public static void AddHighscoreEntry(int score, string name)
    {
        Highscores highscores = LoadHighscores();

        if (string.IsNullOrWhiteSpace(name))
        {
            name = "---";
        }

        bool findMatch = false;
        foreach (HighScoreEntry item in highscores.highscoreEntryList)
        {
            if (name == item.name)
            {
                // Chỉ cập nhật nếu điểm mới cao hơn điểm cũ.
                if (score > item.score)
                {
                    item.score = score;
                }

                findMatch = true;
                break;
            }
        }

        if (!findMatch)
        {
            highscores.highscoreEntryList.Add(new HighScoreEntry { score = score, name = name });
        }

        SaveHighscores(highscores);
    }

    // Xóa toàn bộ danh sách điểm cao.
    public void ClearListOfScoreEntry()
    {
        Highscores highscores = new Highscores();
        SaveHighscores(highscores);
    }

    // Tạo một dòng UI cho một điểm cao.
    private void CreateHighscoreEntryTransform(HighScoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        float templateHeight = 20f;

        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int rank = transformList.Count + 1;
        string rankString;
        switch (rank)
        {
            case 1:
                rankString = "1ST";
                break;
            case 2:
                rankString = "2ND";
                break;
            case 3:
                rankString = "3RD";
                break;
            default:
                rankString = rank + "TH";
                break;
        }

        SetChildText(entryTransform, "positionText", rankString);
        SetChildText(entryTransform, "scoreText", highscoreEntry.score.ToString());
        SetChildText(entryTransform, "nameText", highscoreEntry.name);

        Transform background = entryTransform.Find("background");
        if (background != null)
        {
            background.gameObject.SetActive(rank % 2 == 1);
        }

        if (rank == 1)
        {
            SetChildTextColor(entryTransform, "positionText", Color.green);
            SetChildTextColor(entryTransform, "scoreText", Color.green);
            SetChildTextColor(entryTransform, "nameText", Color.green);
        }

        transformList.Add(entryTransform);
    }

    private void SetChildText(Transform parent, string childName, string value)
    {
        Transform child = parent.Find(childName);
        if (child == null) return;

        Text text = child.GetComponent<Text>();
        if (text != null)
        {
            text.text = value;
        }
    }

    private void SetChildTextColor(Transform parent, string childName, Color color)
    {
        Transform child = parent.Find(childName);
        if (child == null) return;

        Text text = child.GetComponent<Text>();
        if (text != null)
        {
            text.color = color;
        }
    }
}
