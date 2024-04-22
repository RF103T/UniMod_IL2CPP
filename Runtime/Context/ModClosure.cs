using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets.ResourceLocators;

namespace Katas.UniMod
{
    /// <summary>
    /// Default implementation of a mod closure that can be rebuilt at any time with a collection of mod loaders.
    /// </summary>
    public sealed class ModClosure : IModClosure
    {
        public IReadOnlyCollection<IMod> Mods { get; }

        private Dictionary<string, List<string>> _resConflictModMap;
        private Dictionary<string, List<string>> _modConflictResMap;

        private readonly IModHost _host;
        private readonly List<Mod> _mods;
        private readonly Dictionary<IModLoader, UniTaskCompletionSource<bool>> _loadingOperations;

        private Dictionary<string, Mod> _modsById;

        public ModClosure(IModHost host)
        {
            _host = host;
            _mods = new List<Mod>();
            _loadingOperations = new Dictionary<IModLoader, UniTaskCompletionSource<bool>>();
            Mods = _mods.AsReadOnly();

            _resConflictModMap = new Dictionary<string, List<string>>();
            _modConflictResMap = new Dictionary<string, List<string>>();
        }

        public void RebuildClosure(IEnumerable<IModLoader> loaders)
        {
            _modsById = Mod.ResolveClosure(loaders, _host);
            _mods.Clear();

            if (_modsById is not null)
                _mods.AddRange(_modsById.Values);
        }

        public IMod GetMod(string id)
        {
            if (_modsById is null || string.IsNullOrEmpty(id))
                return null;

            return _modsById.TryGetValue(id, out Mod mod) ? mod : null;
        }

        public UniTask<bool> TryLoadAllModsAsync()
        {
            return WhenAll(_mods.Select(TryLoadModAsync));
        }

        public UniTask<bool> TryLoadModsAsync(params string[] ids)
        {
            return ids is null ? UniTask.FromResult(false) : WhenAll(ids.Select(TryLoadModAsync));
        }

        public UniTask<bool> TryLoadModsAsync(IEnumerable<string> ids)
        {
            return ids is null ? UniTask.FromResult(false) : WhenAll(ids.Select(TryLoadModAsync));
        }

        public UniTask<bool> TryLoadModAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
                return UniTask.FromResult(false);
            if (_modsById is not null && _modsById.TryGetValue(id, out Mod mod))
                return TryLoadModAsync(mod);

            return UniTask.FromResult(false);
        }

        public bool TryGetConflictAssetsByModId(string modId, out IReadOnlyList<string> results)
        {
            results = null;
            if (_modConflictResMap.TryGetValue(modId, out var conflictResKeys))
            {
                results = conflictResKeys.AsReadOnly();
                return true;
            }
            return false;
        }

        public bool TryGetConflictModsByAddressablesKey(string addressablesKey, out IReadOnlyList<string> results)
        {
            results = null;
            if (_resConflictModMap.TryGetValue(addressablesKey, out var conflictModIds))
            {
                results = conflictModIds.AsReadOnly();
                return true;
            }
            return false;
        }

        private async UniTask<bool> TryLoadModAsync(Mod mod)
        {
            // don't load the mod if it has any issues
            if (mod.Issues != 0)
                return false;

            if (mod.IsLoaded)
                return true;

            // ideally all mod loader implementations should properly handle multiple concurrent calls to LoadAsync, but just in case lets handle that here too
            if (_loadingOperations.TryGetValue(mod.Loader, out UniTaskCompletionSource<bool> operation))
                return await operation.Task;

            _loadingOperations[mod.Loader] = operation = new UniTaskCompletionSource<bool>();

            // try to load all mod dependencies first
            if (mod.Dependencies.Count > 0)
            {
                bool didDependenciesLoad = await WhenAll(mod.Dependencies.Select(TryLoadModAsync));

                if (!didDependenciesLoad)
                {
                    Debug.LogError($"Failed to load mod {mod.Id}: at least one dependency failed to load.");
                    operation.TrySetResult(false);
                    return false;
                }
            }

            // try to load the mod
            try
            {
                await mod.LoadAsync();

                if (mod.ContainsAssets)
                {
                    var locatorMap = mod.ResourceLocator switch
                    {
                        ResourceLocationMap mapLocator => mapLocator.Locations,
                        EmbeddedModAssetsLocator embeddedLocator => embeddedLocator.Locations,
                        _ => throw new NotSupportedException($"Not supported resource locator type {mod.ResourceLocator.GetType()}")
                    };
                    foreach (var locatorPair in locatorMap)
                    {
                        var locatorKey = locatorPair.Key.ToString();
                        if (locatorKey.StartsWith("tot-mod-"))
                        {
                            if (!_resConflictModMap.TryGetValue(locatorKey, out List<string> modIds))
                            {
                                modIds = new List<string>() { mod.Id };
                                _resConflictModMap.Add(locatorKey, modIds);
                            }
                            else
                            {
                                modIds.Add(mod.Id);
                            }

                            if (!_modConflictResMap.TryGetValue(mod.Id, out List<string> resKeys))
                            {
                                resKeys = new List<string>() { locatorKey };
                                _modConflictResMap.Add(mod.Id, resKeys);
                            }
                            else
                            {
                                resKeys.Add(locatorKey);
                            }
                        }
                    }
                }

                operation.TrySetResult(true);
                return true;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load mod {mod.Id}\n{exception}");
                operation.TrySetResult(false);
                return false;
            }
        }

        private static async UniTask<bool> WhenAll(IEnumerable<UniTask<bool>> tasks)
        {
            bool[] results = await UniTask.WhenAll(tasks);
            return results.All(result => result);
        }
    }
}