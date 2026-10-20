using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    public GameObject BrickPre;

    public float width = 0.15f;
    public float height = 0.07f;

    [SerializeField]private int x;
    [SerializeField]private int y;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i > y; i--)
        {
            for(int j = 0; j< x; j++)
            {
                GameObject go = Instantiate(BrickPre, transform);
                go.transform.localPosition = new Vector3(width * j, height * i, 0);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
