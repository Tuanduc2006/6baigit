using System.Collections.Generic;

namespace Assets.Scripts
{
    /*
     * Highscores.cs
     * Tác dụng: Class bao ngoài để lưu danh sách điểm cao.
     * Unity JsonUtility cần class Serializable và public để lưu/đọc ổn định.
     */
    [System.Serializable]
    public class Highscores
    {
        public List<HighScoreEntry> highscoreEntryList = new List<HighScoreEntry>();
    }
}
