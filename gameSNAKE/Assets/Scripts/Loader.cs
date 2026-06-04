using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Loader.cs
 * Tác dụng: Quản lý chuyển scene thông qua scene Loading.
 * - Có enum Scene để gọi tên scene an toàn hơn.
 * - Khi muốn chuyển scene, script sẽ vào Loading trước.
 * - LoaderCallback trong scene Loading sẽ gọi tiếp để load scene đích.
 */
public static class Loader
{

    // Danh sách các scene trong game.
    public enum Scene
    {
        Game,
        Loading,
        MainMenu
    }

    // Action lưu hành động load scene đích sau khi vào scene Loading.
    private static Action loaderCallbackAction;

    // Gọi hàm này khi muốn load scene.
    public static void Load(Scene scene)
    {
        // Lưu lại hành động cần làm sau khi scene Loading chạy.
        loaderCallbackAction = () => {
            SceneManager.LoadScene(scene.ToString());
        };

        // Load scene Loading trước.
        SceneManager.LoadScene(Scene.Loading.ToString());
    }

    // Hàm được LoaderCallback gọi ở scene Loading.
    public static void LoaderCallback()
    {
        if (loaderCallbackAction != null)
        {
            // Thực hiện hành động load scene đích.
            loaderCallbackAction();

            // Xóa callback để tránh gọi lại nhiều lần.
            loaderCallbackAction = null;
        }
    }
}
