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

public class NewTargeting : MonoBehaviour
{
    private PhotonView photonView;

    private List<Action> masterClientActions;
    private List<Action> otherPlayerActions;

    public List<SkillSO> skills = new List<SkillSO>();
    public Button[] skillButtons;
    public Button[] attackBodyPartButtons;
    public Button[] defendBodyPartButtons;
    public GameObject cancelButtonPrefab;
    public Transform cancelButtonParent;

    public Button confirmButton;

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

        // Get the PhotonView component
        photonView = GetComponent<PhotonView>();

        // Add listener to confirm button
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);

        // Initialize the lists in Awake()
        masterClientActions = new List<Action>();
        otherPlayerActions = new List<Action>();

        FightCalc fightCalc = GetComponent<FightCalc>();  // Assuming FightCalc is on the same GameObject
    }

    void Update()
    {
        if (!titansAssigned)
        {
            if (GameManager.instance.clientID == 1 && BattleSet.instance.playerTitan != null)
            {
                playerTitan = BattleSet.instance.playerTitan;
                titansAssigned = true;
                startScript();
            }
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
        Debug.Log("SkillButtoncheck");

        currentAction = new Action();
        currentAction.skill = currentSkill;
        currentAction.targetsNeeded = currentSkill.numberOfTargets;  // new field indicating how many targets are needed

        chosenActions.Add(currentAction);
    }

    public void ChooseBodyPart(BodyPart target, ActionType actionType)
    {
        if (currentSkill != null && currentAction.targets.Count < currentSkill.numberOfTargets)
        {
            // Add target to the current action
            currentAction.targets.Add(target);

            // Create a new GameObject to hold the image
            GameObject newImage = new GameObject(currentSkill.skillName);
            newImage.transform.SetParent(attackBodyPartButtons[(int)target].transform, false);

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

            GridLayoutGroup grid = attackBodyPartButtons[(int)target].GetComponentInChildren<GridLayoutGroup>();
            if (grid == null)
            {
                grid = attackBodyPartButtons[(int)target].gameObject.AddComponent<GridLayoutGroup>();
            }

            int maxPerRow = 2;
            // This line is changed: now we're getting the child count of the button
            int rows = (attackBodyPartButtons[(int)target].transform.childCount - 1) / maxPerRow + 1;
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = maxPerRow;
            grid.cellSize = new Vector2(attackBodyPartButtons[(int)target].GetComponent<RectTransform>().rect.width / maxPerRow,
                                        attackBodyPartButtons[(int)target].GetComponent<RectTransform>().rect.height / rows);

            // Store the image with the action so we can delete it later
            currentAction.targetImages.Add(newImage);

            if (currentAction.targets.Count == currentSkill.numberOfTargets)
            {
                CreateCancelButton(currentAction);
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
        // Convert chosenActions list to a format that can be sent over the network.
        var chosenActionsData = ConvertChosenActionsToData(chosenActions);

        if (!PhotonNetwork.IsMasterClient)
        {
            // Send chosenActionsData to the master client via an RPC
            photonView.RPC("ReceiveChosenActions", RpcTarget.MasterClient, chosenActionsData);
        }
        else
        {
            // If the client is the master client, store the chosen actions in masterClientActions
            masterClientActions = chosenActions;

            Debug.Log("I am the master client, so I don't need to send the action list.");
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
        // Deserialize the JSON data into a list of SerializableAction objects
        List<SerializableAction> receivedSerializableActions = JsonUtility.FromJson<List<SerializableAction>>(chosenActionsJson);

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
        }

        Debug.Log("I've reeceivedd actionlist from enemy (non-master client)");
    }


}
