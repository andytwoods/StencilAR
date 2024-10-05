using System.Collections;
using UnityEngine.Networking;
using UnityEngine;
using System.IO;
using System;

public class ImageManager : MonoBehaviour
{

    public GameObject image;
    public TextMesh retrieveInstructions;
    private string status = "";
    private ImageInfo image_info;
    private string image_url;

    void Start()
    {
        PlaneSetTextureAndResize();
        SetupButton();
        // Invoke("RetrieveBtn", 1);
        // Invoke("RetrieveBtn", 3);
        
    }

    // below used for testing rotation issues without needing to use a headset
    //public void Update()
    //{
    //    image.transform.Rotate(Vector3.up * 100 *  Time.deltaTime);
    //}

    void SetupButton()
    {
        retrieveInstructions.text = "Click 'Go!' to get a temporary web address \nwhere you can tell StencilAR where\n" +
                                    " to find your image. You have 5 minutes\n to visit that address once you have clicked";
    }


    Texture getTexture(Material material)
    {
        Shader shader = material.shader;

        for (int i = 0; i < shader.GetPropertyCount(); i++)
        {
            string propertyName = shader.GetPropertyName(i);
            var propertyType = shader.GetPropertyType(i);

            if (propertyType == UnityEngine.Rendering.ShaderPropertyType.Texture)
            {
                return material.GetTexture(propertyName);
            }
        }
        return null;
    }

    void PlaneSetTextureAndResize(Texture texture = null)
    {
        image.SetActive(false);

        string parent_name = image.name;

        Transform[] image_children = image.transform.GetComponentsInChildren<Transform>();

        bool is_front = true;

        for (var i = 0; i < image_children.Length; i++)
        {
            GameObject image_side = image_children[i].gameObject;
            if (image_side.name == parent_name) continue;

            Renderer m_Renderer = image_side.GetComponent<Renderer>();
            if (texture == null)
            {
                texture = getTexture(m_Renderer.material);
            }

            m_Renderer.material.SetTexture("_MainTex", texture);

            float image_width = texture.width;
            float image_height = texture.height;
            float unit_dimension = .1f;
            float image_scale_width = unit_dimension;
            float image_scale_height = image_height / image_width * unit_dimension;

        
            if (is_front)
            {
                image_side.transform.localScale = new Vector3(.1f, image_scale_width, image_scale_height);
                is_front = false;
            }
            else
            {
                image_side.transform.localScale = new Vector3(.1f, -image_scale_width, image_scale_height);
            }

        }
        image.SetActive(true);
        image.transform.Rotate(Vector3.up * 100 *  Time.deltaTime);
    }


    private string UrlSuffix = "http://127.0.0.1:8000/"; // "https://stencilar.com/"; 
    // "https://stencilar.com/";

    public void RetrieveBtn()
    {
        if (status == "")
        {
            status = "requested";
            retrieveInstructions.text = "please wait...";
            StartCoroutine(GetCode());
        }
        else if (status == "requested")
        {
            retrieveInstructions.text = "please wait...";
        }
        else if (status == "good to download")
        {
            retrieveInstructions.text = "Click to download your image";
            StartCoroutine(DownloadImage());
        }
        else
        {
            throw new Exception();
        }


        
        
    }
    
    public class ImageInfo
    {
        public string code;
        public string url;
    }
    
    IEnumerator GetCode()
    {
        string MediaUrl = UrlSuffix + "code/" + headset_id() + "/";
        
        UnityEngine.Networking.UnityWebRequest request = UnityWebRequest.Get(MediaUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            retrieveInstructions.text = "No success connecting\nto the backend (" + UrlSuffix + ").\nAre you connected to wifi?\nClick 'Go!' to try again";
            status = "";
        }
        else
        {
            image_info = JsonUtility.FromJson<ImageInfo>(request.downloadHandler.text);
            retrieveInstructions.text =
                "Visit " + image_info.url + "\nThere, tell StencilAR where your image\nis. After, click 'Go!' to download\nthat image" +
                " to your headset";
            status = "good to download";
        }
    }

    IEnumerator DownloadImage()
    {
        string MediaUrl = UrlSuffix + "img/" + headset_id() + "/";
        Debug.Log("NO PHOTO TO DOWNLOAD (" + image_url + ") -- " + MediaUrl);
        retrieveInstructions.text = "Requesting image link..." ;
        
        UnityEngine.Networking.UnityWebRequest request = UnityWebRequest.Get(MediaUrl);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            status = "";
            SetupButton();
        }
        
        else if (request.downloadHandler.text == "no image")
        {
            Debug.Log("NO PHOTO TO DOWNLOAD (" + image_url + ") -- " + request.error);
            retrieveInstructions.text = "Could not find a url to an image!\n Please visit this location to add your url: \n" + image_info.url;
        }

        else
        {
            image_url = request.downloadHandler.text;
            retrieveInstructions.text = "Retrieved image link...\nDownloading image...";
            Debug.Log(image_url);
            UnityEngine.Networking.UnityWebRequest image_request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(image_url);
            yield return image_request.SendWebRequest();
            
            if (image_request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log("1111");
                retrieveInstructions.text = "We are having trouble opening that image.\nCan you check if it loads\nin your browser?";
            }
            else
            {
                while (!image_request.isDone)
                {
                    yield return image_request;
                }
                
                Debug.Log("IMAGE DOWNLOAD SUCCESS");
                Texture texture = ((DownloadHandlerTexture)image_request.downloadHandler).texture;
                
     
                Debug.Log(texture);
                retrieveInstructions.text = "Downloaded image! Download again?";
                PlaneSetTextureAndResize(texture);
            }
        }
    }


    
    private string headset_id()
    {
        return SystemInfo.deviceUniqueIdentifier.ToString().ToLower();
    }
}
