using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    public TextMeshProUGUI loginMessage;

    [Header("Components")]
    [SerializeField] private TextMeshProUGUI _userInputTextMesh;
    private NFTManager _NFTManager;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = this;

        _NFTManager = GetComponent<NFTManager>();

        loginMessage.gameObject.SetActive(false);
    }

    public void SubmitWalletAddress()
    {
        string input = _userInputTextMesh.text.Trim();

        input = input.Remove(input.Length - 1);

        Debug.Log("User input: " + input);

        if (input.Length != 42) // Temp solution
        {
            loginMessage.gameObject.SetActive(true);
            loginMessage.text = "Login failed. Please enter a valid wallet address.";
            return;
        }

        _NFTManager.LoadNFTs(input);

        loginMessage.gameObject.SetActive(true);
        loginMessage.text = "Attempting Login. . .";
    }
}
