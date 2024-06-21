using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class conjurationArchive : MonoBehaviour
{
    // Start is called before the first frame update

    public Image i;
    public TMP_Text t;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setUp(Sprite s , string txt)
    {
        i.sprite = s;
        t.SetText(txt);
        this.gameObject.SetActive(true);
    }
}
