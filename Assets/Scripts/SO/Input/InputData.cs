using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InputData", menuName = "SO/InputData", order = 1)]
public class InputData : ScriptableObject
{
    [System.Serializable]
    public class InputButtons
    {
        public KeyCode button = KeyCode.None;
    }
    [Header("Move")]
    public List<InputButtons> move;
   

}
