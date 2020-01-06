using UnityEngine;
using UnityEngine.UI;

public class PopupStat : MonoBehaviour
{
    public float autoDestroytime = 1.5f;
    public Text textField = null;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.Destroy(this.gameObject, autoDestroytime);
    }

    //Orient the camera after all movement is completed this frame to avoid jittering
    void LateUpdate()
    {
        ShowFaceToCamera();
    }

    //Billboard
    void ShowFaceToCamera()
    {
        //Rotate towards the camera
        transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
           Camera.main.transform.rotation * Vector3.up);
    }

    public void WriteText(string text)
    {
        textField.text = text;
    }
}
