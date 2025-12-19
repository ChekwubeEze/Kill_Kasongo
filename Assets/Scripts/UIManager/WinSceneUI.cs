using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.UIManager
{
    public class WinSceneUI: MonoBehaviour
    {
        [SerializeField] private TMP_Text winnerText;

        ulong n = WinData.WinnerClientId;
        private void Start()
        {
            if(n == 0)
            {
                winnerText.text = "Eze Wins!!!!!";
            }
            else
            {
                winnerText.text = "Niraj Wins!!!!!";
            }
            //winnerText.text = $"Player {WinData.WinnerClientId} Wins!";
        }
    }
}
