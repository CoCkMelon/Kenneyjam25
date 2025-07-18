using UnityEngine;
using System.Collections.Generic;
using System.IO;

[System.Serializable]
public class StoryLocation
{
    public string name;
    public string description;
    public List<string> values;
}

public enum StoryChoice
{
    HonestPath,
    CompassionatePath,
    CourageousPath,
    LogicalPath,
    HumblePath,
    PerseverantPath,
    RespectfulPath,
    ResponsiblePath,
    FairPath,
    HarmoniousPath
}

public enum StoryLocationEnum
{
    VillageEntrance,
    VillageCenter,
    ForestPath,
    MountainTemple,
    AncientLibrary,
    HermitsCave,
    CrystalChamber,
    FinalTrial,
    EndingHarmony,
    EndingCourage,
    EndingWisdom,
    EndingCompassion,
    EndingBalance
}

public class StoryRouteManager : MonoBehaviour
{
    public static StoryRouteManager Instance;
    
    [Header("Story Progress")]
    public StoryLocationEnum currentLocation = StoryLocationEnum.VillageEntrance;
    public Dictionary<string, int> valuePoints = new Dictionary<string, int>();
    
    [Header("Story Content")]
    private Dictionary<StoryLocationEnum, StoryLocationData> locationContent = new Dictionary<StoryLocationEnum, StoryLocationData>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeValues();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        LoadLocationContent();
        BeginStory();
    }
    
    void InitializeValues()
    {
        valuePoints["Honesty"] = 0;
        valuePoints["Integrity"] = 0;
        valuePoints["Empathy"] = 0;
        valuePoints["Courage"] = 0;
        valuePoints["Respect"] = 0;
        valuePoints["Fairness"] = 0;
        valuePoints["Responsibility"] = 0;
        valuePoints["Compassion"] = 0;
        valuePoints["Perseverance"] = 0;
        valuePoints["Humility"] = 0;
        valuePoints["Logic"] = 0;
        valuePoints["HardWork"] = 0;
        valuePoints["Planning"] = 0;
        valuePoints["PhysicalActivity"] = 0;
        valuePoints["Harmony"] = 0;
    }
    
    void LoadLocationContent()
    {
        // Load content from YAML files - simplified without YamlDotNet dependency
        // In a real implementation, you would load from the YAML files
        // For now, we'll use hardcoded content
    }
    
    public void BeginStory()
    {
        currentLocation = StoryLocationEnum.VillageEntrance;
        ProcessCurrentLocation();
    }
    
    public void MakeChoice(StoryChoice choice)
    {
        switch (currentLocation)
        {
            case StoryLocationEnum.VillageEntrance:
                ProcessVillageEntrance(choice);
                break;
            case StoryLocationEnum.VillageCenter:
                ProcessVillageCenter(choice);
                break;
            case StoryLocationEnum.ForestPath:
                ProcessForestPath(choice);
                break;
            case StoryLocationEnum.MountainTemple:
                ProcessMountainTemple(choice);
                break;
            case StoryLocationEnum.AncientLibrary:
                ProcessAncientLibrary(choice);
                break;
            case StoryLocationEnum.HermitsCave:
                ProcessHermitsCave(choice);
                break;
            case StoryLocationEnum.CrystalChamber:
                ProcessCrystalChamber(choice);
                break;
            case StoryLocationEnum.FinalTrial:
                ProcessFinalTrial(choice);
                break;
            default:
                Debug.Log("Unknown location!");
                break;
        }
    }
    
    void ProcessCurrentLocation()
    {
        Debug.Log($"Current location: {currentLocation}");
        // Display location content and choices
    }
    
    void ProcessVillageEntrance(StoryChoice choice)
    {
        if (choice == StoryChoice.HonestPath)
        {
            AwardValue("Honesty", 10);
            AwardValue("Integrity", 5);
            currentLocation = StoryLocationEnum.VillageCenter;
            Debug.Log("You speak truthfully to the gatekeeper. They respect your honesty.");
        }
        else if (choice == StoryChoice.HumblePath)
        {
            AwardValue("Humility", 10);
            AwardValue("Respect", 5);
            currentLocation = StoryLocationEnum.VillageCenter;
            Debug.Log("You approach with humility and respect.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessVillageCenter(StoryChoice choice)
    {
        if (choice == StoryChoice.CompassionatePath)
        {
            AwardValue("Compassion", 15);
            AwardValue("Empathy", 10);
            AwardValue("PhysicalActivity", 5); // Helping with physical tasks
            currentLocation = StoryLocationEnum.ForestPath;
            Debug.Log("You help the struggling merchant, showing compassion.");
        }
        else if (choice == StoryChoice.FairPath)
        {
            AwardValue("Fairness", 15);
            AwardValue("Logic", 10);
            AwardValue("Responsibility", 10);
            currentLocation = StoryLocationEnum.AncientLibrary;
            Debug.Log("You mediate the dispute fairly, using logic and taking responsibility.");
        }
        else if (choice == StoryChoice.HumblePath)
        {
            AwardValue("Humility", 15);
            AwardValue("Respect", 10);
            AwardValue("Planning", 5); // Observing and planning
            currentLocation = StoryLocationEnum.AncientLibrary;
            Debug.Log("You observe with humility, learning before acting.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessForestPath(StoryChoice choice)
    {
        if (choice == StoryChoice.CourageousPath)
        {
            AwardValue("Courage", 20);
            AwardValue("HardWork", 15);
            AwardValue("PhysicalActivity", 20); // Combat requires physical effort
            currentLocation = StoryLocationEnum.MountainTemple;
            Debug.Log("You face the beast with courage, engaging in hard physical combat.");
        }
        else if (choice == StoryChoice.PerseverantPath)
        {
            AwardValue("Perseverance", 20);
            AwardValue("Planning", 15);
            AwardValue("Logic", 10);
            AwardValue("PhysicalActivity", 15); // Long journey requires endurance
            currentLocation = StoryLocationEnum.HermitsCave;
            Debug.Log("You persevere through the long journey, planning carefully.");
        }
        else if (choice == StoryChoice.HumblePath)
        {
            AwardValue("Humility", 15);
            AwardValue("Responsibility", 10);
            AwardValue("Planning", 10);
            currentLocation = StoryLocationEnum.VillageCenter;
            Debug.Log("You humbly return for help, showing responsibility.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessMountainTemple(StoryChoice choice)
    {
        if (choice == StoryChoice.RespectfulPath)
        {
            AwardValue("Respect", 20);
            AwardValue("Humility", 15);
            AwardValue("PhysicalActivity", 10); // Temple rituals involve movement
            currentLocation = StoryLocationEnum.CrystalChamber;
            Debug.Log("You show respect to the ancient temple spirits.");
        }
        else if (choice == StoryChoice.LogicalPath)
        {
            AwardValue("Logic", 20);
            AwardValue("Planning", 15);
            currentLocation = StoryLocationEnum.CrystalChamber;
            Debug.Log("You solve the temple puzzles through logical reasoning.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessAncientLibrary(StoryChoice choice)
    {
        if (choice == StoryChoice.LogicalPath)
        {
            AwardValue("Logic", 25);
            AwardValue("Planning", 20);
            AwardValue("HardWork", 15); // Research requires dedication
            currentLocation = StoryLocationEnum.CrystalChamber;
            Debug.Log("You research logically, working hard to understand the truth.");
        }
        else if (choice == StoryChoice.HumblePath)
        {
            AwardValue("Humility", 20);
            AwardValue("Respect", 15);
            currentLocation = StoryLocationEnum.CrystalChamber;
            Debug.Log("You approach ancient knowledge with humility and respect.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessHermitsCave(StoryChoice choice)
    {
        if (choice == StoryChoice.CompassionatePath)
        {
            AwardValue("Compassion", 20);
            AwardValue("Empathy", 15);
            AwardValue("PhysicalActivity", 10); // Helping hermit with daily tasks
            currentLocation = StoryLocationEnum.CrystalChamber;
            Debug.Log("You show compassion to the lonely hermit.");
        }
        else if (choice == StoryChoice.PerseverantPath)
        {
            AwardValue("Perseverance", 20);
            AwardValue("HardWork", 15);
            currentLocation = StoryLocationEnum.CrystalChamber;
            Debug.Log("You persevere through the hermit's challenging teachings.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessCrystalChamber(StoryChoice choice)
    {
        if (choice == StoryChoice.HarmoniousPath)
        {
            AwardValue("Harmony", 25);
            AwardValue("Logic", 15);
            AwardValue("PhysicalActivity", 10);
            AwardValue("Planning", 10);
            currentLocation = StoryLocationEnum.FinalTrial;
            Debug.Log("You achieve harmony, balancing all aspects like Noether's theorem.");
        }
        ProcessCurrentLocation();
    }
    
    void ProcessFinalTrial(StoryChoice choice)
    {
        // Determine ending based on accumulated values
        StoryLocationEnum ending = DetermineEnding();
        currentLocation = ending;
        ProcessCurrentLocation();
    }
    
    StoryLocationEnum DetermineEnding()
    {
        // Calculate which values are highest to determine ending
        if (valuePoints["Harmony"] >= 20 && IsBalanced())
        {
            return StoryLocationEnum.EndingHarmony;
        }
        else if (valuePoints["Courage"] >= 30)
        {
            return StoryLocationEnum.EndingCourage;
        }
        else if (valuePoints["Logic"] >= 30)
        {
            return StoryLocationEnum.EndingWisdom;
        }
        else if (valuePoints["Compassion"] >= 30)
        {
            return StoryLocationEnum.EndingCompassion;
        }
        else
        {
            return StoryLocationEnum.EndingBalance;
        }
    }
    
    bool IsBalanced()
    {
        // Check if all core values are reasonably balanced (harmony principle)
        string[] coreValues = { "Logic", "PhysicalActivity", "Planning", "HardWork" };
        int minValue = int.MaxValue;
        int maxValue = int.MinValue;
        
        foreach (string value in coreValues)
        {
            int points = valuePoints[value];
            minValue = Mathf.Min(minValue, points);
            maxValue = Mathf.Max(maxValue, points);
        }
        
        // Values should be within 80% of each other for balance
        return (float)minValue / maxValue >= 0.8f;
    }
    
    void AwardValue(string valueName, int points)
    {
        if (valuePoints.ContainsKey(valueName))
        {
            valuePoints[valueName] += points;
            Debug.Log($"Awarded {points} {valueName} points. Total: {valuePoints[valueName]}");
        }
    }
    
    public Dictionary<string, int> GetValuePoints()
    {
        return new Dictionary<string, int>(valuePoints);
    }
    
    public StoryLocationEnum GetCurrentLocation()
    {
        return currentLocation;
    }
}

[System.Serializable]
public class StoryLocationData
{
    public string name;
    public string description;
    public List<string> availableChoices;
}
