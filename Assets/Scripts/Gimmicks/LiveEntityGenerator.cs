using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
    [SerializeField]
    Switch powerSwitch;
    [SerializeField]
    bool reverse;

    LiveEntity[] liveEntities = { };
    int currentSpawnInterval;

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Handles.Label(
            transform.position,
            characterName);
    }
#endif

    void FixedUpdate()
    {
        //倒されたものはリストから除外
        List<LiveEntity> liveEntitiesList =
            new List<LiveEntity>(liveEntities);
        liveEntitiesList.RemoveAll(where => !where);
        liveEntities = liveEntitiesList.ToArray();

        // パワースイッチがオフの時は動作しない
        if (powerSwitch) 
        {
            bool notActive = !powerSwitch.GetActive();
            if (reverse) { notActive = !notActive; }
            if (notActive) { return; }
        }

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
                    new string[] { characterName }, new int[] { 0 }, 0, null);
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
