﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatDropper : MonoBehaviour
{
    public GameObject hatPrefab;
    public PlayerMovement playerMovement;
    public GameObject [] hatSprites;
    // spawn locations between (-7, 6) for X and (-4.6, 4.6) for Y
    private int spawnTime = 3;
    private int index = 0;
    void Start()
    {
        StartCoroutine(EnemyDrop(10));
    }

    void Shuffle () {
        for (int i = 0; i < hatSprites.Length; i++ ) {
            GameObject [] temp = { hatSprites[i], hatSprites[i] };
            int randomIndex = Random.Range (i, hatSprites.GetLength (0));
            hatSprites[i] = hatSprites[randomIndex];
            hatSprites[randomIndex] = temp[0];
        }
    }
    IEnumerator EnemyDrop(int spawn) {
        // todo
        int count = 0;
        while(count<spawn) {
            yield return new WaitForSeconds(spawnTime);
            if (index >= hatSprites.GetLength(0)) {
                Shuffle();
                index = 0;
            }
            Vector3 loc = new Vector3(Random.Range(-7, 6), Random.Range(-4, 4) + 0.6f, 0);
            GameObject newHat = Instantiate(hatPrefab, loc, Quaternion.identity);
            // Debug.Log("Copying texture, " + hatSprites[index].GetComponent<Renderer>().material.mainTexture.name);
            newHat.GetComponent<SpriteRenderer>().sprite = hatSprites[index].GetComponent<SpriteRenderer>().sprite;
            newHat.GetComponent<HatBehavior>().hatType = hatSprites[index].GetComponent<HatBehavior>().hatType;
            newHat.GetComponent<HatBehavior>().playerMovement = playerMovement;
            count ++; index++;
            yield return new WaitForSeconds(spawnTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}