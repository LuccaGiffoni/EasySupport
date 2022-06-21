using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class FakeLogin : MonoBehaviour
{
    // Serialize fields
    [Header("TextMeshPro")]
    [SerializeField] private TextMeshProUGUI usernameField;
    [SerializeField] private TextMeshProUGUI passwordField;

    [Header("Game Objects")]
    [SerializeField] private GameObject serviceOrderList;
    [SerializeField] private GameObject loginMenu;

    // Public fields
    public static string username;
    public static string password;

    // Configuring scene
    private void Start()
    {
        loginMenu.SetActive(true);

        loginMenu.transform.position = Camera.main.transform.position + new Vector3(0, 0, 0.5f);
        serviceOrderList.SetActive(false);
    }

    // Set the user
    public void LoginUsername()
    {
        username = usernameField.text;
        password = passwordField.text;

        serviceOrderList.SetActive(true);
        loginMenu.SetActive(false);
    }

    // Start the operation
    public void StartOperation()
    {
        SceneManager.LoadScene("Operations");
    }
}
