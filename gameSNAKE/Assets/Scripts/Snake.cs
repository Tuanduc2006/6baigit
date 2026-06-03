using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/*
 * Snake.cs
 * Tác dụng: Đây là script quan trọng nhất, điều khiển toàn bộ con rắn.
 * - Nhận input phím mũi tên.
 * - Di chuyển rắn theo từng ô lưới.
 * - Xử lý rắn ăn thức ăn và dài ra.
 * - Tạo thân rắn.
 * - Cập nhật vị trí từng đoạn thân rắn.
 * - Kiểm tra rắn tự đâm vào thân để Game Over.
 * - Xoay đầu và thân rắn theo hướng di chuyển.
 */
public class Snake : MonoBehaviour
{
    // Các hướng di chuyển của rắn.
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    // Trạng thái sống/chết của rắn.
    private enum State
    {
        Alive,
        Dead
    }

    // Trạng thái hiện tại của rắn.
    private State state;

    // Hướng di chuyển hiện tại.
    private Direction gridMoveDirection;

    // Vị trí đầu rắn trên lưới.
    private Vector2Int gridPosition;

    // Bộ đếm thời gian để rắn di chuyển theo nhịp, không phải mỗi frame.
    private float gridMoveTimer;

    // Thời gian giữa mỗi lần rắn di chuyển. Số càng nhỏ rắn càng nhanh.
    private float gridMoveTimerMax;

    // Bàn chơi, dùng để kiểm tra thức ăn và giới hạn lưới.
    private LevelGrid levelGrid;

    // Độ dài thân rắn hiện tại.
    private int snakeBodySize;

    // Danh sách lịch sử vị trí đầu rắn, dùng cho thân rắn đi theo.
    private List<SnakeMovePosition> snakeMovePositionList;

    // Danh sách các đoạn thân rắn đã tạo.
    private List<SnakeBodyPart> snakeBodyPartList;

    // Nhận LevelGrid từ GameLogic.
    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        // Vị trí ban đầu của rắn.
        gridPosition = new Vector2Int(10, 10);

        // Tốc độ ban đầu: cứ 0.2 giây rắn đi 1 ô.
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;

        // Rắn bắt đầu đi sang phải.
        gridMoveDirection = Direction.Right;

        // Khởi tạo danh sách lịch sử di chuyển.
        snakeMovePositionList = new List<SnakeMovePosition>();

        // Ban đầu chưa có thân.
        snakeBodySize = 0;

        // Khởi tạo danh sách thân rắn.
        snakeBodyPartList = new List<SnakeBodyPart>();

