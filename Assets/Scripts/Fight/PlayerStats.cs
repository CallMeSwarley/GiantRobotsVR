using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    /// <summary>
    /// Karma weight, how much karma influences the success rate of dodging and disarming
    /// should be between 0 and 1!
    /// </summary>
    private float karmaWeight = 0.8f;

    private int karmaScore = 50;
    /// <summary>
    /// Karma score above 50 incresaes the probability of dodging and disarming, below decrerses it, 50 is neutral
    /// </summary>
    public int KarmaScore
    {
        get => karmaScore;
        set
        {
            karmaScore = Mathf.Clamp(value, 0, 100);
            PlayerPrefs.SetInt("karmaScore", karmaScore);
            PlayerPrefs.Save();
        }
    }

    public float KarmaScoreNormalized
    {
        get => Mathf.Clamp01(karmaScore / 100f);
    }

    /// <summary>
    /// gets dodge success rate, influenced by karma
    /// </summary>
    public new float dodgeSuccessRate
    {
        get => Mathf.Clamp(base.dodgeSuccessRate + (KarmaScoreNormalized - 0.5f) * karmaWeight * (1 - base.dodgeSuccessRate), 0f, 1f);
    }

    /// <summary>
    /// gets disarm success rate, influenced by karma
    /// </summary>
    public new float disarmSuccessRate
    {
        get
        {
            return Mathf.Clamp(base.disarmSuccessRate + (KarmaScoreNormalized - 0.5f) * karmaWeight * (1 - base.disarmSuccessRate), 0f, 1f);
        }
    }

    /// <summary>
    /// gets shootDamage, influenced by karma
    /// </summary>
    public new int shootDamage
    {
        get
        {
            float modifier = (KarmaScoreNormalized - 0.5f) * 10;          // Compute modifier [-5, 5]
            int adjustedDamage = base.shootDamage + Mathf.RoundToInt(modifier);

            return Mathf.Max(1, adjustedDamage); // Ensure damage is at least 1
        }
    }

    /// <summary>
    /// get price of the items, influenced by karma, default is 100
    /// </summary>
    /// <returns></returns>
    public int itemPrice()
    {
        return 100 + Mathf.RoundToInt((KarmaScoreNormalized - 0.5f) * -100); // Compute modifier [-50, 50]
    }

    /// <summary>
    /// adds Pricemoney to Player, influenced by karma
    /// </summary>
    public void addFightReward()
    {
        int reward = 100 + Mathf.RoundToInt((KarmaScoreNormalized - 0.5f) * 100); // Compute modifier [-50, 50]
        PlayerPrefs.SetInt("Money", PlayerPrefs.GetInt("Money", 0) + reward);
    }

    /// <summary>
    /// Load the settings from PlayerPrefs, if not found, gets the default values
    /// </summary>
    public override void LoadGameSettings()
    {
        lifePoints = PlayerPrefs.GetInt("lifePoints", 100);
        shieldCount = PlayerPrefs.GetInt("shieldCount", 3);
        base.shootDamage = PlayerPrefs.GetInt("shootDamage", 20);
        loadCapacity = PlayerPrefs.GetInt("loadCapacity", 3);
        base.dodgeSuccessRate = PlayerPrefs.GetFloat("dodgeSuccessRate", 0.5f);
        base.disarmSuccessRate = PlayerPrefs.GetFloat("disarmSuccessRate", 0.5f);
        karmaScore = PlayerPrefs.GetInt("karmaScore", 50);
        PlayerPrefs.GetInt("Money", 0);
        //Debug.Log("Life Points: " + lifePoints);
        //Debug.Log("Shield Count: " + shieldCount);
        //Debug.Log("Shoot Damage: " + shootDamage);
        //Debug.Log("Load Capacity: " + loadCapacity);
        //Debug.Log("Dodge Success Rate: " + dodgeSuccessRate);
        //Debug.Log("Disarm Success Rate: " + disarmSuccessRate);
        //Debug.Log("Karma Score: " + karmaScore);
        //Debug.Log("Money: " + PlayerPrefs.GetInt("Money"));
    }
}
