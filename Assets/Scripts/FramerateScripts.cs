using UnityEngine;

public class FramerateScripts : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Application.targetFrameRate = 120; //3Dモデルを動かすため120フレーム制限
    }
}
