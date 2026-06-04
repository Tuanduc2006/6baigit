using System.Collections.Generic;
using UnityEngine;

/*
 * Snake.cs
 * Tác dụng: Điều khiển toàn bộ con rắn.
 * - Nhận input phím mũi tên / WASD trên máy tính.
 * - Nhận thao tác vuốt trên điện thoại Android.
 * - Di chuyển rắn theo từng ô lưới.
 * - Xử lý rắn ăn thức ăn và dài ra.
 * - Tạo thân rắn, cập nhật vị trí thân rắn.
 * - Kiểm tra rắn tự đâm vào thân để Game Over.
 */
public class Snake : MonoBehaviour
{
    private enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    private enum State
    {
        Alive,
        Dead
    }

    [Header("Điều khiển Android")]
    [SerializeField] private float minSwipeDistance = 40f;

    private State state;
    private Direction gridMoveDirection;
    private Vector2Int gridPosition;
    private float gridMoveTimer;
    private float gridMoveTimerMax;
    private LevelGrid levelGrid;
    private int snakeBodySize;
    private List<SnakeMovePosition> snakeMovePositionList;
    private List<SnakeBodyPart> snakeBodyPartList;

    private Vector2 touchStartPosition;
    private bool hasTouchStartPosition;

    public void Setup(LevelGrid levelGrid)
    {
        this.levelGrid = levelGrid;
    }

    private void Awake()
    {
        gridPosition = new Vector2Int(10, 10);
        gridMoveTimerMax = .2f;
        gridMoveTimer = gridMoveTimerMax;
        gridMoveDirection = Direction.Right;
        snakeMovePositionList = new List<SnakeMovePosition>();
        snakeBodySize = 0;
        snakeBodyPartList = new List<SnakeBodyPart>();
        state = State.Alive;
    }

    private void Update()
    {
        if (state == State.Alive)
        {
            HandleInput();
            HandleGridMovement();
        }
    }

    private void HandleInput()
    {
        // Điều khiển bằng bàn phím khi test trong Unity/PC.
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            TrySetMoveDirection(Direction.Up);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            TrySetMoveDirection(Direction.Down);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            TrySetMoveDirection(Direction.Left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            TrySetMoveDirection(Direction.Right);
        }

        // Điều khiển bằng vuốt màn hình khi build APK Android.
        HandleTouchInput();
    }

    private void HandleTouchInput()
    {
        if (Input.touchCount <= 0)
        {
            return;
        }

        Touch touch = Input.GetTouch(0);

        if (touch.phase == TouchPhase.Began)
        {
            touchStartPosition = touch.position;
            hasTouchStartPosition = true;
            return;
        }

        if ((touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled) && hasTouchStartPosition)
        {
            Vector2 swipeDelta = touch.position - touchStartPosition;
            hasTouchStartPosition = false;

            if (swipeDelta.magnitude < minSwipeDistance)
            {
                return;
            }

            if (Mathf.Abs(swipeDelta.x) > Mathf.Abs(swipeDelta.y))
            {
                TrySetMoveDirection(swipeDelta.x > 0 ? Direction.Right : Direction.Left);
            }
            else
            {
                TrySetMoveDirection(swipeDelta.y > 0 ? Direction.Up : Direction.Down);
            }
        }
    }

    private void TrySetMoveDirection(Direction newDirection)
    {
        if (newDirection == Direction.Up && gridMoveDirection != Direction.Down)
        {
            gridMoveDirection = Direction.Up;
        }
        else if (newDirection == Direction.Down && gridMoveDirection != Direction.Up)
        {
            gridMoveDirection = Direction.Down;
        }
        else if (newDirection == Direction.Left && gridMoveDirection != Direction.Right)
        {
            gridMoveDirection = Direction.Left;
        }
        else if (newDirection == Direction.Right && gridMoveDirection != Direction.Left)
        {
            gridMoveDirection = Direction.Right;
        }
    }

    private void HandleGridMovement()
    {
        if (levelGrid == null)
        {
            return;
        }

        gridMoveTimer += Time.deltaTime;

        if (gridMoveTimer >= gridMoveTimerMax)
        {
            gridMoveTimer -= gridMoveTimerMax;

            SoundManager.PlaySound(SoundManager.Sound.SnakeMove);

            SnakeMovePosition previousSnakeMovePosition = null;
            if (snakeMovePositionList.Count > 0)
            {
                previousSnakeMovePosition = snakeMovePositionList[0];
            }

            SnakeMovePosition snakeMovePosition = new SnakeMovePosition(previousSnakeMovePosition, gridPosition, gridMoveDirection);
            snakeMovePositionList.Insert(0, snakeMovePosition);

            Vector2Int gridMoveDirectionVector = GetDirectionVector(gridMoveDirection);
            gridPosition += gridMoveDirectionVector;
            gridPosition = levelGrid.ValidateGridPosition(gridPosition);

            bool snakeAteFood = levelGrid.TrySnakeEatFood(gridPosition);
            if (snakeAteFood)
            {
                snakeBodySize++;
                CreateSnakeBodyPart();
                SoundManager.PlaySound(SoundManager.Sound.SnakeEat);
            }

            if (snakeMovePositionList.Count >= snakeBodySize + 1)
            {
                snakeMovePositionList.RemoveAt(snakeMovePositionList.Count - 1);
            }

            UpdateSnakeBodyParts();
            CheckGameOverByBodyCollision();

            transform.position = new Vector3(gridPosition.x, gridPosition.y);
            transform.eulerAngles = new Vector3(0, 0, GetAngleFromVector(gridMoveDirectionVector) - 90);
        }
    }

