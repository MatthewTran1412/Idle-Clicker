using System;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClicker
{
    public class SkillController : MonoBehaviour
    {
        Skill skill;
        [SerializeField] Image skillImage; 
        [SerializeField] Text skillDescription;
        [SerializeField] Button upgradeBtn;
        private void Start()
        {
            skillImage.sprite = skill.SkillImage;
            skillDescription.text = skill.name.Contains("Cooldown")?$"{Math.Round(skill.BaseValue,2)}s -> {Math.Round(skill.BaseValue - skill.Value,2)}s": $"{Math.Round(skill.BaseValue, 2)}/lift -> {Math.Round(skill.BaseValue + skill.Value, 2)}/lift";
            upgradeBtn.GetComponentInChildren<Text>().text = skill.Price.ToString();
        }
        private void FixedUpdate()
        {
            if (GameManager.instance.Money < skill.Price)
            {
                upgradeBtn.interactable = false;
                upgradeBtn.GetComponentInChildren<Text>().color = Color.red;
            }
            else
            {
                upgradeBtn.interactable = true;
                upgradeBtn.GetComponentInChildren<Text>().color = Color.white;
            }
        }
        public void Upgrade()
        {
            GameManager.instance.Money -= skill.Price;
            switch(skill.name)
            {
                case "Reduce Cooldown":
                    skill.BaseValue -= skill.Value;
                    PlayerController.instance.DecreaseTimer(skill.BaseValue*skill.Value);
                    break;
                case "Gain":
                    skill.BaseValue += skill.Value;
                    PlayerController.instance.IncreaseGainValue(skill.BaseValue + skill.Value);
                    break;
                case "Offline":
                    skill.BaseValue += skill.Value;
                    break;
            }
            skill.Level++;
            PlayerPrefs.SetInt(skill.name,skill.Level);
            GameManager.instance.OpenUpgrade();
        }
        public void SetSkill(Skill skill)=>this.skill=skill;
    }
}