        // Trạng thái ban đầu là sống.
        state = State.Alive;
    }

    private void Update()
    {
        // Nếu rắn còn sống thì cho phép điều khiển và di chuyển.
        switch (state)
        {
            case State.Alive:
                HandleInput();
                HandleGridMovement();
                break;
            case State.Dead:
                // Khi chết thì không xử lý di chuyển nữa.
                break;
        }
    }

    // Xử lý input từ bàn phím.
    private void HandleInput()
    {
        // Không cho quay ngược 180 độ để tránh rắn tự đâm ngay vào thân.
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (gridMoveDirection != Direction.Down)
            {
                gridMoveDirection = Direction.Up;
            }
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (gridMoveDirection != Direction.Up)
            {
                gridMoveDirection = Direction.Down;
            }
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (gridMoveDirection != Direction.Right)
            {
                gridMoveDirection = Direction.Left;
            }
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (gridMoveDirection != Direction.Left)
            {
                gridMoveDirection = Direction.Right;
            }
        }
    }

    // Xử lý di chuyển của rắn theo từng ô lưới.
    private void HandleGridMovement()
    {
        // Cộng thời gian mỗi frame.
        gridMoveTimer += Time.deltaTime;

        // Chỉ di chuyển khi đủ thời gian quy định.
        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            // Phát âm thanh di chuyển.
            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);

            // Lấy vị trí trước đó của đầu rắn để thân rắn theo sau.
            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            // Lưu vị trí hiện tại trước khi đầu rắn di chuyển.
            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            // Chuyển hướng enum thành vector di chuyển trên lưới.
            Vector2Int gridMoveDirectionVector;
            switch (gridMoveDirection)
            {
                default:
                case Direction.Right:
                    gridMoveDirectionVector = new Vector2Int(1, 0);
                    break;
                case Direction.Left:
                    gridMoveDirectionVector = new Vector2Int(-1, 0);
                    break;
                case Direction.Up:
                    gridMoveDirectionVector = new Vector2Int(0, 1);
                    break;
                case Direction.Down:
                    gridMoveDirectionVector = new Vector2Int(0, -1);
                    break;
            }

            // Cập nhật vị trí đầu rắn.
            gridPosition += gridMoveDirectionVector;

            // Nếu rắn đi ra ngoài lưới thì cho xuất hiện ở phía đối diện.
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            // Kiểm tra rắn có ăn được thức ăn không.
            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                // Tăng chiều dài thân rắn.
                snakeBodySize++;

                // Tạo thêm một đoạn thân mới.
                CreateSnakeBodyPart();

                // Phát âm thanh ăn thức ăn.
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            // Giữ danh sách lịch sử vị trí vừa đủ với độ dài thân rắn.
            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            // Cập nhật vị trí các đoạn thân theo lịch sử di chuyển.
            UpdateSnakeBodyParts();

            // Kiểm tra đầu rắn có đâm vào thân không.
            foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
            {
                Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
                if (gridPosition == snakeBodyPartGridPosition)
                {
                    Debug.Log("GAME OVER");

                    // Chuyển sang trạng thái chết.
                    state = State.Dead;

                    // Báo GameLogic hiển thị Game Over.
                    GameLogic.SnakeDied();

                    // Phát âm thanh chết.
                    SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                }
            }

            // Cập nhật vị trí GameObject đầu rắn trong scene.
            transform.position = new Vector3(gridPosition.x, gridPosition.y);

            // Xoay đầu rắn theo hướng di chuyển.
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);

        }
    }

    // Tạo thêm một đoạn thân rắn mới.
    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    // Cập nhật vị trí từng đoạn thân rắn.
    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
        }
    }

    // Chuyển vector hướng thành góc xoay.
    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    // Trả về vị trí đầu rắn.
    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    // Trả về danh sách toàn bộ vị trí của rắn gồm đầu và thân.
    // LevelGrid dùng hàm này để không sinh thức ăn trùng lên rắn.
    public List<Vector2Int> GetFullSnakePositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int> { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    // Class con đại diện cho một đoạn thân rắn.
    private class SnakeBodyPart
    {
        // Vị trí di chuyển mà đoạn thân này đang bám theo.
        private SnakeMovePosition snakeMovePosition;

        // Transform của GameObject thân rắn.
        private Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            // Tạo GameObject thân rắn mới.
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));

            // Gán sprite thân rắn.
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameResources.instance.snakeBodySprite;

            // Sắp xếp layer hiển thị để thân sau nằm dưới thân trước.
            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;

            // Lưu transform để cập nhật vị trí và góc xoay.
            transform = snakeBodyGameObject.transform;
        }

        // Gán vị trí di chuyển cho đoạn thân và cập nhật hình ảnh trong scene.
        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;

            // Cập nhật vị trí thân rắn.
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            // Xác định góc xoay của thân theo hướng hiện tại và hướng trước đó.
            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up: // Đang đi lên.
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left: // Trước đó đi sang trái.
                            angle = 0 + 45;
                            break;
                        case Direction.Right: // Trước đó đi sang phải.
                            angle = 0 - 45;
                            break;
                    }
                    break;
                case Direction.Down: // Đang đi xuống.
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left: // Trước đó đi sang trái.
                            angle = 180 - 45;
                            break;
                        case Direction.Right: // Trước đó đi sang phải.
                            angle = 180 + 45;
                            break;
                    }
                    break;
                case Direction.Left: // Đang đi sang trái.
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down: // Trước đó đi xuống.
                            angle = -45;
                            break;
                        case Direction.Up: // Trước đó đi lên.
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Right: // Đang đi sang phải.
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90;
                            break;
                        case Direction.Down: // Trước đó đi xuống.
                            angle = 45;
                            break;
                        case Direction.Up: // Trước đó đi lên.
                            angle = -45;
                            break;
                    }
                    break;
            }

            // Gán góc xoay cho đoạn thân.
            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        // Trả về vị trí hiện tại của đoạn thân.
        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }

    }

    // Class con lưu một mốc di chuyển của đầu rắn.
    // Thân rắn sẽ đi theo các mốc này để tạo cảm giác nối đuôi nhau.
    private class SnakeMovePosition
    {

        // Mốc di chuyển trước đó.
        private SnakeMovePosition previousSnakeMovePosition;

        // Vị trí trên lưới tại mốc này.
        private Vector2Int gridPosition;

        // Hướng di chuyển tại mốc này.
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        // Trả về vị trí của mốc di chuyển.
        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        // Trả về hướng hiện tại.
        public Direction GetDirection()
        {
            return direction;
        }

        // Trả về hướng trước đó để tính góc xoay thân rắn khi quẹo.
        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                // Nếu chưa có mốc trước thì mặc định là đi sang phải.
                return Direction.Right;
            }
            else
            {
                return previousSnakeMovePosition.direction;
            }
        }
    }

}
