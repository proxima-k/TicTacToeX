using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EmotePack", menuName = "Emote/EmotePack")]
public class EmotePackSO : ScriptableObject
{
    public List<Emote> Emotes;
}


[Serializable]
public class Emote {
    public string AnimationName;
    public bool Loop;
    public float LoopAmount;
}