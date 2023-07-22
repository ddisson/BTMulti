using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class NewTargeting2 : MonoBehaviour
{
    public List<SkillSO> skills = new List<SkillSO>();
    public Button[] skillButtons;
    public Button[] attackBodyPartButtons;
    public Button[] defendBodyPartButtons;
    public GameObject cancelButtonPrefab;
    public Transform cancelButtonParent;

    private SkillSO currentSkill;
    private Action currentAction;
    public List<Action> chosenActions = new List<Action>();
    public Dictionary<Button, Action> cancelButtonDictionary = new Dictionary<Button, Action>();

    public bool titansAssigned = false;
    public Titan playerTitan;

    public Dictionary<Button, int> imageCountPerButton = new Dictionary<Button, int>();


    void Update()
    {
        if (!titansAssigned)
        {
            if (GameManager.instance.clientID == 2 && BattleSet.instance.enemyTitan != null)
            {
                playerTitan = BattleSet.instance.enemyTitan;
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



}
