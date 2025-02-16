using UnityEngine;

public interface ReviceSpeedChange
{
    public void ChangeSpeed(float speedMultiplier);
    public void ChangeSpeed(float speedMultiplier, float speedTime);

    public void ChangeSpeedOnExit(float speedTime);
}