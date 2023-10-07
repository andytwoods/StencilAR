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
        Renderer m_Renderer = image.GetComponent<Renderer>();
        if(texture == null)
        {
            texture = getTexture(m_Renderer.material);
        }
        if(texture == null)
        {
            Debug.Log("NO Texture on Graffiti object");
        }

        m_Renderer.material.SetTexture("_MainTex", texture);

        float image_width = texture.width, image_height = texture.height;
        float unit_dimension = .1f;
        float image_scale_width = unit_dimension;
        float image_scale_height = image_height / image_width * unit_dimension;


        Debug.Log(image_scale_width.ToString() + " " + image_scale_height.ToString());
        image.transform.localScale = new Vector3(.1f, image_scale_width, image_scale_height);

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
