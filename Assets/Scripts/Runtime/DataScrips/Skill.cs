using UnityEngine;

namespace IdleClicker
{
    [CreateAssetMenu(fileName = "Skill", menuName = "Idle Clicker/Skill", order = 0)]
    public class Skill : ScriptableObject
    {
        public Sprite SkillImage;
        public float BaseValue;
        public float Value { get => BaseValue * 5 / 100;}
        public int Level = 1;
        public int Price { get => 15 + (15  * Level); }
    }
}
