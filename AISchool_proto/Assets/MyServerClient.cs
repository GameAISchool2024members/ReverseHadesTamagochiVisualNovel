using UnityEngine;
using LLMUnity;
using UnityEngine.UI;
using TMPro;


namespace LLMUnitySamples
{
    public class MyServerClientInteraction
    {
        TMP_InputField playerText;
        TMP_Text AIText;
        LLMClient llm;



        public MyServerClientInteraction(TMP_InputField playerText, TMP_Text AIText, LLMClient llm)
        {
            this.playerText = playerText;
            this.AIText = AIText;
            this.llm = llm;
        }

        public void Start()
        {
            playerText.onSubmit.AddListener(onInputFieldSubmit);
            playerText.Select();
        }

        public void onInputFieldSubmit(string message)
        {
            playerText.interactable = false;
            AIText.text = "...";
            _ = llm.Chat(message, SetAIText, AIReplyComplete);
        }

        public void SetAIText(string text)
        {
            AIText.text = text;
        }

        public void AIReplyComplete()
        {
            playerText.interactable = true;
            //playerText.Select();
            playerText.text = "";
        }
    }

    public class MyServerClient : MonoBehaviour
    {
        public LLM llm;
        public TMP_InputField playerText1;
        public TMP_Text AIText1;
        MyServerClientInteraction interaction1;

        public LLMClient llmClient;
        public TMP_InputField playerText2;
        public TMP_Text AIText2;
        MyServerClientInteraction interaction2;

        void Start()
        {
            interaction1 = new MyServerClientInteraction(playerText1, AIText1, llm);
            //interaction2 = new MyServerClientInteraction(playerText2, AIText2, llmClient);
            interaction1.Start();
            //interaction2.Start();
        }

        public void CancelRequests()
        {
            llm.CancelRequests();
            llmClient.CancelRequests();
            interaction1.AIReplyComplete();
            interaction2.AIReplyComplete();
        }

        public void ExitGame()
        {
            Debug.Log("Exit button clicked");
            Application.Quit();
        }
    }
}
