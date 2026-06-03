using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * LoaderCallback.cs
 * Tác dụng: Gắn vào object trong scene Loading.
 * - Sau frame đầu tiên của scene Loading, nó gọi Loader.LoaderCallback().
 * - Nhờ vậy game có thể chuyển từ Loading sang scene đích.
 */
public class LoaderCallback : MonoBehaviour
{
    // Đảm bảo callback chỉ chạy đúng 1 lần.
    private bool firstUpdate = true;

    private void Update()
    {
        if (firstUpdate)
        {
            firstUpdate = false;

            // Gọi hàm load scene đích đã lưu trong Loader.
            Loader.LoaderCallback();
        }
    }
}
