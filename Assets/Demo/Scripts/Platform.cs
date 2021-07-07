using UnityEngine;

public class Platform : MonoBehaviour
{
    public void Randomize()
    {
        transform.localRotation = Quaternion.Euler(
            Random.Range(-60f, 10f), 0,
            Random.Range(-45f, 45f));
    }
}
