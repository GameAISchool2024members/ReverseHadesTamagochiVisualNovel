using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class villagerManager : MonoBehaviour
{
    public GameObject villagerPrefab;
    public string[] villagernames;

    // Start is called before the first frame update
    void Start()
    {
        activeVillagers = new List<villagerScript>();
    }

    public float cd = 1f, c = 0f;

    public static List<villagerScript> activeVillagers;

    // Update is called once per frame
    void Update()
    {
        if(activeVillagers.Count <= 10)
        {
            if ((c + cd) < Time.time)
            {
                c = Time.time;
                GameObject clone = Instantiate(villagerPrefab, Vector3.zero, Quaternion.identity);

                villagerScript vs = clone.GetComponentInChildren<villagerScript>();
                vs.enter();
                //activeVillagers.Add(vs);

                clone.GetComponentInChildren<SpriteSwapDemo>().SpriteSheetName = villagernames[Random.Range(0, villagernames.Length)];
            }
        }
        else if(Random.Range(0f,1f) > 0.7f) // chance to kill a villager
        {
            //Debug.Log(activeVillagers.Count);
            villagerScript vs = activeVillagers[Random.Range(0, activeVillagers.Count)];
            activeVillagers.Remove(vs);

            vs.die();

        }
    }
}
