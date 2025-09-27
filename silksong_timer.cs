using BepInEx;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Reflection;
using System.IO;
using System;
using GlobalEnums;

namespace silksong_timer;

[Serializable]
public class Keybinds
{
    public string StartTimer = "f8";
    public string EndTimer = "f9";
    public string CancelTimer = "f10";
}

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class silksong_timer : BaseUnityPlugin
{
    private TimerUI timerUI;

    private const string MENU_TITLE = "Menu_Title";
    private const string QUIT_TO_MENU = "Quit_To_Menu";

    private string sceneStartTimer = "";
    private string sceneEndTimer = "";

    private Keybinds keybinds = new Keybinds();
    private int time = 0;

    private bool timerPaused = true;

    private GameState prevGameState = GameState.PLAYING;
    private bool lookForTele = false;

    private int[] history = new int[5];
    private int history_num = 0;
    private int pb = 0;

    private bool ShouldTickTimer()
    {
        if (timerPaused)
        {
            return false;
        }

        UIState ui_state = GameManager.instance.ui.uiState;
        string scene_name = GameManager.instance.GetSceneNameString();
        string next_scene = GameManager.instance.nextSceneName;

        bool loading_menu = (scene_name != MENU_TITLE && next_scene == "")
            || (scene_name != MENU_TITLE && next_scene == MENU_TITLE || scene_name == QUIT_TO_MENU);

        GameState game_state = GameManager.instance.GameState;


        if (game_state == GameState.PLAYING && prevGameState == GameState.MAIN_MENU)
        {
            lookForTele = true;
        }

        if (lookForTele && (game_state != GameState.PLAYING && game_state != GameState.ENTERING_LEVEL))
        {
            lookForTele = false;
        }

        bool accepting_input = GameManager.instance.inputHandler.acceptingInput;
        HeroTransitionState hero_transition_state;
        try
        {
            hero_transition_state = GameManager.instance.hero_ctrl.transitionState;
        }
        catch (Exception e)
        {
            hero_transition_state = HeroTransitionState.WAITING_TO_TRANSITION;
        }

        bool scene_load_activation_allowed = false;
        if (GameManager.instance.sceneLoad != null)
        {
            scene_load_activation_allowed = GameManager.instance.sceneLoad.IsActivationAllowed;
        }

        // big thing
        bool r0 = (lookForTele);
        bool r1 = ((game_state == GameState.PLAYING || game_state == GameState.ENTERING_LEVEL)
                    && ui_state != UIState.PLAYING);
        bool r2 = (game_state != GameState.PLAYING && game_state != GameState.CUTSCENE && !accepting_input);
        bool r3 = ((game_state == GameState.EXITING_LEVEL && scene_load_activation_allowed)
                    || game_state == GameState.LOADING);
        bool r4 = (hero_transition_state == HeroTransitionState.WAITING_TO_ENTER_LEVEL);
        bool r5 = (ui_state != UIState.PLAYING
                    && (loading_menu
                        || (ui_state != UIState.PAUSED && ui_state != UIState.CUTSCENE && !(next_scene == "")))
                    && next_scene != scene_name);

        bool is_game_time_paused = r0 || r1 || r2 || r3 || r4 || r5;
        if (is_game_time_paused)
        {
            Logger.LogInfo($"{r0}{r1}{r2}{r3}{r4}{r5} {next_scene}");
        }

        prevGameState = game_state;
        return !is_game_time_paused;
    }

    private void endTimer()
    {
        history[history_num] = time;
        history_num += 1;
        history_num %= 5;

        Logger.LogInfo($"Timed: {getTimeText(time)}");
        if (pb == 0 || time < pb)
        {
            pb = time;
            Logger.LogInfo($"Got a pb: {getTimeText(time)}");
        }
    }

    private void startTimer()
    {
        time = 0;
        timerPaused = false;

        Logger.LogInfo("Started timer");
    }

    private void FixedUpdate()
    {
        if (Input.GetKeyDown(keybinds.StartTimer))
        {
            sceneStartTimer = SceneManager.GetActiveScene().name;
            Logger.LogInfo("Set start scene");
        }
        if (Input.GetKeyDown(keybinds.EndTimer))
        {
            sceneEndTimer = SceneManager.GetActiveScene().name;
            Logger.LogInfo("Set end scene");
        }
        if (Input.GetKeyDown(keybinds.CancelTimer))
        {
            Logger.LogInfo("Canceled");
            time = 0;
            timerPaused = true;
        }

        if (ShouldTickTimer())
        {
            if (timerUI == null)
            {
                timerUI = new TimerUI();
            }
            time += 1;
            // Logger.LogInfo(getTimeText(time));
            timerUI.setTime(getTimeText(time));
        }
    }

    public void onActiveSceneChanged(Scene from, Scene to)
    {
        if (to.name == sceneStartTimer)
        {
            startTimer();
        }
        else if (to.name == sceneEndTimer)
        {
            endTimer();
        }
    }


    private string getTimeText(int ticks)
    {
        int milis = (ticks * 100) / 50 % 100;
        int seconds = ticks / 50 % 60;
        int minutes = ticks / 50 / 60;

        return $"{minutes}:{seconds:00}.{milis:00}";
    }

    private void Awake()
    {
        SceneManager.activeSceneChanged += onActiveSceneChanged;
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has loaded!");
    }
}

