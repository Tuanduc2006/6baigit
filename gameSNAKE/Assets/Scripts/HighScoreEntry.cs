using System;
using System.Collections;
using System.Collections.Generic;

namespace Assets.Scripts
{
    /*
     * HighScoreEntry.cs
     * Tác dụng: Lưu dữ liệu của một dòng điểm cao.
     * Mỗi entry gồm:
     * - score: điểm số.
     * - name: tên người chơi.
     */
    [System.Serializable]
    public class HighScoreEntry
    {
        // Điểm của người chơi.
        public int score;

        // Tên người chơi.
        public string name;
    }
}
