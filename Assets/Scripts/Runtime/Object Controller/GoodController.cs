using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClicker
{
    public class GoodController : MonoBehaviour
    {
        Dumbbell dumbbell;
        [SerializeField] Image dumbImage;
        [SerializeField] Text dumbName;
        [SerializeField] Text dumbPrice;
        [SerializeField] Button buyBtn;
        // Start is called before the first frame update
        private void Awake()=>buyBtn.onClick.AddListener(()=>GameManager.instance.Transaction(dumbbell));
        void Start()
        {
            dumbImage.sprite = dumbbell.Image;
            dumbName.text = dumbbell.DumbbellsName;
            dumbPrice.text=dumbbell.Price.ToString();
            buyBtn.GetComponentInChildren<Text>().text = dumbbell.isUnlock ? "Owned" : "Buy";
            buyBtn.interactable=dumbbell.isUnlock?false:true;
        }

        public void SetDumbbell(Dumbbell dumbbell)=>this.dumbbell= dumbbell;
    }
}
