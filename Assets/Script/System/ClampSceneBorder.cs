using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClampSceneBorder : MonoBehaviour
{
    void Update()
    {
        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x, -2.7f, 0.53f), Mathf.Clamp(transform.position.y, -0.16f, 0.53f));
    }
}