    private Vector2Int GetDirectionVector(Direction direction)
    {
        switch (direction)
        {
            default:
            case Direction.Right:
                return new Vector2Int(1, 0);
            case Direction.Left:
                return new Vector2Int(-1, 0);
            case Direction.Up:
                return new Vector2Int(0, 1);
            case Direction.Down:
                return new Vector2Int(0, -1);
        }
    }

    private void CheckGameOverByBodyCollision()
    {
        foreach (SnakeBodyPart snakeBodyPart in snakeBodyPartList)
        {
            Vector2Int snakeBodyPartGridPosition = snakeBodyPart.GetGridPosition();
            if (gridPosition == snakeBodyPartGridPosition)
            {
                Debug.Log("GAME OVER");
                state = State.Dead;
                GameLogic.SnakeDied();
                SoundManager.PlaySound(SoundManager.Sound.SnakeDie);
                break;
            }
        }
    }

    private void CreateSnakeBodyPart()
    {
        snakeBodyPartList.Add(new SnakeBodyPart(snakeBodyPartList.Count));
    }

    private void UpdateSnakeBodyParts()
    {
        for (int i = 0; i < snakeBodyPartList.Count; i++)
        {
            if (i < snakeMovePositionList.Count)
            {
                snakeBodyPartList[i].SetSnakeMovePosition(snakeMovePositionList[i]);
            }
        }
    }

    private float GetAngleFromVector(Vector2Int dir)
    {
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;
        return n;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public List<Vector2Int> GetFullSnakePositionList()
    {
        List<Vector2Int> gridPositionList = new List<Vector2Int> { gridPosition };
        foreach (SnakeMovePosition snakeMovePosition in snakeMovePositionList)
        {
            gridPositionList.Add(snakeMovePosition.GetGridPosition());
        }
        return gridPositionList;
    }

    private class SnakeBodyPart
    {
        private SnakeMovePosition snakeMovePosition;
        private Transform transform;

        public SnakeBodyPart(int bodyIndex)
        {
            GameObject snakeBodyGameObject = new GameObject("SnakeBody", typeof(SpriteRenderer));

            if (GameResources.instance != null)
            {
                snakeBodyGameObject.GetComponent<SpriteRenderer>().sprite = GameResources.instance.snakeBodySprite;
            }

            snakeBodyGameObject.GetComponent<SpriteRenderer>().sortingOrder = -bodyIndex;
            transform = snakeBodyGameObject.transform;
        }

        public void SetSnakeMovePosition(SnakeMovePosition snakeMovePosition)
        {
            this.snakeMovePosition = snakeMovePosition;
            transform.position = new Vector3(snakeMovePosition.GetGridPosition().x, snakeMovePosition.GetGridPosition().y);

            float angle;
            switch (snakeMovePosition.GetDirection())
            {
                default:
                case Direction.Up:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 0;
                            break;
                        case Direction.Left:
                            angle = 45;
                            break;
                        case Direction.Right:
                            angle = -45;
                            break;
                    }
                    break;
                case Direction.Down:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 180;
                            break;
                        case Direction.Left:
                            angle = 135;
                            break;
                        case Direction.Right:
                            angle = 225;
                            break;
                    }
                    break;
                case Direction.Left:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = -90;
                            break;
                        case Direction.Down:
                            angle = -45;
                            break;
                        case Direction.Up:
                            angle = 45;
                            break;
                    }
                    break;
                case Direction.Right:
                    switch (snakeMovePosition.GetPreviousDirection())
                    {
                        default:
                            angle = 90;
                            break;
                        case Direction.Down:
                            angle = 45;
                            break;
                        case Direction.Up:
                            angle = -45;
                            break;
                    }
                    break;
            }

            transform.eulerAngles = new Vector3(0, 0, angle);
        }

        public Vector2Int GetGridPosition()
        {
            return snakeMovePosition.GetGridPosition();
        }
    }

    private class SnakeMovePosition
    {
        private SnakeMovePosition previousSnakeMovePosition;
        private Vector2Int gridPosition;
        private Direction direction;

        public SnakeMovePosition(SnakeMovePosition previousSnakeMovePosition, Vector2Int gridPosition, Direction direction)
        {
            this.previousSnakeMovePosition = previousSnakeMovePosition;
            this.gridPosition = gridPosition;
            this.direction = direction;
        }

        public Vector2Int GetGridPosition()
        {
            return gridPosition;
        }

        public Direction GetDirection()
        {
            return direction;
        }

        public Direction GetPreviousDirection()
        {
            if (previousSnakeMovePosition == null)
            {
                return Direction.Right;
            }

            return previousSnakeMovePosition.direction;
        }
    }
}
