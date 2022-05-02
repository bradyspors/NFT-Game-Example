using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Networking;

public class SetYTThumb : MonoBehaviour
{
    public GameObject prefab;
    public Material mat;

    // Start is called before the first frame update
    IEnumerator Start()
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
            GameObject go = Instantiate(prefab, 10 * UnityEngine.Vector3.down * counter, UnityEngine.Quaternion.identity);
            Material m = new Material(mat);
            go.GetComponent<Renderer>().material = m;
            yield return SetTexture(uri, m);

            counter++;
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