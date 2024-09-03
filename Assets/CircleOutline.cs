using UnityEngine;

public class CircleOutline : MonoBehaviour
{
    public float radius = 1f;
    public int numPoints = 100;
    public LineRenderer lineRenderer;

    void Start()
    {
        DrawCircle();
    }

    void DrawCircle()
    {
        lineRenderer.positionCount = numPoints + 1;
        float angleStep = 360f / numPoints;
        float angle = 0f;

        for (int i = 0; i <= numPoints; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            lineRenderer.SetPosition(i, new Vector3(x, y, 0f));
            angle += angleStep;
        }
    }
}
