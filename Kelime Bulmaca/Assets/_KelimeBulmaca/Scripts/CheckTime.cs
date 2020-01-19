using UnityEngine;

public class CheckTime : MonoBehaviour
{
    public void TimeIsOver()
    {
        GameManager.Instance.GameEndedAction(false);
    }
}