using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Content.Scripts.Global;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Content.Scripts.Boot
{
    [System.Serializable]
    public class LoaderData
    {
        public enum LoadType
        {
            AssetsLoad,
            SceneLoad,
            AllLoad
        }
        [SerializeField] private string scene;
        [SerializeField] private List<AssetReference> assets;
        [SerializeField] private List<AssetReference> unloadAssets;
        [SerializeField] private LoadType loadType;

        public LoaderData(string scene, List<AssetReference> assets, List<AssetReference> unloadAssets, LoadType loadType)
        {
            this.scene = scene;
            this.assets = assets;
            this.unloadAssets = unloadAssets;
            this.loadType = loadType;
        }

        public LoadType Type => loadType;

        public List<AssetReference> Assets => assets;

        public string Scene => scene;

        public List<AssetReference> UnloadAssets => unloadAssets;
    }
    
    public class Loader : MonoBehaviour
    {
        [SerializeField] private Canvas canvas;
        [SerializeField] private RectTransform bar;
        [SerializeField] private CrossSceneFader fader;
        [SerializeField] private LoaderData loaderData;


        public event Action OnLoadStart;

        public LoaderData Data => loaderData;


        public void Load(GameDataObject gameData)
        {
            transform.parent = null;
            DontDestroyOnLoad(gameObject);
            canvas.enabled = true;
            
            OnLoadStart?.Invoke();
            
            StartCoroutine(LoadAsyncScene(gameData));
        }
        

        IEnumerator LoadAsyncScene(GameDataObject gameData)
        {
            
            bar.SetXLocalScale(0);
            
            yield return LocalizationSettings.InitializationOperation;
            
            if (Data.Type == LoaderData.LoadType.AssetsLoad || Data.Type == LoaderData.LoadType.AllLoad)
            {
                var id = 0;
                
                var fullAssetsList = new List<AssetReference>(Data.Assets);
                fullAssetsList.AddRange(Data.UnloadAssets);
                
                foreach (var asset in fullAssetsList)
                {
                    if (!gameData.TempData.IsLoaded(asset))
                    {
                        var loaded = asset.LoadAssetAsync<Object>();
                        while (!loaded.IsDone)
                        {
                            yield return null;
                        }
                        if (loaded.Status == AsyncOperationStatus.Succeeded)
                        {
                            gameData.TempData.AddObject(asset, loaded.Result);
                        }
                    }
                    else
                    {
                        if (Data.UnloadAssets.Contains(asset))
                        {
                            gameData.TempData.Unload(asset);
                        }
                    }
                    id++;
                    bar.SetXLocalScale(Mathf.Clamp01(id / (float) Data.Assets.Count));
                }
            }
            yield return new WaitForSeconds(0.5f);

            bar.SetXLocalScale(0);
            if (Data.Type == LoaderData.LoadType.SceneLoad || Data.Type == LoaderData.LoadType.AllLoad)
            {
                var operation = SceneManager.LoadSceneAsync(Data.Scene, LoadSceneMode.Single);
                operation.allowSceneActivation = false;
                fader.OnBlackScreen += delegate
                {
                    Destroy(gameObject);
                };
                //wait until the asynchronous scene fully loads
                while (!operation.isDone)
                {
                    bar.SetXLocalScale(operation.progress / 0.9f);
                    if (operation.progress >= 0.9f)
                    {
                        operation.allowSceneActivation = true;
                        fader.Fade();
                        break;
                    }

                    yield return null;
                }
            }
        }

        public void SetLoadData(LoaderData loaderData)
        {
            this.loaderData = loaderData;
        }

        public void LoadWithData(LoaderData data, GameDataObject controllerGameData)
        {
            SetLoadData(data);
            Load(controllerGameData);
        }
    }
}
