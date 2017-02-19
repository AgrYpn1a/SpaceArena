using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHelpPanel : MonoBehaviour
{
    [SerializeField]
    private GameObject content;
    [SerializeField]
    private UnityEngine.UI.Button btnClose;

    void Start()
    {
        btnClose.onClick.AddListener(() =>
        {
            content.SetActive(false);
        });
    }
}
