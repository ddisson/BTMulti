using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum BodyPart { Head, Torso, RightHand, LeftHand, Legs };

public class DamageSystem : MonoBehaviour
{
    //received data storage is Dic
    [SerializeField] public Dictionary<int, AttackingSkill> playerDraftAttackDictionary = new Dictionary<int, AttackingSkill>();
    [SerializeField] public Dictionary<int, SkillSO> playerDraftBlockDictionary = new Dictionary<int, SkillSO>();

    [SerializeField] public Dictionary<int, SkillSO> enemyOneAttackDictionary = new Dictionary<int, SkillSO>();
    [SerializeField] public Dictionary<int, SkillSO> enemyOneBlockDictionary = new Dictionary<int, SkillSO>();


    //then we put it in other fprmat Dic
    [SerializeField] public Dictionary<BodyPart, List<AttackingSkill>> playerAttackDictionary = new Dictionary<BodyPart, List<AttackingSkill>>();
    [SerializeField] public Dictionary<BodyPart, List<SkillSO>> playerBlockDictionary = new Dictionary<BodyPart, List<SkillSO>>();

    //tEnemy final Dic woth bodies
    [SerializeField] public Dictionary<BodyPart, List<SkillSO>> enemyAttackDictionary = new Dictionary<BodyPart, List<SkillSO>>();
    [SerializeField] public Dictionary<BodyPart, List<SkillSO>> enemyBlockDictionary = new Dictionary<BodyPart, List<SkillSO>>();

    //[SerializeField] public Dictionary<int, SkillSO> playerBlockDictionary = new Dictionary<int, SkillSO>();

    
    

    BattleSystem battleSystem;
    Titan titan;

    #region Singleton
    public static DamageSystem instance;

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

    // Start is called before the first frame update
    void  Start()
    {
        battleSystem = BattleSystem.instance;
        
        
    }


    //add target (index) and skill attached to it to draft dic with 10 targets
    public void AddToAttackDictionary(int index, AttackingSkill skill)
    {
        playerDraftAttackDictionary.Add(index, skill);
        Debug.Log(playerDraftAttackDictionary);
        foreach (var pair in playerDraftAttackDictionary)
        {
            Debug.Log(pair.Key);
            Debug.Log(pair.Value.skillName);
        }
            
        //Debug.Log(playerAttackDictionary);

    }

    //same with block
    public void AddToBlockDictionary(int index, SkillSO skill)
    {
        playerDraftBlockDictionary.Add(index, skill);
    }

    //test so you can delite after
    public void TestAtDic()
    {
        Debug.Log("LOOOOOOOK");
        foreach (var pair in playerDraftAttackDictionary)
        {
            Debug.Log(pair.Key);
            Debug.Log(pair.Value.skillName);
        }

    }

    public void PlayerResult()
    {
        ConvertToFinalAttackDictionary();
        foreach (var pair in playerAttackDictionary)
        {
            Debug.Log(pair);
            foreach (var skill in pair.Value)
            {
                Debug.Log(skill.name);
            } 
        }
        ConvertToFinalBlockDictionary();
        CreateEnemyActions();
//        Fight(playerAttackDictionary, enemyBlockDictionary);

    }

