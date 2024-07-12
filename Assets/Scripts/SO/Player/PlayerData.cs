using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Warbuzz.Player
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "SO/PlayerData", order = 1)]
    public class PlayerData : ScriptableObject
    {
        public int health;
        public int shield;
        public int flyEnergy;
    }
}

