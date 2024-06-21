using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
//using UnityEngine.Random;

[System.Serializable]
public class ResponseData
{
    public string prompt_id;
}
public class ComfyPromptCtr : MonoBehaviour
{
    public InputField nInput,promptJsonInput;

    public GameObject[] spells;
    public GameObject result;
    public static GameObject activeSpell = null;

    public TMP_InputField pInput;

    public static ComfyPromptCtr instance;

    public static bool generating = false;

    private void Start()
    {
        instance = this;
        // QueuePrompt("pretty man","watermark");
        pInput.onSubmit.AddListener(myQPrompt);
    }

    public void QueuePrompt()
    {
        StartCoroutine(QueuePromptCoroutine(pInput.text,nInput.text));
    }

    public string AddToPromptText = ", pixel art";
    public string NegativePromptText = "text, watermark, human";

    public void myQPrompt(string message)
    {
        gameLoop.instance.handlePlayerInput(pInput.text);
    }

    public void startGeneration(string message)
    {
        generating = true;
        pInput.interactable = false;
        StartCoroutine(QueuePromptCoroutine(pInput.text + AddToPromptText, NegativePromptText));
        result.SetActive(false);
        activeSpell = spells[UnityEngine.Random.Range(0, spells.Length)];
        activeSpell.SetActive(true);
    }

    public int resolutionImageX = 200, resolutionImageY = 200;

    private IEnumerator QueuePromptCoroutine(string positivePrompt,string negativePrompt)
    {
        string url = "http://127.0.0.1:8188/prompt";
        string promptText = GeneratePromptJson();
        promptText = promptText.Replace("Pprompt", positivePrompt);
        promptText = promptText.Replace("Nprompt", negativePrompt);


        int rng = UnityEngine.Random.Range(0,100000);
        promptText = promptText.Replace("1052505839183015", ""+rng);
        promptText = promptText.Replace("\"width\": 1280", "\"width\": " + resolutionImageX);
        promptText = promptText.Replace("\"height\": 720", "\"height\": " + resolutionImageY);

        Debug.Log(promptText);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(promptText);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Prompt queued successfully." + request.downloadHandler.text);

            ResponseData data = JsonUtility.FromJson<ResponseData>(request.downloadHandler.text);
            Debug.Log("Prompt ID: " + data.prompt_id);
            GetComponent<ComfyWebsocket>().promptID = data.prompt_id;
           // GetComponent<ComfyImageCtr>().RequestFileName(data.prompt_id);
        }
    }
    public string promptJson;

private string GeneratePromptJson()
    {
 string guid = Guid.NewGuid().ToString();

    string promptJsonWithGuid = $@"
{{
    ""id"": ""{guid}"",
    ""prompt"": {promptJson}
}}";

    return promptJsonWithGuid;
    }
}
