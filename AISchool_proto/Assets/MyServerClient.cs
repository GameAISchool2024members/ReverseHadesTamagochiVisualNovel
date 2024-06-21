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

        public void ask(string message)
        {
            Debug.Log("1");
            if (gameLoop.instance.LLMAnswer != null)
                return;

            Debug.Log("2");
            _ = llm.Chat(message, receive, done, false);
        }

        string answer = "";

        public void done()
        {
            Debug.Log("done! " + answer);
            gameLoop.instance.LLMAnswer = answer;
            gameLoop.instance.callback1();
        }

        public void receive(string text)
        {
            //gameLoop.instance.LLMAnswer = text;
            answer = text;
            //Debug.Log(text);
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
        public MyServerClientInteraction interaction1;

        public LLMClient llmClient;
        public TMP_InputField playerText2;
        public TMP_Text AIText2;
        public MyServerClientInteraction interaction2;

        public static MyServerClient instance;

        void Start()
        {
            instance = this;

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
