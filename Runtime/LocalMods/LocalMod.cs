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
    /// Mod implementation for mods installed locally.
    /// </summary>
    public sealed class LocalMod : IMod
    {
        public readonly string ModFolder;

        public IModContext Context { get; }
        public ModInfo Info { get; }
        public ModIncompatibilities Incompatibilities { get; }
        public bool IsLoaded { get; private set; }
        public IResourceLocator ResourceLocator { get; private set; }
        public IReadOnlyList<Assembly> LoadedAssemblies => _loadedAssemblies;
        
        private readonly List<Assembly> _loadedAssemblies = new();
        private UniTaskCompletionSource _loadOperation;

        public LocalMod(IModContext context, string modFolder, ModInfo info)
        {
            Context = context;
            ModFolder = modFolder;
            Info = info;
            Incompatibilities = Context.CompatibilityChecker.GetIncompatibilities(Info.Target);
        }

        public async UniTask LoadAsync()
        {
            if (_loadOperation != null)
            {
                await _loadOperation.Task;
                return;
            }
            
            _loadOperation = new UniTaskCompletionSource();

            try
            {
                await InternalLoadAsync();
                _loadOperation.TrySetResult();
            }
            catch (Exception exception)
            {
                _loadOperation.TrySetException(exception);
                throw;
            }
        }

        public UniTask<Sprite> LoadThumbnailAsync()
        {
            throw new NotImplementedException();
        }

        private async UniTask InternalLoadAsync()
        {
            if (IsLoaded)
                return;
            
            // check all dependencies are loaded
            foreach ((string modId, string version) in Info.Dependencies)
            {
                IMod mod = Context.GetMod(modId);
                
                if (mod is not { IsLoaded: true })
                    throw CreateLoadFailedException($"Missing dependency, {modId} is not loaded");
            }
            
            // check mod's platform
            if (!UniModUtility.IsPlatformCompatible(Info.Target.Platform))
                throw CreateLoadFailedException($"This mod was built for {Info.Target.Platform} platform");
            
            // check if the mod was built for this version of the app
            if (string.IsNullOrEmpty(Info.Target.TargetVersion))
                Debug.LogWarning($"[{Info.ModId}] could not get the app version that this mod was built for. The mod is not guaranteed to work and the application could crash or be unstable");
            if (Info.Target.TargetVersion != Application.version)
                Debug.LogWarning($"[{Info.ModId}] this mod was built for app version {Info.Target.TargetVersion}, so it is not guaranteed to work and the application could crash or be unstable");
            
            // load assemblies
            if (Info.Type is ModType.ContentAndAssemblies or ModType.Assemblies)
            {
                string assembliesFolder = Path.Combine(ModFolder, UniMod.AssembliesFolder);
                await UniModUtility.LoadAssembliesAsync(assembliesFolder, _loadedAssemblies);
            }
            
            // load content
            if (Info.Type is ModType.ContentAndAssemblies or ModType.Content)
            {
                // load mod content catalog
                string catalogPath = Path.Combine(ModFolder, UniMod.AddressablesCatalogFileName);
                if (!File.Exists(catalogPath))
                    throw CreateLoadFailedException($"Couldn't find mod's Addressables catalogue at {catalogPath}");
                
                ResourceLocator = await Addressables.LoadContentCatalogAsync(catalogPath, true);
            }

            // run startup script and methods
            try
            {
                await UniModUtility.RunStartupObjectFromContentAsync(this);
                await UniModUtility.RunStartupMethodsFromAssembliesAsync(this);
            }
            catch (Exception exception)
            {
                throw CreateLoadFailedException($"Something went wrong while running mod startup.\n{exception}");
            }
            
            IsLoaded = true;
            Debug.Log($"[UniMod] {Info.ModId} loaded!");
        }

        private Exception CreateLoadFailedException(string message)
        {
            return new Exception($"Failed to load {Info.ModId}: {message}");
        }
    }
}