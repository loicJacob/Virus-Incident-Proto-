
using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float life = 100;

    public void GetHit(float damage)
    {
        life -= damage;

        Debug.Log("Player get hit, current life = " + life);

        if (life <= 0)
            Die();
    }

    private void Die()
    {
        throw new NotImplementedException();
    }
}
