﻿using UnityEngine.Events;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

public class DialogueManager : MonoBehaviour
{
    public Camera dialogueCamera;
    public GameObject[] ToDisable;
    public UnityEvent OnStartDialogue;
    public Queue<string> sentences;
    public string[] FromFileDialogue;
    private int index;
    private float _textSpeed = 2f;
    private string sentence;
    public bool _isdialogue = false;
    private FirstPersonAIO player;
    private PlayerActions _playeractions;
    public bool displayingdialogue = false;
    private GameObject _dialogue;
    private Dialogue dialogueScript;
    public Sentence sentence1;

    NPC npc;
    private void Start() 
    {
        sentences = new Queue<string>();
        player = GameObject.FindWithTag("Player").GetComponent<FirstPersonAIO>();
        _playeractions = GameObject.FindWithTag("Player").GetComponent<PlayerActions>();
        _dialogue = _playeractions.dialogue_gameobject;
        dialogueScript = _dialogue.GetComponent<Dialogue>();
    }
    private void Awake()
    {
        if (OnStartDialogue == null)
            OnStartDialogue = new UnityEvent();
        npc = GetComponent<NPC>();
    }

    public void say(GameObject caller) 
    {

        if(caller == null)
        {
            Debug.Log("Dialogue is null");
        }
        player.playerCanMove = false;
        _isdialogue = true;
        UpdateFile();
        dialogueScript._name.text = caller.name;
        //Debug.Log(caller.name);
        player.lockAndHideCursor = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        //Debug.Log("Starting Convo");
        npc.agent.isStopped = true;
        npc.GetComponentInChildren<Animator>().enabled = false;
        
        npc.enabled = false;
       
        DialogueSystem.instance.Attach(this);

        sentences.Clear();
        /*
        foreach(string sentence in FromFileDialogue)
        {
            sentences.Enqueue(sentence);
        }
        */
        DisplayNextSentence();
        
        OnStartDialogue.Invoke();
    }


    public void EndDialogue()
    {
        //Debug.Log("Done");
        DialogueSystem.instance.Detach();
        npc.GetComponentInChildren<Animator>().enabled = true;
        npc.enabled = true;
        npc.agent.isStopped = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _isdialogue = false;
        player.playerCanMove = true;
        _playeractions = GameObject.FindWithTag("Player").GetComponent<PlayerActions>();
        _playeractions._indialogue = false;
        _playeractions.dialogue_gameobject.SetActive(false);
    }


    private void OnDestroy()
    {
        OnStartDialogue.RemoveAllListeners();
    }
    private void UpdateFile()
    {
        string path;
        path = "Assets/NPC dialogues/SentenceTesting.txt";
        StreamReader reader = new StreamReader(path);
        FromFileDialogue = File.ReadAllLines(path);
    }
    public void OptionsActive()
    {
        dialogueScript.DialogueText.gameObject.SetActive(false);
        dialogueScript._name.gameObject.SetActive(false);

        var options = _dialogue.GetComponentsInChildren<Button>(true);

        index = 0;
        foreach (Button a in options)
        {
            if (index >= sentence1.GetPaths())
                break;
            a.gameObject.SetActive(true);
            a.interactable = true;
            AddButtonListener(a, index);
            a.GetComponentInChildren<Text>().text = sentence1.GetSentence(index).text;
            index++;
        }
    }
    private void DisplayNextSentence()
    {
        if (sentence1.answer != null)
        {
            sentence = sentence1.answer;
            StartCoroutine(Type());
        }

        if (sentence1.quest != null)
        {
            //start quest
        }
    }

    IEnumerator Type()
    {
        displayingdialogue = true;
        dialogueScript.DialogueText.text = "";
        foreach(char letter in sentence)
        {
            dialogueScript.DialogueText.text += letter;
            yield return new WaitForSeconds(_textSpeed*Time.deltaTime);
        }
        displayingdialogue = false;
        // if (!sentence1.HasPaths())
        // {
        //     Debug.Log("Has Ended");
        //     EndDialogue();
        // }
    }
    private void Choices(int index)
    {
        Debug.Log(index);

        var options = _dialogue.GetComponentsInChildren<Button>(true);

        foreach (Button a in options)
        {
            a.gameObject.SetActive(false);
            a.interactable = false;
        }
        sentence1 = sentence1.choices[index];
        dialogueScript.DialogueText.gameObject.SetActive(true);
        dialogueScript._name.gameObject.SetActive(true);
        displayingdialogue = false;
        DisplayNextSentence();
    }
    private void AddButtonListener(Button a, int index)
    {
        a.onClick.AddListener(() =>
        {
            Choices(index);
        }
        );
    }
}
