using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;

public class SaveGameManager : MonoBehaviour {

    public void SaveState(GameObject camera)
    {
        BinaryFormatter binaryFormater = new BinaryFormatter();
        FileStream fileStream = File.Create(Application.persistentDataPath + "/gamedata.dat");
        GameData data = new GameData();
        data.bestDistance = camera.GetComponent<Game>().bestDistance;
        data.goldEgg = camera.GetComponent<Game>().goldEgg;

        data.nestlingCount = camera.GetComponent<Game>().nestlingCount;
        data.houngryLevel = camera.GetComponent<Game>().houngryLevel;
        data.ghostLevel = camera.GetComponent<Game>().ghostLevel;
        data.speedControlLevel = camera.GetComponent<Game>().speedControlLevel;
        data.extraLifeCount = camera.GetComponent<Game>().extraLifeCount;

        for(int i = 0; i < camera.GetComponent<Game>().nestlingCount; i++)
        {
            try
            {
                // Nie istnieje piskle w data nestlings.
                data.nestlings[i].level = camera.GetComponent<Game>().nestling[i].level;
                data.nestlings[i].experience = camera.GetComponent<Game>().nestling[i].experience;
                data.nestlings[i].ghostBonusTime_ExtraPercent = camera.GetComponent<Game>().nestling[i].ghostBonusTime_ExtraPercent;
                data.nestlings[i].slowAndFastGUI_ExtraPercent = camera.GetComponent<Game>().nestling[i].slowAndFastGUI_ExtraPercent;
                data.nestlings[i].doubleGold = camera.GetComponent<Game>().nestling[i].doubleGold;
                data.nestlings[i].requireExperience = camera.GetComponent<Game>().nestling[i].requireExperience;
            } 
            catch
            {
                data.nestlings.Add(new Nestlings());
                data.nestlings[i].level = camera.GetComponent<Game>().nestling[i].level;
                data.nestlings[i].experience = camera.GetComponent<Game>().nestling[i].experience;
                data.nestlings[i].ghostBonusTime_ExtraPercent = camera.GetComponent<Game>().nestling[i].ghostBonusTime_ExtraPercent;
                data.nestlings[i].slowAndFastGUI_ExtraPercent = camera.GetComponent<Game>().nestling[i].slowAndFastGUI_ExtraPercent;
                data.nestlings[i].doubleGold = camera.GetComponent<Game>().nestling[i].doubleGold;
                data.nestlings[i].requireExperience = camera.GetComponent<Game>().nestling[i].requireExperience;
            }
        }

        binaryFormater.Serialize(fileStream, data);
        fileStream.Close();
    }

    public void LoadState(GameObject camera)
    {
        try
        {
            BinaryFormatter binaryFormater = new BinaryFormatter();
            FileStream fileStream = File.Open(Application.persistentDataPath + "/gamedata.dat", FileMode.Open);
            GameData gameData = (GameData)binaryFormater.Deserialize(fileStream);
            

            camera.GetComponent<Game>().bestDistance = gameData.bestDistance;
            camera.GetComponent<Game>().goldEgg = gameData.goldEgg;

            camera.GetComponent<Game>().nestlingCount = gameData.nestlingCount;
            camera.GetComponent<Game>().houngryLevel = gameData.houngryLevel;
            camera.GetComponent<Game>().ghostLevel = gameData.ghostLevel;
            camera.GetComponent<Game>().speedControlLevel = gameData.speedControlLevel;
            camera.GetComponent<Game>().extraLifeCount = gameData.extraLifeCount;

            for (int i = 0; i < gameData.nestlings.Count; i++)
            {
                try {
                    camera.GetComponent<Game>().nestling[i].experience = gameData.nestlings[i].experience;
                    camera.GetComponent<Game>().nestling[i].level = gameData.nestlings[i].level;
                    camera.GetComponent<Game>().nestling[i].slowAndFastGUI_ExtraPercent = gameData.nestlings[i].slowAndFastGUI_ExtraPercent;
                    camera.GetComponent<Game>().nestling[i].ghostBonusTime_ExtraPercent = gameData.nestlings[i].ghostBonusTime_ExtraPercent;
                    camera.GetComponent<Game>().nestling[i].doubleGold = gameData.nestlings[i].doubleGold;
                    camera.GetComponent<Game>().nestling[i].requireExperience = gameData.nestlings[i].requireExperience;
                }
                catch
                {
                    camera.GetComponent<Game>().nestling.Add(new Nestlings());
                    camera.GetComponent<Game>().nestling[i].experience = gameData.nestlings[i].experience;
                    camera.GetComponent<Game>().nestling[i].level = gameData.nestlings[i].level;
                    camera.GetComponent<Game>().nestling[i].slowAndFastGUI_ExtraPercent = gameData.nestlings[i].slowAndFastGUI_ExtraPercent;
                    camera.GetComponent<Game>().nestling[i].ghostBonusTime_ExtraPercent = gameData.nestlings[i].ghostBonusTime_ExtraPercent;
                    camera.GetComponent<Game>().nestling[i].doubleGold = gameData.nestlings[i].doubleGold;
                    camera.GetComponent<Game>().nestling[i].requireExperience = gameData.nestlings[i].requireExperience;
                }
            }
            fileStream.Close();
        }
        catch
        {

        }
    }
}

[System.Serializable]
class GameData
{
    public int bestDistance;
    public int goldEgg;
    public int nestlingCount;
    public int ghostLevel;
    public int houngryLevel;
    public int speedControlLevel;
    public int extraLifeCount;

    public List<Nestlings> nestlings = new List<Nestlings>();
}
