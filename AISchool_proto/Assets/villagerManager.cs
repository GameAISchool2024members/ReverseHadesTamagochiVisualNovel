using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class villagerManager : MonoBehaviour
{
    public GameObject villagerPrefab;
    public string[] villagernames;

    public static villagerManager instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        activeVillagers = new List<villagerScript>();
    }

    public float cd = 1f, c = 0f;

    public static List<villagerScript> activeVillagers;

    public static int maxVillagers = 2;

    public void createVillager(string problem)
    {
        // TODO
    }

    public void killAllVillagers()
    {
        // TODO
        foreach(villagerScript v in activeVillagers)
        {
            v.die();
        }
    }

    public void createVillagers(List<string> problems)
    {
        // TODO
        foreach (string s in problems)
            problemQueue.Enqueue(s);
    }

    public Queue<string> problemQueue = new Queue<string>();

    public AudioSource entering;

    // Update is called once per frame
    void Update()
    {
        if(problemQueue.Count > 0 && activeVillagers.Count <= gameLoop.instance.villagerCounter)
        {
            if ((c + cd) < Time.time)
            {
                c = Time.time;
                GameObject clone = Instantiate(villagerPrefab, Vector3.zero, Quaternion.identity);

                villagerScript vs = clone.GetComponentInChildren<villagerScript>();

                entering.PlayOneShot(entering.clip);

                vs.enter(problemQueue.Dequeue());
                //activeVillagers.Add(vs);

                clone.GetComponentInChildren<SpriteSwapDemo>().SpriteSheetName = villagernames[Random.Range(0, villagernames.Length)];
            }
        }
        /*
        else if(Random.Range(0f,1f) > 0.7f) // chance to kill a villager
        {
            //Debug.Log(activeVillagers.Count);
            villagerScript vs = activeVillagers[Random.Range(0, activeVillagers.Count)];
            activeVillagers.Remove(vs);

            vs.die();

        }*/
    }
}
