using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;


public enum BattleState { BattleSet, NewRound, ActionPhase, EnemyTurn, FightPhase, RoundResult, Win, Lost }
public enum BodyPart { Head, Torso, RightHand, LeftHand, Legs }



public class BattleSystem : MonoBehaviour
{

    //public PhotonView photonView;

    public BattleState state;

    [SerializeField] public GameObject playerPrefab;

    public GameObject enemyPrefab;

    public Transform enemyPosition;
    public Transform playerPosition;

    public Titan playerTitan;
    public Titan enemyTitan;

    public BattleHud playerHud;
    public BattleHud enemyHud;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI battleConsole;
    [SerializeField] GameObject playerDummy;
    [SerializeField] GameObject enemyDummy;
    [SerializeField] GameObject skillPanel;

    [SerializeField] Button[] skillButtons;

    [Header("For Targeting")]
    [SerializeField] Canvas enemyHeadCanvas;
    [SerializeField] Canvas enemyTorsoCanvas;
    [SerializeField] Canvas enemyRHCanvas;
    [SerializeField] Canvas enemyLHCanvas;
    [SerializeField] Canvas enemyLegsCanvas;

    [SerializeField] List<Image> enemyHeadImages = new List<Image>();
    [SerializeField] List<Image> enemyTorsoImages = new List<Image>();
    [SerializeField] List<Image> enemyRHImages = new List<Image>();
    [SerializeField] List<Image> enemyLHImages = new List<Image>();
    [SerializeField] List<Image> enemyLegsImages = new List<Image>();

    [SerializeField] Canvas playerHeadCanvas;
    [SerializeField] Canvas playerTorsoCanvas;
    [SerializeField] Canvas playerRHCanvas;
    [SerializeField] Canvas playerLHCanvas;
    [SerializeField] Canvas playerLegsCanvas;

    [SerializeField] List<Image> playerHeadImages = new List<Image>();
    [SerializeField] List<Image> playerTorsoImages = new List<Image>();
    [SerializeField] List<Image> playerRHImages = new List<Image>();
    [SerializeField] List<Image> playerLHImages = new List<Image>();
    [SerializeField] List<Image> playerLegsImages = new List<Image>();

    //public Action action = new Action();
    [SerializeField] private int targetsCounter;
    public bool targeting = false;

    public GameObject confirmBtn;
    public GameObject actionButtonPrefab;

    [Header("AP")]
    [SerializeField] GameObject APCounterGUI;
    [SerializeField] Canvas APCanvas;
    [SerializeField] Canvas APChosenSkillsCanvas;
    [SerializeField] List<Button> actionButtons = new List<Button>();
    [SerializeField] GameObject chosenSkillAPCost;
    [SerializeField] Canvas chosenSkillAPCostCanvas;


    [SerializeField] TextMeshProUGUI APCounterText;
    [SerializeField] TextMeshProUGUI APCounterSubText;
    [SerializeField] int APCurrent = 4;
    public List<TextMeshProUGUI> chosenSkillAPCostList = new List<TextMeshProUGUI>();


    [Header("BattleRound")]

    public SkillSO selectedSkill;
    public List<Action> actionList = new List<Action>();
    public List<Action> enemyActionList = new List<Action>();

    public List<Action> attackList = new List<Action>();
    public List<Action> blockList = new List<Action>();

    public List<Action> enemyAttackList = new List<Action>();
    public List<Action> enemyBlockList = new List<Action>();

    private bool blocked = false;


    #region Singleton
    public static BattleSystem instance;
    

    //public static BattleSystem Instance { get { return instance; } }
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    #endregion

    void Start()
    {
        NewRoundBegins();

    }



    private void NewRoundBegins()
    {
        battleConsole.text = "Choose your actions, Titan!";

        //UpdateSkillCD();

        ResetAPCounter();

        //ShowDummies

        //StartRoundTimer

        //APCounterGUI.SetActive(true);
        //confirmBtn.SetActive(true);
        //APCounterText.text = currentAP.ToString();
  
    }

    private void ResetAPCounter()
    {
        APCurrent = 4;
        APCounterText.text = APCurrent.ToString();
    }

