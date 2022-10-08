using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace Katas.UniMod
{
    public interface IModContext : IModSourceGroup, ILocalModInstaller
    {
        IReadOnlyList<IMod> Mods { get; }

        IMod GetMod(string modId);
        UniTask RefreshAsync();
    }
}