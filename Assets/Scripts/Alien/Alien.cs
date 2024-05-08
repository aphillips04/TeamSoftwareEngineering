using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public abstract class Alien : MonoBehaviour
{
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

    //tbh start and update dont need to be here but oh well
    public abstract void Start();
    public abstract void Update();
    public abstract void React(Tool tool);
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
    protected void InitGenericActions()
    {
        //optional functionality - if actions are put here into Alien.cs they can be added to allActions without needing to do it in the child class
    }
    protected void ChooseAction()
    {
        UpdateWeights();//updates the weights based on the emotions

        //choose an action entirely based on its weight
        //https://stackoverflow.com/questions/56692/random-weighted-choice
        float totalWeight = 0;
        foreach(var action in allActions)
        {
            totalWeight += action.Weight;
        }
        float randomNum = Random.Range(0, totalWeight);
        foreach(var action in allActions)
        {
            if (randomNum < action.Weight)
            {
                action.action(); //important to remember the ACTION is in charge of registering itself and telling update to call it next frame
                return;
            }
            randomNum -= action.Weight;
        }

    }

    //not entirely convinced having generic actions is a good idea but it's possible for simple ones like idle, movetowardplayer
}