    public void OnSkillSelected(int index)
    {

        //put clicked skill on a timely variable
        selectedSkill = playerTitan.skills[index];

        if (selectedSkill.APCost <= APCurrent)
        {
            if (selectedSkill.skillType == SkillType.Attacking)
            {
                //LightEnemyDummy();
                //
            }
            else if (selectedSkill.skillType == SkillType.Blocking)
            {
                //LightPlayerDummy();
            }

            CreateAction(index);
        }

    }

    private void CreateAction(int index)
    {
        //make all other skillButton unclickable and enable targeting mode
        EnableTargeting(index);

        //update AP
        APCurrent -= selectedSkill.APCost;
        APCounterText.text = APCurrent.ToString();

        // create new Action
        Action action = new Action();

        //add to action list
        actionList.Add(action);

        //count its index based on number of elements 
        action.actionIndex = actionList.Count - 1;

        //load selected skill
        action.skill = playerTitan.skills[index];

        //action type
        action.actionType = action.GetActionType(action.skill);

        //set the target counter based on numebr of targets of skill
        targetsCounter = selectedSkill.numberOfTargets;

        //create new button at canvas
        GameObject button = Instantiate(actionButtonPrefab) as GameObject;
        button.transform.SetParent(APChosenSkillsCanvas.transform, false);

        //put skill icon on this button
        Image image = button.GetComponent<Image>();
        image.sprite = selectedSkill.skillIcon;

        //add listener to pass right index to destroy this action images from Dummy
        button.GetComponent<Button>().onClick.AddListener(() => CancelAction(action.actionIndex));

        //add text element to other canvas
        GameObject buttonText = Instantiate(chosenSkillAPCost) as GameObject;
        buttonText.transform.SetParent(chosenSkillAPCostCanvas.transform, false);
        TextMeshProUGUI text = buttonText.GetComponent<TextMeshProUGUI>();
        chosenSkillAPCostList.Add(text);
        chosenSkillAPCostList.Last().text = action.skill.APCost.ToString();
    }

    private void EnableTargeting(int index)
    {
        targeting = true;
        foreach (var skillButton in skillButtons)
        {
            if (skillButton != skillButtons[index])
            {
                skillButton.interactable = false;
            }
        }
    }

    private void DisableTargeting()
    {
        selectedSkill = null;
        targeting = false;
        foreach (var skillButton in skillButtons)
        {
            skillButton.interactable = true;
        }
    }

    public void CancelAction(int actionIndex)
    {
        Debug.Log("HIIIIIIIII");
        int deletedIndex = actionIndex; // why it is here?
        //destroy imges
        foreach (var image in actionList[actionIndex].targetImages)
        {
            Destroy(image);
        }

        //update AP
        APCurrent += actionList[actionIndex].skill.APCost;
        APCounterText.text = APCurrent.ToString();

        //destroy action
        foreach (var item in actionList)
        {
            Debug.Log(item);
        }
        foreach (var item in actionButtons)
        {
            Debug.Log(item);
        }
        actionList.RemoveAt(actionIndex);

        //Destory this action text cost element
        Destroy(chosenSkillAPCostCanvas.transform.GetChild(actionIndex).gameObject);
        
        chosenSkillAPCostList.RemoveAt(actionIndex);

        //Destroy(actionButtons[actionIndex].GetComponent<Button>());
        //actionButtons.RemoveAt(actionIndex);


        //and re-sort
        foreach (var action in actionList)
        {
            if (action.actionIndex > deletedIndex)
            {
                action.actionIndex -= 1;
            }
        }
        //for (int i = deletedIndex + 1; i < actionButtons.Count; i++)
        //{
        //    actionButtons[i] = actionButtons[i - 1];
        //}

        //make all other skillButton unclickable and enable targeting mode
        DisableTargeting();

    }


