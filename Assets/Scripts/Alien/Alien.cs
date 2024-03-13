using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
public abstract class Alien : MonoBehaviour
{
    protected enum EmotionsEnum { Happiness, Calmness, Fear, Anger } //just examples for now
    protected struct WeightedDelegate
    {
        public float Weight;
        public Action action;
    }
    protected float[] Emotions = new float[Enum.GetNames(typeof(EmotionsEnum)).Length]; // when emotions need to be accesed do Emotions[(int)_emotionName_]

    protected bool needNewAction = true;
    protected delegate bool Action();
    protected Action currentAction;
    protected float actionTimer;
    protected List<WeightedDelegate> allActions;

    //tbh start and update dont need to be here but oh well
    public abstract void Start();
    public abstract void Update();
    public abstract void React(Tool tool);
    protected abstract void InitActions(); // this function will populate allActions and should be called in Start() -- needs to be defined on a per-subclass basis since they will all have different actions
    protected abstract void UpdateWeights(); // BASED ON EMOTIONS
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
