using UnityEngine;
using System;
using UnityEngine.AI;
using System.Data;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class StarAlien : Alien
{
    #region legacyMembers
    public int happiness
    {
        get { return _happiness; }
        set { if (value <= 0) _happiness = 1; else _happiness = value; }
    }
    private int _happiness;
    public int calmness = 5; // Higher is happier, higher is calmer
    [HideInInspector]
    public float relationship
    {
        get { return (happiness + calmness) / 2; }
        private set { }
    }
    #endregion

    
    //private Dictionary<EmotionsEnum, double> Emotions = new Dictionary<EmotionsEnum, double>();
    //this is a dictionary here but using an enum I suppose you could also assign each one an int value (in the enum) and then it can be a straight array
    //not entirely sure this is an important implementation decision
    //but *technically* we don't *need* a dictionary since we only need to store one float per member of the enum and that is all known at compile time
    //I think i'm just trying to over-optimise (getting a bit c++ brained with arrays vs "real" generic containers)


    //navmesh could be moved to parent too
    private NavMeshAgent nav;
    public Transform HomeSpot;

    private new MeshRenderer renderer;
    private PlayerUIManager UIManager;
    private Transform MeshTransform;
    
    
    #region unityMethods
    /// <summary>
    /// Start is the initial setup function, called before the first frame update
    /// </summary>
    /// 
    public override void Start()
    {
        MeshTransform = transform.Find("StarMesh");
        MeshRenderer[] renderCandidates = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer render in renderCandidates)
        {
            if (render.CompareTag("StarEye"))
                renderer = render;
        }
        
        happiness = 5;
        nav = GetComponent<NavMeshAgent>();
        nav.SetDestination(HomeSpot.position);
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");

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
        Color skinColour = new (
            .65f + (happiness *  .035f), // The change is calculated through: (end - start) / (steps - 1)
            .00f + (happiness *  .100f), // There are 11 steps of from 0 to 11
            .65f + (happiness * -.065f)  // The start value was RGB(166, 0, 166), end value was RGB(255, 255, 0) 
        );
        renderer.material.color = skinColour;

        // Set the spin speed of the alien based on its calmness

        
    }
     void FixedUpdate()
    {
        DoStarBobbing();
        DoStarSpin();
    }
    #endregion
    #region bobAndSpin
    void DoStarBobbing()
    {
        MeshTransform.position += Vector3.up * 0.01f *  Mathf.Sin( 2 * Time.time);
    }
    void DoStarSpin()
    {
        //this needs looking at it completely murders the AI controller
        //I think it just needs to rotate the child mesh instead of the parent empty
        MeshTransform.Rotate(Vector3.right, 10/happiness,Space.Self);
    }
    #endregion

    
    #region Virtuals
    //From parent clas
    override public void React(Tool tool)
    {
        Debug.Log(string.Format("reacted to {0}", tool.toolType));
        if (tool.toolType == Tools.Touch_Gently)
        {
            happiness = Math.Min(10, happiness + 1); 
        }
        else if (tool.toolType == Tools.Touch_Roughly)
        {
            happiness = Math.Min(10, happiness - 1);
        }
        else if (tool.toolType == Tools.Feed_Treat)
        {
            calmness = Math.Min(10, calmness + 1);
        }
        else if (tool.toolType == Tools.Feed_LiveAnimal)
        {
            calmness = Math.Min(10, calmness - 1);
        }
        else if (tool.toolType == Tools.Item_Oscilliscope)
        {
        }
        // Update the UI
        UIManager.UpdateRelationshipBar(relationship);
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