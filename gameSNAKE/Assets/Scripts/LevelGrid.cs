using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * LevelGrid.cs
 * Tác dụng: Quản lý bàn chơi dạng lưới.
 * - Lưu kích thước bàn chơi.
 * - Sinh thức ăn ngẫu nhiên.
 * - Kiểm tra rắn có ăn trúng thức ăn không.
 * - Xử lý đi xuyên tường: ra mép này sẽ xuất hiện ở mép đối diện.
 */
public class LevelGrid
{
    // Vị trí hiện tại của thức ăn trên lưới.
    private Vector2Int foodGridPosition;

    // GameObject đại diện cho thức ăn trong scene.
    private GameObject foodGameObject;

    // Chiều rộng và chiều cao của bàn chơi.
    private int width;
    private int height;

    // Tham chiếu tới rắn để kiểm tra vị trí thân rắn.
    private Snake snake;

    // Constructor tạo bàn chơi với kích thước truyền vào.
    public LevelGrid(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    // Kết nối bàn chơi với rắn và sinh thức ăn đầu tiên.
    public void Setup(Snake snake)
    {
        this.snake = snake;

        SpawnFood();
    }

    // Sinh thức ăn ở vị trí ngẫu nhiên không trùng với rắn.
    private void SpawnFood()
    {
        do
        {
            // Random vị trí trong giới hạn width x height.
            foodGridPosition = new Vector2Int(Random.Range(0, width), Random.Range(0, height));
        } while (snake.GetFullSnakePositionList().IndexOf(foodGridPosition) != -1);

        // Tạo object Food mới và gắn SpriteRenderer.
        foodGameObject = new GameObject("Food", typeof(SpriteRenderer));
        foodGameObject.GetComponent<SpriteRenderer>().sprite = GameResources.instance.foodSprite;

        // Đưa thức ăn tới đúng vị trí trên lưới.
        foodGameObject.transform.position = new Vector3(foodGridPosition.x, foodGridPosition.y);
    }

    // Kiểm tra rắn có ăn thức ăn không.
    public bool TrySnakeEatFood(Vector2Int snakeGridPosition)
    {
        if (snakeGridPosition == foodGridPosition)
        {
            // Nếu đầu rắn chạm thức ăn thì xóa thức ăn cũ.
            Object.Destroy(foodGameObject);

            // Sinh thức ăn mới.
            SpawnFood();

            // Cộng điểm.
            GameLogic.AddScore();

            return true;
        }
        else
        {
            return false;
        }
    }

    // Kiểm tra vị trí rắn nếu vượt khỏi bàn chơi thì cho xuất hiện ở mép đối diện.
    public Vector2Int ValidateGridPosition(Vector2Int gridPosition)
    {
        if (gridPosition.x < 0)
        {
            gridPosition.x = width - 1;
        }
        if (gridPosition.x > width - 1)
        {
            gridPosition.x = 0;
        }
        if (gridPosition.y < 0)
        {
            gridPosition.y = height - 1;
        }
        if (gridPosition.y > height - 1)
        {
            gridPosition.y = 0;
        }
        return gridPosition;
    }
}