    public void OnBodyPartClickTest(int index)
    {
        //check if we clicked skill beforfehand
        if (selectedSkill !=null && targeting)
        {
            if (selectedSkill.skillType == SkillType.Attacking)
            {
                switch (index)
                {
                    case 0:
                        addImageOnTarget(BodyPart.Head, enemyHeadCanvas, enemyHeadImages);

                        break;
                    case 1:
                        addImageOnTarget(BodyPart.Torso, enemyTorsoCanvas, enemyTorsoImages);

                        break;
                    case 2:
                        addImageOnTarget(BodyPart.RightHand, enemyRHCanvas, enemyRHImages);

                        break;
                    case 3:
                        addImageOnTarget(BodyPart.LeftHand, enemyLHCanvas, enemyLHImages);

                        break;
                    case 4:
                        addImageOnTarget(BodyPart.Legs, enemyLegsCanvas, enemyLegsImages);

                        break;

                    default:
                        break;
                }
            }
            if (selectedSkill.skillType == SkillType.Blocking)
            {
                switch (index)
                {
                    case 0:
                        addImageOnTarget(BodyPart.Head, playerHeadCanvas, playerHeadImages);

                        break;
                    case 1:
                        addImageOnTarget(BodyPart.Torso, playerTorsoCanvas, playerTorsoImages);

                        break;
                    case 2:
                        addImageOnTarget(BodyPart.RightHand, playerRHCanvas, playerRHImages);

                        break;
                    case 3:
                        addImageOnTarget(BodyPart.LeftHand, playerLHCanvas, playerLHImages);

                        break;
                    case 4:
                        addImageOnTarget(BodyPart.Legs, playerLegsCanvas, playerLegsImages);

                        break;

                    default:
                        break;
                }
            }//convert index to bodypart and adds to last element in actionList

            targetsCounter--;
            if (targetsCounter > 0) //if not all targets were tapped - continue
            {
                battleConsole.text = "Choose " + targetsCounter + " more target for selected skill";
            }
            else if (targetsCounter == 0) // of all - ten autoconfirm and next action pan
            {
                
                DisableTargeting();
                battleConsole.text = "Choose new Actions";
            }
        }
        else
        {
            battleConsole.text = "Choose and tap skill first";
        }
    }


    private void addImageOnTarget(BodyPart bodyPart, Canvas canvasPart, List<Image> bodyPartImageList)
    {
        //we take last action and do add bodypart where we want to target
        actionList.Last().targets.Add(bodyPart);

        //create gameObject to add then images of skill
        GameObject imagetarget = new GameObject();

        //add this GO to this action
        actionList.Last().targetImages.Add(imagetarget);

        //put in in Game to canvas
        actionList.Last().targetImages.Last().transform.SetParent(canvasPart.transform, false);

        //add sprite and image to this GO to bodypart list in inspector
        Image image = actionList.Last().targetImages.Last().AddComponent<Image>();
        bodyPartImageList.Add(image);
        image.sprite = selectedSkill.skillIcon;
    }

