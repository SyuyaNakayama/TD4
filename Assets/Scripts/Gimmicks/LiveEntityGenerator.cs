using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiveEntityGenerator : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer visual;
    [SerializeField]
    Sprite[] sprites = { };
    [SerializeField]
    ResourcePalette resourcePalette;
    [SerializeField]
    string characterName;
    [SerializeField]
    string teamID;
    [SerializeField]
    int maxLiveEntityNum;
    [SerializeField]
    int spawnInterval;

    LiveEntity[] liveEntities = { };
    int currentSpawnInterval;

    void FixedUpdate()
    {
        //倒されたものはリストから除外
        List<LiveEntity> liveEntitiesList =
            new List<LiveEntity>(liveEntities);
        liveEntitiesList.RemoveAll(where => !where);
        liveEntities = liveEntitiesList.ToArray();

        //最大数に達していなければ
        if (liveEntities.Length < maxLiveEntityNum)
        {
            //残り時間を減らす
            currentSpawnInterval =
                Mathf.Clamp(currentSpawnInterval - 1, 0, spawnInterval);
            if (currentSpawnInterval <= 0)
            {
                Array.Resize(ref liveEntities, liveEntities.Length + 1);
                liveEntities[liveEntities.Length - 1] =
                    LiveEntity.Spawn(resourcePalette,
                    transform.position, transform.rotation, false, teamID,
                    new string[] { characterName }, new int[] { 0 }, 0);
                currentSpawnInterval = spawnInterval;
            }
        }
        else
        {
            //残り時間を最大に
            currentSpawnInterval = spawnInterval;
        }

        //残り時間に応じて見た目を変える
        visual.sprite =
            sprites[Mathf.Clamp(currentSpawnInterval / 2, 0, sprites.Length - 1)];
    }
}
