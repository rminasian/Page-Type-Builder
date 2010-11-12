﻿using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using PageTypeBuilder.Configuration;
using PageTypeBuilder.Discovery;
using PageTypeBuilder.Synchronization;
using InitializationModule=EPiServer.Web.InitializationModule;

namespace PageTypeBuilder
{
    [ModuleDependency(typeof(InitializationModule))]
    public class Initializer : IInitializableModule 
    {
        public void Initialize(InitializationEngine context)
        {
            PageTypeSynchronizer synchronizer = new PageTypeSynchronizer(new PageTypeDefinitionLocator(), Configuration);
            synchronizer.SynchronizePageTypes();

            DataFactory.Instance.LoadedPage += DataFactory_LoadedPage;
            DataFactory.Instance.LoadedChildren += DataFactory_LoadedChildren;
            DataFactory.Instance.LoadedDefaultPageData += DataFactory_LoadedPage;
        }

        public void Preload(string[] parameters)
        {
            throw new System.NotImplementedException();
        }

        public void Uninitialize(InitializationEngine context)
        {
            DataFactory.Instance.LoadedPage -= DataFactory_LoadedPage;
            DataFactory.Instance.LoadedChildren -= DataFactory_LoadedChildren;
            DataFactory.Instance.LoadedDefaultPageData -= DataFactory_LoadedPage;
        }

        static void DataFactory_LoadedPage(object sender, PageEventArgs e)
        {
            if(e.Page == null)
                return;

            e.Page = PageTypeResolver.Instance.ConvertToTyped(e.Page);
        }

        static void DataFactory_LoadedChildren(object sender, ChildrenEventArgs e)
        {
            for (int i = 0; i < e.Children.Count; i++)
            {
                e.Children[i] = PageTypeResolver.Instance.ConvertToTyped(e.Children[i]);
            }
        }

        private PageTypeBuilderConfiguration Configuration
        {
            get
            {
                return PageTypeBuilderConfiguration.GetConfiguration();
            }
        }
    }
}