using System.Collections;
using System;
using UnityEngine;
using UnityEngine.UI;
using ZXing;
using TMPro;
using Microsoft.MixedReality.Toolkit.Input;

public class CodeScanner : MonoBehaviour
{
    // Public fields
    [Header("TextMeshPro")]
    [SerializeField] private TextMeshPro console;

    // GameObejct's component
    [Header("GameObject's Components")]
    [SerializeField] private SpeechInputHandler qrCodeListener;
    [SerializeField] private SpeechInputHandler ocrListener;

    // Visual fields
    [Header("Materials")]
    [SerializeField] private Material successGreen;
    [SerializeField] private Material redFailure;
    [SerializeField] private Material grayOriginal;

    [Header("Console")]
    [SerializeField] private GameObject visualBackground;
    [SerializeField] private GameObject failureIcon;
    [SerializeField] private GameObject successIcon;

    // Instance
    private CodeScanner scanner;

    // Auxiliar variables
    private WebCamTexture _webcamTexture;
    string outcome = string.Empty;

    // Activates the camera and makes it try to recognize a code at all time
    public void StartAutoDetectMode()
    {
        // Setting instance
        scanner = this;

        // Configuring scene
        visualBackground.GetComponent<Renderer>().material = grayOriginal;
        failureIcon.SetActive(false);
        successIcon.SetActive(false);

        // Displaying instructions to user
        console.text = "Sua câmera está ligada.\nProcure pelo código e fixe a câmera nele até identificá-lo.";

        // Activating SpeechHandler for QRCode and deactivating it from OCR
        try
        {
            qrCodeListener.enabled = true;
            ocrListener.enabled = false;

            Debug.Log("SpeechInputHandler from QRCode: " + qrCodeListener.isActiveAndEnabled + " & SpeechInputHandler from OCR: " + ocrListener.isActiveAndEnabled);
        }
        catch (Exception ex)
        {
            Debug.Log("SpeechInputHandler error! " + ex.Message);
        }

        _webcamTexture = new WebCamTexture(Screen.width, Screen.height);

        // Startin auto-detect
        StartCoroutine(GetQRCodeIE());
    }
    
    // Reload camera view and start auto-detect again
    public void Reload()
    {
        console.text = "Sua câmera está ligada.\nProcure pelo código e fixe a câmera nele até identificá-lo.";
        StartCoroutine(GetQRCodeIE());
    }

    // Auto-detect any code
    IEnumerator GetQRCodeIE()
    {
        // Creating a barcode reader
        IBarcodeReader barCodeReader = new BarcodeReader();

        _webcamTexture.Play();

        // Texture with camera's view
        var cameraView = new Texture2D(_webcamTexture.width, _webcamTexture.height, TextureFormat.ARGB32, false);

        // While the outcome is null, try to get some response
        while (_webcamTexture.isPlaying)
        {
            try
            {
                if (Snap(cameraView, barCodeReader)) break;
            }
            catch (Exception ex) { Debug.LogWarning(ex.Message); }
            yield return null;
        }
    }


    // Reading the code data
    bool Snap(Texture2D cameraView, IBarcodeReader barCodeReader)
    {
        // Setting camera's view to Texture2D
        cameraView.SetPixels32(_webcamTexture.GetPixels32());

        // Decoding image using ZXing library
        var codeResult = barCodeReader.Decode(cameraView.GetRawTextureData(), _webcamTexture.width, _webcamTexture.height, RGBLuminanceSource.BitmapFormat.ARGB32);

        // Dealing with the result
        if (codeResult != null)
        {
            // Getting result from decoder
            outcome = codeResult.Text;

            // Logging result for user
            if (!string.IsNullOrEmpty(outcome))
            {
                console.text = outcome;

                // Verifying outcome with parameter from database
                if (outcome.Contains("https://www.microsoft.com/en-us/hololens"))
                {
                    console.text = outcome + "\n\nO código encontrado está de acordo com o requisitado";
                    visualBackground.GetComponent<Renderer>().material = successGreen;
                    failureIcon.SetActive(false);
                    successIcon.SetActive(true);
                }
                else
                {
                    console.text = outcome + "\n\nO código encontrado não está de acordo. Diga 'Reload' para reiniciar.";
                    visualBackground.GetComponent<Renderer>().material = redFailure;
                    failureIcon.SetActive(true);
                    successIcon.SetActive(false);
                }

                return true;
            }
        }

        return false;
    }
}
