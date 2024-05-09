using UnityEngine;
using System;
using UnityEngine.AI;
using System.Data;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class Creeper : Alien
{
    [Header("References")]
    private PlayerUIManager UIManager;
    public Transform PlayerTransform;
    private GameObject player;

    [Header("Components")]
    private Animator animator;
    private SkinnedMeshRenderer EyeRenderer;

    [Header("Constants")]
    public float baseDistance = 20;

    [Header("State Variables")]
    public float EmotionDecayRate = 0;
    public float InterestLevel = 0.5f; // This is a level from -1 to 1, larger absolute values indicate more interest (be that negative or positive interest)
    public float InterestDecayRate = 1f/16384f; // This determines the rate at which the interest will decay back to neutral
    public float InterestAction = 0.125f; // This determines the interest gain after an action
    private float InterestThreshold = 0.4f; // This determines the threshold for the alien to stay interested

    [HideInInspector]
    // Relationship is calculated from the emotion values.
    public float relationship
    {
        get { return (Emotions[(int)EmotionsEnum.Happiness] + Emotions[(int)EmotionsEnum.Calmness]) / 2; }
        private set { }
    }

    #region unityMethods
    // Start is the initial setup function, called before the first frame update
    public override void Start()
    {
        // Find references to Creeper's own components
        nav = GetComponent<NavMeshAgent>();
        animator = gameObject.GetComponentInChildren<Animator>();
        SkinnedMeshRenderer[] renderCandidates = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer render in renderCandidates)
        {
            if (render.CompareTag("CreeperEye"))
                EyeRenderer = render;
        }

        // Initialise emotion and fatigue values
        Emotions[(int)(EmotionsEnum.Happiness)] =5;
        Emotions[(int)(EmotionsEnum.Calmness)] = 5;

        BaseEmotions[(int)(EmotionsEnum.Happiness)] = 5;
        BaseEmotions[(int)(EmotionsEnum.Calmness)] = 5;
        for (int i = 0; i < BaseEmotionFatigue.Length; i++)
        {
           BaseEmotionFatigue[i] = 1.0f;
        }
        Array.Copy(BaseEmotionFatigue, EmotionFatigue, BaseEmotionFatigue.Length);
        

        // Find references to external components
        player = GameObject.FindGameObjectWithTag("Player");
        playerscript = player.GetComponent<PlayerController>();
        book = BookUI.GetComponent<Book>();
        UIManager = player.GetComponentInChildren<PlayerUIManager>();
    }
    // Update is the object's game loop function, called once per frame
    public override void Update()
    {
        // Set the speed of the alien based on its happiness
        float happiness = Emotions[(int)EmotionsEnum.Happiness];
        if (happiness >= (20.0f / 3.0f)) { 
            // If it is very happy sit down
            animator.SetBool("Sitting", true);
            nav.speed = 0;
        }
        else
        {
            // Otherwise decrease the speed with high happiness
            animator.SetBool("Sitting", false);
            nav.speed = 7 - ((happiness / 10.0f) * 7);
        }

        // Set the eye colour of the alien based on its calmness
        float calmness = Emotions[(int)EmotionsEnum.Calmness];
        Color eyeColour = Color.Lerp(Color.red, Color.white, calmness / 10.0f);
        EyeRenderer.material.color = eyeColour;

        

        // Reduce the emotion values and the fatigue from repeated actions to a baseline level
        DecayEmotions(ref Emotions, BaseEmotions, EmotionDecayRate);
        DecayEmotions(ref EmotionFatigue,  BaseEmotionFatigue,  FatigueDecayRate);

        // If the alien is interested in the player and can see the player -- look at the player
        if (Math.Abs(InterestLevel) > 0.3 && InCreeperRoom()) transform.LookAt(player.transform.position);
    }
    // FixedUpdate is an alternative update, called once per tick at a fixed rate
    void FixedUpdate()
    {
        // Decay towards 0
        InterestLevel += InterestLevel == 0 ? 0 : (InterestLevel < 0 ? InterestDecayRate : -InterestDecayRate);
        // Resets nav.destination to current player position to stop alien getting stuck
        if (Math.Abs(InterestLevel) < 0.3 && !lostInterest) { lostInterest = true; nav.SetDestination(PlayerTransform.position); }
        else if (Math.Abs(InterestLevel) > 0.3 && lostInterest) lostInterest = false;

        if (Math.Abs(InterestLevel) < 0.3 || !InCreeperRoom()) DoIdleMovement(nav.destination); // RoomP1 = (47.5, 0, 46.5) RoomP2 = (-18.5, 0, 22.5)
        else DoPlayerDistance();
    }
    #endregion
    // Set the distance away from the player the Creeper will stay
    void DoPlayerDistance()
    {
        // Walk towards the player and stop a certain distance away
        nav.SetDestination(PlayerTransform.position);
        nav.stoppingDistance = baseDistance - relationship + playersTool();
        MoveFromPlayer();
    }
    // Move away from the player if the Creeper is too close
    void MoveFromPlayer()
    {
        Vector3 ToPlayer = player.transform.position - transform.position;
        // If still outside the stopping distance
        if (Vector3.Magnitude(ToPlayer) < nav.stoppingDistance - 1)
        {
            // Twice the stopping distance, directed away from the player
            Vector3 targetPosition = ToPlayer.normalized * nav.stoppingDistance * -2;
            nav.destination = targetPosition;
        }
    }
    #region Room
    // Check if the player is in the Creeper's room
    bool InCreeperRoom()
    {
        // The rooms are 33.7 units away from each other on the X axis
        return (
            -15.2 < player.transform.position.x && player.transform.position.x < 16.8 // Ensure player is in the correct x range
        ) && (
            48.2 > player.transform.position.z && player.transform.position.z > 20.8 // Ensure player is in the correct z range
        );
    }
    #endregion


    #region Virtuals
    // React to the tool that was used by the player
    override public void React(Tool tool)
    {
        Debug.Log(string.Format("reacted to {0}", tool.toolType));
        // Update emotions based on Star's preferences to the different interactions
        if (tool.toolType == Tools.Touch_Gently)
        {
            UpdateEmotion(EmotionsEnum.Happiness, -1);
        }
        else if (tool.toolType == Tools.Touch_Roughly)
        {
            UpdateEmotion(EmotionsEnum.Happiness, 1);
        }
        else if (tool.toolType == Tools.Feed_Treat)
        {
            UpdateEmotion(EmotionsEnum.Calmness, -1);
        }
        else if (tool.toolType == Tools.Feed_LiveAnimal)
        {
            UpdateEmotion(EmotionsEnum.Calmness, 1);
        }
        else if (tool.toolType == Tools.Item_Oscilliscope) { } // This is an unused tool type
        // Update the interest value as a tool has been used 
        UpdateInterest(tool.toolType);
    }
    // Updates the interest by a set amount after an action.
    // If the update would cause the alien to become 
    // uninterested set it to the threshold plus the gain
    protected override void UpdateInterest(Tools type)
    {
        if (type == Tools.Touch_Gently || type == Tools.Feed_Treat)
        {
            if (InterestLevel < 0f && InterestLevel >= -InterestThreshold - InterestAction) InterestLevel = InterestThreshold + InterestAction;
            else InterestLevel += InterestAction;
        }
        else if (type == Tools.Touch_Roughly || type == Tools.Feed_LiveAnimal)
        {
            if (InterestLevel > 0f && InterestLevel <= InterestThreshold + InterestAction) InterestLevel = -InterestThreshold - InterestAction;
            else InterestLevel -= InterestAction;
        }
        if (InterestLevel > 1) InterestLevel = 1;
        else if (InterestLevel < -1) InterestLevel = -1;
    }
    // Show combos in the book if their emotion states have been seen
    public override void TryUnlockCombos()
    {
        float happiness = Emotions[(int)EmotionsEnum.Happiness];
        float calmness = Emotions[(int)EmotionsEnum.Calmness];

        // Checking emotion boundaries for all combo cases
        if (happiness > (20.0f / 3.0f))
        {
            myPage.ActivateCombo("HappyHigh");

        }
        else if ((happiness < (20.0f / 3.0f)) || (happiness > (10.0f / 3.0f)))
        {
            myPage.ActivateCombo("HappyMid");
        }
        else if (happiness < (10.0f / 3.0f))
        {
            myPage.ActivateCombo("HappyLow");
        }

        if (calmness > (20.0f / 3.0f))
        {
            myPage.ActivateCombo("CalmHigh");

        }
        else if ((calmness < (20.0f / 3.0f)) || (calmness > (10.0f / 3.0f)))
        {
            myPage.ActivateCombo("CalmMid");
        }
        else if (calmness < (10.0f / 3.0f))
        {
            myPage.ActivateCombo("CalmLow");
        }
    }
}
#endregion