using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[CreateAssetMenu(fileName = "New Dialogue", menuName = "New Dialogue", order = 0)]
public class DialogueSO : ScriptableObject
{
    [System.Serializable]
    public class Dialogue
    {
        public bool isPlayerSpeaking;
        [TextArea] public string textDialogue;
        public FontStyles fontStlye = FontStyles.Bold;
        public AudioClip vocalSFX;
    }

    [SerializeField] Dialogue[] dialogue;

    public Dialogue[] GetAllDialogue()
    {
        return dialogue;
    }
}
