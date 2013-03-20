using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Text;

namespace NuBot.Infrastructure
{
    public class TwoLevelDirectoryCatalog : ComposablePartCatalog, INotifyComposablePartCatalogChanged
    {
        private readonly IEnumerable<DirectoryCatalog> _subcatalogs;

        public string Path { get; private set; }
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changed;
        public event EventHandler<ComposablePartCatalogChangeEventArgs> Changing;

        public override IQueryable<ComposablePartDefinition> Parts
        {
            get
            {
                return _subcatalogs.SelectMany(c => c.Parts).AsQueryable();
            }
        }

        public IEnumerable<string> SubDirectoryNames
        {
            get { return _subcatalogs.Select(c => c.Path); }
        }
        
        public TwoLevelDirectoryCatalog(string path)
        {
            Path = path;

            _subcatalogs = Directory.GetDirectories(Path).Select(dir => new DirectoryCatalog(dir));

            foreach (var catalog in _subcatalogs)
            {
                catalog.Changed += OnChanged;
                catalog.Changing += OnChanging;
            }
        }

        public override IEnumerator<ComposablePartDefinition> GetEnumerator()
        {
            return Parts.GetEnumerator();
        }

        public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(ImportDefinition definition)
        {
            return _subcatalogs.SelectMany(c => c.GetExports(definition));
        }

        protected override void Dispose(bool disposing)
        {
            foreach (var catalog in _subcatalogs)
            {
                catalog.Dispose();
            }
        }

        private void OnChanged(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var changed = Changed;
            if (changed != null)
            {
                changed(this, e);
            }
        }

        private void OnChanging(object sender, ComposablePartCatalogChangeEventArgs e)
        {
            var changing = Changing;
            if (changing != null)
            {
                changing(this, e);
            }
        }
    }
}
