using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Networking;

using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace Katas.UniMod
{
    /// <summary>
    /// Mod loader implementation for mods installed locally.
    /// </summary>
    public sealed class LocalModLoader : IModLoader
    {
        public readonly string ModFolder;

        public ModInfo Info { get; }
        public string Source { get; }
        public bool IsLoaded { get; private set; }
        public bool ContainsAssets { get; }
        public bool ContainsAssemblies { get; }
        public IResourceLocator ResourceLocator { get; private set; }
        public IReadOnlyList<Assembly> LoadedAssemblies { get; }

        private readonly string _assembliesFolder;
        private readonly string _catalogPath;
        private readonly List<Assembly> _loadedAssemblies;

        private AppDomain _gameAppDomain;

        private UniTaskCompletionSource _loadOperation;
        private UniTaskCompletionSource<Sprite> _thumbnailOperation;

        public LocalModLoader(string modFolder, ModInfo info, string source = LocalModSource.SourceLabel)
        {
            ModFolder = modFolder;
            _assembliesFolder = Path.Combine(modFolder, UniModRuntime.AssembliesFolder);
            _catalogPath = Path.Combine(modFolder, UniModRuntime.AssetsFolder, UniModRuntime.AddressablesCatalogFileName);
            _loadedAssemblies = new List<Assembly>();

            Info = info;
            Source = source;
            ContainsAssets = File.Exists(_catalogPath);
            ContainsAssemblies = Directory.Exists(_assembliesFolder);
            ResourceLocator = EmptyLocator.Instance;
            LoadedAssemblies = _loadedAssemblies.AsReadOnly();
        }

        public void SetGameAppDomain(AppDomain domain)
        {
            _gameAppDomain = domain;
        }

        public async UniTask LoadAsync(IMod mod)
        {
            if (_loadOperation != null)
            {
                await _loadOperation.Task;
                return;
            }

            _loadOperation = new UniTaskCompletionSource();

            try
            {
                await InternalLoadAsync(mod);
                _loadOperation.TrySetResult();
            }
            catch (Exception exception)
            {
                _loadOperation.TrySetException(exception);
                throw;
            }
        }

        public async UniTask<Sprite> GetThumbnailAsync()
        {
            if (_thumbnailOperation is not null)
                return await _thumbnailOperation.Task;

            _thumbnailOperation = new UniTaskCompletionSource<Sprite>();

            try
            {
                Sprite thumbnail = await LoadThumbnailAsync();
                _thumbnailOperation.TrySetResult(thumbnail);
                return thumbnail;
            }
            catch (Exception exception)
            {
                _thumbnailOperation.TrySetException(exception);
                throw;
            }
        }

        private async UniTask InternalLoadAsync(IMod mod)
        {
            if (IsLoaded)
                return;

            if (ContainsAssemblies)
            {
                if (_gameAppDomain == null)
                {
                    throw new Exception($"Failed to load the mod assemblies. Please use LocalModLoader.SetGameAppDomain to set AppDomain.");
                }
                await UniModUtility.LoadAssembliesAsync(_gameAppDomain, _assembliesFolder);
            }

            if (ContainsAssets)
                ResourceLocator = await Addressables.LoadContentCatalogAsync(_catalogPath, true);

            await UniTask.WaitUntil(() => Input.GetKeyDown(KeyCode.Q));

            // run startup script and methods
            await UniModUtility.RunModStartupScriptAsync(mod);
            UniModUtility.RunInitializeMethod(_gameAppDomain, mod);

            IsLoaded = true;
        }

        private async UniTask<Sprite> LoadThumbnailAsync()
        {
            string path = Path.Combine(ModFolder, UniModRuntime.ThumbnailFile);
            if (!File.Exists(path))
                return null;

            using UnityWebRequest request = UnityWebRequestTexture.GetTexture(path);
            await request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
                throw new Exception($"Failed to load the mod thumbnail from: {path}.\nResult: {request.result}\nError: {request.error}");

            Texture2D texture = DownloadHandlerTexture.GetContent(request);
            texture.filterMode = FilterMode.Bilinear;

            return UniModUtility.CreateSpriteFromTexture(texture);
        }
    }
}