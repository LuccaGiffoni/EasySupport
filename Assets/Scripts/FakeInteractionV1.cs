using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Input;

public class FakeInteractionV1 : MonoBehaviour
{
    // Fields to all scripts access
    public static Operation activeOperation;

    // List to allocate all operations
    public static List<Operation> operations;

    // Public fields
    [Header("Main Menu")]
    [SerializeField] private TextMeshPro instruction;
    [SerializeField] private TextMeshPro header;

    [Header("Functions")]
    [SerializeField] private GameObject ocr;
    [SerializeField] private GameObject qrcode;
    [SerializeField] private GameObject measure;

    [Header("General GameObjects")]
    [SerializeField] private Image image;
    [SerializeField] private Material imageMaterial;
    [SerializeField] private GameObject imageCanvas;
    [SerializeField] private GameObject epiParent;
    [SerializeField] private GameObject commentBox;
    [SerializeField] private TextMeshProUGUI commentText;
    [SerializeField] private Slider progressBar;

    [Header("Buttons")]
    [SerializeField] private GameObject nextButton;
    [SerializeField] private GameObject previousButtonBackplate;
    [SerializeField] private GameObject previousButton;
    [SerializeField] private Material originalColor;

    // GameObejct's component
    [Header("SpeechInputHandlers")]
    [SerializeField] private SpeechInputHandler qrCodeListener;
    [SerializeField] private SpeechInputHandler ocrListener;
    [SerializeField] private SpeechInputHandler measureListener;

    // Instantiate EPIs
    [Header("Grid view position")]
    public float x_Start, y_Start;
    public int ColumnLength;
    public float x_Space, y_Space;

    // Progress Bar's auxiliar variables
    private float fraction = 1f;

    // Private class for Operations
    public class Operation
    {
        public string Instruction { get; private set; }
        public string Description { get; private set; }
        public int Function { get; private set; }
        public int Index { get; private set; }
        public string Parameter { get; private set; }
        public string Equipment { get; private set; }
        public string[] EPIs { get; private set; }
        public string Video { get; set; }
        public string Image { get; set; }
        public int Result { get; set; }
        public string Comment { get; internal set; }

        public Operation() { }

        public Operation(string instruction, string description, int function, int index)
        {
            Instruction = instruction;
            Description = description;
            Function = function;
            Index = index;
        }

        public Operation(string instruction, string description, int function, int index, string parameter) : this(instruction, description, function, index) => Parameter = parameter;

        public Operation(string instruction, string description, int function, int index, string equipment, string[] ePIs) : this(instruction, description, function, index)
        {
            Equipment = equipment;
            EPIs = ePIs;
        }
    }

    private void GetSceneBackToOriginal()
    {
        DestroyAllEpis();
        imageCanvas.SetActive(false);
        nextButton.GetComponent<Renderer>().material = originalColor;
        previousButtonBackplate.GetComponent<Renderer>().material = originalColor;
        previousButton.GetComponent<Interactable>().IsEnabled = true;
    }

    // Creating fake operations and adding it to a list
    private void Start()
    {
        // Configuring visual
        GetSceneBackToOriginal();
        fraction = 1f;
        previousButton.GetComponent<Interactable>().IsEnabled = false;
        previousButtonBackplate.GetComponent<Renderer>().material.color = Color.gray;
        qrCodeListener.enabled = false;
        ocrListener.enabled = false;
        measureListener.enabled = false;

        // Creating a new list
        operations = new List<Operation>();

        // Creating a EPIs array
        string[] epis = { "Boots", "Gloves", "Mask" };

        // Creating operations
        var op1 = new Operation("Verifique, no seu menu pulso, os EPIs necessários para a operação e o equipamento em 3D", "Verificação de EPIs", 0, 1, "Machine", epis);
        var op2 = new Operation("Com os EPIs, retire o equipamento da tomada. Caso necessário, veja o vídeo de demonstração, através do seu menu de pulso.", "Remoção da energia", 0, 2) { Video = "https://www.youtube.com/watch?v=eqFqtAJMtYE" };
        var op3 = new Operation("Valide a etiqueta de número de série do equipamento - veja a imagem para ajudar a encontrá-la.", "Leitura da etiqueta de série", 1, 3, "EQP001") { Image = "MachineImage" };
        var op4 = new Operation("Valide o código QR de identificador do equipamento", "Validação de Código QR", 2, 4, "SRS001");
        var op5 = new Operation("Meça o tamanho da tampa. Deve ser maior do que 20 centímetros.", "Medição", 3, 5, "20");
        var op6 = new Operation("Guarde todos os seus EPIs e ferramentas e encerre o processo de inspeção", "Finalização da Ordem de Serviço", 0, 6);

        // Adding operations to list
        operations.Add(op1);
        operations.Add(op2);
        operations.Add(op3);
        operations.Add(op4);
        operations.Add(op5);
        operations.Add(op6);

        // Ordering list by index
        operations = operations.OrderBy(o => o.Index).ToList();

        // Setting activeOperation and configuring scene usinf first operation
        activeOperation = operations[0];
        ConfigureScene(activeOperation, "default");

        // Configuring progressBar
        fraction /= operations.Count();
        progressBar.value = 0;
    }

