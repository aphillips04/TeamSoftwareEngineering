using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
public abstract class Alien : MonoBehaviour
{
    [Header("References")]
    public GameObject BookUI;
    public GameObject RoomP1obj;
    public GameObject RoomP2obj;
    protected Book book;
    public PlayerController playerscript;

    [Header("Components")]
    protected NavMeshAgent nav;
    public PageScript myPage;

    [Header("Constants")]

    [Header("State Variables")]
    public float FatigueModifier = 1.0f; // Fatigue modifier -- this is a divisor -- higher values will make the alien respond LESS to each repeated action -- response decays to normal over time
    public float FatigueDecayRate = 10.0f; // Affects how quickly fatigue decays back to normal
    protected bool lostInterest = false;

    [Header("Emotion Storage")]
    protected float[] Emotions = new float[Enum.GetNames(typeof(EmotionsEnum)).Length]; // when emotions need to be accesed do Emotions[(int)_emotionName_]
    public float[] EmotionFatigue = new float[Enum.GetNames(typeof(EmotionsEnum)).Length];
    public float[] BaseEmotionFatigue = new float[Enum.GetNames(typeof(EmotionsEnum)).Length];
    protected float[] BaseEmotions = new float[Enum.GetNames(typeof(EmotionsEnum)).Length];
    protected enum EmotionsEnum {Happiness, Calmness} //just examples for now

    #region abstracts
    // These methods are overridden by inheriting aliens
    public abstract void Start();
    public abstract void Update();
    public abstract void React(Tool tool);
   
    protected abstract void UpdateInterest(Tools type);
    
    public abstract void TryUnlockCombos();
    #endregion
    // TODO!
    protected void DoIdleMovement(Vector3 navDest)
    {
        Vector3 RoomP1 = RoomP1obj.transform.position;
        Vector3 RoomP2 = RoomP2obj.transform.position;
        // only change desitnation if the alien is close to the current one
        Vector3 playerPos = playerscript.gameObject.transform.position; playerPos.y = 0.08f;
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
    // Returns a distance modifier based on what tool the player is holding
    protected int playersTool()
    {
        Tools CurrentTool = playerscript.ActiveTool.toolType;

        if (CurrentTool == Tools.Touch_Gently || CurrentTool == Tools.Feed_Treat) return -5;
        else if (CurrentTool == Tools.Touch_Roughly || CurrentTool == Tools.Feed_LiveAnimal) return 5;
        else return 0;
    }
    // Update an emotion by "value"
    // Always use UpdateEmotion for runtime updates as it will affect the fatigue
    protected void UpdateEmotion(EmotionsEnum emotion, float value) 
    {
        float currentValue = Emotions[(int)emotion];
        Debug.Log(EmotionFatigue[(int)emotion]);
        float newValue = currentValue+ (value * EmotionFatigue[(int)emotion]);
        // Constrain the new value
        if (newValue < 0)
            newValue = 0;
        else if (newValue > 10)
            newValue = 10;
        // Set the new value
        Emotions[(int)emotion] = newValue;
        EmotionFatigue[(int)emotion] /= FatigueModifier;
        Debug.Log("Emotion change " + currentValue + "to " + newValue);
    }
    // Set an emotion to "newValue"
    protected void SetEmotion(EmotionsEnum emotion, float newValue)
    {
        // Constrain the new value
        if (newValue < 0)
            newValue = 0;
        else if (newValue > 10)
            newValue = 10;
        // Set the new value
        Emotions[(int)emotion] = newValue;
    }
    // Get the value of an emotion
    protected float GetEmotion(EmotionsEnum emotion)
    {
       return Emotions[(int)emotion];
    }
    //Modify the emotion value toward a base value based on a decay factor
    protected void DecayEmotions(ref float[] _Emotions,  float[] _BaseEmotions,  float _DecayRate)
    {
        for (int i = 0; i < _Emotions.Length; i++)
        {
            // Calculate the difference from the base
            float diff = _Emotions[i] - _BaseEmotions[i];
            // If we will overshoot - set the value directly
            if (Mathf.Abs(diff) < _DecayRate / 2)
                _Emotions[i] = _BaseEmotions[i];
            // Otherwise move the emotion value toward the base value
            else if (diff != 0 && _DecayRate != 0)
            {
                float DecayDirection = Mathf.Abs(diff) / diff;
                _Emotions[i] -= _DecayRate * Time.deltaTime * DecayDirection;
            }
        }
    }
}
