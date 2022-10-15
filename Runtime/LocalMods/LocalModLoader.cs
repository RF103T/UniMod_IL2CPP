﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace Katas.UniMod
{
    /// <summary>
    /// Mod loader implementation for mods installed locally.
    /// </summary>
    public sealed class LocalModLoader : IModLoader
    {
        public readonly string ModFolder;

        public ModInfo Info { get; }
        public bool IsLoaded { get; private set; }
        public bool ContainsAssets { get; }
        public bool ContainsAssemblies { get; }
        public IResourceLocator ResourceLocator { get; private set; }
        public IReadOnlyList<Assembly> LoadedAssemblies => _loadedAssemblies;
        
        private readonly string _assembliesFolder;
        private readonly string _catalogPath;
        private readonly List<Assembly> _loadedAssemblies = new();
        private UniTaskCompletionSource _loadOperation;

        public LocalModLoader(string modFolder, ModInfo info)
        {
            ModFolder = modFolder;
            Info = info;
            
            _assembliesFolder = Path.Combine(ModFolder, UniMod.AssembliesFolder);
            _catalogPath = Path.Combine(ModFolder, UniMod.AddressablesCatalogFileName);
            ContainsAssets = File.Exists(_catalogPath);
            ContainsAssemblies = Directory.Exists(_assembliesFolder);
            ResourceLocator = EmptyResourceLocator.Instance;
        }

        public async UniTask LoadAsync(IModContext context, IMod mod)
        {
            if (_loadOperation != null)
            {
                await _loadOperation.Task;
                return;
            }
            
            _loadOperation = new UniTaskCompletionSource();

            try
            {
                await InternalLoadAsync(context, mod);
                _loadOperation.TrySetResult();
            }
            catch (Exception exception)
            {
                _loadOperation.TrySetException(exception);
                throw;
            }
        }

        public UniTask<Texture2D> LoadThumbnailAsync()
        {
            throw new NotImplementedException();
        }

        private async UniTask InternalLoadAsync(IModContext context, IMod mod)
        {
            if (IsLoaded)
                return;
            
            if (ContainsAssemblies)
                await UniModUtility.LoadAssembliesAsync(_assembliesFolder, _loadedAssemblies);
            
            if (ContainsAssets)
                ResourceLocator = await Addressables.LoadContentCatalogAsync(_catalogPath, true);

            // run startup script and methods
            await UniModUtility.RunModStartupFromAssetsAsync(context, mod);
            await UniModUtility.RunStartupMethodsFromAssembliesAsync(LoadedAssemblies, context, mod);
            
            IsLoaded = true;
        }
    }
}