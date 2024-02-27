using UnityEngine;
using System;
using UnityEngine.AI;
using System.Data;


public class StarAlien : MonoBehaviour, IAlien
{
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

    private NavMeshAgent nav;
    public Transform HomeSpot;

    private new MeshRenderer renderer;
    private PlayerUIManager UIManager;

    /// <summary>
    /// Start is the initial setup function, called before the first frame update
    /// </summary>
    void Start()
    {
       
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
    }

    /// <summary>
    /// Update is the objects game loop function, called once per frame
    /// </summary>
    void Update()
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
    void DoStarBobbing()
    {
        transform.position += Vector3.up * 0.01f *  Mathf.Sin( 2 * Time.time);
    }
    void DoStarSpin()
    {
        //this needs looking at it completely murders the AI controller
        //I think it just needs to rotate the child mesh instead of the parent empty
        transform.Rotate(Vector3.right, 10/happiness,Space.Self);
    }
    /// <summary>
    /// How the alien responds to the players action
    /// </summary>



    //IAlien
    public void React(Tool tool)
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
}