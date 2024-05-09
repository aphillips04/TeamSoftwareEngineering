using UnityEngine;
using System;
using UnityEngine.AI;
using System.Data;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class StarAlien : Alien
{
    #region legacyMembers

    [HideInInspector]
    public float relationship
    {
        get { return (Emotions[(int)EmotionsEnum.Happiness] + Emotions[(int)EmotionsEnum.Calmness]) / 2; }
        private set { }
    }
    #endregion
    
    //private Dictionary<EmotionsEnum, double> Emotions = new Dictionary<EmotionsEnum, double>();
    //this is a dictionary here but using an enum I suppose you could also assign each one an int value (in the enum) and then it can be a straight array
    //not entirely sure this is an important implementation decision
    //but *technically* we don't *need* a dictionary since we only need to store one float per member of the enum and that is all known at compile time
    //I think i'm just trying to over-optimise (getting a bit c++ brained with arrays vs "real" generic containers)

    private new MeshRenderer renderer;
    private PlayerUIManager UIManager;
    private Transform MeshTransform;
    public Transform PlayerTransform;
    public float EmotionDecayRate = 0;
    public int ToPlayer = 0;
    public float InterestLevel = 0.5f; // This is a level from -1 to 1, larger absolute values indicate more interest (be that negative or positive interest)
    public float InterestDecayRate = 1f/16384f; // This determines the rate at which the interest will decay back to neutral

    public float baseDistance = 20;
    #region unityMethods
    /// <summary>
    /// Start is the initial setup function, called before the first frame update
    /// </summary>
    /// 
    private GameObject player;
    public override void Start()
    {
        MeshTransform = transform.Find("StarMesh");
        MeshRenderer[] renderCandidates = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer render in renderCandidates)
        {
            if (render.CompareTag("StarEye"))
                renderer = render;
        }
        //THESE NEED MOVING AT SOME POINT
        Emotions[(int)(EmotionsEnum.Happiness)] = 5;
        Emotions[(int)(EmotionsEnum.Calmness)] = 5;

        BaseEmotions[(int)(EmotionsEnum.Happiness)] = 5;
        BaseEmotions[(int)(EmotionsEnum.Calmness)] = 5;
        for (int i = 0; i < BaseEmotionFatigue.Length; i++)
        {
           BaseEmotionFatigue[i] = 1.0f;
        }
        EmotionFatigue = BaseEmotionFatigue;
        nav = GetComponent<NavMeshAgent>();

        player = GameObject.FindGameObjectWithTag("Player");
        playerscript = player.GetComponent<PlayerController>();
        book = BookUI.GetComponent<Book>();
        // Wait until player ui manager is ready
        UIManager = player.GetComponentInChildren<PlayerUIManager>();
        //relationship bar needs to be sorted (i think removed idk)
        UIManager.InitRelationshipBar();
        UIManager.SetRelationshipBar(relationship);
        currentAction = Idle;
    }

    /// <summary>
    /// Update is the objects game loop function, called once per frame
    /// </summary>
    public override void Update()
    {
        // Set the colour of the alien based on its happiness
        float happiness = Emotions[(int)EmotionsEnum.Happiness];
        Color skinColour = new (
            .65f + (happiness *  .035f), // The change is calculated through: (end - start) / (steps - 1)
            .00f + (happiness *  .100f), // There are 11 steps of from 0 to 11
            .65f + (happiness * -.065f)  // The start value was RGB(166, 0, 166), end value was RGB(255, 255, 0) 
        );
        renderer.material.color = skinColour;
        
        // Set the spin speed of the alien based on its calmness

        DecayEmotions(Emotions,BaseEmotions,EmotionDecayRate);
        DecayEmotions(EmotionFatigue, BaseEmotionFatigue, FatigueDecayRate);

        if (Math.Abs(InterestLevel) > 0.3 && InStarRoom()) transform.LookAt(player.transform.position);
    }
    void FixedUpdate()
    {
        InterestLevel += InterestLevel == 0 ? 0 : (InterestLevel < 0 ? InterestDecayRate : -InterestDecayRate);
        if (Math.Abs(InterestLevel) < 0.3 && !lostInterest) { lostInterest = true; nav.SetDestination(PlayerTransform.position); }
        else if (Math.Abs(InterestLevel) > 0.3 && lostInterest) lostInterest = false;

        DoStarBobbing();
        DoStarSpin();
        if (Math.Abs(InterestLevel) < 0.3 || !InStarRoom()) DoIdleMovement(nav.destination); // RoomP1 = (47.5, 0, 46.5) RoomP2 = (-18.5, 0, 22.5)
        else DoPlayerDistance();
        MoveToDestination();
    }
    #endregion
    void DoPlayerDistance()
    {
        nav.SetDestination(PlayerTransform.position);
        nav.stoppingDistance = baseDistance - relationship + playersTool();
    }
    void MoveToDestination()
    {
        Vector3 ToPlayer = player.transform.position - transform.position;
        if (Vector3.Magnitude(ToPlayer) < nav.stoppingDistance - 1)
        {
            Vector3 targetPosition = ToPlayer.normalized * nav.stoppingDistance * -2;
            nav.destination = targetPosition;
        }
    }
    #region BobSpinRoom
    void DoStarBobbing()
    {
        MeshTransform.position += Vector3.up * 0.01f *  Mathf.Sin( 2 * Time.time);
    }
    void DoStarSpin()
    {
        //this needs looking at it completely murders the AI controller
        //I think it just needs to rotate the child mesh instead of the parent empty
        float happiness = GetEmotion(EmotionsEnum.Happiness);
        if (happiness == 0) { Debug.Log("SADA"); }
        float spinSpeed = 10 / happiness; 
        MeshTransform.Rotate(Vector3.right,spinSpeed,Space.Self);
        //Debug.Log(spinSpeed);
    }
    bool InStarRoom()
    {
        // FOR EVERY ALIEN INCREASE X VALUE BY 33.7
        return (
            -48.9 < player.transform.position.x && player.transform.position.x < -16.9 // Ensure player is in the correct x range
        ) && (
            48.2 > player.transform.position.z && player.transform.position.z > 20.8 // Ensure player is in the correct z range
        );
    }
    #endregion

    
    #region Virtuals
    //From parent clas
    override public void React(Tool tool)
    {
        Debug.Log(string.Format("reacted to {0}", tool.toolType));
        if (tool.toolType == Tools.Touch_Gently)
        {
            UpdateEmotion(EmotionsEnum.Happiness, 1);
        }
        else if (tool.toolType == Tools.Touch_Roughly)
        {
            UpdateEmotion(EmotionsEnum.Happiness,- 1);
        }
        else if (tool.toolType == Tools.Feed_Treat)
        {
            UpdateEmotion(EmotionsEnum.Calmness, 1);
        }
        else if (tool.toolType == Tools.Feed_LiveAnimal)
        {
            UpdateEmotion(EmotionsEnum.Calmness,- 1);
        }
        else if (tool.toolType == Tools.Item_Oscilliscope)
        {
        }
        UpdateInterest(tool.toolType);
        // Update the UI
        UIManager.UpdateRelationshipBar(relationship);
    }
    protected override void UpdateInterest(Tools type)
    {
        if (type == Tools.Touch_Gently || type == Tools.Feed_Treat)
        {
            if (InterestLevel < 0f && InterestLevel >= -0.425f) InterestLevel = 0.425f;
            else InterestLevel += 0.125f;
        }
        else if (type == Tools.Touch_Roughly || type == Tools.Feed_LiveAnimal)
        {
            if (InterestLevel > 0f && InterestLevel <= 0.425f) InterestLevel = -0.425f;
            else InterestLevel -= 0.125f;
        }
        if (InterestLevel > 1) InterestLevel = 1;
        else if (InterestLevel < -1) InterestLevel = -1;
    }
    public override void TryUnlockCombos()
    {
        activePage = book.ActivePage.GetComponent<PageScript>();
        CurrentCombos = book.GetCurrentCombos();
        float happiness = Emotions[(int)EmotionsEnum.Happiness];
        float calmness = Emotions[(int)EmotionsEnum.Calmness];
        if (happiness > (1.0f / 3.0f))
        {
            //if happiness high
            activePage.ActivateCombo("HappyHigh");
                        
        }
        else if ((happiness < (1.0f/3.0f)) || (happiness > (-1.0f / 3.0f))){
            //happiness middle
            activePage.ActivateCombo("HappyMid");
        }
        else if (happiness < (-1.0f / 3.0f))
        {
            //happiness low
            activePage.ActivateCombo("HappyLow");
        }

        if (calmness > (1.0f / 3.0f))
        {
            //if calmness high
            activePage.ActivateCombo("CalmHigh");

        }
        else if ((calmness < (1.0f / 3.0f)) || (calmness > (-1.0f / 3.0f)))
        {
            //calmness middle
            activePage.ActivateCombo("CalmMid");
        }
        else if (calmness < (-1.0f / 3.0f))
        {
            //calmness low

            activePage.ActivateCombo("CalmLow");
        }
    }
    protected override void InitActions()
    {
        throw new NotImplementedException();
    }
    protected override void UpdateWeights()
    {
        throw new NotImplementedException();
    }
    #endregion

    /// <summary>
    /// How the alien responds to the players action
    /// </summary>

    protected override void UpdateEmotions()
    {
        //Emotions[(int)EmotionsEnum.Happiness] = 1234;
       // Emotions[(int)EmotionsEnum.Calmness] = 2345;
    }

    private void UpdateEmotionFatigue()
    {

    }
    //down here we need actions
    //we will also need some way to vary the mood randomly based on the day
    //perhaps with some sort of carryover based on previous day behaviour

    #region actions
    bool Idle()
    {
        const float IdleTimerMin = 1.0f;
        const float IdleTimerMax = 5.0f;
        
        if (needNewAction) // on first call - set yourself as the active action
        { 
            actionTimer = Random.Range(IdleTimerMin, IdleTimerMax); // timer between 0 and 5 seconds
            needNewAction = false;
            currentAction = Idle;
        }
        //when timer up return true
        if (actionTimer > IdleTimerMax)
        {
            return true;//returning true tells update that we need a new action
        }
        //idle does nothing
        actionTimer += Time.deltaTime;
        return false;
    }
    #endregion
}