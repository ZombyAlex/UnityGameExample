using UnityEngine;

public class Circle : MonoBehaviour
{
    public float radius = 0.0f;
    public int numSegments = 128;
    public Color color;

    private LineRenderer lr;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Draw()
    {
        //lr.startColor = color;
        //lr.endColor = color;
        
        lr.positionCount = numSegments + 1;

        float deltaTheta = (float)(2.0 * Mathf.PI) / numSegments;
        float theta = 0f;

        for (int i = 0; i < numSegments + 1; i++)
        {
            float x = radius * Mathf.Cos(theta);
            float y = radius * Mathf.Sin(theta);
            Vector3 pos = new Vector3(x, y, 0);
            lr.SetPosition(i, pos);
            theta += deltaTheta;
        }
    }

    public void SetRange(float range)
    {
        if (Mathf.Abs(radius - range) > 0.01f)
        {
            radius = range;
            Draw();
        }
    }
}
