using UnityEngine;
using UnityEngine.SceneManagement;
using BepInEx;
using BepInEx.Configuration;

using GlobalEnums;
using System;

namespace silksong_timer;

public class Keybinds
{
    public ConfigEntry<KeyboardShortcut> SetStartScene;
    public ConfigEntry<KeyboardShortcut> SetEndScene;
    public ConfigEntry<KeyboardShortcut> CancelTimer;
    public ConfigEntry<KeyboardShortcut> StartTimer;
    public ConfigEntry<KeyboardShortcut> EndTimer;
    public ConfigEntry<KeyboardShortcut> ResetPb;

    public Keybinds(ConfigFile Config)
    {
        SetStartScene = Config.Bind("Shortcuts", "SetStartScene", new KeyboardShortcut(KeyCode.F8), "");
        SetEndScene = Config.Bind("Shortcuts", "SetEndScene", new KeyboardShortcut(KeyCode.F9), "");
        CancelTimer = Config.Bind("Shortcuts", "CancelTimer", new KeyboardShortcut(KeyCode.F10), "Cancel the timer (does not affect pb)");
        StartTimer = Config.Bind("Shortcuts", "StartTimer", new KeyboardShortcut(KeyCode.None), "");
        EndTimer = Config.Bind("Shortcuts", "EndTimer", new KeyboardShortcut(KeyCode.None), "");
        ResetPb = Config.Bind("Shortcuts", "ResetPb", new KeyboardShortcut(KeyCode.None), "");
    }
}

[BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
public class silksong_timer : BaseUnityPlugin
{
    private TimerDisplay timerDisplay;

    private const string MENU_TITLE = "Menu_Title";
    private const string QUIT_TO_MENU = "Quit_To_Menu";

    private Trigger startTrigger = new SceneTrigger("");
    private Trigger endTrigger = new SceneTrigger("");

    private Keybinds keybinds;
    private double time = 0.0;

    private bool timerPaused = true;

    private GameState prevGameState = GameState.PLAYING;
    private bool lookForTele = false;

    private double[] history = new double[5];
    private int history_num = 0;
    private double pb = 0;

    private int funSceneCount = 0;

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
            // Logger.LogInfo($"{r0}{r1}{r2}{r3}{r4}{r5} {next_scene}");
        }

        prevGameState = game_state;
        return !is_game_time_paused;
    }

    private void resetPb()
    {
        pb = 0;
        timerDisplay.setPbTime(getTimeText(pb));
    }

    private void endTimer()
    {
        history[history_num] = time;
        history_num += 1;
        history_num %= 5;

        // Logger.LogInfo($"Timed: {getTimeText(time)}");
        if (pb == 0 || time < pb)
        {
            pb = time;
            // Logger.LogInfo($"Got a pb: {getTimeText(time)}");
            timerDisplay.setPbTime(getTimeText(pb));
        }
        timerPaused = true;
    }

    private void startTimer()
    {
        time = 0;
        timerPaused = false;

        Logger.LogInfo("Started timer");
    }

    private void LateUpdate()
    {
        if (startTrigger.active() && timerPaused)
        {
            startTimer();
        }

        if (endTrigger.active())
        {
            endTimer();
        }

        if (keybinds.SetStartScene.Value.IsDown())
        {
            startTrigger = new SceneTrigger(SceneManager.GetActiveScene().name);
            resetPb();
            Logger.LogInfo("Set start scene");
        }
        if (keybinds.SetEndScene.Value.IsDown())
        {
            endTrigger = new SceneTrigger(SceneManager.GetActiveScene().name);
            resetPb();
            Logger.LogInfo("Set end scene");
        }
        if (keybinds.CancelTimer.Value.IsDown())
        {
            Logger.LogInfo("Canceled");
            time = 0;
            timerPaused = true;
        }
        if (keybinds.ResetPb.Value.IsDown())
            resetPb();
        if (keybinds.StartTimer.Value.IsDown())
            startTimer();
        if (keybinds.EndTimer.Value.IsDown())
            endTimer();

        if (ShouldTickTimer())
        {
            time += Time.unscaledDeltaTime;
            // Logger.LogInfo(getTimeText(time));

            timerDisplay.setTime(getTimeText(time));
        }
    }

    public void onActiveSceneChanged(Scene from, Scene to)
    {
        if (funSceneCount == 3)
            timerDisplay = new TimerDisplay();

        funSceneCount++;
    }

    private string getTimeText(double t)
    {
        int milis = (int)(t * 100) % 100;
        int seconds = (int)(t) % 60;
        int minutes = (int)(t) / 60;

        return $"{minutes}:{seconds:00}.{milis:00}";
    }

    private void Awake()
    {
        SceneManager.activeSceneChanged += onActiveSceneChanged;
        keybinds = new Keybinds(Config);
        Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} has loaded!");
    }
}

