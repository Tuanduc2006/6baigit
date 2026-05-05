using UnityEngine;

[CreateAssetMenu(fileName = "New Card Data", menuName = "MemoryMatch/CardData")]
public class CardData : ScriptableObject
{
    public int cardID; // Mã ID để game biết 2 thẻ có giống nhau không
    public Sprite frontSprite; // Hình ảnh mặt trước của thẻ (Pokemon)
}