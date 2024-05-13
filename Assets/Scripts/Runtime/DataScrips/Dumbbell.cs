using UnityEngine;

namespace IdleClicker
{
    [CreateAssetMenu(fileName = "Dumbbell", menuName = "Idle Clicker/Dumbbell", order = 0)]
    public class Dumbbell : ScriptableObject
    {
        public bool isSelected;
        public bool isUnlock;
        public string DumbbellsName;
        public int Value;
        public int Price;
        public GameObject Prefabs;
        public Sprite Image;
    }
}
