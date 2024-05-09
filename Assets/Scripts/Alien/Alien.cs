using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public abstract class Alien : MonoBehaviour
{
    protected NavMeshAgent nav;
    public PageScript myPage;
    protected enum EmotionsEnum {Happiness, Calmness} //just examples for now
    public GameObject BookUI;
    protected Book book;
    protected List<ComboScript> CurrentCombos;
    protected PageScript activePage;
    public float FatigueModifier = 4.0f; // Fatigue modifier -- this is a divisor -- higher values will make the alien respond LESS to each repeated action -- response decays to normal over time
    public float FatigueDecayRate = .1f; // Affects how quickly fatigue decays back to normal
    protected struct WeightedDelegate
    {
        public float Weight;
        public Action action;
    }
    protected float[] Emotions = new float[Enum.GetNames(typeof(EmotionsEnum)).Length]; // when emotions need to be accesed do Emotions[(int)_emotionName_]
    protected float[] EmotionFatigue = new float[Enum.GetNames(typeof(EmotionsEnum)).Length];
    protected float[] BaseEmotionFatigue = new float[Enum.GetNames(typeof(EmotionsEnum)).Length];
    protected float[] BaseEmotions = new float[Enum.GetNames(typeof(EmotionsEnum)).Length];

    protected bool needNewAction = true;
    protected delegate bool Action();
    protected Action currentAction;
    protected float actionTimer;
    protected List<WeightedDelegate> allActions;

    public PlayerController playerscript;
    public GameObject RoomP1obj;
    public GameObject RoomP2obj;
    protected bool lostInterest = false;

    //tbh start and update dont need to be here but oh well
    public abstract void Start();
    public abstract void Update();
    public abstract void React(Tool tool);
    protected void DoIdleMovement(Vector3 navDest)
    {
        Vector3 RoomP1 = RoomP1obj.transform.position;
        Vector3 RoomP2 = RoomP2obj.transform.position;
        // only change desitnation if the alien is close to the current one
        Vector3 playerPos = playerscript.gameObject.transform.position;playerPos.y = 0.08f;
        if (
            Vector3.Distance(transform.position, navDest) > 3.0f &&
            Vector3.Distance(playerPos, navDest) > 0.1f &&
            !(Vector3.Distance(playerPos, transform.position) < 7.0f)
        ) return;
        Vector3 idlePos = playerPos;
        while (Math.Abs(playerPos.x - idlePos.x) < 5.0f) idlePos.x = Random.Range(RoomP1.x, RoomP2.x);
        while (Math.Abs(playerPos.z - idlePos.z) < 5.0f) idlePos.z = Random.Range(RoomP2.z, RoomP1.z);
        nav.SetDestination(idlePos);
        nav.stoppingDistance = 0;
    }
    protected int playersTool()
    {
        Tools CurrentTool = playerscript.ActiveTool.toolType;

        if (CurrentTool == Tools.Touch_Gently || CurrentTool == Tools.Feed_Treat) return -5;
        else if (CurrentTool == Tools.Touch_Roughly || CurrentTool == Tools.Feed_LiveAnimal) return 5;
        else return 0;
    }
    protected abstract void UpdateInterest(Tools type);
    protected abstract void UpdateEmotions();
    public abstract void TryUnlockCombos();
    protected abstract void InitActions(); // this function will populate allActions and should be called in Start() -- needs to be defined on a per-subclass basis since they will all have different actions
    protected abstract void UpdateWeights(); // BASED ON EMOTIONS
    protected void UpdateEmotion(EmotionsEnum emotion, float value) //always use UpdateEmotion for runtime updates as it will affect the fatigue
    {
        float currentValue = Emotions[(int)emotion];
        //Debug.Log(EmotionFatigue[(int)emotion]);
        float newValue = currentValue+ (value * EmotionFatigue[(int)emotion]);
        if (newValue < 0)
            newValue = 0;
        else if (newValue > 10)
            newValue = 10;
        Emotions[(int)emotion] = newValue;
        EmotionFatigue[(int)emotion] /= FatigueModifier;
    }
    protected void SetEmotion(EmotionsEnum emotion, float newValue)
    {
        if (newValue < 0)
            newValue = 0;
        else if (newValue > 10)
            newValue = 10;
        Emotions[(int)emotion] = newValue;
    }
    protected float GetEmotion(EmotionsEnum emotion)
    {
       return Emotions[(int)emotion];
    }
    protected void DecayEmotions(float[] _Emotions, float[] _BaseEmotions, float _DecayRate)
    {
        for (int i = 0; i < _Emotions.Length; i++)
        {
            float diff = _Emotions[i] - _BaseEmotions[i];
            if (Mathf.Abs(diff) < _DecayRate / 2)
                _Emotions[i] = _BaseEmotions[i];
            else if (diff != 0 && _DecayRate != 0)
            {
                float DecayDirection = Mathf.Abs(diff) / diff;
                _Emotions[i] -= _DecayRate * Time.deltaTime * DecayDirection;
            }
        }
    }
}
