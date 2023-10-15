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

    // Start is called before the first frame update
    void Start()
    {
        //my_text.text = "34433 " + imageGameObject.ToString();

    }

    // Update is called once per frame
    void Update()
    {
        //if (OVRInput.GetDown(button, controller))
        //{
        //    passthrough.hidden = !passthrough.hidden;
        //}
    }

    public void SetOpacity(float value)
    {
        passthrough.textureOpacity = value;
    }

    public void SetImage(UnityEngine.UI.Image image)
    {
        activeImage = image;
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


    Material getImageMaterial()
    {
        return imageGameObject.GetComponent<Renderer>().material;
    }

    public void SetImageColor(float val)
    {
        Material m = getImageMaterial();
        m.SetFloat("_Hue", val);
    }

    public void SetImageTransparency(float val)
    {
        Material m = getImageMaterial();
        m.SetFloat("_Transparency", val);
    }

    public void SetImageBrightness(float val)
    {
        Material m = getImageMaterial();
        m.SetFloat("_Brightness", val);
    }

    public void SetImageContrast(float val)
    {
        Material m = getImageMaterial();
        m.SetFloat("_Contrast", val);
    }


    public void SetImageSaturation(float val)
    {
        Material m = getImageMaterial();
        m.SetFloat("_Saturation", val);
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
