using System.Collections;
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
    // Object cha chứa các dòng điểm cao.
    private Transform entryContainer;

    // Dòng mẫu để nhân bản ra nhiều dòng điểm.
    private Transform entryTemplate;

    // Danh sách các dòng UI đã tạo.
    private List<Transform> highscoreEntryTransformList;

    private void Awake()
    {
        // Tìm container chứa bảng xếp hạng.
        entryContainer = transform.Find("highscoreEntryContainer");

        // Tìm template dòng điểm, tên trong scene là HSET.
        entryTemplate = entryContainer.Find("HSET");

        // Ẩn template gốc, chỉ dùng nó để clone.
        entryTemplate.gameObject.SetActive(false);

        // Đọc chuỗi JSON lưu trong PlayerPrefs.
        string jsonString = PlayerPrefs.GetString("highscoreTable");

        // Chuyển JSON thành object Highscores.
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Nếu đã có dữ liệu điểm cao thì hiển thị lên UI.
        if (highscores != null)
        {
            // Sắp xếp điểm từ cao đến thấp.
            highscores.highscoreEntryList.Sort((a, b) => b.score.CompareTo(a.score));

            highscoreEntryTransformList = new List<Transform>();

            // Tạo tối đa 11 dòng do điều kiện i == 10 thì break sau khi đã tạo dòng thứ 11.
            // Nếu muốn đúng top 10, có thể đổi thành: if (i >= 10) break; đặt trước khi tạo.
            for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
            {
                CreateHighscoreEntryTransform(highscores.highscoreEntryList[i], entryContainer, highscoreEntryTransformList);
                if (i == 10)
                {
                    break;
                }
            }
        }
    }

    // Thêm một điểm mới vào bảng xếp hạng.
    public static void AddHighscoreEntry(int score, string name)
    {
        // Đọc dữ liệu điểm cao cũ.
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Nếu chưa có dữ liệu thì tạo mới danh sách.
        if (highscores == null)
        {
            highscores = new Highscores();
            highscores.highscoreEntryList = new List<HighScoreEntry>();
        }

        // Kiểm tra xem tên người chơi đã tồn tại chưa.
        bool findMatch = false;
        foreach (HighScoreEntry item in highscores.highscoreEntryList)
        {
            if (name == item.name)
            {
                // Nếu trùng tên thì cập nhật điểm mới cho tên đó.
                item.score = score;
                findMatch = true;
                break;
            }
        }

        // Nếu chưa có tên này thì thêm entry mới.
        if (!findMatch)
        {
            highscores.highscoreEntryList.Add(new HighScoreEntry { score = score, name = name });
        }

        // Chuyển object về JSON rồi lưu vào PlayerPrefs.
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    // Xóa toàn bộ danh sách điểm cao.
    public void ClearListOfScoreEntry()
    {
        // Đọc dữ liệu hiện tại.
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        if (highscores != null)
        {
            // Xóa tất cả entry trong danh sách.
            highscores.highscoreEntryList.Clear();

            // Lưu lại danh sách rỗng.
            string json = JsonUtility.ToJson(highscores);
            PlayerPrefs.SetString("highscoreTable", json);
            PlayerPrefs.Save();
        }
    }

    // Tạo một dòng UI cho một điểm cao.
    private void CreateHighscoreEntryTransform(HighScoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        // Chiều cao mỗi dòng trong bảng.
        float templateHight = 20f;

        // Nhân bản dòng mẫu.
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();

        // Đặt vị trí dòng mới thấp dần theo số lượng dòng đã có.
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        // Tính thứ hạng.
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

        // Gán text thứ hạng.
        entryTransform.Find("positionText").GetComponent<Text>().text = rankString;

        // Gán text điểm.
        int score = highscoreEntry.score;
        entryTransform.Find("scoreText").GetComponent<Text>().text = score.ToString();

        // Gán text tên.
        string name = highscoreEntry.name;
        entryTransform.Find("nameText").GetComponent<Text>().text = name;

        // Bật nền xen kẽ cho dòng lẻ để dễ nhìn.
        entryTransform.Find("background").gameObject.SetActive(rank % 2 == 1);

        // Nếu là hạng 1 thì đổi màu chữ sang xanh.
        if (rank == 1)
        {
            entryTransform.Find("positionText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("scoreText").GetComponent<Text>().color = Color.green;
            entryTransform.Find("nameText").GetComponent<Text>().color = Color.green;
        }

        // Lưu dòng vừa tạo vào danh sách.
        transformList.Add(entryTransform);
    }
}