    //test Update script to destroy Images of selected skill
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            //Destroy Images from Dummy. here index [0] for example
            foreach (var image in actionList[0].targetImages)
            {
                    Destroy(image);
            }

        }
    }

    public void OnConfirmBtn()
    {
        Debug.Log(actionList.Count);
        Debug.Log(actionList[0].actionType);
        //put all player attack actions to attackactionlist //put all player block actions to blockactionlist
        foreach (var action in actionList)
        {
            if (action.actionType == ActionType.Attack)
            {
                attackList.Add(action);
               // actionList.Remove(actionG);
            }
            else if (action.actionType == ActionType.Block)
            {
                blockList.Add(action);
                //actionList.Remove(actionG);
            }
        }
        Debug.Log(attackList.Count);
        //Create enemy actions (test mode)
        EnemyActions();
        StartCoroutine("Pause");
        Debug.Log("Let the fight begin");
        Fight();

    }

    private void Fight()
    {
        UseGlobalEffects();
        PlayerAttack();
        EnemyAttacks();

    }

    private void EnemyAttacks() // refactor this to one method wit playerattack
    {
        while (enemyAttackList.Any())
        {
            Action action = new Action();
            BodyPart target = new BodyPart();

            action = enemyAttackList[0];
            enemyAttackList.RemoveAt(0);

            for (int i = 0; i < action.targets.Count; i++)
            {
                //reset blocked bool
                blocked = false;

                //we take first target of Attack
                target = action.targets[i];

                //ckeck if it is blocked (we search for same target in block list of enemy)
                foreach (var playerActions in blockList)
                {
                    if (playerActions.targets.Contains(target)) //if we find
                    {
                        blocked = true; // we mark that
                        Debug.Log("Blocked by player");
                        //play that skill
                        //add block formula
                        //add damage as modifier
                        //block event

                    }

                }
                if (!blocked) // if no block then that means we do all damage
                {
                    Debug.Log("No block detected, I m enemy and Ive just attacked!!!!");
                }
            }
        }
    }

    private void PlayerAttack()
    {


        //loop all action attack list until it is not empty
        while (attackList.Any())
        {
            //create timely local vars for comfort
            Action action = new Action();
            BodyPart target = new BodyPart();

            //take skill in action one by one
            action = attackList[0];
            attackList.RemoveAt(0);
            // action.skill.EffectOnCast();

            //get targets of that skill one by one
            for (int i = 0; i < action.targets.Count; i++)
            {
                //reset blocked bool
                blocked = false;

                //we take first target of Attack
                target = action.targets[i];

                //ckeck if it is blocked (we search for same target in block list of enemy)
                foreach (var enemyAction in enemyBlockList)
                {
                    if (enemyAction.targets.Contains(target)) //if we find
                    {
                        blocked = true; // we mark that
                        Debug.Log("Blocked");
                        //play that skill
                        //add block formula
                        //add damage as modifier
                        //block event

                    }

                }
                if (!blocked) // if no block then that means we do all damage
                {
                    Debug.Log("No block detected, lets crush him!");
                }
            }
        }
        //find first atacking skill, put in on timely var currentAction and remove from the list (so not to find it again

        Debug.Log("NO more attack actions left!");
    }

    private void UseGlobalEffects()
    {
        //player  globals
        foreach (var action in actionList)
        {
            action.skill.EffectOnGlobal();
            Debug.Log("player eefct on global skill" + action.skill.skillName);
        }

        //enemy  globals
        foreach (var actiond in enemyActionList)
        {
            actiond.skill.EffectOnGlobal();
            Debug.Log("enemy eefct on global skill" + actiond.skill.skillName);
        }
    }

    IEnumerator Pause()
    {
        yield return new WaitForSeconds(1);
    }

}

////load prefabs for both playwers
//private void SetBattle()
//{

//    Debug.Log("I want to create player");
//    SetPlayer();    //load prefab from resourses and sets it's hud
//    SetEnemy();
//    NewRoundBegins();
//}

////load player prefab from resourses (not resoures currently) and sets it's hud
//private void SetPlayer()
//{
//    SetTitan();
//    //playerHud.SetHud(playerTitan);
//    SetSkills(playerTitan);

//}

//private void SetTitan()
//{
//    //GameObject playerGO = Instantiate(Resources.Load("Player") as GameObject, playerPosition); //see if we can use same script for both titans
//    GameObject playerGO = Instantiate(playerPrefab as GameObject, playerPosition);
//    playerTitan = playerGO.GetComponent<Titan>();
//    Debug.Log(playerTitan);
//    playerTitan.SetStats();
//}

////load images of the skills )from titan prefab to game
//private void SetSkills(Titan titan)
//{
//    Image skill1IMG;

//    for (int i = 0; i < skillButtons.Length; i++)
//    {
//        if (titan.skills[i] != null)
//        {
//            skill1IMG = skillButtons[i].GetComponent<Image>();
//            skill1IMG.sprite = titan.skills[i].skillIcon;
//        }
//    }
//}

////load enemy prefab from resourses and sets it's hud
//private void SetEnemy()
//{
//    Debug.Log("Creating Enemy");
//    GameObject enemyGO = Instantiate(Resources.Load("Enemy") as GameObject, enemyPosition);
//    enemyTitan = enemyGO.GetComponent<Titan>();
//    //enemyHud.SetHud(enemyTitan);
//}

//private void EnemyActions()
//{

//    //create action 1 atacking + blocking
//    Action enemyAction1 = new Action();
//    enemyAction1.actionIndex = 0;
//    enemyAction1.skill = enemyTitan.skills[0];
//    enemyAction1.targets.Add(BodyPart.Head);
//    enemyActionList.Add(enemyAction1);
//    enemyAction1.actionType = enemyAction1.GetActionType(enemyAction1.skill);
//    enemyAttackList.Add(enemyAction1);

