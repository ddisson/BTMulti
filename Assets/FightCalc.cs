using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FightCalc : MonoBehaviour
{
    // Lists to store actions
    private List<Action> masterClientActions;
    private List<Action> otherPlayerActions;


    //test - delete please
    public void test()
    {
        Debug.Log("fightCalc is here baby)");
    }

    // Function to start the battle calculation
    public void StartBattleCalculation(List<Action> masterClientActions, List<Action> otherPlayerActions)
    {
        this.masterClientActions = masterClientActions;
        Debug.Log(masterClientActions);
        this.otherPlayerActions = otherPlayerActions;
        Debug.Log(otherPlayerActions);

        // Run 'EffectOnCast' for all chosen skills for both players
        foreach (var action in masterClientActions)
        {
            Debug.Log($"{action.actionType} Action with {action.skill.skillType} skill of the master");
            action.skill.EffectOnCast(); // Ensure 'EffectOnCast' method is implemented in SkillSO
        }

        foreach (var action in otherPlayerActions)
        {
            Debug.Log($"{action.actionType} Action with {action.skill.skillType} skill of the otherePlayer");
            action.skill.EffectOnCast();
        }

        // Loop while there are still actions in the lists
        while (masterClientActions.Count > 0 && otherPlayerActions.Count > 0)
        {
            // Process actions for player 1 and player 2
            ProcessActions(masterClientActions, otherPlayerActions);
            ProcessActions(otherPlayerActions, masterClientActions);
        }

        Debug.Log("End of fight");
    }

    // Function to process actions
    private void ProcessActions(List<Action> playerActions, List<Action> opponentActions)
    {
        // For each action in player's actions list
        foreach (var action in playerActions)
        {
            Debug.Log(action.actionType);

            // Reset the block
            bool isBlocked = false;

            // Only interested in Attack actions
            if (action.actionType == ActionType.Attack)
            {
                // For each target in the action's targets
                foreach (var target in action.targets)
                {
 

                    // For each action in the opponent's actions
                    foreach (var oppAction in opponentActions)
                    {
                        // Only interested in Block actions that are blocking the current target
                        if (oppAction.actionType == ActionType.Block && oppAction.targets.Contains(target))
                        {
                            // Run 'EffectsOnBlock' method of the block action's skill
                            oppAction.skill.EffectOnBlock(); // Ensure 'EffectsOnBlock' method is implemented in SkillSO

                            isBlocked = true;
                            Debug.Log($"Action: {action}, Target: {target} is blocked");
                            //BattleConsole.instance.AddLine($"Action: {action}, Target: {target} is blocked");

                        }
                    }

                    // If the target isn't blocked, run the attack action's 'EffectsOnAttack' method
                    if (isBlocked)
                    {
                        BattleConsole.instance.AddLine($"Action: {action}, Target: {target} is blocked");

                    }


                    // If the target isn't blocked, run the attack action's 'EffectsOnAttack' method
                    if (!isBlocked)
                    {
                        action.skill.EffectOnAttack(); // Ensure 'EffectsOnAttack' method is implemented in SkillSO
                        Debug.Log($"Action: {action}, Target: {target} is done");
                        BattleConsole.instance.AddLine($"Action: {action}, Target: {target} is done");
  
                    }
                }

                // Remove the action from the list once it has been processed
                playerActions.Remove(action);
                return;
            }
        }
    }
}

