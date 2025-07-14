using UnityEngine;

public class GameMainScripts : MonoBehaviour
{
    private float _walkSpeed = 4f; // 歩行スピード（スペル修正）
    private float _rotateSpeed = 0.6f; // 回転スピード

    private Animator _animator;
    private bool _isWalking;

    // Animatorパラメータのハッシュ化
    private static readonly int IsWalkingHash = Animator.StringToHash("isWalking");

    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    void Update()
    {
        bool moveKeyPressed = false;

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, _walkSpeed * Time.deltaTime);
            moveKeyPressed = true;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, -_rotateSpeed, 0);
            moveKeyPressed = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, -_walkSpeed * Time.deltaTime);
            moveKeyPressed = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, _rotateSpeed, 0);
            moveKeyPressed = true;
        }

        if (moveKeyPressed && !_isWalking)
        {
            WalkingAnim();
        }
        else if (!moveKeyPressed && _isWalking)
        {
            Stop();
        }

        Debug.Log("isWalking:" + _animator.GetBool(IsWalkingHash));

        _animator.SetBool("isWalking", moveKeyPressed);
    }

    void WalkingAnim()
    {
        _isWalking = true;
        if (_animator != null)
        {
            _animator.SetBool(IsWalkingHash, true);
        }
    }

    void Stop()
    {
        _isWalking = false;
        if (_animator != null)
        {
            _animator.SetBool(IsWalkingHash, false);
        }
    }
}