using System.Collections;
using UnityEngine.Networking;
using UnityEngine;

public class ImageManager : MonoBehaviour
{

    public GameObject image;
    public TextMesh retrieveInstructions;

    void Start()
    {
        retrieveInstructions.text = UrlSuffix + BaseURL();
        PlaneSetTextureAndResize();
        //RetrieveBtn();
  
    }

    // below used for testing rotation issues without needing to use a headset
    //public void Update()
    //{
    //    image.transform.Rotate(Vector3.up * 100 *  Time.deltaTime);
    //}


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
    }


    private string UrlSuffix = "www.pubpub.social/gmr/";

    public void RetrieveBtn()
    {
        string url = UrlSuffix + BaseURL() + "img/";
        StartCoroutine(DownloadImage(url));
    }

    IEnumerator DownloadImage(string MediaUrl)
    {
        UnityEngine.Networking.UnityWebRequest request = UnityEngine.Networking.UnityWebRequestTexture.GetTexture(MediaUrl);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log("NO PHOTO TO DOWNLOAD (" + MediaUrl + ") -- " + request.error);
        else
        {
            Texture texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            PlaneSetTextureAndResize(texture);

        }
    }

    private string BaseURL()
    {
        return SystemInfo.deviceUniqueIdentifier.ToString().Substring(0, 4).ToLower() + "/";
    }
}