//    Action enemyAction2 = new Action();
//    enemyAction2.actionIndex = 1;
//    enemyAction2.skill = enemyTitan.skills[1];
//    enemyAction2.targets.Add(BodyPart.Head);
//    enemyActionList.Add(enemyAction2);
//    enemyAction2.actionType = enemyAction2.GetActionType(enemyAction2.skill);
//    enemyBlockList.Add(enemyAction2);

//    Action enemyAction3 = new Action();
//    enemyAction3.actionIndex = 2;
//    enemyAction3.skill = enemyTitan.skills[1];
//    enemyAction3.targets.Add(BodyPart.Torso);
//    enemyActionList.Add(enemyAction3);
//    enemyAction3.actionType = enemyAction3.GetActionType(enemyAction3.skill);
//    enemyBlockList.Add(enemyAction3);



//}


//global copy

//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using TMPro;
//using UnityEngine;
//using UnityEngine.UI;
//using Photon.Pun;


//public enum BattleState { BattleSet, NewRound, ActionPhase, EnemyTurn, FightPhase, RoundResult, Win, Lost }
//public enum BodyPart { Head, Torso, RightHand, LeftHand, Legs }



//public class BattleSystem : MonoBehaviour
//{

//    //public PhotonView photonView;

//    public BattleState state;

//    [SerializeField] public GameObject playerPrefab;

//    public GameObject enemyPrefab;

//    public Transform enemyPosition;
//    public Transform playerPosition;

//    public Titan playerTitan;
//    public Titan enemyTitan;

//    public BattleHud playerHud;
//    public BattleHud enemyHud;

//    [Header("UI")]
//    [SerializeField] TextMeshProUGUI battleConsole;
//    [SerializeField] GameObject playerDummy;
//    [SerializeField] GameObject enemyDummy;
//    [SerializeField] GameObject skillPanel;

//    [SerializeField] Button[] skillButtons;

//    [Header("For Targeting")]
//    [SerializeField] Canvas enemyHeadCanvas;
//    [SerializeField] Canvas enemyTorsoCanvas;
//    [SerializeField] Canvas enemyRHCanvas;
//    [SerializeField] Canvas enemyLHCanvas;
//    [SerializeField] Canvas enemyLegsCanvas;

//    [SerializeField] List<Image> enemyHeadImages = new List<Image>();
//    [SerializeField] List<Image> enemyTorsoImages = new List<Image>();
//    [SerializeField] List<Image> enemyRHImages = new List<Image>();
//    [SerializeField] List<Image> enemyLHImages = new List<Image>();
//    [SerializeField] List<Image> enemyLegsImages = new List<Image>();

//    [SerializeField] Canvas playerHeadCanvas;
//    [SerializeField] Canvas playerTorsoCanvas;
//    [SerializeField] Canvas playerRHCanvas;
//    [SerializeField] Canvas playerLHCanvas;
//    [SerializeField] Canvas playerLegsCanvas;

//    [SerializeField] List<Image> playerHeadImages = new List<Image>();
//    [SerializeField] List<Image> playerTorsoImages = new List<Image>();
//    [SerializeField] List<Image> playerRHImages = new List<Image>();
//    [SerializeField] List<Image> playerLHImages = new List<Image>();
//    [SerializeField] List<Image> playerLegsImages = new List<Image>();

//    //public Action action = new Action();
//    [SerializeField] private int targetsCounter;
//    public bool targeting = false;

//    public GameObject confirmBtn;
//    public GameObject actionButtonPrefab;

//    [Header("AP")]
//    [SerializeField] GameObject APCounterGUI;
//    [SerializeField] Canvas APCanvas;
//    [SerializeField] Canvas APChosenSkillsCanvas;
//    [SerializeField] List<Button> actionButtons = new List<Button>();
//    [SerializeField] GameObject chosenSkillAPCost;
//    [SerializeField] Canvas chosenSkillAPCostCanvas;


//    [SerializeField] TextMeshProUGUI APCounterText;
//    [SerializeField] TextMeshProUGUI APCounterSubText;
//    [SerializeField] int APCurrent = 4;
//    public List<TextMeshProUGUI> chosenSkillAPCostList = new List<TextMeshProUGUI>();


//    [Header("BattleRound")]

