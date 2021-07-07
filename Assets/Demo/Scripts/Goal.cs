using UnityEngine;
using System.Collections;

public class Goal : MonoBehaviour
{
    private Collider m_Collider;
    private Material m_Material;
    private Color m_Color;
    private Vector3 m_DefPos;

    public void Initialize()
    {
        m_Collider = GetComponent<Collider>();
        m_Material = GetComponent<Renderer>().material;
        m_Color = m_Material.GetColor("_Color");
        m_DefPos = transform.localPosition;
    }

    public void SetPosition(float normAngle)
    {
        var rot = Quaternion.AngleAxis(normAngle * 180, Vector3.up);
        transform.localPosition = rot * m_DefPos;
        transform.localRotation = rot;

        SetEnabled(false);
    }

    public void SetEnabled(bool enable = true)
    {
        m_Collider.enabled = enable;

        m_Color.a = enable ? 0.9f : 0.2f;
        StartCoroutine(ChangeColor(m_Color, enable ? 0 : 0.25f));
    }

    public bool IsEnabled() => m_Collider.enabled;

    public void Highlight()
    {
        m_Material.SetColor("_Color", Color.green);
    }

    private IEnumerator ChangeColor(Color color, float delay)
    {
        yield return new WaitForSeconds(delay);
        m_Material.SetColor("_Color", color);
    }
}
