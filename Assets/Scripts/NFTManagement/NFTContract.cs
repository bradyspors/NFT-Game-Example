using UnityEngine.Networking;
using ABI.Contracts.nft.ContractDefinition;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Nethereum.JsonRpc.UnityClient;
using Nethereum.ABI.FunctionEncoding.Attributes;

public class NFTContract : MonoBehaviour
{
    private static string _url = "https://ropsten.infura.io/v3/a2dfb3140f9540af83799479673daaa4";
    private static string _account; // = "0xfc18ABdfc1070268Ba093f9336A5367803Fc6820";
    private static string _contractAddress = "0x9011919c739aF57CA685CA504A0c28C2D6217A96";

    public static bool successfulLogin;

    //private string privateKey = "";

    public static void SetAccount(string accountID)
    {
        _account = accountID;
    }

    public static IEnumerator GetURI(BigInteger tokenID, Action<string> output)
    {
        var queryRequest = new QueryUnityRequest<UriFunction, UriOutputDTO>(_url, _account);

        yield return queryRequest.Query(new UriFunction() { Id = tokenID }, _contractAddress);

        output(queryRequest.Result.ReturnValue1);
    }

    public static IEnumerator GetIDs(Action <List<BigInteger> > output)
    {
        successfulLogin = false;

        var queryRequest = new QueryUnityRequest<GetAllTokensFunction, GetAllTokensOutputDTO>(_url, _account);

        yield return queryRequest.Query(new GetAllTokensFunction() { Account = _account }, _contractAddress);

        if (queryRequest.Result == null)
        {
            LoginManager.Instance.loginMessage.gameObject.SetActive(true);
            LoginManager.Instance.loginMessage.text = "Login failed. Please enter a valid wallet address.";
            yield break;
        }


        successfulLogin = true;
        output(queryRequest.Result.ReturnValue1);
    }
}
