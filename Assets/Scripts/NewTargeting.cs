using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class SerializableAction
{
    public int skillId;
    public List<int> targets = new List<int>();
}

public enum BodyPart { Head, Torso, RightHand, LeftHand, Legs }

public class NewTargeting : MonoBehaviour
{
    public static NewTargeting instance;

    private PhotonView photonView;

    private List<Action> masterClientActions;
    private List<Action> otherPlayerActions;

    public List<SkillSO> skills = new List<SkillSO>();
    public Button[] skillButtons;
    public Button[] attackBodyPartButtons;
    public Button[] defendBodyPartButtons;
    public GameObject cancelButtonPrefab;
    public Transform cancelButtonParent;

    // Variables to hold glowing outline effects for buttons
    private Outline[] attackButtonOutlines;
    private Outline[] defendButtonOutlines;

    public Button confirmButton;
    private bool masterClientConfirmed = false;
    private bool otherPlayerConfirmed = false;

    private SkillSO currentSkill;
    private Action currentAction;
    public List<Action> chosenActions = new List<Action>();
    public Dictionary<Button, Action> cancelButtonDictionary = new Dictionary<Button, Action>();

    public bool titansAssigned = false;
    public Titan playerTitan;

    public Dictionary<Button, int> imageCountPerButton = new Dictionary<Button, int>();

    public FightCalc fightCalc;


    private void Awake()
    {

        Debug.Log("NewTargeting instance created.");  // Add this line

        if (instance != null)
        {
            Debug.LogError("More than one instance of NewTargeting found!");
            return;
        }
        instance = this;


        // Get the PhotonView component
        photonView = GetComponent<PhotonView>();

        // Add listener to confirm button
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        // Initialize the lists in Awake()
        masterClientActions = new List<Action>();
        otherPlayerActions = new List<Action>();


        fightCalc = GetComponent<FightCalc>();

        if (fightCalc != null)
        {
            Debug.Log("FightCalc has been successfully attached.");
            fightCalc.test();
        }
        else
        {
            Debug.LogError("FightCalc is not attached.");
        }


    }

    public void Initialize(Button[] attackBodyPartButtons, Button[] defendBodyPartButtons)
    {
        this.attackBodyPartButtons = attackBodyPartButtons;
        this.defendBodyPartButtons = defendBodyPartButtons;

        attackButtonOutlines = InitializeButtonOutlines(attackBodyPartButtons);
        defendButtonOutlines = InitializeButtonOutlines(defendBodyPartButtons);

        // Disable and hide all buttons at the start
        EnableAndShowButtons(attackBodyPartButtons, false);
        EnableAndShowButtons(defendBodyPartButtons, false);
    }

    private void EnableAndShowButtons(Button[] buttons, bool enable)
    {
        foreach (Button button in buttons)
        {
            button.enabled = enable;
            button.gameObject.SetActive(enable);
        }
    }

    private Outline[] InitializeButtonOutlines(Button[] buttons)
    {
        Outline[] outlines = new Outline[buttons.Length];
        for (int i = 0; i < buttons.Length; i++)
        {
            Outline outline = buttons[i].GetComponent<Outline>();
            if (outline == null)
            {
                outline = buttons[i].gameObject.AddComponent<Outline>();
                outline.effectColor = Color.cyan; // Color for glow effect
                outline.effectDistance = new Vector2(5, 5); // Distance for glow effect
            }
            outlines[i] = outline;
            outline.enabled = false; // Disable glow by default
        }
        return outlines;
    }

    private void SetButtonGlow(Outline[] outlines, bool enable)
    {
        foreach (Outline outline in outlines)
        {
            outline.enabled = enable;
        }
    }

    void Update()
    {
        // Check if the Titan has been assigned to this player.
        if (!titansAssigned && playerTitan != null)
        {
            titansAssigned = true;
            startScript();
        }
    }

    private void startScript()
    {
        for (int i = 0; i < skillButtons.Length; i++)
        {
            int index = i;
            skillButtons[i].onClick.AddListener(() => ChooseSkill(playerTitan.skills[index]));
        }

        for (int i = 0; i < attackBodyPartButtons.Length; i++)
        {
            BodyPart bodyPart = (BodyPart)i;
            attackBodyPartButtons[i].onClick.AddListener(() => ChooseBodyPart(bodyPart, ActionType.Attack));
        }

        for (int i = 0; i < defendBodyPartButtons.Length; i++)
        {
            BodyPart bodyPart = (BodyPart)i;
            defendBodyPartButtons[i].onClick.AddListener(() => ChooseBodyPart(bodyPart, ActionType.Block));
        }



    }