    // Configure scene for each operation
    private void ConfigureScene(Operation operation, string moveParameter)
    {
        GetSceneBackToOriginal();

        GameObject monitor = GameObject.Find("Placa(Clone)");
        Destroy(monitor);

        // Setting instruction for user
        instruction.text = operation.Instruction;
        header.text = "Passo " + operation.Index + " | " + operation.Description;

        // Setting progressBar
        switch (moveParameter)
        {
            case "next": progressBar.value += fraction; break;
            case "previous": progressBar.value -= fraction; break;
            case "default": progressBar.value = 0; break;
        }

        // Switch case to set function to scene
        switch (operation.Function)
        {
            case 0:
                ocr.SetActive(false);
                qrcode.SetActive(false);
                measure.SetActive(false);

                qrCodeListener.enabled = false;
                ocrListener.enabled = false;
                measureListener.enabled = false;
                break;

            case 1:
                ocr.SetActive(true);
                qrcode.SetActive(false);
                measure.SetActive(false);

                qrCodeListener.enabled = false;
                ocrListener.enabled = true;
                measureListener.enabled = false;
                break;

            case 2:
                ocr.SetActive(false);
                qrcode.SetActive(true);
                measure.SetActive(false);

                qrCodeListener.enabled = true;
                ocrListener.enabled = false;
                measureListener.enabled = false;
                break;

            case 3:
                ocr.SetActive(false);
                qrcode.SetActive(false);
                measure.SetActive(true);

                qrCodeListener.enabled = false;
                ocrListener.enabled = false;
                measureListener.enabled = true;
                break;
        }
    }

    #region Operation's result
    // Set the operation result as 'result' and goes for the next one or to the PublishResults scene if is the last operation inside list
    public void SetSuccess()
    {
        activeOperation.Result = 1;

        if (activeOperation.Index >= 1 && activeOperation.Index < operations.Count)
        {
            activeOperation = operations[activeOperation.Index];

            if (activeOperation.Index == operations.Count)
            {
                ConfigureScene(activeOperation, "next");
                nextButton.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                ConfigureScene(activeOperation, "next");
            }
        }
        else if (activeOperation.Index == operations.Count)
        {
            FinishOrder();
        }
    }

    // Set the operation result as 'failure' and goes for the next one or to the PublishResults scene if is the last operation inside list
    public void SetFailure()
    {
        activeOperation.Result = 0;

        if (activeOperation.Index >= 1 && activeOperation.Index < operations.Count)
        {
            activeOperation = operations[activeOperation.Index];

            if (activeOperation.Index == operations.Count)
            {
                ConfigureScene(activeOperation, "next");
                nextButton.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                ConfigureScene(activeOperation, "next");
            }
        }
        else if (activeOperation.Index == operations.Count)
        {
            FinishOrder();
        }
    }

