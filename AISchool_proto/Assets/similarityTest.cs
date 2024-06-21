using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class similarityTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    bool done = false;
    public SentenceSimilarity ss;

    public string input;
    public string[] comparisons;

    public TextAsset tokenizer, config;

    public static similarityTest instance;

    // Update is called once per frame
    void Update()
    {
        
        if(!done)
        {
            instance = this;
            SentenceSimilarityUtils.SentenceSimilarityUtils_.tokenizer = tokenizer;
            SentenceSimilarityUtils.SentenceSimilarityUtils_.config = config;
            //ss.GetSimilarityScores(input, comparisons);
            done = true;
        }
    }

    public List<Tuple<string, float>> getSimilarityScores(string input, string[] comparisons)
    {
        return ss.GetSimilarityScores(input, comparisons);
    }

    public TMP_InputField i1, i2;
    public TMP_Text t;

    public void onPress()
    {
        string[] i2s = { i2.text };
        t.SetText("" + ss.GetSimilarityScores(i1.text, i2s)[0].Item2 );
    }
}
