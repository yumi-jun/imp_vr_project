/**
* Copyright (c) 2020 Nodesk Inc. All rights reserved.
*/

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class WordController : MonoBehaviour
{

    [SerializeField]private TMP_Text text;
    [SerializeField]private Color selectColor;
    [SerializeField]private Color unSelectColor;
    private Image background;
    
    
    void Awake()
    {
        background = GetComponent<Image>();
    }

    public void SetWord(string word)
    {
        text.text = word;
    }
    
    public string GetWord()
    {
        return text.text;
    }

    public void Select()
    {
        background.color = selectColor;
    }
    
    public void UnSelect()
    {
        background.color = unSelectColor;
    }
}
