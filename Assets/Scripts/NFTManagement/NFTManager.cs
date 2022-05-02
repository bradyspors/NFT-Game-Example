using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;

public class NFTManager : MonoBehaviour
{
    public static NFTManager Instance;

    public List<string> itemIDs = new List<string>();

    public Material material;

    [Header("Components")]
    [SerializeField] private SceneController _sceneController;

    private void Awake()
    {
        Instance = this;
    }

    public void LoadNFTs(string accountID)
    {
        NFTContract.SetAccount(accountID);

        StartCoroutine(StartLoadNFTs());
    }

    public IEnumerator StartLoadNFTs()
    {
        List<BigInteger> ids = new List<BigInteger>();

        yield return NFTContract.GetIDs((output) => ids = output);

        List<string> urls = new List<string>();
        int counter = 0;
        foreach (BigInteger id in ids)
        {
            string uri = "";
            yield return NFTContract.GetURI(id, (output) => uri = output);
            urls.Add(uri);
            
            Debug.Log(uri);

            counter++;
        }

        if (!NFTContract.successfulLogin)
            yield break;

        itemIDs.AddRange(urls);

        _sceneController.LoadNextScene();
    }

    IEnumerator SetTexture(string uri, Material mat)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            Texture newTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
            mat.mainTexture = newTexture;
        }
    }
}
