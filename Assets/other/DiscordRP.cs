using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscordRP : MonoBehaviour
{
    static Discord.Discord discord;
    static long startTime;

    void Awake()
    {
        startTime = System.DateTimeOffset.Now.ToUnixTimeSeconds();
    }

    void Start()
    {
        discord = new Discord.Discord(1370858718161408241, (ulong)Discord.CreateFlags.NoRequireDiscord);
        ChangeActivity();
    }

    void OnDisable()
    {
        discord.Dispose();
    }

    public static void ChangeActivity()
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

    void Update()
    {
        discord.RunCallbacks();
    }
}
