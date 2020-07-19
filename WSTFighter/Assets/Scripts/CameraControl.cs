using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    public float maxX;
    public float minX;
    public bool updating;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (updating)
        {
            Camera.main.transform.position = CenterPos();
        }
    }

    private Vector3 CenterPos()
    {
        Vector3 center = Vector3.Lerp(MatchManager.instance.players[0].transform.position, MatchManager.instance.players[1].transform.position, .5f);
        Vector3 pos = Camera.main.transform.position;
        float xPos = Mathf.Clamp(center.x, minX, maxX);
        pos.x = xPos;
        return pos;
    }
}
