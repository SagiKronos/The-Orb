using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using TheOrb.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace TheOrb.Saving
{
    public class SavingSystem : MonoBehaviour
    {
        const string defaultSaveFile = "save";
        [SerializeField] float fadeInTime = 0.5f;
        [SerializeField] Fader fader;
        private static bool wasSpawned;
        

        private void Awake()
        {
            if (wasSpawned)
            {
                Destroy(gameObject);
                return;
            }

            wasSpawned = true;
            fader = FindObjectOfType<Fader>();
            DontDestroyOnLoad(gameObject);
        }

        IEnumerator Start()
        {
            fader.FadeOutImmediate();
            yield return fader.FadeIn(fadeInTime);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                Load();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                Save();
            }
        }


        public IEnumerator LoadLastScene()
        {
            Dictionary<string, object> state = LoadFile(defaultSaveFile);
            int buildIndex = SceneManager.GetActiveScene().buildIndex;
            if (state.ContainsKey("lastSceneBuildIndex"))
            {
                buildIndex = (int)state["lastSceneBuildIndex"];
            }
            if (buildIndex != SceneManager.GetActiveScene().buildIndex)
                yield return SceneManager.LoadSceneAsync(buildIndex);
            RestoreState(state);
        }

        public void NewGame()
        {
            SceneManager.LoadScene(1);
        }

        private void Save()
        {
            Dictionary<string, object> state = LoadFile(defaultSaveFile);
            CaptureState(state);
            SaveFile(defaultSaveFile, state);
        }

        public void Load()
        {
            StartCoroutine(LoadLastScene());
        }

        private void Delete(string saveFile)
        {
            File.Delete(GetPathFromSaveFile(saveFile));
        }

        private Dictionary<string, object> LoadFile(string saveFile)
        {
            string path = GetPathFromSaveFile(saveFile);
            if (!File.Exists(path))
            {
                return new Dictionary<string, object>();
            }
            using (FileStream stream = File.Open(path, FileMode.Open))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                return (Dictionary<string, object>)formatter.Deserialize(stream);
            }
        }

        private void SaveFile(string saveFile, object state)
        {
            string path = GetPathFromSaveFile(saveFile);
 
            using (FileStream stream = File.Open(path, FileMode.Create))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, state);
            }
        }

        private void CaptureState(Dictionary<string, object> state)
        {
            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                state[saveable.GetUniqueIdentifier()] = saveable.CaptureState();
            }

            state["lastSceneBuildIndex"] = SceneManager.GetActiveScene().buildIndex;
        }

        private void RestoreState(Dictionary<string, object> state)
        {
            foreach (var item in GameObject.FindGameObjectsWithTag("Pickup"))
            {
                Destroy(item.gameObject);
            }    

            foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
            {
                string id = saveable.GetUniqueIdentifier();
                if (state.ContainsKey(id))
                {
                    saveable.RestoreState(state[id]);
                }
            }
        }

        private string GetPathFromSaveFile(string saveFile)
        {
            return Path.Combine(Application.persistentDataPath, saveFile + ".sav");
        }
    }
}