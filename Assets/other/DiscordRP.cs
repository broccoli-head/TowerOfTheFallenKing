using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordRP : MonoBehaviour
{

#if UNITY_STANDALONE_LINUX
    void Start()
    {
        Debug.Log("Discord RP is not supported on Linux.");
    }
    public static void ChangeActivity() { }


#else

    static Discord.Discord discord;
    static long startTime;


    void Awake()
    {
        startTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    void Start()
    {
        try
        {
            discord = new Discord.Discord(1370858718161408241, (ulong)Discord.CreateFlags.NoRequireDiscord);
            ChangeActivity();
        }
        catch (System.Exception e)
        {
            discord = null;
            Debug.Log("Discord is not running. " + e.Message);
        }
        
    }

    void Update()
    {
        if (discord != null)
            discord.RunCallbacks();
    }

    void OnDisable()
    {
        if (discord != null)
            discord.Dispose();
    }

    public static void ChangeActivity()
    {
        if (discord != null)
        {
            var activityManager = discord.GetActivityManager();
            var activity = new Discord.Activity
            {
                State = "Playing",
                Details = "On " + Level.CurrentlyOnRoom,
                Timestamps = { Start = startTime }
            };

            activityManager.UpdateActivity(activity, (result) =>
            {
                Debug.Log("Discord activity updated successfully.");
            });
        }
    }

#endif
}