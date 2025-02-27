using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodObject : CellObject
{
    public override void PlayerEntered()
    {
        // 푸드 없애기
        Destroy(gameObject);

        // 플레이어의체력(food) 늘리기
        Debug.Log("Food increased");
    }
}
