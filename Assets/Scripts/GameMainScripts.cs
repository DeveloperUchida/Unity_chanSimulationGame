using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameMainScripts : MonoBehaviour
{
    //歩行スピード
    private float WarkSppped = 4;
    //回転スピード
    private float RotateSpeed = 2;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, WarkSppped);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, RotateSpeed, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -WarkSppped);
        }
    }
}
