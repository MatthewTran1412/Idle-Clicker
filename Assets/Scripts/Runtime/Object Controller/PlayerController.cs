using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace IdleClicker
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
        public bool isRush;
        [SerializeField] Transform rightHand;
        [SerializeField] Transform spawnPoint;
        [SerializeField] Transform floatTingCanvas;
        [SerializeField] Slider coolDown;
        [SerializeField] Slider rushTime;
        [SerializeField] Image rushTimeColor;
        [SerializeField] GameObject LiftHelp;
        [SerializeField] GameObject RecordHelp;
        float lastime;
        public float timer { get; private set; }
        float rushTimer;
        float rushCooldown;
        public float gainValue { get; private set; }
        bool isCooldown;
        public bool isRushCooldown { get; private set; }
        bool isLifting;
        bool isNewGame = true;
        Animator anim;
        private void Awake()=>instance=this;
        private void Start()
        {
            timer= GameManager.instance.skills.Find(x => x.name.Contains("Cooldown")).BaseValue;
            lastime=Time.time;
            coolDown.maxValue = GameManager.instance.skills.Find(x => x.name.Contains("Cooldown")).BaseValue;
            coolDown.value = 0;
            gainValue = GameManager.instance.skills.Find(x => x.name.Contains("Gain")).BaseValue;
            isLifting = true;
            anim=GetComponent<Animator>();
            rushTimer = 10f;
            rushCooldown = 60f;
            if (PlayerPrefs.HasKey("IsNewGame"))
            {
                isNewGame = false;
                LiftHelp.SetActive(false);
            }
            if (PlayerPrefs.HasKey("Last Rush"))
            {
                TimeSpan time = DateTime.Now.Subtract(DateTime.Parse(PlayerPrefs.GetString("Last Rush")));
                int value = time.Seconds + TimeSpan.FromMinutes(time.Minutes).Seconds + TimeSpan.FromHours(time.Hours).Seconds;
                if (value > 60)
                {
                    rushCooldown = 0;
                    isRushCooldown = false;
                }
                else
                {
                    rushCooldown -= value;
                    isRushCooldown = true;
                }
            }
        }
        private void FixedUpdate()
        {
            if (Time.time - lastime > timer)
            {
                isCooldown = false;
                coolDown.value = 0;
                if (Time.time - lastime > timer + 0.5f && !isNewGame)
                {
                    if (isRush) RushTime();
                    else PlayerLifting();
                }
            }
            if (isCooldown) coolDown.value += Time.deltaTime;
            if (isRush)
            {
                rushTimer -= Time.deltaTime;
                RushTimeSlider();
            }
            if (rushTimer <= 0)
            {
                rushTimer = 1;
                StartCoroutine(EndPushUp());
            }
            if (isRushCooldown)
            {
                rushCooldown -= Time.deltaTime;
                RushTimeSlider();
            }
            if (rushCooldown <= 0) isRushCooldown = false;

        }
        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(0))
            {
                LiftHelp.SetActive(false);
                isNewGame = false;
                PlayerPrefs.SetInt("IsNewGame", 1);
                if (!isCooldown && isLifting) PlayerLifting();
                if (!isLifting)
                {
                    RecordHelp.SetActive(false);
                    anim.SetTrigger("Throw");
                }
            }
        }
        void PlayerLifting()
        {
            if (!isLifting) return;
            anim.SetTrigger("Lift");
            Lift();
        }
        void RushTime()
        {
            anim.SetTrigger("Push");
            Lift();
        }
        void Lift()
        {
            var go = Instantiate(Resources.Load<GameObject>("Prefabs/System/FloatingText"), floatTingCanvas);
            go.GetComponentInChildren<Text>().text = "+" + GameManager.instance.dumbbell.Value.ToString();
            go.transform.localPosition += new Vector3(UnityEngine.Random.Range(-500f, 500f), UnityEngine.Random.Range(-500f, 500f), 0);
            Destroy(go, 1f);
            GameManager.instance.Lifting();
            lastime = Time.time;
            isCooldown = true;
        }
        public IEnumerator StartPushUp()
        {
            isRushCooldown= false;
            DestroyDumbbell();
            anim.SetBool("Push Up", true);
            yield return new WaitForSeconds(anim.runtimeAnimatorController.animationClips.First(x=>x.name== "Idle To Push Up").length+0.5f);
            timer = 0;
            isRush = true;  
        }
        IEnumerator EndPushUp()
        {
            isRush = false;
            timer = GameManager.instance.skills.Find(x => x.name.Contains("Cooldown")).BaseValue;
            anim.SetBool("Push Up", false);
            yield return new WaitForSeconds(anim.runtimeAnimatorController.animationClips.First(x => x.name == "Push Up To Idle").length+0.5f);
            Instantiate(GameManager.instance.dumbbell.Prefabs, rightHand);
            rushTimer = 10f;
            isRushCooldown = true;
            rushCooldown = 60;
            PlayerPrefs.SetString("Last Rush", DateTime.Now.ToString());
        }
        public void TurnLeft()
        {
            DestroyDumbbell();
            transform.eulerAngles = new Vector3(0, 90, 0);
        }
        public void TurnRight()
        {
            Instantiate(GameManager.instance.dumbbell.Prefabs,rightHand);
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        public void IncreaseGainValue(float amount) => gainValue += amount;
        public void DecreaseTimer(float amount)
        {
            GameManager.instance.skills.Find(x => x.name.Contains("Cooldown")).BaseValue -= amount;
            timer = GameManager.instance.skills.Find(x => x.name.Contains("Cooldown")).BaseValue;
        }
        public void DecreaseMuscle(int value)=> rightHand.localScale -= new Vector3(0.002f * value, 0.002f * value, 0.002f * value);
        public void InitDumbbell(GameObject Prefabs)=> Instantiate(Prefabs,rightHand);
        public void SetBoolLifting(bool isActive)=>isLifting=isActive;
        public void ThrowBall()
        {
            var go = Instantiate(Resources.Load<GameObject>("Prefabs/Objects/Metal Ball"),spawnPoint.position,Quaternion.identity);
            go.GetComponent<Rigidbody>().AddForce(new Vector3(GameManager.instance.Strenght/2, 0,0), ForceMode.Impulse);
            GameManager.instance.SetCamToTarget(go.transform);
            RecordHelp.SetActive(false);
        }
        public void DestroyDumbbell() => Destroy(rightHand.GetChild(rightHand.childCount - 1).gameObject);
        void RushTimeSlider()
        {
            if(isRush)
            {
                rushTime.maxValue = 10;
                rushTime.value = rushTimer;
                rushTimeColor.color = Color.green;
            }
            if(isRushCooldown)
            {
                rushTime.maxValue = 60;
                rushTime.value = rushCooldown;
                rushTimeColor.color = Color.red;
            }
        }
    }
}
