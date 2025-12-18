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

        private void Start()
        {
            winnerText.text = $"Player {WinData.WinnerClientId} Wins!";
        }
    }
}
