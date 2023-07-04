using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExampleUseCase : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI walletDisplayObj;
    [SerializeField]
    TMP_InputField alertInput;

    private VLink VLinker;

    // Start is called before the first frame update
    void Start()
    {
        walletDisplayObj.text = PlayerPrefs.GetString("walletAddress");
        VLinker = GameObject.Find("VLinker").GetComponent<VLink>();
    }

    public void CallAlert()
    {
        VLinker.Publish("Function: alert(\"" + alertInput.text + "\")");
    }
}