    public void ChooseSkill(SkillSO chosenSkill)
    {
        currentSkill = chosenSkill;
        currentAction = new Action();
        currentAction.skill = currentSkill;
        currentAction.targetsNeeded = currentSkill.numberOfTargets;

        chosenActions.Add(currentAction);

        // Determine if the skill is attacking or blocking and enable corresponding buttons
        if (currentSkill.skillType == SkillType.Attacking)
        {
            currentAction.actionType = ActionType.Attack;
            EnableAndShowButtons(attackBodyPartButtons, true);
            SetButtonGlow(attackButtonOutlines, true);
        }
        else if (currentSkill.skillType == SkillType.Blocking)
        {
            currentAction.actionType = ActionType.Block;
            EnableAndShowButtons(defendBodyPartButtons, true);
            SetButtonGlow(defendButtonOutlines, true);
        }
    }

    public void ChooseBodyPart(BodyPart target, ActionType actionType)
    {
        if (currentSkill != null && currentAction.targets.Count < currentSkill.numberOfTargets)
        {
            // Add target to the current action
            currentAction.targets.Add(target);

            // Decide which set of buttons to use based on the action type
            Button[] actionButtons;
            if (actionType == ActionType.Attack)
            {
                actionButtons = attackBodyPartButtons;
            }
            else // ActionType.Block
            {
                actionButtons = defendBodyPartButtons;
            }

            // Create a new GameObject to hold the image
            GameObject newImage = new GameObject(currentSkill.skillName);
            newImage.transform.SetParent(actionButtons[(int)target].transform, false);

            // Add the Image component and set it to the skill's icon
            Image newImageComponent = newImage.AddComponent<Image>();
            newImageComponent.sprite = currentSkill.skillIcon;
            newImageComponent.preserveAspect = true;

            // Set the image RectTransform to fill the parent button
            RectTransform rectTransform = newImage.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 0);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.offsetMin = new Vector2(0, 0);
            rectTransform.offsetMax = new Vector2(0, 0);

            GridLayoutGroup grid = actionButtons[(int)target].GetComponentInChildren<GridLayoutGroup>();
            if (grid == null)
            {
                grid = actionButtons[(int)target].gameObject.AddComponent<GridLayoutGroup>();
            }

            int maxPerRow = 2;
            int rows = (actionButtons[(int)target].transform.childCount - 1) / maxPerRow + 1;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = maxPerRow;
            grid.cellSize = new Vector2(actionButtons[(int)target].GetComponent<RectTransform>().rect.width / maxPerRow,
                                        actionButtons[(int)target].GetComponent<RectTransform>().rect.height / rows);

            // Store the image with the action so we can delete it later
            currentAction.targetImages.Add(newImage);

            if (currentAction.targets.Count == currentSkill.numberOfTargets)
            {
                CreateCancelButton(currentAction);

                // Disable glow when all targets are chosen
                if (actionType == ActionType.Attack)
                    SetButtonGlow(attackButtonOutlines, false);
                else // ActionType.Block
                    SetButtonGlow(defendButtonOutlines, false);
            }
        }
    }



    public void CreateCancelButton(Action action)
    {
        // Instantiate new cancel button
        GameObject newButton = Instantiate(cancelButtonPrefab, cancelButtonParent);

        // Get the Button component and set it to call CancelAction() when clicked
        Button buttonComponent = newButton.GetComponent<Button>();
        buttonComponent.onClick.AddListener(() => CancelAction(action));

        // Get the Image component and set the sprite to the skill icon
        Image imageComponent = newButton.GetComponent<Image>();
        if (imageComponent != null)
        {
            imageComponent.sprite = action.skill.skillIcon;
        }

        // Store the button with the action so we can delete it later
        action.cancelButton = buttonComponent;
    }





    public void ChooseAction()
    {
        if (currentAction != null && currentAction.targets.Count >= currentSkill.numberOfTargets)
        {
            // Pass the current action when creating the cancel button
            CreateCancelButton(currentAction);

            // Reset current action to null
            currentAction = null;
        }
    }


    public void CancelAction(Action actionToCancel)
    {
        // Remove the action from the chosen actions
        chosenActions.Remove(actionToCancel);

        // Destroy the images on the targets
        foreach (GameObject image in actionToCancel.targetImages)
        {
            Destroy(image);
        }

        // Destroy the cancel button
        Destroy(actionToCancel.cancelButton.gameObject);
    }

    private void OnConfirmButtonClicked()
    {
        foreach (var action in chosenActions)
        {
            Debug.Log($"{action.actionType} Action with {action.skill.skillType} skill of the otherePlayer");
        }
        // Convert chosenActions list to a format that can be sent over the network.
        var chosenActionsData = ConvertChosenActionsToData(chosenActions);

        Debug.Log($"OnConfirmButtonClicked - Actions data: {chosenActionsData}");
        // rest of your code

        if (!PhotonNetwork.IsMasterClient)
        {
            // Send chosenActionsData to the master client via an RPC
            photonView.RPC("ReceiveChosenActions", RpcTarget.MasterClient, chosenActionsData);
        }
        else
        {
            // If the client is the master client, store the chosen actions in masterClientActions
            masterClientActions = chosenActions;

            // Set masterClientConfirmed to true
            masterClientConfirmed = true;

            Debug.Log("I am the master client, so I don't need to send the action list.");
        }

        if (masterClientConfirmed && otherPlayerConfirmed)
        {

            Debug.Log("BothConfirmed");

            if (otherPlayerActions.Count > 0)
            {
                Debug.Log($"First action skill name: {otherPlayerActions[0].skill.skillName}");
            }
            else
            {
                Debug.Log("No actions received");
            }
            {
                Debug.Log($"In OnConfirmButtonClicked: {otherPlayerActions.Count} actions in otherPlayerActions");

                fightCalc.StartBattleCalculation(masterClientActions, otherPlayerActions);
            }
        }
        else
        {
            Debug.Log("Only One playeer confirmed");
        }

    }



    private string ConvertChosenActionsToData(List<Action> chosenActions)
    {
        List<SerializableAction> serializableActions = new List<SerializableAction>();

        foreach (Action action in chosenActions)
        {
            SerializableAction serializableAction = new SerializableAction();
            serializableAction.skillId = action.skill.skillID;
            foreach (BodyPart bodyPart in action.targets)
            {
                serializableAction.targets.Add((int)bodyPart);
            }

            serializableActions.Add(serializableAction);
        }

        return JsonUtility.ToJson(new SerializableActionList() { actions = serializableActions });
    }

    [System.Serializable]
    public class SerializableActionList
    {
        public List<SerializableAction> actions;
    }

    //not used
    private List<Action> ConvertDataToChosenActions(string chosenActionsData)
    {
        List<Action> receivedActions = new List<Action>();

        SerializableActionList serializableActionList = JsonUtility.FromJson<SerializableActionList>(chosenActionsData);

        foreach (SerializableAction serializableAction in serializableActionList.actions)
        {
            Action action = new Action();
            action.skill = SkillDB.instance.skillDictionary[serializableAction.skillId];
            action.actionType = action.GetActionType(action.skill);

            foreach (int target in serializableAction.targets)
            {
                action.targets.Add((BodyPart)target);
            }

            receivedActions.Add(action);
        }

        return receivedActions;
    }

    [PunRPC]
    public void ReceiveChosenActions(string chosenActionsJson)
    {
        Debug.Log("ReceiveChosenActions called");
        // rest of your code

        Debug.Log($"ReceiveChosenActions called with data: {chosenActionsJson}");
        // rest of your code

        // Deserialize the JSON data into a list of SerializableAction objects
        //List<SerializableAction> receivedSerializableActions = JsonUtility.FromJson<List<SerializableAction>>(chosenActionsJson);

        SerializableActionList receivedActionList = JsonUtility.FromJson<SerializableActionList>(chosenActionsJson);
        List<SerializableAction> receivedSerializableActions = receivedActionList.actions;


        // Convert the received SerializableAction objects into Action objects
        List<Action> receivedActions = new List<Action>();
        foreach (SerializableAction serializableAction in receivedSerializableActions)
        {
            Action action = new Action();

            // Lookup the SkillSO from the skill ID
            action.skill = SkillDB.instance.skillDictionary[serializableAction.skillId];

            // Convert the target IDs back into BodyPart enums
            foreach (int targetId in serializableAction.targets)
            {
                action.targets.Add((BodyPart)targetId);
            }

            receivedActions.Add(action);
        }

        // If the client is the master client, store the received actions in otherPlayerActions
        if (PhotonNetwork.IsMasterClient)
        {
            otherPlayerActions = receivedActions;
            foreach (var action in otherPlayerActions)
            {
                if (action.skill.skillType == SkillType.Attacking)
                {
                    action.actionType = ActionType.Attack;
                }
                else if (action.skill.skillType == SkillType.Blocking)
                {
                    action.actionType = ActionType.Block;
                }
                else
                {
                    Debug.Log("BUUUUUUUUUUG Wtih sjilltype");
                }

            }

            // Log the number of actions received and the skill name of the first action
            if (otherPlayerActions.Count > 0)
            {
                Debug.Log($"First action skill name: {otherPlayerActions[0].skill.skillName}");
            }
            else
            {
                Debug.Log("No actions received");
            }
            Debug.Log($"Received {otherPlayerActions.Count} actions. First action skill name: {otherPlayerActions[0].skill.skillName}");

            // Set otherPlayerConfirmed to true
            otherPlayerConfirmed = true;
        }


        Debug.Log("I've received an action list from the other player (non-master client).");


        // If both players have confirmed their actions, start the FightCalc script
        if (masterClientConfirmed && otherPlayerConfirmed)
        {
            // Start your FightCalc script here, passing in masterClientActions and otherPlayerActions

            Debug.Log("Im at receive meethod");
            if (otherPlayerActions.Count > 0)
            {
                Debug.Log($"First action skill name: {otherPlayerActions[0].skill.skillName}");
            }
            else
            {
                Debug.Log("No actions received");
            }
            Debug.Log(masterClientActions[1].skill.skillName);
            Debug.Log(otherPlayerActions[0].skill.skillName);
            fightCalc.StartBattleCalculation(masterClientActions, otherPlayerActions);


        }
    }


}
