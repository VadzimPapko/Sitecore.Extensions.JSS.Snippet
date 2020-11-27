using JssSnippet.Services;
using Microsoft.Extensions.DependencyInjection;
using Sitecore.DependencyInjection;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.LayoutService.ItemRendering.Pipelines.GetLayoutServiceContext;
using Sitecore.LayoutService.Presentation.Pipelines.RenderJsonRendering;
using Sitecore.LayoutService.Serialization;
using Sitecore.LayoutService.Serialization.Pipelines.GetFieldSerializer;

namespace JssSnippet.Configurators
{
    public class RegisterDependencies : IServicesConfigurator
    {
        public void Configure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IConfiguration, Configuration>();
            serviceCollection.AddSingleton<IFieldRenderer, FieldRenderer>();
            serviceCollection.AddSingleton<ISerializerService, SerializerService>();
            serviceCollection.AddSingleton<IRenderJsonRenderingPipeline, RenderJsonRenderingPipeline>();
            serviceCollection.AddSingleton<IGetFieldSerializerPipeline, GetFieldSerializerPipeline>();
            serviceCollection.AddSingleton<IPlaceholderRenderingService, PlaceholderRenderingService>();
            serviceCollection.AddSingleton<ILayoutServiceContext, PipelineLayoutServiceContext>();
            serviceCollection.AddSingleton<IGetLayoutServiceContextPipeline, GetLayoutServiceContextPipeline>();
            serviceCollection.AddSingleton<ILayoutService, CustomLayoutService>();
        }
    }
}