    // Set the operation result as 'not done' and goes for the next one or to the PublishResults scene if is the last operation inside list
    public void JumpOperation()
    {
        activeOperation.Result = 2;

        if (activeOperation.Index >= 1 && activeOperation.Index < operations.Count)
        {
            activeOperation = operations[activeOperation.Index];

            if (activeOperation.Index == operations.Count)
            {
                ConfigureScene(activeOperation, "next");
                nextButton.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                ConfigureScene(activeOperation, "next");
            }
        }
        else if (activeOperation.Index == operations.Count)
        {
            FinishOrder();
        }
    }
    #endregion

    #region Operation navigation
    // Move to the very next operation inside operations list
    public void MoveToNextOperation()
    {
        if (activeOperation.Index >= 1 && activeOperation.Index < operations.Count)
        {
            activeOperation = operations[activeOperation.Index];

            if (activeOperation.Index == operations.Count)
            {
                ConfigureScene(activeOperation, "next");
                nextButton.GetComponent<Renderer>().material.color = Color.green;
            }
            else
            {
                ConfigureScene(activeOperation, "next");
            }
        }
        else if (activeOperation.Index == operations.Count)
        {
            FinishOrder();
        }
    }

    // Move to previous operation inside operations list
    public void MoveToPreviousOperation()
    {
        if (activeOperation.Index > 1 && activeOperation.Index <= operations.Count)
        {
            activeOperation = operations[activeOperation.Index - 2];

            if (activeOperation.Index == 1)
            {
                ConfigureScene(activeOperation, "previous");
                previousButton.GetComponent<Interactable>().IsEnabled = false;
                previousButtonBackplate.GetComponent<Renderer>().material.color = Color.gray;
            }
            else
            {
                ConfigureScene(activeOperation, "previous");
            }
        }
        else if (activeOperation.Index == 1)
        {
            previousButtonBackplate.GetComponent<Renderer>().material.color = Color.gray;
            previousButton.GetComponent<Interactable>().IsEnabled = false;
        }
    }
    #endregion

    #region Resources
    // Open a video using YouTube API
    public void OpenVideo()
    {
        UnityEngine.WSA.Launcher.LaunchUri(activeOperation.Video, true);
    }

    public void Open3DModels()
    {
        if (activeOperation.Equipment == "Machine")
        {
            Instantiate(Resources.Load("Placa"));
        }
        else
        {
            Debug.Log("There is no prefab to display");
        }
    }
    // Set the UI Image visible and set it's texture
    public void OpenImage()
    {
        if (!imageCanvas.activeInHierarchy)
        {
            imageCanvas.SetActive(true);
        }
        else
        {
            imageCanvas.SetActive(false);
        }

        image.GetComponent<Renderer>().material = imageMaterial;
    }

    // Create all EPIs from operation inside a parente in a grid view
    public void InstantiateEpis()
    {
        var localList = activeOperation.EPIs.ToList();
        Debug.Log("There are " + localList.Count + " PPEs inside the list");

        for (int i = 0; i < localList.Count; i++)
        {
            var prefab = Resources.Load(localList[i]) as GameObject;
            Debug.Log(localList[i]);
            Vector3 position = new Vector3(epiParent.transform.position.x + (x_Space * (i % ColumnLength)), epiParent.transform.position.y + (-y_Space * (i / ColumnLength)), 0);
            var gameObject = Instantiate(Resources.Load(localList[i]), position, prefab.transform.rotation) as GameObject;
            gameObject.transform.SetParent(epiParent.transform, false);
        }
    }

    // Get text from comment box and save it
    public void SaveComment()
    {
        activeOperation.Comment = commentText.text;
        commentText.text = "";
        commentBox.SetActive(true);
    }

    // Open the comment box
    public void SetCommentVisibility()
    {
        if (commentBox.activeInHierarchy)
        {
            commentBox.SetActive(false);
        }
        else if (!commentBox.activeInHierarchy)
        {
            commentBox.SetActive(true);
        }
    }

    // Clean scene by detsroying all EPIs inside of it
    private void DestroyAllEpis()
    {
        foreach (UnityEngine.Transform t in epiParent.transform)
        {
            Destroy(t.gameObject);
        }
    }
    #endregion

    // Goes to the final scene
    public void FinishOrder()
    {
        SceneManager.LoadScene("PublishResults");
    }
}