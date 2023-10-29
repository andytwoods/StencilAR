using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class PassthroughManager : MonoBehaviour
{
    public OVRPassthroughLayer passthrough;
    public List<Gradient> colourMapGradient;
    public OVRInput.Button button;
    public OVRInput.Controller controller;
    public GameObject imageGameObject;
    public UnityEngine.UI.Image activeImage;
    public TextMesh my_text;

    private GameObject front_face;
    private GameObject back_face;

    // Start is called before the first frame update
    void Start()
    {
        GetFaces();

    }

    // used for debugging issues without having to connect the Quest
    void Update()
    {
        //imageGameObject.transform.Rotate(Vector3.up * 100 *  Time.deltaTime);
        //SetImageSaturation(Random.Range(0f, 1f));
    }

    public void SetOpacity(float value)
    {
        passthrough.textureOpacity = value;
    }


    public void SetRealWorldColor(string col)
    {

        passthrough.overridePerLayerColorScaleAndOffset = true;

        if (col == "red")
        {
            passthrough.colorScale = new Vector4(1f, 0f, 0f, 1f);
        }
        else if (col == "green")
        {

            passthrough.colorScale = new Vector4(0f, 1f, 0f, 1f);
        }

        else if (col == "blue")
        {
            passthrough.colorScale = new Vector4(0f, 0f, 1f, 1f);
        }

        else
        {
            passthrough.colorScale = new Vector4(1f, 1f, 1f, 1f);
        }
    }

    public void SetRealWorldContrast(float val)
    {

        passthrough.colorMapEditorContrast = val;
    }

    public void SetRealWorldEdgeColour(string col)
    {

        passthrough.edgeRenderingEnabled = col != "reset";
        passthrough.edgeColor = getColor(col);
    }

    // https://blog.immersive-insiders.com/oculus-passthrough-part-3/
    public void SetRealWorldPoster(float val)
    {
        passthrough.colorMapEditorPosterize = val;
    }

    public void GetFaces()
    {
        Transform[] all_children = imageGameObject.transform.GetComponentsInChildren<Transform>();


        bool is_front = true;

        for (var i = 0; i < all_children.Length; i++)
        {
            GameObject image_side = all_children[i].gameObject;
            if (image_side.name == imageGameObject.name)
            {
                continue;
            }
            if (is_front)
            {
                is_front = false;
                front_face = image_side;
            }
            else
            {
                back_face = image_side;
            }
        }
    }


    public void SetImageColor(float val)
    {
        front_face.GetComponent<Renderer>().material.SetFloat("_Hue", val);
        back_face.GetComponent<Renderer>().material.SetFloat("_Hue", val);
    }

    public void SetImageTransparency(float val)
    {
        front_face.GetComponent<Renderer>().material.SetFloat("_Transparency", val);
        back_face.GetComponent<Renderer>().material.SetFloat("_Transparency", val);
    }

    public void SetImageBrightness(float val)
    {
        front_face.GetComponent<Renderer>().material.SetFloat("_Brightness", val);
        back_face.GetComponent<Renderer>().material.SetFloat("_Brightness", val);
    }

    public void SetImageContrast(float val)
    {
        front_face.GetComponent<Renderer>().material.SetFloat("_Contrast", val);
        back_face.GetComponent<Renderer>().material.SetFloat("_Contrast", val);
    }


    public void SetImageSaturation(float val)
    {
        front_face.GetComponent<Renderer>().material.SetFloat("_Saturation", val);
        back_face.GetComponent<Renderer>().material.SetFloat("_Saturation", val);
    }

    private Color getColor(string col)
    {
        if (col == "red") { 
            return new Color(1f, 0f, 0f, 1f);
        }
        if (col == "green")
        {

            return new Color(0f, 1f, 0f, 1f);
        }

        if (col == "blue")
        {
            return new Color(0f, 0f, 1f, 1f);
        }

        return new Color(0f, 1f, 1f, 1f);
        
    }

}
