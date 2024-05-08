using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public abstract class Alien : MonoBehaviour
{
    protected NavMeshAgent nav;

    protected enum EmotionsEnum {Happiness, Calmness} //just examples for now

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
    protected void DoIdleMovement()
    {
        // idly move around their room
    }
    protected abstract void UpdateEmotions();
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