//    public SkillSO selectedSkill;
//    public List<Action> actionList = new List<Action>();
//    public List<Action> enemyActionList = new List<Action>();

//    public List<Action> attackList = new List<Action>();
//    public List<Action> blockList = new List<Action>();

//    public List<Action> enemyAttackList = new List<Action>();
//    public List<Action> enemyBlockList = new List<Action>();

//    private bool blocked = false;


//    #region Singleton
//    public static BattleSystem instance;


//    //public static BattleSystem Instance { get { return instance; } }
//    private void Awake()
//    {
//        if (instance != null && instance != this)
//        {
//            Destroy(this.gameObject);
//        }
//        else
//        {
//            instance = this;
//        }
//    }
//    #endregion

//    //void Start()
//    //{

//    //    state = BattleState.BattleSet;
//    //   // SetBattle();

//    //}



//    private void NewRoundBegins()
//    {
//        battleConsole.text = "Choose your actions, Titan!";

//        //UpdateSkillCD();

//        ResetAPCounter();

//        //ShowDummies

//        //StartRoundTimer

//        //APCounterGUI.SetActive(true);
//        //confirmBtn.SetActive(true);
//        //APCounterText.text = currentAP.ToString();

//    }

//    private void ResetAPCounter()
//    {
//        APCurrent = 4;
//        APCounterText.text = APCurrent.ToString();
//    }

//    public void OnSkillSelected(int index)
//    {

//        //put clicked skill on a timely variable
//        selectedSkill = playerTitan.skills[index];

//        if (selectedSkill.APCost <= APCurrent)
//        {
//            if (selectedSkill.skillType == SkillType.Attacking)
//            {
//                //LightEnemyDummy();
//                //
//            }
//            else if (selectedSkill.skillType == SkillType.Blocking)
//            {
//                //LightPlayerDummy();
//            }

//            CreateAction(index);
//        }

//    }

//    private void CreateAction(int index)
//    {
//        //make all other skillButton unclickable and enable targeting mode
//        EnableTargeting(index);

//        //update AP
//        APCurrent -= selectedSkill.APCost;
//        APCounterText.text = APCurrent.ToString();

//        // create new Action
//        Action action = new Action();

//        //add to action list
//        actionList.Add(action);

//        //count its index based on number of elements 
//        action.actionIndex = actionList.Count - 1;

//        //load selected skill
//        action.skill = playerTitan.skills[index];

//        //action type
//        action.actionType = action.GetActionType(action.skill);

//        //set the target counter based on numebr of targets of skill
//        targetsCounter = selectedSkill.numberOfTargets;

//        //create new button at canvas
//        GameObject button = Instantiate(actionButtonPrefab) as GameObject;
//        button.transform.SetParent(APChosenSkillsCanvas.transform, false);

//        //put skill icon on this button
//        Image image = button.GetComponent<Image>();
//        image.sprite = selectedSkill.skillIcon;

//        //add listener to pass right index to destroy this action images from Dummy
//        button.GetComponent<Button>().onClick.AddListener(() => CancelAction(action.actionIndex));

//        //add text element to other canvas
//        GameObject buttonText = Instantiate(chosenSkillAPCost) as GameObject;
//        buttonText.transform.SetParent(chosenSkillAPCostCanvas.transform, false);
//        TextMeshProUGUI text = buttonText.GetComponent<TextMeshProUGUI>();
//        chosenSkillAPCostList.Add(text);
//        chosenSkillAPCostList.Last().text = action.skill.APCost.ToString();
//    }

//    private void EnableTargeting(int index)
//    {
//        targeting = true;
//        foreach (var skillButton in skillButtons)
//        {
//            if (skillButton != skillButtons[index])
//            {
//                skillButton.interactable = false;
//            }
//        }
//    }

//    private void DisableTargeting()
//    {
//        selectedSkill = null;
//        targeting = false;
//        foreach (var skillButton in skillButtons)
//        {
//            skillButton.interactable = true;
//        }
//    }

//    public void CancelAction(int actionIndex)
//    {
//        Debug.Log("HIIIIIIIII");
//        int deletedIndex = actionIndex; // why it is here?
//        //destroy imges
//        foreach (var image in actionList[actionIndex].targetImages)
//        {
//            Destroy(image);
//        }

