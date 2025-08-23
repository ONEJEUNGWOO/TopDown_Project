using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageInfo
{
    public int stageKey;
    public WaveData[] waves;

    public StageInfo (int stageKey, WaveData[] waves)
    {
        this.stageKey = stageKey;
        this.waves = waves;
    }
}

[System.Serializable]
public class WaveData
{
    public MonsterSpawnData[] monsters;
    public bool hasBoss;
    public string boosType;

    public WaveData(MonsterSpawnData[] monsters, bool hasBoss, string boosType)
    {
        this.monsters = monsters;
        this.hasBoss = hasBoss;
        this.boosType = boosType;
    }
}