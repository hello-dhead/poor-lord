using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StageGroupData
{
    public List<StageData> StageDataGroup = new List<StageData>();
}

[System.Serializable]
public class StageData
{
    public List<MonsterData> MonsterGroupList = new List<MonsterData>();
}

[System.Serializable]
public class MonsterData
{
    public string MonsterType;
    public int MonsterHP;
    public int MonsterDamage;
    public float SummonTerm;
}