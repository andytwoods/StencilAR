using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InteractionManager : MonoBehaviour
{
    public OVRPassthroughLayer passthough;
    private OVRInput.Controller r_controller = OVRInput.Controller.RTouch;
    private OVRInput.Button button = OVRInput.Button.PrimaryIndexTrigger;
    public TextMesh my_text;
    public List<Gradient> colourMapGradient;
    public ToggleGroup orientationToggle;

    public GameObject imageGameObject;

    GameObject hoverObject = null;
    GameObject grabObject = null;
    GameObject prevObject = null;

    // all-purpose timer to use for blending after object is grabbed/released
    float grabTime = 0.0f;
    // the grabbed object's transform relative to the controller
    Vector3 localGrabOffset = Vector3.zero;
    Quaternion localGrabRotation = Quaternion.identity;
    // the camera and grabbing hand's world position when grabbing
    Vector3 camGrabPosition = Vector3.zero;
    Quaternion camGrabRotation = Quaternion.identity;
    Vector3 handGrabPosition = Vector3.zero;
    Quaternion handGrabRotation = Quaternion.identity;
    Vector3 cursorPosition = Vector3.zero;
    float rotationOffset = 0.0f;
    private string default_orientation = "upright";
    private string orientation = "upright";


    private void Start()
    {
        Toggle[] toggles = orientationToggle.GetComponentsInChildren<Toggle>(); 

        foreach (Toggle toggle in toggles)
        {
            toggle.onValueChanged.AddListener((value) => OrientationChanged(toggle, value));

        }
    }

    void _rotateGameObjectX(float angle)
    {
        Transform t = imageGameObject.transform;
        t.rotation = Quaternion.Euler(angle, t.eulerAngles.y, t.eulerAngles.z);
    }

    void OrientationChanged(Toggle toggle, bool state)
    {
        if (toggle.name == "Upright")
        {
            orientation = "upright";
            _rotateGameObjectX(0);
            
        }


        else if (toggle.name == "Table")
        {
            orientation = "table";
            _rotateGameObjectX(90);
        }
        else throw new System.Exception();
        

    }


    // Update is called once per frame
    void Update()
    {
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, r_controller);
        //selected.transform.Rotate(Vector3.up * -thumbstick.x);

        Vector3 controllerPos = OVRInput.GetLocalControllerPosition(r_controller);
        Quaternion controllerRot = OVRInput.GetLocalControllerRotation(r_controller);

        FindHoverObject(controllerPos, controllerRot);

        if (hoverObject)
        {
            if (OVRInput.GetDown(button, r_controller))
            {
                // grabbing
                grabObject = checkIfParentGameObject(hoverObject);
                GrabHoverObject(grabObject, controllerPos, controllerRot);
            }
        }
        else {
            DeselectObject();
        }

        if (OVRInput.GetUp(button, r_controller))
        {
            DeselectObject();
        }


        if (grabObject)
        {
            grabTime += Time.deltaTime * 5;
            grabTime = Mathf.Clamp01(grabTime);
            ManipulateObject(grabObject, controllerPos, controllerRot);
        }

    }

    void DeselectObject()
    {
        if (grabObject) prevObject = grabObject;
        grabObject = null;
    }



    void GrabHoverObject(GameObject grbObj, Vector3 controllerPos, Quaternion controllerRot)
    {
        localGrabOffset = Quaternion.Inverse(controllerRot) * (grabObject.transform.position - controllerPos);
        localGrabRotation = Quaternion.Inverse(controllerRot) * grabObject.transform.rotation;
        rotationOffset = 0.0f;
        if (grabObject.GetComponent<GrabObject>())
        {
            grabObject.GetComponent<GrabObject>().Grab(r_controller);
            grabObject.GetComponent<GrabObject>().grabbedRotation = grabObject.transform.rotation;

        }
        handGrabPosition = controllerPos;
        handGrabRotation = controllerRot;
        camGrabPosition = Camera.main.transform.position;
        camGrabRotation = Camera.main.transform.rotation;
    }

    public void Scale(float value)
    {
        GameObject myObj = grabObject ? grabObject : prevObject;
        if (!myObj) return;
        float new_scale = myObj.transform.localScale.x + value;
        if (new_scale < .005f)
        {
            new_scale = .005f;
        }
        else if (new_scale > 4)
        {
            new_scale = 4;
        }
        myObj.transform.localScale = new Vector3(new_scale, new_scale, new_scale);

    }



    //min -180, max 180
    public void Rotation(float value)
    {
        GameObject myObj = grabObject ? grabObject : prevObject;
        if (!myObj) return;

        myObj.transform.Rotate(Vector3.right * value);

    }

    //min -180, max 180
    public void Tilt(float value)
    {
        GameObject myObj = grabObject ? grabObject : prevObject;
       
        if (!myObj) return;

        myObj.transform.Rotate(Vector3.forward * value);

    }
    public void TiltFixed(float value){
        myObj.transform.Rotation
    }





    Vector3 ClampScale(Vector3 localScale, float thumb_y)
    {
        float newscaleX = localScale.x + thumb_y;
        float newscaleY = localScale.y + thumb_y;
        float newscaleZ = localScale.z + thumb_y;


        //my_text.text = newscaleX.ToString() + " " + newscaleY.ToString() + " " + newscaleZ.ToString();

        return new Vector3(newscaleX, newscaleY, newscaleZ);
    }

    public void SetRealWorldColourMapGradient(int index)
    {
        passthough.colorMapEditorGradient = colourMapGradient[index];
    }

    void FindHoverObject(Vector3 controllerPos, Quaternion controllerRot)
    {
        RaycastHit[] objectsHit = Physics.RaycastAll(controllerPos, controllerRot * Vector3.forward);
        float closestObject = Mathf.Infinity;
        float rayDistance = 2.0f;
        bool showLaser = true;
        Vector3 labelPosition = Vector3.zero;
        foreach (RaycastHit hit in objectsHit)
        {
            float thisHitDistance = Vector3.Distance(hit.point, controllerPos);
            if (thisHitDistance < closestObject)
            {
                hoverObject = hit.collider.gameObject;
                closestObject = thisHitDistance;
                rayDistance = grabObject ? thisHitDistance : thisHitDistance - 0.1f;
            }
        }

        if (objectsHit.Length == 0)
        {
            hoverObject = null;
        }


        // if intersecting with an object, grab it
        Collider[] hitColliders = Physics.OverlapSphere(controllerPos, 0.05f);
        foreach (var hitCollider in hitColliders)
        {
            // use the last object, if there are multiple hits.
            // If objects overlap, this would require improvements.
            hoverObject = checkIfParentGameObject(hitCollider.gameObject);
        }

    }

    GameObject checkIfParentGameObject(GameObject go)
    {
        if(go.transform.parent == null)
        {
            return go;
        }
        return go.transform.parent.gameObject;
    }

    // the heavy lifting code for moving objects
    void ManipulateObject(GameObject obj, Vector3 controllerPos, Quaternion controllerRot)
    {
        bool useDefaultManipulation = true;
        Vector2 thumbstick = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick, r_controller);

        if (obj.GetComponent<GrabObject>())
        {
            useDefaultManipulation = false;
            switch (obj.GetComponent<GrabObject>().objectManipulationType)
            {
                case GrabObject.ManipulationType.Default:
                    useDefaultManipulation = true;
                    break;
                case GrabObject.ManipulationType.ForcedHand:
                    obj.transform.position = Vector3.Lerp(obj.transform.position, controllerPos, grabTime);
                    obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, controllerRot, grabTime);
                    break;
                case GrabObject.ManipulationType.DollyHand:
                    float targetDist = localGrabOffset.z + thumbstick.y * 0.01f;
                    targetDist = Mathf.Clamp(targetDist, 0.1f, 2.0f);
                    localGrabOffset = Vector3.forward * targetDist;
                    obj.transform.position = Vector3.Lerp(obj.transform.position, controllerPos + controllerRot * localGrabOffset, grabTime);
                    obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, controllerRot, grabTime);
                    break;
                case GrabObject.ManipulationType.DollyAttached:
                    obj.transform.position = controllerPos + controllerRot * localGrabOffset;
                    obj.transform.rotation = controllerRot * localGrabRotation;
                    ClampGrabOffset(ref localGrabOffset, thumbstick.y);
                    break;
               
                case GrabObject.ManipulationType.Menu:
                    Vector3 targetPos = handGrabPosition + (handGrabRotation * Vector3.forward * 0.4f);
                    Quaternion targetRotation = Quaternion.LookRotation(targetPos - camGrabPosition);
                    obj.transform.position = Vector3.Lerp(obj.transform.position, targetPos, grabTime);
                    obj.transform.rotation = Quaternion.Lerp(obj.transform.rotation, targetRotation, grabTime);
                    break;
                default:
                    useDefaultManipulation = true;
                    break;
            }
        }

        if (useDefaultManipulation)
        {
            obj.transform.position = controllerPos + controllerRot * localGrabOffset;
            if (orientation == default_orientation) { obj.transform.Rotate(Vector3.forward * thumbstick.x); }
            else obj.transform.Rotate(Vector3.up * thumbstick.x);
            ClampGrabOffset(ref localGrabOffset, thumbstick.y);
        }
    }

    void ClampGrabOffset(ref Vector3 localOffset, float thumbY)
    {
        Vector3 projectedGrabOffset = localOffset + Vector3.forward * thumbY * 0.01f;
        if (projectedGrabOffset.z > 0.1f)
        {
            localOffset = projectedGrabOffset;
        }
    }

}
