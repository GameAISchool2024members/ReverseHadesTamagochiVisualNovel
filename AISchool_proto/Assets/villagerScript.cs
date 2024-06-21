using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class villagerScript : MonoBehaviour
{

    public Animator VillagerAnimator, PositionAnimator;
    public string problem;

    public GameObject bubble;
    public TMP_Text txt;

    // x -4 to 1
    // y -0.5 to -2
    public Vector2 targetOffset;
    public float enterTime = 2f;

    private Vector2 startPosition;  // Start position of the lerp
    private float timeElapsed;

    public void enter()
    {
        VillagerAnimator.SetTrigger("walk");
        PositionAnimator.SetTrigger("enter");

        setOffsetRandomly();

        if(!villagerManager.activeVillagers.Contains(this))
           villagerManager.activeVillagers.Add(this);

        startPosition = Vector2.zero;  // Starting at (0,0)
        timeElapsed = 0;
        lerpTo = true;

        StartCoroutine(arrived());

    }

    public float maxDistanceBetweenVillagers = 1f;

    void setOffsetRandomly()
    {
        bool reroll = true;
        int maxAttempts = 10; // You can adjust this limit based on your needs.
        int attempts = 0;

        while (reroll && attempts < maxAttempts)
        {
            attempts++;
            targetOffset = new Vector2(Random.Range(-5f, 2f), Random.Range(-2.2f, 0.5f));
            //Debug.Log("pos: " + targetOffset);
            reroll = false;

            foreach (villagerScript v in villagerManager.activeVillagers)
            {
                if(v != this)
                {
                    //Debug.Log("Checking distance between " + targetOffset + " and " + v.targetOffset);
                    float d = Vector2.Distance(targetOffset, v.targetOffset);
                    if (Mathf.Abs(d) < maxDistanceBetweenVillagers)
                    {
                        reroll = true;
                        //Debug.Log("setting true because d = " + d);
                    }
                }
            }
        }

    }


    IEnumerator arrived()
    {
        yield return new WaitForSeconds(enterTime);

        VillagerAnimator.SetTrigger("nowalk");
        txt.SetText(problem);
        bubble.SetActive(true);

        //gameObject.SetActive(false);
    }

    public void leave()
    {

    }

    public void die()
    {
        if (VillagerAnimator == null)
            return;
        VillagerAnimator.SetTrigger("die");
        bubble.SetActive(false);
        StartCoroutine(kms());
    }
    IEnumerator kms()
    {

        yield return new WaitForSeconds(5);
        Destroy(this.wrapper.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public bool done = false, lerpTo = false, lerpFrom = false;
    public GameObject wrapper;

    // Update is called once per frame
    void Update()
    {

        if(lerpTo)
        {
            if (timeElapsed < enterTime)
            {
                // Calculate the fraction of time that has passed
                float t = timeElapsed / enterTime;

                // Perform the lerp from startPosition to targetPosition
                Vector2 position = Vector2.Lerp(startPosition, targetOffset, t);
                wrapper.transform.position = new Vector3(position.x, position.y, wrapper.transform.position.z);

                // Increment the time elapsed by the time since last frame
                timeElapsed += Time.deltaTime;
            }
            else
            {
                // Optionally, set the position directly to the target to ensure it ends exactly at the target
                wrapper.transform.position = new Vector3(targetOffset.x, targetOffset.y, wrapper.transform.position.z);
                lerpTo = false;
            }
        }
    }
}
