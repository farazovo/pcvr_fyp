using UnityEngine;

public class Scroll_floor : MonoBehaviour
{
    public float speed = -0.5f;

    void Update()
    {
        float offset = Time.time * speed;

        foreach (Transform child in transform)
        {
            Renderer rend = child.GetComponent<Renderer>();
            if (rend != null)
            {
                rend.material.SetTextureOffset("_MainTex", new Vector2(0, offset));
            }
        }
    }
}