//        //update AP
//        APCurrent += actionList[actionIndex].skill.APCost;
//        APCounterText.text = APCurrent.ToString();

//        //destroy action
//        foreach (var item in actionList)
//        {
//            Debug.Log(item);
//        }
//        foreach (var item in actionButtons)
//        {
//            Debug.Log(item);
//        }
//        actionList.RemoveAt(actionIndex);

//        //Destory this action text cost element
//        Destroy(chosenSkillAPCostCanvas.transform.GetChild(actionIndex).gameObject);

//        chosenSkillAPCostList.RemoveAt(actionIndex);

//        //Destroy(actionButtons[actionIndex].GetComponent<Button>());
//        //actionButtons.RemoveAt(actionIndex);


//        //and re-sort
//        foreach (var action in actionList)
//        {
//            if (action.actionIndex > deletedIndex)
//            {
//                action.actionIndex -= 1;
//            }
//        }
//        //for (int i = deletedIndex + 1; i < actionButtons.Count; i++)
//        //{
//        //    actionButtons[i] = actionButtons[i - 1];
//        //}

//        //make all other skillButton unclickable and enable targeting mode
//        DisableTargeting();

//    }


//    public void OnBodyPartClickTest(int index)
//    {
//        //check if we clicked skill beforfehand
//        if (selectedSkill != null && targeting)
//        {
//            if (selectedSkill.skillType == SkillType.Attacking)
//            {
//                switch (index)
//                {
//                    case 0:
//                        addImageOnTarget(BodyPart.Head, enemyHeadCanvas, enemyHeadImages);

//                        break;
//                    case 1:
//                        addImageOnTarget(BodyPart.Torso, enemyTorsoCanvas, enemyTorsoImages);

//                        break;
//                    case 2:
//                        addImageOnTarget(BodyPart.RightHand, enemyRHCanvas, enemyRHImages);

//                        break;
//                    case 3:
//                        addImageOnTarget(BodyPart.LeftHand, enemyLHCanvas, enemyLHImages);

//                        break;
//                    case 4:
//                        addImageOnTarget(BodyPart.Legs, enemyLegsCanvas, enemyLegsImages);

//                        break;

//                    default:
//                        break;
//                }
//            }
//            if (selectedSkill.skillType == SkillType.Blocking)
//            {
//                switch (index)
//                {
//                    case 0:
//                        addImageOnTarget(BodyPart.Head, playerHeadCanvas, playerHeadImages);

//                        break;
//                    case 1:
//                        addImageOnTarget(BodyPart.Torso, playerTorsoCanvas, playerTorsoImages);

//                        break;
//                    case 2:
//                        addImageOnTarget(BodyPart.RightHand, playerRHCanvas, playerRHImages);

//                        break;
//                    case 3:
//                        addImageOnTarget(BodyPart.LeftHand, playerLHCanvas, playerLHImages);

//                        break;
//                    case 4:
//                        addImageOnTarget(BodyPart.Legs, playerLegsCanvas, playerLegsImages);

//                        break;

//                    default:
//                        break;
//                }
//            }//convert index to bodypart and adds to last element in actionList

//            targetsCounter--;
//            if (targetsCounter > 0) //if not all targets were tapped - continue
//            {
//                battleConsole.text = "Choose " + targetsCounter + " more target for selected skill";
//            }
//            else if (targetsCounter == 0) // of all - ten autoconfirm and next action pan
//            {

//                DisableTargeting();
//                battleConsole.text = "Choose new Actions";
//            }
//        }
//        else
//        {
//            battleConsole.text = "Choose and tap skill first";
//        }
//    }


//    private void addImageOnTarget(BodyPart bodyPart, Canvas canvasPart, List<Image> bodyPartImageList)
//    {
//        //we take last action and do add bodypart where we want to target
//        actionList.Last().targets.Add(bodyPart);

//        //create gameObject to add then images of skill
//        GameObject imagetarget = new GameObject();

//        //add this GO to this action
//        actionList.Last().targetImages.Add(imagetarget);

//        //put in in Game to canvas
//        actionList.Last().targetImages.Last().transform.SetParent(canvasPart.transform, false);