    //convert draft dic to final to final dictionay === переписать как метод, чтобы понимть что куда и не копировать
    private void ConvertToFinalAttackDictionary()
    {
        foreach (var pair in playerDraftAttackDictionary)
        {

            switch (pair.Key)
            {
                case 0:
                    if (playerAttackDictionary.ContainsKey(BodyPart.Head)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.Head].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.Head, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 1:
                    if (playerAttackDictionary.ContainsKey(BodyPart.Head)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.Head].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.Head, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 2:
                    if (playerAttackDictionary.ContainsKey(BodyPart.Torso)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.Torso].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.Torso, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 3:
                    if (playerAttackDictionary.ContainsKey(BodyPart.Torso)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.Torso].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.Torso, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 4:
                    if (playerAttackDictionary.ContainsKey(BodyPart.RightHand)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.RightHand].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.RightHand, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 5:
                    if (playerAttackDictionary.ContainsKey(BodyPart.RightHand)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.RightHand].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.RightHand, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 6:
                    if (playerAttackDictionary.ContainsKey(BodyPart.LeftHand)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.LeftHand].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.LeftHand, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 7:
                    if (playerAttackDictionary.ContainsKey(BodyPart.LeftHand)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.LeftHand].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.LeftHand, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 8:
                    if (playerAttackDictionary.ContainsKey(BodyPart.Legs)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.Legs].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.Legs, new List<AttackingSkill> { pair.Value });
                    }
                    break;
                case 9:
                    if (playerAttackDictionary.ContainsKey(BodyPart.Legs)) // if was already targeted - then add Skill to value list
                    {
                        playerAttackDictionary[BodyPart.Legs].Add(pair.Value);
                    }
                    else // if target for the first time - create Key and value in Dic
                    {
                        playerAttackDictionary.Add(BodyPart.Legs, new List<AttackingSkill> { pair.Value });
                    }
                    break;

                default:
                    break;
            }
        }
        
    }
    //convert draft dic to final to final dictionay === переписать как метод, чтобы понимть что куда и не копировать
    private void ConvertToFinalBlockDictionary()
    {
        foreach (var pair in playerDraftBlockDictionary)
        {

            switch (pair.Key)
            {
                case 0:
                    playerBlockDictionary[BodyPart.Head].Add(pair.Value);
                    break;
                case 1:
                    playerBlockDictionary[BodyPart.Head].Add(pair.Value);
                    break;
                case 2:
                    playerBlockDictionary[BodyPart.Torso].Add(pair.Value);
                    break;
                case 3:
                    playerBlockDictionary[BodyPart.Torso].Add(pair.Value);
                    break;
                case 4:
                    playerBlockDictionary[BodyPart.RightHand].Add(pair.Value);
                    break;
                case 5:
                    playerBlockDictionary[BodyPart.RightHand].Add(pair.Value);
                    break;
                case 6:
                    playerBlockDictionary[BodyPart.LeftHand].Add(pair.Value);
                    break;
                case 7:
                    playerBlockDictionary[BodyPart.LeftHand].Add(pair.Value);
                    break;
                case 8:
                    playerBlockDictionary[BodyPart.Legs].Add(pair.Value);
                    break;
                case 9:
                    playerBlockDictionary[BodyPart.Legs].Add(pair.Value);
                    break;

                default:
                    break;
            }
        }

    }

    public void CreateEnemyActions()
    {
        //hardcore option for test
        CreateEnemyAttackDictionary();
        CreateEnemyBlockDictionary();


        //options throuw switch, that has 3 outputs with method for each - 1 - only attack / 2 - atack and block / 3 - only block
    }

    //hardcodede enemy atack target & skill
    private void CreateEnemyAttackDictionary()
    {
        enemyOneAttackDictionary.Add(0, battleSystem.enemyTitan.skills[0]);
        enemyAttackDictionary.Add(BodyPart.Head, new List<SkillSO> { battleSystem.enemyTitan.skills[0] });
    }

    //hardcodede enemy block target & skill
    private void CreateEnemyBlockDictionary()
    {
        enemyOneBlockDictionary.Add(0, battleSystem.enemyTitan.skills[1]);
        enemyBlockDictionary.Add(BodyPart.Head, new List<SkillSO> { battleSystem.enemyTitan.skills[0] });
    }

    //метод сапоставления. мы берем и в каждой существующей паре атакующего ищем в дике блоккера такой же Ки (часть тела). Если там 
    //private void Fight(Dictionary<BodyPart, List<AttackingSkill>> playerAttackDic, Dictionary<BodyPart, List<SkillSO>> enemyBlockDic)
    //{
    //    foreach (var pair in playerAttackDic)
    //    {
    //        if (pair.Value != null)
    //        {
                
    //            if (enemyBlockDic.TryGetValue(pair.Key, out List<SkillSO> value)) //если нашли скиллы в блоке этой же части тела
    //            {
    //                float blockedDamage = AttackDamageCalculate(battleSystem.playerTitan, pair.Value);
    //                float counterDamage = blockedDamage / 2;
                    
    //                Debug.Log("Enemy blocked " + counterDamage);
    //            }
    //            else
    //            {
    //                Debug.Log("Success attack!");
    //                float dealedDamage = AttackDamageCalculate(battleSystem.playerTitan, pair.Value);
    //                Debug.Log("You dealed " + dealedDamage);
    //            }
    //        }
    //    }
    //}
   

    //private float AttackDamageCalculate(Titan attacker, List<AttackingSkill> attackSkills)
    //{
    //    float damage = attacker.GetDamage();
    //    Debug.Log(attackSkills);

    //    foreach (var skill in attackSkills)
    //        {
    //        Debug.Log(skill.name);
    //            damage += skill.GetDamage();
    //        }
    //        Debug.Log(damage);

        
    //        return damage;

    //}



    public void PlayerOneAttackChoice()
    {

    }
 

    // Update is called once per frame

}
