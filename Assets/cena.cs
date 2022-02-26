using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class cena : MonoBehaviour
{
    private DefaultInput _defaultInput;

    private void Awake()
    {
        _defaultInput = new DefaultInput();

        _defaultInput.Menu.clique.performed += e => Clique();

    }

    public void Clique()
    {
        Debug.Log("here");

        SceneManager.LoadScene("Game");
        
    }
}
