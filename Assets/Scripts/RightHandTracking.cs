using UnityEngine;
using Oculus;

public class TrackRightController : MonoBehaviour
{
    // Reference to the object that needs to move
    public GameObject quest2_controllers_div;

    void Update()
    {
        // Get the right controller position and rotation
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Quaternion rightControllerRotation = OVRInput.GetLocalControllerRotation(OVRInput.Controller.RTouch);

        // Update the position and rotation of the quest2_controllers_div
        quest2_controllers_div.transform.position = rightControllerPosition;
        quest2_controllers_div.transform.rotation = rightControllerRotation;
    }
}