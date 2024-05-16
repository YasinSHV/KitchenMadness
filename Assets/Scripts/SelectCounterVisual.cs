using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCounterVisual : MonoBehaviour
{
    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject selectedVisual;

    private void Start()
    {
        Player.Instance.OnCounterSelected += Player_OnCounterSelected;
    }

    private void Player_OnCounterSelected(object sender, Player.OnCounterSelectedEventArgs e)
    {
        if (e.selectedCounter == clearCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show() 
    {
        selectedVisual.SetActive(true);
    }

    private void Hide() 
    {
        selectedVisual.SetActive(false);
    }
}
