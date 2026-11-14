using BepInEx;
using UnityEngine;
using System.IO;
using System;
using System.Reflection;

namespace FnafITPUnlocker
{
    [BepInPlugin("dev.anishpalo.fnafitp", "FNAF ITP Unlocker", "1.0.0")]
    public class Plugin : BaseUnityPlugin
    {
        private void Awake()
        {
            Logger.LogInfo("========================================");
            Logger.LogInfo(">>> INITIALIZING BRIDGE <<<");
            
            // Create a completely hidden game object
            GameObject bridgeObj = new GameObject("FnafFileBridge");
            
            // Hide it from the engine so the game's scene purger cannot find or delete it
            bridgeObj.hideFlags = HideFlags.HideAndDontSave;
            
            // Attach our custom file-checking loop to it
            bridgeObj.AddComponent<BridgeRunner>();
            
            Logger.LogInfo(">>> BRIDGE ATTACHED AND RUNNING <<<");
            Logger.LogInfo("========================================");
        }
    }

    public class BridgeRunner : MonoBehaviour
    {
        private string gameDir;
        private string unlockFile;
        private float checkTimer = 0f;

        private void Awake()
        {
            gameDir = BepInEx.Paths.GameRootPath;
            unlockFile = Path.Combine(gameDir, "unlock.txt");
        }

        private void Update()
        {
            checkTimer += Time.deltaTime;
            if (checkTimer < 0.5f) return;
            checkTimer = 0f;

            try 
            {
                string targetFile = null;
                if (File.Exists(unlockFile)) targetFile = unlockFile;
                else if (File.Exists(unlockFile + ".txt")) targetFile = unlockFile + ".txt";
                
                if (targetFile != null)
                {
                    string[] lines = File.ReadAllLines(targetFile);
                    foreach (string line in lines)
                    {
                        string target = line.Trim();
                        if (!string.IsNullOrEmpty(target))
                        {
                            if (target.ToUpper() == "DUMP")
                            {
                                PrintAchievementsToConsole();
                            }
                            else if (target.ToUpper() == "UNLOCK_ALL")
                            {
                                UnlockEverythingNatively();
                            }
                            else
                            {
                                UnlockSingleNatively(target);
                            }
                        }
                    }
                    File.Delete(targetFile);
                }
            }
            catch (IOException) { }
            catch (Exception ex)
            {
                Debug.LogError($">>> UNLOCK ERROR: {ex.Message} <<<");
            }
        }

        private void UnlockSingleNatively(string targetId)
        {
            try
            {
                var db = SingletonSO<AchievementDatabaseScript>.Instance;
                if (db == null || db.Achievements == null)
                {
                    Debug.LogError(">>> UNLOCK FAILED: Database is null. <<<");
                    return;
                }

                bool found = false;
                for (int i = 1; i < db.Achievements.Length; i++)
                {
                    var ach = db.Achievements[i];
                    if (ach == null) continue;

                    bool matches = false;
                    var fields = ach.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                    
                    // Normalize the target string (removes spaces, punctuation, makes uppercase)
                    string normTarget = targetId.ToUpper().Replace(" ", "_").Replace("'", "").Replace("!", "").Replace("?", "").Replace("-", "_").Replace(",", "").Replace(".", "");

                    foreach (var f in fields)
                    {
                        if (f.FieldType == typeof(string))
                        {
                            string val = (string)f.GetValue(ach);
                            if (!string.IsNullOrEmpty(val))
                            {
                                string normVal = val.ToUpper().Replace(" ", "_").Replace("'", "").Replace("!", "").Replace("?", "").Replace("-", "_").Replace(",", "").Replace(".", "");
                                if (normVal == normTarget || val.Equals(targetId, StringComparison.OrdinalIgnoreCase))
                                {
                                    matches = true;
                                    break;
                                }
                            }
                        }
                    }

                    if (matches)
                    {
                        found = true;
                        int goal = 1;
                        var goalProp = ach.GetType().GetProperty("GoalAmount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (goalProp != null) goal = (int)goalProp.GetValue(ach, null);

                        var updateMethod = typeof(AchievementManager).GetMethod("UpdateAchievement", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                        if (updateMethod != null)
                        {
                            updateMethod.Invoke(null, new object[] { ach, goal });
                            Debug.Log($">>> NATIVE UNLOCK TRIGGERED FOR: {targetId} <<<");
                        }
                        break;
                    }
                }
                if (!found) Debug.LogError($">>> COULD NOT FIND {targetId} IN INTERNAL DATABASE <<<");
            }
            catch (Exception ex)
            {
                Debug.LogError($">>> NATIVE UNLOCK ERROR: {ex.Message} <<<");
            }
        }

        private void UnlockEverythingNatively()
        {
            try
            {
                var db = SingletonSO<AchievementDatabaseScript>.Instance;
                if (db == null || db.Achievements == null) return;

                int count = 0;
                var updateMethod = typeof(AchievementManager).GetMethod("UpdateAchievement", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                
                for (int i = 1; i < db.Achievements.Length; i++)
                {
                    var ach = db.Achievements[i];
                    if (ach != null && updateMethod != null)
                    {
                        int goal = 1;
                        var goalProp = ach.GetType().GetProperty("GoalAmount", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                        if (goalProp != null) goal = (int)goalProp.GetValue(ach, null);
                        
                        updateMethod.Invoke(null, new object[] { ach, goal });
                        count++;
                    }
                }
                Debug.Log($">>> NATIVELY UNLOCKED ALL {count} ACHIEVEMENTS <<<");
            }
            catch (Exception ex)
            {
                Debug.LogError($">>> UNLOCK ALL ERROR: {ex.Message} <<<");
            }
        }

        private void PrintAchievementsToConsole()
        {
            try
            {
                // Tapping directly into the Singleton architecture
                var db = SingletonSO<AchievementDatabaseScript>.Instance;
                if (db == null || db.Achievements == null)
                {
                    Debug.LogError(">>> DUMP FAILED: Database is null. Ensure you are fully loaded into the Main Menu. <<<");
                    return;
                }

                Debug.Log("================ INTERNAL ACHIEVEMENT LIST ================");
                int count = 0;

                // The loop from the game's code starts at index 1
                for (int i = 1; i < db.Achievements.Length; i++)
                {
                    var ach = db.Achievements[i];
                    if (ach == null) continue;

                    Debug.Log($"--- Achievement {i} ---");
                    
                    // Use reflection to extract every single string variable inside the object
                    var fields = ach.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (var f in fields)
                    {
                        if (f.FieldType == typeof(string))
                        {
                            string val = (string)f.GetValue(ach);
                            if (!string.IsNullOrEmpty(val))
                            {
                                Debug.Log($"{f.Name}: {val}");
                            }
                        }
                    }
                    count++;
                }
                Debug.Log($"================ TOTAL PROCESSED: {count} ================");
            }
            catch (Exception ex)
            {
                Debug.LogError($">>> CONSOLE DUMP ERROR: {ex.Message} <<<");
            }
        }
    }
}