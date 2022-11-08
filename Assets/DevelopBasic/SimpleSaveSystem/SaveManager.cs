using System.Collections;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SimpleSaveSystem{
    public class SaveManager : Singleton<SaveManager>{
        public const string SAVEFILE_NAME = "save.json";
        public const string SAVEFILE_DIRECTOR = "/saves/";
        protected override void Awake(){
            base.Awake();
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
        }
        /// <summary>
        /// 1.Capture the state
        /// 2.Save the progress into file
        /// </summary>
        public static void SaveGame(){
            string path = Application.persistentDataPath + SAVEFILE_DIRECTOR;
            Dictionary<System.Guid, object> progress = new Dictionary<System.Guid, object>();
            CaptureState(progress);
            Save(path, progress);
        }
        /// <summary>
        /// 1.Load the Game Progress from the path
        /// 2.Restore the state into saveable behavior
        /// </summary>
        public static void LoadGame(){
            string path = Application.persistentDataPath + SAVEFILE_DIRECTOR;
            var progress = Load(path);
            if(progress == null){
                Debug.LogWarning("No Valid Save Data");
                return;
            }
            RestoreState(progress);
        }
    #region Save and Load Tool Function
        /// <summary>
        /// return the deserialized progress data from the path
        /// </summary>
        static Dictionary<System.Guid, object> Load(string path){
            if(!File.Exists(path+SAVEFILE_NAME)){
                return null;
            }
            try{
                //TO DO: 1.Read the file from the path; 2.Deserialize the file into Dictionary<System.Guid, object>; 3.Return the Dictionary
                // example code using Json -- TO DO: Might want to replace with Newtonsoft.Json
                var save = DeserializeData(File.ReadAllText(path+SAVEFILE_NAME));
                return save;
            }
            catch{
                Debug.LogErrorFormat("Failed to load file at {0}", path+SAVEFILE_NAME);
                return null;
            }
        }
        /// <summary>
        /// Write the serialized progress data into the path
        /// </summary>
        static void Save(string path, object saveData){
            if(!Directory.Exists(path)){
                Directory.CreateDirectory(path);
            }
            //TO DO: 1.Serialize the save data; 2.Write the save Data into the file.
            // example code using Json -- TO DO: Might want to replace with Newtonsoft.Json
            File.WriteAllText(path+SAVEFILE_NAME, SerializeData(saveData));
        }
        /// <summary>
        /// Restore the state of every "SaveableBehavior" in the current active scene from the save progress
        /// </summary>
        static void RestoreState(Dictionary<System.Guid, object> progress){
            ISaveable[] saveables = Service.FindComponentsOfTypeIncludingDisable<ISaveable>();
            foreach(ISaveable saveable in saveables){
                if(progress.TryGetValue(saveable.guid, out object value)){
                    saveable.RestoreState(value);
                }
            }
        }
        /// <summary>
        /// Capture the state of every "ISaveable" in the current active scene into save progress
        /// </summary>
        static void CaptureState(Dictionary<System.Guid, object> progress){
            ISaveable[] saveables = Service.FindComponentsOfTypeIncludingDisable<ISaveable>();
            foreach(var saveable in saveables){
                progress[saveable.guid] = saveable.CaptureState();
            }
        }
    #endregion

    #region Serialization
        /// <summary>
        /// Deserialized Json data into Dictionary
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        static Dictionary<System.Guid, object> DeserializeData(string saveData){
            return JsonConvert.DeserializeObject<Dictionary<System.Guid, object>>(saveData);
        }
        /// <summary>
        /// Serialized the save data into Json format
        /// </summary>
        /// <param name="saveData"></param>
        /// <returns></returns>
        static string SerializeData(object saveData){
            return JsonConvert.SerializeObject(saveData);
        }
        /// <summary>
        /// Serialized the save data in Binary format
        /// </summary>
        /// <param name="file"></param>
        /// <param name="saveData"></param>
        static void SerializeData(FileStream file, object saveData){
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(file, saveData);
        }
        /// <summary>
        /// Deserialize the save file from Binary format
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        static object DeserializeData(FileStream file){
            BinaryFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(file);
        } 
    #endregion
    }
}

