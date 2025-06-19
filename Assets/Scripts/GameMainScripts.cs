using UnityEngine;

public class GameMainScripts : MonoBehaviour
{

    private float WarkSppped = 4; //歩行スピード

    private float RotateSpeed = 0.6f; //回転スピード

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, WarkSppped * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -RotateSpeed, 0);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -WarkSppped * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, RotateSpeed, 0);
        }
    }
}
