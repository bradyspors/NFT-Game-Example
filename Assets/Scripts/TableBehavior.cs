using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class TableBehavior : MonoBehaviour
{
    public GameObject iconDisplay;
    bool _playerInteracted;
    
    [SerializeField] private TextMeshProUGUI _tableDescription;
    [SerializeField] private GameObject _dragonGlassDagger;
    [SerializeField] private string _dragonGlassID = "https://img.youtube.com/vi/p3MIwgz6mHw/hqdefault.jpg";

    // Start is called before the first frame update
    void Start()
    {
        _playerInteracted = false;

        if (NFTManager.Instance != null && NFTManager.Instance.itemIDs.Count > 0)
        {
            Material material = iconDisplay.GetComponent<Renderer>().material;

            StartCoroutine(SetTexture(NFTManager.Instance.itemIDs[NFTManager.Instance.itemIDs.Count - 1], material));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (NFTManager.Instance != null && collision.transform == ControlPlayer.Instance.transform)
        {
            _tableDescription.text = "Owned token item count: " + NFTManager.Instance.itemIDs.Count;

            // Check player has specific item
            if (NFTManager.Instance.itemIDs.Count > 0 && 
                NFTManager.Instance.itemIDs[NFTManager.Instance.itemIDs.Count - 1] == _dragonGlassID &&
                !_playerInteracted)
            {
                Instantiate(_dragonGlassDagger, ControlPlayer.Instance.weaponHolder.transform);

                _playerInteracted = true;
            }
        }
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
