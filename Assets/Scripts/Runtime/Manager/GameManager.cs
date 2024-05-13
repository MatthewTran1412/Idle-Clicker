using Cinemachine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClicker
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager instance { get; private set; }
        [Header("Value")]
        public int Money;
        public int Strenght { get; private set; }
        bool exchangeOpened;
        bool shopingOpened;
        bool UpgradeOpened;
        Vector3 baseCamPostion;
        DateTime lastLogin;
        public Dumbbell dumbbell { get; private set; }
        public List<Skill> skills { get; private set; }

        [Header("UI")]
        [SerializeField] Text MoneyText;
        [SerializeField] Text StrenghtText;
        [Space]
        [SerializeField] Image dumbbellImage;
        [SerializeField] Text dumbbellName;
        [Space]
        [SerializeField] InputField inputStrength;
        [SerializeField] Text outputMoney;
        [SerializeField] Text warning;
        [Space]
        [SerializeField] Text furthestScore;
        [SerializeField] Text currentScore;
        
        [Header("Dumbbells")]
        [SerializeField]List<Dumbbell> listDumbbells;
        
        [Header("Target")]
        [SerializeField] GameObject LiftingWindow;
        [SerializeField] GameObject LiftingCamera;
        [Space]
        [SerializeField] GameObject ThrowingWindow;
        public GameObject ThrowingCamera;
        [Space]
        [SerializeField] GameObject chosingDumbbell;
        [SerializeField] GameObject exchangeWindow;
        [SerializeField] GameObject shop;
        [SerializeField] GameObject upGradeWindow;
        [Space]
        [SerializeField] GameObject ResultWindow;
        [Space]
        [SerializeField] Transform dumbbellsContent;
        [SerializeField] Transform shopContent;
        [SerializeField] Transform skillContent;
        private void Awake()
        {
            instance = this;
            inputStrength.onValueChanged.AddListener(delegate { ShowOutput();});
        }
        private void Start() => GameSetup();
        //Lifting
        void GameSetup()
        {
            Application.targetFrameRate = 60;
            listDumbbells = Resources.LoadAll<Dumbbell>("Data/Dumbbells").ToList();
            skills = Resources.LoadAll<Skill>("Data/Skills").ToList();
            SetupDumbbells();
            SetupSkills();
            SetDumbells(listDumbbells[PlayerPrefs.HasKey("Dummbell")?PlayerPrefs.GetInt("Dummbell") :0]);
            Money = PlayerPrefs.HasKey("Money")?PlayerPrefs.GetInt("Money"):0;
            Strenght = PlayerPrefs.HasKey("Strenght")?PlayerPrefs.GetInt("Strenght"):0;
            if (PlayerPrefs.HasKey("Last Login")) lastLogin = DateTime.Parse(PlayerPrefs.GetString("Last Login"));
            StartCoroutine(CountWhenLogoff());
            baseCamPostion = ThrowingCamera.transform.position;
        }
        public void SetDumbells(Dumbbell newDumb,Dumbbell oldDumb=null)
        {
            PlayerPrefs.SetInt("Dummbell",FindPosOfDumb(newDumb));
            PlayerPrefs.SetInt(newDumb.name,1);
            dumbbell = newDumb;
            dumbbell.isSelected = true;
            dumbbellImage.sprite = dumbbell.Image;
            dumbbellName.text = dumbbell.DumbbellsName + "\n" + dumbbell.Value;
            PlayerController.instance.InitDumbbell(dumbbell.Prefabs);
            if (oldDumb) oldDumb.isSelected = false;
            chosingDumbbell.SetActive(false);
        }
        int FindPosOfDumb(Dumbbell newDumb)
        { 
            for (int i = 0;i<listDumbbells.Count;i++)
                if (listDumbbells[i].name.Equals(newDumb.name)) return i;
            return -1;
        }
        public void Lifting()
        {
            Strenght += dumbbell.Value * (int)PlayerController.instance.gainValue;
            PlayerPrefs.SetInt("Strenght", Strenght);
            UpdateValueText();
        }
        public void Exchange()
        {
            if (Strenght < int.Parse(inputStrength.text))
            {
                warning.text = "not enough strength";
                return;
            }
            Money += int.Parse(inputStrength.text) / 2;
            Strenght -= int.Parse(inputStrength.text);
            PlayerController.instance.DecreaseMuscle(int.Parse(inputStrength.text));
            UpdateValueText();
            exchangeWindow.SetActive(false);
            warning.text = string.Empty;
            inputStrength.text = "0";
            outputMoney.text = "0";
            Debug.Log("Exchanged");
            PlayerPrefs.SetInt("Money", Money);
        }
        void UpdateValueText()
        {
            MoneyText.text = Money >= 1000000 ? $"{Money / 1000000}M" : Money >= 1000 ? $"{Money / 1000} K" : Money.ToString();
            StrenghtText.text=Strenght>=1000000?$"{Strenght/1000000}M": Strenght >= 1000 ?$"{Strenght/1000} K":Strenght.ToString();
        }
        public void ChosingDumbWindow()
        {
            DestroyContent(dumbbellsContent);
            foreach (var item in listDumbbells)
            {
                var go = Instantiate(Resources.Load<GameObject>("Prefabs/Objects/Dumbbell"), dumbbellsContent);
                go.GetComponent<DumbbellController>().SetDumbbell(item);
            }
            chosingDumbbell.SetActive(true);
        }
        public void OpenShop()
        {
            shopingOpened = !shopingOpened;
            if (shopingOpened)
            {
                DestroyContent(shopContent);
                foreach (var item in listDumbbells)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/Objects/Good"), shopContent);
                    go.GetComponent<GoodController>().SetDumbbell(item);
                }
            }
            shop.SetActive(shopingOpened);
        }
        public void OpenExchange()
        {
            exchangeOpened = !exchangeOpened;
            exchangeWindow.SetActive(exchangeOpened);
        }
        public void OpenUpgrade()
        {
            UpgradeOpened = !UpgradeOpened;
            if(UpgradeOpened)
            {
                DestroyContent(skillContent);
                foreach(var item in skills)
                {
                    var go = Instantiate(Resources.Load<GameObject>("Prefabs/Objects/Skill Content"),skillContent);
                    go.GetComponent<SkillController>().SetSkill(item);
                }
            }
            upGradeWindow.SetActive(UpgradeOpened);
        }
        public void StartRushTime()
        {
            if (PlayerController.instance.isRushCooldown) return;
            StartCoroutine(PlayerController.instance.StartPushUp());
        }
        void ShowOutput()=> outputMoney.text = (int.Parse(inputStrength.text) / 2).ToString();
        public void Transaction(Dumbbell dumbbell)
        {
            if(Money<dumbbell.Price)
            {
                Debug.Log("not enough money");
                return;
            }
            dumbbell.isUnlock = true;
            Money-=dumbbell.Price;
            OpenShop();
        }
        IEnumerator CountWhenLogoff()
        {
            TimeSpan offTime = DateTime.Now.Subtract(lastLogin);
            yield return new WaitUntil(()=> PlayerController.instance.timer>0);
            float value = (offTime.Seconds + TimeSpan.FromMinutes(offTime.Minutes).Seconds + TimeSpan.FromHours(offTime.Hours).Seconds) / PlayerController.instance.timer;
            Strenght += (int)(dumbbell.Value * skills.Find(x => x.name.Contains("Offline")).BaseValue * value);
            UpdateValueText();
        }
        void DestroyContent(Transform target)
        {
            for (int i = 0; i < target.childCount; i++)
                Destroy(target.GetChild(i).gameObject);
        }
        void SetupDumbbells()
        {
            foreach(var item in listDumbbells)
                if(PlayerPrefs.HasKey(item.name))item.isUnlock = true;
        }
        void SetupSkills()
        {
            foreach(var item in skills)
            {
                if(PlayerPrefs.HasKey(item.name))
                {
                    item.Level = PlayerPrefs.GetInt(item.name);
                    for (int i = 0; i < item.Level; i++) item.BaseValue -= item.Value;
                }
            }
        }

        //Throw Record
        public void OpenThrowRecord()
        {
            UpdateScoreText();
            SetupActive(true);
            PlayerController.instance.TurnLeft();
        }
        public void CloseThrowRecord()
        {
            SetupActive(false);
            PlayerController.instance.TurnRight();
        }
        void SetupActive(bool active)
        {
            PlayerController.instance.SetBoolLifting(!active);
            LiftingWindow.SetActive(!active);
            LiftingCamera.SetActive(!active);
            ThrowingWindow.SetActive(active);
            ThrowingCamera.SetActive(active);
        }
        public void SetCamToTarget(Transform target)
        {
            if (target is null)
            {
                ThrowingCamera.transform.position = baseCamPostion;
                ThrowingCamera.GetComponent<CinemachineVirtualCamera>().LookAt = PlayerController.instance.gameObject.transform;
            }
            else
                ThrowingCamera.GetComponent<CinemachineVirtualCamera>().LookAt = target;
        }
        public float CaculateXDistance(Transform target1, Transform target2)
        {
            return target2.position.x- target1.position.x;
        }
        public void UpdateScoreText(Transform target1=null ,Transform target2 = null)
        {
            float score = target1 is null ? 0 : CaculateXDistance(target1, target2);
            currentScore.text = $"Distance :{Mathf.Round(score / 2)} m";
            ResultWindow.GetComponentInChildren<Text>().text = score > PlayerPrefs.GetFloat("FurthestScore")?$"<color=#FF0000>New Record: {Mathf.Round(score)/2}m ( Gained {Mathf.Round((score - PlayerPrefs.GetFloat("FurthestScore")) / 2)} money)</color>" :$"Current score: {Mathf.Round(score)/2}m";
            if (score > PlayerPrefs.GetFloat("FurthestScore"))
            {
                AddMoney(score - PlayerPrefs.GetFloat("FurthestScore"));
                PlayerPrefs.SetFloat("FurthestScore", score);
            }
            furthestScore.text = $"Furthest: {Mathf.Round(PlayerPrefs.GetFloat("FurthestScore")) /2}m";
            if(target1)
            {
                ResultWindow.transform.localScale= new Vector3(0.5f,0.5f,0.5f);
                ResultWindow.transform.DOScale(Vector3.one,0.5f);
                ResultWindow.SetActive(true);
            }

        }
        void AddMoney(float amount) => Money += (int)amount;
        private void OnApplicationQuit()=> PlayerPrefs.SetString("Last Login", DateTime.Now.ToString());

    }
}
