using UnityEngine;
using UnityEngine.UI;

namespace IdleClicker
{
    public class DumbbellController : MonoBehaviour
    {
        Dumbbell dumbbell;
        [Header("Text & Image")]
        [SerializeField] Image dumbbellImage;
        [SerializeField] Text dumbbellName;
        [SerializeField] Text dumbbellUnlock;
        void Start()
        {
            dumbbellImage.sprite = dumbbell.Image;
            dumbbellName.text = dumbbell.DumbbellsName;
            dumbbellUnlock.text = dumbbell.isUnlock == false ? "Locked" :dumbbell.isSelected?"Selected":"Owned";
        }
        public void SetDumbbell(Dumbbell dumbbell)=>this.dumbbell = dumbbell;
        public void Selected()
        {
            if (!dumbbell.isUnlock) return;
            PlayerController.instance.DestroyDumbbell();
            GameManager.instance.SetDumbells(dumbbell,GameManager.instance.dumbbell);
        }
    }
}
