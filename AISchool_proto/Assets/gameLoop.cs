using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;




public class gameLoop : MonoBehaviour
{



    List<string> words = new List<string>
        {
            "sandwich",
            "blanket",
            "book",
            "hammer",
            "screwdriver",
            "map",
            "pencil",
            "flowers",
            "sword",
            "shirt",
            "candle",
            "glasses",
            "key",
            "scarecrow",
            "paper",
            "water bottle",
            "shoes",
            "hat",
            "guitar",
            "paint brush",
            //"camera",
            "candle",
            "broom",
            "lantern",
            "mirror",
            "bucket",
            "fishing net",
            "herbs",
            "rope",
            "flute",
            "cart",
            "chair",
            "shovel",
            "basket",
            "pitchfork",
            "needle",
            "ink",
            "bow",
            "cloak",
            "key",
            "lock",
            "mushroom",
            "cup",
            "hat",
            "shoes",
            "messenger pigeon",
            "dress",
            "ring",
            "hair brush",
            "pendant",
            "parchment",
            "crystal",
            "necklace",
            "seashell",
            "bow and arrow",
            "spoon",
            "envelope",
            "barrel"

        };

    public static gameLoop instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;  
        villagerManager.maxVillagers = villagerCounter;


    }

    public int step = 0;
    public string solution = "";

    void initProblem()
    {
        string problem = words[Random.Range(0, words.Count)];
        solution = problem;

        villagerManager.activeVillagers.Clear();

        //villagerManager.instance.killAllVillagers();

        // TODO remove all villagers

        //villagerManager.instance.problemQueue.Clear();

        int problemsNumber = villagerCounter;

        string problemPrompt = "imagine you have a problem. the answer to the problem is a " + problem + ". for example - bread can solve i am hungry and i want to feed ducks. give me " + problemsNumber + " such problems, make it one short sentence with ten words or less and no commas. return only the problems, not the solutions. do not mention the word " + problem + " in the response. return the response as a list in sqaure brackets, for example [problem 1, problem2]. "; // return the problems in a JSON format {problems:[problem1, problem2, problem3]}. the response has to be in JSON format";

        Debug.Log("Asking LLM: " + problemPrompt);
        step = 0;
        askLLM(problemPrompt);

        // read json out


        /*foreach (string s in problems)
        {
            villagerManager.instance.createVillager(s);
        }*/

    }

    bool valid = false;

    public void callback1()
    {
        if(step == 2)
        {
            if (LLMAnswer.Contains("yes") || LLMAnswer.Contains("Yes"))
                valid = true;
        }

        LLMAnswer = LLMAnswer.Replace("[", "");
        LLMAnswer = LLMAnswer.Replace("]", "");
        LLMAnswer = LLMAnswer.Replace("\"", "");


        string[] substrings = LLMAnswer.Split(',');

        //List<string> problems = splitList(LLMAnswer);
        List<string> problems = new List<string>();

        
        foreach (string s in substrings)
        {
            Debug.Log(s);
            if(s.Length > 2)
               problems.Add(s);
        }

        villagerManager.instance.createVillagers(problems);
    }

    public int score;
    public int remaining_resource;

    public int villagerCounter = 2;

    public GameObject witchSpeechBubble;
    public TMP_Text witchWords;
    public TMP_InputField playerInput;

    public void witchSays(string s)
    {
        witchWords.SetText(s);
        witchSpeechBubble.SetActive(true);
        StartCoroutine(stopSaying());
    }

    public float speechStayTime = 3f;
    IEnumerator stopSaying()
    {
        yield return new WaitForSeconds(speechStayTime);
        witchSpeechBubble.SetActive(false);
    }

    public string[] witchErrorMsgs;

    public void askLLM(string s)
    {
        LLMAnswer = null;

        LLMUnitySamples.MyServerClient.instance.interaction1.ask(s);

    }

    IEnumerator waitForLLM()
    {
        yield return new WaitUntil(() => (LLMAnswer != null));

        Debug.Log("got LLM answer: " + LLMAnswer);

        LLMAnswer = null;
    }

    public string LLMAnswer = null;

    string lastPlayerInput = "";

    public void handlePlayerInput(string s)
    {
        lastPlayerInput = s;
        // TODO validate
        // if not valid -> show error
        if(isNotValidInput(s))
        {
            witchSays(witchErrorMsgs[Random.Range(0, witchErrorMsgs.Length)]);

            playerInput.text = "";
            playerInput.interactable = true;
            playerInput.Select();

            return;
        }

        // generate spellwords
        witchSays(generateSpellWords(s));

        // start generating image
        ComfyPromptCtr.instance.startGeneration(s);

        // evaluate for each villager and handle villager responses + make them leave

        // wait for imageGen to finish

        StartCoroutine(waitForImageGen());
    }

    public float similarityThreshold = 0.75f;
    public int points = 0;

    public TMP_Text pointCounter;

    void updatePoints()
    {
        pointCounter.SetText("" + points);
    }

    IEnumerator waitForImageGen()
    {
        // Wait until the condition is true
        yield return new WaitUntil(() => !ComfyPromptCtr.generating);

        // Code to execute after the condition is true
        Debug.Log("Condition is true");

        // calculate score, increase villager counter 

        string[] tmp = new string[]{ lastPlayerInput };

        List<System.Tuple<string, float>> tup = similarityTest.instance.getSimilarityScores(solution, tmp);

        Debug.Log("tested for similarity: " + solution + " and " + lastPlayerInput + ", got score: " + tup[0].Item2);

        if(tup[0].Item2 >= similarityThreshold)
        {
            // all villagers accept!
            foreach(villagerScript v in villagerManager.activeVillagers)
            {
                v.happy();
                points++;
            }
            villagerManager.maxVillagers = ++villagerCounter;
        }
        else
        {
            foreach (villagerScript v in villagerManager.activeVillagers)
            {
                v.die();
            }
            // handle each villager separately
        }

        /*villagerManager.activeVillagers[Random.Range(0, villagerManager.activeVillagers.Count)].happy();
        villagerManager.maxVillagers = ++villagerCounter;*/
        updatePoints();
        if (villagerCounter >= 10)
            villagerCounter = 9;

        villagerManager.instance.problemQueue.Clear();
        yield return new WaitForSeconds(6f);
        initProblem();
        // have peasants come again

    }

    public static List<string> splitList(string problems)
    {
        string pattern = "\"(.*?)\"";

        Regex regex = new Regex(pattern);
        MatchCollection matches = regex.Matches(problems);

        List<string> problemsList = new List<string>();
        foreach (Match match in matches)
        {
            problemsList.Add(match.Groups[1].Value);
        }

        return problemsList;
    }


    public AudioSource magic;

    public bool isNotValidInput(string s)
    {
        valid = false;
        step = 2;
        //askLLM("Is the following valid input? Valid input contains less than 5 words and must be an object. it is not allowed to be obscene or NSFW. Please answer only with a yes or no. input: " + s);

        // TODO
        if (s.Length <= 2)
            return true;
        return false;
    }

    public string[] problems;

    public string generateSpellWords(string conjuration)
    {
        // TODO
        // LLM command: Create me 2-5 word spell to conjure + conjuration + answer only with the spell words
        magic.PlayOneShot(magic.clip);
        return "abra cadabra";
    }

    // Update is called once per frame

    bool started = false;
    void Update()
    {
        if(!started)
        {
            witchSays("Everyone, come to my hut and start wishing! It wont cost much, just your soul! Hehe");

            initProblem();
            started = true;
        }

    }
}
