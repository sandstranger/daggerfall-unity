using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "GameDataAssets", menuName = "ScriptableObjects/GameDataAssets", order = 1)]
public class GameDataAssetsSO : ScriptableObject
{
    public List<TextAsset> biogs = new List<TextAsset>();
    public List<TextAsset> factions = new List<TextAsset>();
    public List<TextAsset> fonts = new List<TextAsset>();
    public List<TextAsset> quests = new List<TextAsset>();
    public List<TextAsset> tables = new List<TextAsset>();
    public List<TextAsset> text = new List<TextAsset>();
    public List<TextAsset> books = new List<TextAsset>();
    //public TextAsset[] spellIcons;
}