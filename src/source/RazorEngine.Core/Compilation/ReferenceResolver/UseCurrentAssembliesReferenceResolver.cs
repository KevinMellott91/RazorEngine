using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RazorEngine.Compilation.ReferenceResolver
{
    /// <summary>
    /// Resolves the assemblies by using all currently loaded assemblies. See <see cref="IReferenceResolver"/>
    /// </summary>
    public class UseCurrentAssembliesReferenceResolver : IReferenceResolver
    {
        /// <summary>
        /// See <see cref="IReferenceResolver.GetReferences"/>
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeAssemblies"></param>
        /// <returns></returns>
        public IEnumerable<CompilerReference> GetReferences(TypeContext context = null, IEnumerable<CompilerReference> includeAssemblies = null)
        {
            return CompilerServicesUtility
                   .GetLoadedAssemblies()
                   // Hotfix applied from https://github.com/Antaris/RazorEngine/issues/526
                   //.Where(a => !a.IsDynamic && File.Exists(a.Location) && !a.Location.Contains(CompilerServiceBase.DynamicTemplateNamespace))
                   .Where(a => !a.IsDynamic && !a.FullName.Contains("Version=0.0.0.0") && File.Exists(a.Location) && !a.Location.Contains("CompiledRazorTemplates.Dynamic"))
                   .GroupBy(a => a.GetName().Name).Select(grp => grp.First(y => y.GetName().Version == grp.Max(x => x.GetName().Version))) // only select distinct assemblies based on FullName to avoid loading duplicate assemblies
                   .Select(a => CompilerReference.From(a))
                   .Concat(includeAssemblies ?? Enumerable.Empty<CompilerReference>());
        }
    }
}
