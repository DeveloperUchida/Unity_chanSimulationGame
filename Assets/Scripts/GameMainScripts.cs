using UnityEngine;

public class GameMainScripts : MonoBehaviour
{

    private float WarkSppped = 4; //歩行スピード

    private float RotateSpeed = 0.6f; //回転スピード

    private Animator animator;
    private bool isWorking = false; //歩行フラグ
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        bool moveKeyPressd = false;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, WarkSppped * Time.deltaTime);
            moveKeyPressd = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -RotateSpeed, 0);
            moveKeyPressd = true;
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -WarkSppped * Time.deltaTime);
            moveKeyPressd = true;
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, RotateSpeed, 0);
            moveKeyPressd = true;
        }

        //アニメーション切り替え機能
        if (moveKeyPressd && !isWorking)
        {
            WakingAnim();
        }
        else if (isWorking && isWorking)
        {
            stop();
        }
    }
}
