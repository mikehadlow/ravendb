using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using System.Linq;
using Raven.Database.Json;
using Raven.Database.Linq;

namespace Raven.Database.Storage
{
    public class IndexDefinitionStorage
    {
        private const string IndexDefDir = "IndexDefinitions";
        private readonly string path;
        private readonly IDictionary<string, IndexingFunc> indexCache = new Dictionary<string, IndexingFunc>();
        private readonly ILog logger = LogManager.GetLogger(typeof (IndexDefinitionStorage));

        public IndexDefinitionStorage(string path)
        {
            this.path = Path.Combine(path, IndexDefDir);

            if (Directory.Exists(this.path) == false)
                Directory.CreateDirectory(this.path);

            foreach (var index in Directory.GetFiles(this.path, "*.index"))
            {
                var indexDef = File.ReadAllText(index);
                try
                {
                    AddIndex(Path.GetFileNameWithoutExtension(index), indexDef);
                }
                catch (Exception e)
                {
                    logger.Warn("Could not compile index " + index +", skipping bad index", e);
                }
            }
        }

        public string[] IndexNames
        {
            get { return indexCache.Keys.ToArray(); }
        }

        public string AddIndex(string name, string indexDef)
        {
            var transformer = new DynamicQueryCompiler(name, indexDef);
            var generator = transformer.CreateInstance();
            indexCache[transformer.Name] = generator.CompiledDefinition;
            File.WriteAllText(Path.Combine(path,transformer.Name + ".index"), transformer.Query);
            logger.InfoFormat("New index {0}:\r\n{1}\r\nCompiled to:\r\n{2}", transformer.Name, transformer.Query,
                              transformer.CompiledQueryText);
            return transformer.Name;
        }

        public void RemoveIndex(string name)
        {
            indexCache.Remove(name);
            File.Delete(GetIndexPath(name));
            File.Delete(GetIndexSourcePath(name));
        }

        private string GetIndexSourcePath(string name)
        {
            return Path.Combine(path, name + ".index.cs");
        }

        private string GetIndexPath(string name)
        {
            return Path.Combine(path, name + ".index");
        }

        public string GetIndexDefinition(string name)
        {
            var viewFile = GetIndexPath(name);
            if(File.Exists(viewFile) == false)
                throw new InvalidOperationException("Index file does not exists");
            return File.ReadAllText(viewFile);
        }

        public IndexingFunc GetIndexingFunction(string name)
        {
            IndexingFunc value;
            if(indexCache.TryGetValue(name, out value)==false)
                return null;
            return value;
        }

        public IndexCreationOptions FindIndexCreationOptionsOptions(string name, string indexDef)
        {
            if(indexCache.ContainsKey(name))
            {
                return GetIndexDefinition(name) == indexDef
                           ? IndexCreationOptions.Noop
                           : IndexCreationOptions.Update;
            }
            return IndexCreationOptions.Create;
        }
    }
}