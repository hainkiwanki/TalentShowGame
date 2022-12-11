using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    private static SaveData _instance;
    public static SaveData current
    {
        get
        {
            if(_instance == null)
            {
                _instance = new SaveData();
            }
            return _instance;
        }
        set
        {
            _instance = value;
        }
    }

    public PlayerCustomizationData playerCustomization = new PlayerCustomizationData();
    public Dictionary<string, InteractableData> interactableData = new Dictionary<string, InteractableData>();
    public Dictionary<int, SceneData> sceneProgressionData = new Dictionary<int, SceneData>();

    public bool chatCanVoteUpgrade = false;
    public ulong playerMoney = 0;
    public GameData gameData = new GameData();
}

[System.Serializable]
public class SimpData
{
    public bool isMod = false;
    public ulong spawnedNormal = 0;
    public ulong beatenNormal = 0;
    public ulong spawnedBoss = 0;
    public ulong beatenBoss = 0;
}

[System.Serializable]
public class PlayerCustomizationData
{
    public int handIndex = 0;
    public List<int> unlockedHandIndices = new List<int>()
    { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 };
    public int lowerArmIndex = 0;
    public List<int> unlockedLowerArmIndices = new List<int>()
    { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17 ,18 };
    public int upperArmIndex = 0;
    public List<int> unlockedUpperArmIndices = new List<int>() 
    { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 };
    public int chestIndex = 0;
    public List<int> unlockedChestIndices = new List<int>() 
    { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 };
    public int pantsIndex = 0;
    public List<int> unlockedPantsIndices = new List<int>()
    { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28 };
    public int bootsIndex = 0;
    public List<int> unlockedBootsIndices = new List<int>()
    { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19 };

    public Dictionary<EColorCategory, List<Color>> colorsPerCategory = new Dictionary<EColorCategory, List<Color>>()
    {
        { EColorCategory.PRIMARY, new List<Color>(){  
            Colors.toscaRed, Colors.marinerBlue, Colors.scrubGreen, Colors.apacheYellow, Colors.deepBronzeBrown, Colors.judgeGreyBrown, Colors.submarineBlue, Colors.alloyOrange, Colors.regentGreyBlue, Colors.licoriceBlue, Colors.defaultMetalDarkColor,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } },
        { EColorCategory.SECONDARY, new List<Color>(){
            Colors.scrubGreen, Colors.apacheYellow, Colors.toscaRed, Colors.marinerBlue, Colors.deepBronzeBrown, Colors.judgeGreyBrown, Colors.submarineBlue, Colors.alloyOrange, Colors.regentGreyBlue, Colors.licoriceBlue, Colors.defaultMetalDarkColor,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } },
        { EColorCategory.LEATHER_PRIMARY, new List<Color>(){  
            Colors.deepBronzeBrown, Colors.scrubGreen, Colors.apacheYellow, Colors.toscaRed, Colors.marinerBlue, Colors.judgeGreyBrown, Colors.submarineBlue, Colors.alloyOrange, Colors.regentGreyBlue, Colors.licoriceBlue, Colors.defaultMetalDarkColor,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } },
        { EColorCategory.LEATHER_SECONDARY, new List<Color>(){  
            Colors.judgeGreyBrown, Colors.scrubGreen, Colors.apacheYellow, Colors.toscaRed, Colors.marinerBlue, Colors.deepBronzeBrown, Colors.submarineBlue, Colors.alloyOrange, Colors.regentGreyBlue, Colors.licoriceBlue, Colors.defaultMetalDarkColor ,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } },
        { EColorCategory.METAL_PRIMARY, new List<Color>(){  
            Colors.submarineBlue, Colors.alloyOrange, Colors.scrubGreen, Colors.apacheYellow, Colors.toscaRed, Colors.marinerBlue, Colors.deepBronzeBrown, Colors.judgeGreyBrown, Colors.regentGreyBlue, Colors.licoriceBlue, Colors.defaultMetalDarkColor,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } },
        { EColorCategory.METAL_SECONDARY, new List<Color>(){  
            Colors.regentGreyBlue, Colors.scrubGreen, Colors.apacheYellow, Colors.toscaRed, Colors.marinerBlue, Colors.deepBronzeBrown, Colors.judgeGreyBrown, Colors.submarineBlue, Colors.alloyOrange, Colors.licoriceBlue, Colors.defaultMetalDarkColor,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } },
        { EColorCategory.METAL_DARK, new List<Color>(){  
            Colors.licoriceBlue, Colors.defaultMetalDarkColor, Colors.scrubGreen, Colors.apacheYellow, Colors.toscaRed, Colors.marinerBlue, Colors.deepBronzeBrown, Colors.judgeGreyBrown, Colors.submarineBlue, Colors.alloyOrange, Colors.regentGreyBlue,
            Colors.absoluteZeroBlue, Colors.aeroBlue, Colors.aeroLightBlue, Colors.africanViolet, Colors.alabasterWhite, Colors.aliceBlue, Colors.almondWhite, Colors.amaranth, Colors.amaranthPink, Colors.amaranthPurple, Colors.amaranthRed, Colors.amazon,
            Colors.amethyst, Colors.antiqueBrass, Colors.antiqueBronze, Colors.arylideYellow, Colors.atomicTangerine, Colors.auburn, Colors.armyGreen, Colors.aureolin, Colors.battleShipGrey, Colors.bdazzledBlue, Colors.bistre
        } }
    };
    public Dictionary<EColorCategory, int> colorsInUse = new Dictionary<EColorCategory, int>()
    {
        { EColorCategory.PRIMARY, 0 },
        { EColorCategory.SECONDARY, 0 },
        { EColorCategory.LEATHER_PRIMARY, 0 },
        { EColorCategory.LEATHER_SECONDARY, 0 },
        { EColorCategory.METAL_PRIMARY, 0 },
        { EColorCategory.METAL_SECONDARY, 0 },
        { EColorCategory.METAL_DARK, 0 }
    };
}

[System.Serializable]
public class InteractableData
{
    public int progress = 0;
    public bool isToggled = false;
    public bool isCompleted = false;
}

[System.Serializable]
public class SceneData
{
    public int progress = 0;
    public int sceneVisited = 0;
}

[System.Serializable]
public class GameData
{
    public RunData currentRun = new RunData();
    public HashSet<ETwitchSettings> twitchRunSettings = new HashSet<ETwitchSettings>() { 
        ETwitchSettings.MOD_BOSS, ETwitchSettings.MOD_DESPAWN, ETwitchSettings.MOD_FRIENDLY, ETwitchSettings.MOD_MINIVERSION, ETwitchSettings.MOD_SHIELD, ETwitchSettings.MOD_SHOOT, ETwitchSettings.MOD_SPECIAL,
        ETwitchSettings.SIMP_ATTACK, ETwitchSettings.SIMP_HUGE, ETwitchSettings.SIMP_TANK, ETwitchSettings.SIMP_TRANSPARENT,
    };
    public float twitchTimeoutTime = 0.1f;
    public int levelBeaten = 0;
    public Dictionary<int, int> levelCompleteAmount = new Dictionary<int, int>();
    public Dictionary<int, int> levelFailedAmount = new Dictionary<int, int>();
    public Dictionary<string, SimpData> simpStatData = new Dictionary<string, SimpData>();
    public ulong totalMoneyMade = 0;
}

[System.Serializable]
public class RunData
{
    public List<int> selectedRunMods = new List<int>();
}