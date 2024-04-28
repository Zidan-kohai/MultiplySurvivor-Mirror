using UnityEngine;

public interface IPlayer
{
    public bool IsFall();

    public bool IsDeath();

    public bool IsFallOrDeath();

    public Transform GetTransform();

    public void Fall();

    public float Raising();

    public float GetPercentOfRaising();
}
