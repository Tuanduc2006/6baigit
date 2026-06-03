using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts
{
    /*
     * Highscores.cs
     * Tác dụng: Class bao ngoài để lưu danh sách điểm cao.
     * Unity JsonUtility không lưu trực tiếp List đơn lẻ tốt bằng việc bọc List trong một class.
     */
    class Highscores
    {
        // Danh sách các người chơi và điểm số của họ.
        public List<HighScoreEntry> highscoreEntryList;
    }
}
