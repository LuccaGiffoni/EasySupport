using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using Newtonsoft.Json;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Windows.WebCam;
using System;
using Microsoft.MixedReality.Toolkit.Input;

public class CharacterRecognitionAPI : MonoBehaviour
{
    // Computer Vision information
    [Header("Azure Information")]
    public string subKey = "186ce7f1f0994ee294c2da4a70fb931d";
    public string url = "https://easysupportvision.cognitiveservices.azure.com/";

    // Visual fields
    [Header("Materials")]
    [SerializeField] private Material successGreen;
    [SerializeField] private Material redFailure;
    [SerializeField] private Material grayOriginal;

    [Header("Game Objects")]
    [SerializeField] private GameObject visualBackground;
    [SerializeField] private GameObject failureIcon;
    [SerializeField] private GameObject successIcon;
    [SerializeField] private TextMeshPro response;

    // Instance
    public static CharacterRecognitionAPI instance;

    // texture which displays what our camera is seeing
    private WebCamTexture camTex;

    // Set-up scene before starting playing
    private void Awake()
    {
        // Setting instance
        instance = this;

        // Setting all GameObjects to scene default
        response.text = "Diga 'Start' para ligar a câmera";
        failureIcon.SetActive(false);
        successIcon.SetActive(false);
        visualBackground.GetComponent<Renderer>().material = grayOriginal;
    }
    
    // Main method to start all proccess of OCR recognition
    public void SendImageToAzure()
    {
        StartCoroutine(TakePicture());
    }

    #region Camera Controllers

    // Activate HoloLens' camera
    public void TurnOnCamera()
    {
        // Create the camera texture
        camTex = new WebCamTexture(Screen.width, Screen.height);
        camTex.Play();

        // Let the user know what to do next
        response.text = "Diga 'Read' para usar o OCR";
    }

    // Restarting camera view
    public void ReloadCameraView()
    {
        // Restarting camera
        camTex.Stop();
        camTex.Play();

        // Let the user know what to do next
        response.text = "Diga 'Read' para usar o OCR";

        // Setting scene back to default
        failureIcon.SetActive(false);
        successIcon.SetActive(false);
        visualBackground.GetComponent<Renderer>().material = grayOriginal;
    }

    #endregion

    #region IEnumerators

    // takes a picture and converts the data to a byte array
    // then triggers the AppManager.GetImageData method
    IEnumerator TakePicture()
    {
        yield return new WaitForEndOfFrame();

        // create a new texture the size of the web cam texture
        Texture2D screenTex = new Texture2D(camTex.width, camTex.height);

        // read the pixels on the web cam texture and apply them
        screenTex.SetPixels(camTex.GetPixels());
        screenTex.Apply();

        // convert the texture to PNG, then get the data as a byte array
        byte[] byteData = screenTex.EncodeToPNG();

        // send the image data off to the Computer Vision API
        StartCoroutine(GetImageData(byteData));
    }

    // Sends the image to the Computer Vision API and returns a JSON file
    public IEnumerator GetImageData(byte[] imageData)
    {
        string urlFree = url + "vision/v3.2/ocr";

        response.text = "Processando imagem ...";

        // Create a new web request and set the method to POST
        UnityWebRequest webReq = new(urlFree);
        webReq.method = UnityWebRequest.kHttpVerbPOST;

        // Create a download handler to receive the JSON file
        webReq.downloadHandler = new DownloadHandlerBuffer();

        // Upload the image data
        webReq.uploadHandler = new UploadHandlerRaw(imageData)
        {
            contentType = "application/octet-stream"
        };

        // Set the header
        webReq.SetRequestHeader("Ocp-Apim-Subscription-Key", subKey);

        // send the content to the API and wait for a response
        // yield return webReq.SendWebRequest();

        var requisition = webReq.SendWebRequest();

        while (!requisition.isDone)
        {
            response.text = "Carregando (" + requisition.progress * 100 + "%)";

            if (requisition.isDone)
            {
                response.text = webReq.downloadHandler.text;
                Debug.Log(webReq.downloadHandler.text);

                // Convert the content string to a JSON file
                Newtonsoft.Json.Linq.JObject jsonData = Newtonsoft.Json.Linq.JObject.Parse(webReq.downloadHandler.text);

                // get just the text from the JSON file and display on-screen
                try
                {
                    string jsonText = null;

                    if (jsonData.PropertyValues().Contains("unk"))
                    {
                        Debug.Log("JSON Data is empty");
                        jsonText = ("Não foi possível retirar texto algum da imagem.");
                    }
                    else
                    {
                        jsonText = GetTextFromJSON(jsonData);
                    }

                    response.text = jsonText.ToString();
                }
                catch (JsonException e)
                {
                    response.text = e.Message + "\n\n Ocorreu um erro na leitura da imagem. Por favor, tente novamente, dizendo 'Read'.";
                    Debug.Log(e.Message);
                }

                // send the text to the text to speech API
                //TextToSpeech.instance.StartCoroutine("GetSpeech", imageText);
                if (webReq.downloadHandler.text.ToLower().Contains("Ricardo".ToLower()))
                {
                    response.text += "\n\nO código foi encontrado";
                    visualBackground.GetComponent<Renderer>().material = successGreen;
                    successIcon.SetActive(true);
                }
                else
                {
                    response.text += "\n\nO código não foi encontrado. Diga 'Reload' para reiniciar.";
                    visualBackground.GetComponent<Renderer>().material = redFailure;
                    failureIcon.SetActive(false);
                }

                camTex.Stop();
                yield return requisition.webRequest;
                break;
            }
            else
            {
                continue;
            }
        }
    }

    // Returns the text from the JSON data
    private string GetTextFromJSON(Newtonsoft.Json.Linq.JObject jsonData)
    {
        string text = "";
        var lines = jsonData["regions"][0]["lines"];

        // loop through each line
        foreach (Newtonsoft.Json.Linq.JToken line in lines)
        {
            // loop through each word in the line
            foreach (Newtonsoft.Json.Linq.JToken word in line["words"])
            {
                // add the text
                text += word["text"].ToString().ToLower() + " ";
            }
        }

        return text;
    }

    #endregion
}