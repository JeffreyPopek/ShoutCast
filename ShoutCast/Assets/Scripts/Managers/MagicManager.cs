using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class MagicManager : MonoBehaviour
{
    // Cast point for the player
    [SerializeField] private Transform castPoint;
    [SerializeField] private GameObject _player;
    //[SerializeField] private Transform teleportPoint;


    [SerializeField] private SpeechRecognitionTest speech;

    [SerializeField] private GameObject[] spellBook;

    private static MagicManager _instance;

    private string gm_LastSaid = "";

    private GameObject _spellToCast;

    private float _distance = 5f;

    // Spellbook
    private Dictionary<string, string> SpellBook = new Dictionary<string, string>()
    {
        // Key is the incantation, value is the spell name
        {"Firebolt", "Firebolt"},
        {"O raging fire, offer us a great and blazing gift. Fireball", "Fireball"},
        {"I call a refreshing burbling stream here and now. Water Ball", "Waterball"},
        {"Restore my strength. Light Healing", "LightHealing"},
        {"I call forth the Arcane Gateway to transport me. Teleport", "Teleport"},
        {"Give me some mana. Greater Mana Restoration", "GreaterManaRestoration"},
        {"Speed up time. Turn Time", "TurnTime"},
        {"Blast my enemies away. Sonic Blast", "SonicBlast"}
    };


    // Teleport
    // public void SetPlayerPosition()
    // {
    //     _player.transform.position = teleportPoint.transform.position;
    // }

    // public void SetTeleportPosition(int choice)
    // {
    //     if (choice == 1)
    //     {
    //         teleportPoint.transform.position += _player.transform.forward * _distance;
    //         //teleportPoint.transform.position = teleportPoint.transform.position + new Vector3(5, 0, 5);
    //     }
    //     else if (choice == 2)
    //     {
    //         teleportPoint.transform.position = teleportPoint.transform.position - new Vector3(5, 10, 5);
    //     }
    // }

    public void GiveSpellXP(BaseSpell spell)
    {
        // give xp to that magic type
        // check if rank up

        SpellRanks tempRank = spell.GetThisSpellRank();
        int xpToGet = PlayerStatsManager.Instance.GetSpellXP(tempRank);
        Elements tempElement;

        switch (spell._element)
        {
            case Elements.Fire:
                Debug.Log("Giving Fire XP");
                PlayerStatsManager.Instance.fireLevel += xpToGet;
                tempElement = Elements.Fire;
                break;
            
            case Elements.Water:
                Debug.Log("Giving Water XP");
                PlayerStatsManager.Instance.waterLevel += xpToGet;
                tempElement = Elements.Water;
                break;
            
            case Elements.Earth:
                Debug.Log("Giving Earth XP");
                PlayerStatsManager.Instance.earthLevel += xpToGet;
                tempElement = Elements.Earth;
                break;
            
            case Elements.Wind:
                Debug.Log("Giving Wind XP");
                PlayerStatsManager.Instance.windLevel += xpToGet;
                tempElement = Elements.Wind;
                break;
            
            default:
                Debug.Log("No element found to give XP. Setting default type to fire");
                tempElement = Elements.Fire;
                break;
        }

        int levelToCheck = PlayerStatsManager.Instance.GetMagicTypeLevel(tempElement);
        
        PlayerStatsManager.Instance.CheckRankStatus(tempElement, tempRank);
    }


    private MagicManager() {
        _instance = this;
    }    
 
    public static MagicManager Instance 
    {
        get {
            if(_instance==null) 
            {
                _instance = new MagicManager();
            }
 
            return _instance;
        }
    }

    public void CastSpell(string incantation)
    {
        GetSpellFromIncantation(incantation);

        if (PlayerStatsManager.Instance.GetCurrentMana() >= _spellToCast.GetComponent<BaseSpell>().manaCost)
        {
            PlayerStatsManager.Instance.UseMana(GetManaCost(_spellToCast.GetComponent<BaseSpell>()));
            Instantiate(_spellToCast, castPoint.position, castPoint.rotation);
            GiveSpellXP(_spellToCast.GetComponent<BaseSpell>());
        }
        else
        {
            Debug.Log("YOU HAVE NO MANA");
        }
        
    }

    public float GetManaCost(BaseSpell spell)
    {
        return spell.manaCost;
    }
    
    public float CalculateDamage(BaseSpell spell, int playerIntelligenceLevel)
    {
        /*
        damage = attack * attack / defense
        attack = spell base damage + (int level / 2)
        */
        
        float totalDamage = 0;

       totalDamage = spell.baseDamage + playerIntelligenceLevel;
        
        // add enemy defence later
        //        totalDamage = ((spellbaseDamage + (playerIntelligenceLevel / 2)) * 2) / enemyDefence;

        return totalDamage;
    }


    public string GetLastSaid()
    {
        return gm_LastSaid;
    }
    
    // Getting spell from voice
    
    public void GetSpellFromIncantation(string incantation)
    {
        int smallestDistance = 0;
        int prevSmallestDistance = 99999999;
        string keyToGet = "";
        
        foreach (var key in SpellBook.Keys) // loop through keys
        {
            smallestDistance = CalculateLevenshteinDistance(key, incantation);
            if (smallestDistance < prevSmallestDistance)
            {
                prevSmallestDistance = smallestDistance;
                keyToGet = key;
            }
        }
        
        // If said incantation is too off from the original then don't do anything
        int incantationTolerance = 30;
        if (smallestDistance > incantationTolerance)
        {
            // Tell player incantation has failed
            //Debug.Log("No matching incantation");
            //return;
        }
        Debug.Log("Casting: " + SpellBook[keyToGet]);

        // Get spell prefab
        _spellToCast = GetSpellGameObject(SpellBook[keyToGet]);
    }

    private GameObject GetSpellGameObject(string spellName)
    {
        foreach (var spell in spellBook)
        {
            if (spell.name == spellName)
            {
                //Debug.Log(spell.name);
                return spell;
            }
        }
        
        Debug.Log("No Spell found in GetSpellGameObject()");
        return null;
    }
    
    
    // The lower the number the closer the two strings are to matching
    public static int CalculateLevenshteinDistance(string source1, string source2)
    {
        var source1Length = source1.Length;
        var source2Length = source2.Length;

        var matrix = new int[source1Length + 1, source2Length + 1];

        // First calculation, if one entry is empty return full length
        if (source1Length == 0)
            return source2Length;

        if (source2Length == 0)
            return source1Length;

        // Initialization of matrix with row size source1Length and columns size source2Length
        for (var i = 0; i <= source1Length; matrix[i, 0] = i++){}
        for (var j = 0; j <= source2Length; matrix[0, j] = j++){}

        // Calculate rows and collumns distances
        for (var i = 1; i <= source1Length; i++)
        {
            for (var j = 1; j <= source2Length; j++)
            {
                var cost = (source2[j - 1] == source1[i - 1]) ? 0 : 1;

                matrix[i, j] = Mathf.Min(
                    Mathf.Min(matrix[i - 1, j] + 1, matrix[i, j - 1] + 1),
                    matrix[i - 1, j - 1] + cost);
            }
        }
        // return result
        //Debug.Log(matrix[source1Length, source2Length]);
        return matrix[source1Length, source2Length];
    }
}