//        //add sprite and image to this GO to bodypart list in inspector
//        Image image = actionList.Last().targetImages.Last().AddComponent<Image>();
//        bodyPartImageList.Add(image);
//        image.sprite = selectedSkill.skillIcon;
//    }

//    //test Update script to destroy Images of selected skill
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.T))
//        {
//            //Destroy Images from Dummy. here index [0] for example
//            foreach (var image in actionList[0].targetImages)
//            {
//                Destroy(image);
//            }

//        }
//    }

//    public void OnConfirmBtn()
//    {
//        Debug.Log(actionList.Count);
//        Debug.Log(actionList[0].actionType);
//        //put all player attack actions to attackactionlist //put all player block actions to blockactionlist
//        foreach (var action in actionList)
//        {
//            if (action.actionType == ActionType.Attack)
//            {
//                attackList.Add(action);
//                // actionList.Remove(actionG);
//            }
//            else if (action.actionType == ActionType.Block)
//            {
//                blockList.Add(action);
//                //actionList.Remove(actionG);
//            }
//        }
//        Debug.Log(attackList.Count);
//        //Create enemy actions (test mode)
//        EnemyActions();
//        StartCoroutine("Pause");
//        Debug.Log("Let the fight begin");
//        Fight();

//    }

//    private void Fight()
//    {
//        UseGlobalEffects();
//        PlayerAttack();
//        EnemyAttacks();

//    }

//    private void EnemyAttacks() // refactor this to one method wit playerattack
//    {
//        while (enemyAttackList.Any())
//        {
//            Action action = new Action();
//            BodyPart target = new BodyPart();

//            action = enemyAttackList[0];
//            enemyAttackList.RemoveAt(0);

//            for (int i = 0; i < action.targets.Count; i++)
//            {
//                //reset blocked bool
//                blocked = false;

//                //we take first target of Attack
//                target = action.targets[i];

//                //ckeck if it is blocked (we search for same target in block list of enemy)
//                foreach (var playerActions in blockList)
//                {
//                    if (playerActions.targets.Contains(target)) //if we find
//                    {
//                        blocked = true; // we mark that
//                        Debug.Log("Blocked by player");
//                        //play that skill
//                        //add block formula
//                        //add damage as modifier
//                        //block event

//                    }

//                }
//                if (!blocked) // if no block then that means we do all damage
//                {
//                    Debug.Log("No block detected, I m enemy and Ive just attacked!!!!");
//                }
//            }
//        }
//    }

//    private void PlayerAttack()
//    {


//        //loop all action attack list until it is not empty
//        while (attackList.Any())
//        {
//            //create timely local vars for comfort
//            Action action = new Action();
//            BodyPart target = new BodyPart();

//            //take skill in action one by one
//            action = attackList[0];
//            attackList.RemoveAt(0);
//            // action.skill.EffectOnCast();

//            //get targets of that skill one by one
//            for (int i = 0; i < action.targets.Count; i++)
//            {
//                //reset blocked bool
//                blocked = false;

//                //we take first target of Attack
//                target = action.targets[i];

//                //ckeck if it is blocked (we search for same target in block list of enemy)
//                foreach (var enemyAction in enemyBlockList)
//                {
//                    if (enemyAction.targets.Contains(target)) //if we find
//                    {
//                        blocked = true; // we mark that
//                        Debug.Log("Blocked");
//                        //play that skill
//                        //add block formula
//                        //add damage as modifier
//                        //block event

//                    }

//                }
//                if (!blocked) // if no block then that means we do all damage
//                {
//                    Debug.Log("No block detected, lets crush him!");
//                }
//            }
//        }
//        //find first atacking skill, put in on timely var currentAction and remove from the list (so not to find it again

//        Debug.Log("NO more attack actions left!");
//    }

//    private void UseGlobalEffects()
//    {
//        //player  globals
//        foreach (var action in actionList)
//        {
//            action.skill.EffectOnGlobal();
//            Debug.Log("player eefct on global skill" + action.skill.skillName);
//        }

//        //enemy  globals
//        foreach (var actiond in enemyActionList)
//        {
//            actiond.skill.EffectOnGlobal();
//            Debug.Log("enemy eefct on global skill" + actiond.skill.skillName);
//        }
//    }

//    IEnumerator Pause()
//    {
//        yield return new WaitForSeconds(1);
//    }

//}