using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.LayoutService.Configuration;
using Sitecore.LayoutService.ItemRendering;
using Sitecore.Mvc.Presentation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JssSnippet.Services
{
    public class CustomLayoutService : LayoutService
    {
        public CustomLayoutService(
            IPlaceholderRenderingService placeholderService, 
            ILayoutServiceContext serviceContext) : base(placeholderService, serviceContext)
        {
        }

        public override RenderedItem Render(
            Item item, 
            IRenderingConfiguration renderingConfiguration, 
            RenderOptions renderOptions = null)
        {
            return CustomRenderedItem(item, renderingConfiguration, renderOptions: renderOptions);
        }

        private RenderedItem CustomRenderedItem(
            Item item,
            IRenderingConfiguration renderingConfiguration,
            Guid? parentItemId = null,
            RenderOptions renderOptions = null)
        {
            var isSnippetItemRenders = item.TemplateID == Constants.SnippedItemTemplateId;
            var renderedItem = isSnippetItemRenders
                ? GetRenderedElement(renderingConfiguration, item)
                : base.Render(item, renderingConfiguration, renderOptions);
            Assert.ArgumentNotNull(renderedItem, nameof(renderedItem));

            var snippetElements = renderedItem.Elements
                .Where(e => GetSnippetComponentCondition(item, e, parentItemId)).ToList();

            if (!snippetElements.Any())
            {
                return renderedItem;
            }

            if (ContainsRecursiveSnippet(item, snippetElements, renderingConfiguration))
            {
                throw new InvalidOperationException($"The Snippet trying to render itself. Root Snippet ID {item.ID}");
            }

            foreach (var snippetElement in snippetElements)
            {
                var element = snippetElement as RenderedJsonRendering;

                if (element == null)
                {
                    continue;
                }

                var dataSourceItem = Context.Database.GetItem(element.DataSource);
                var rendered = CustomRenderedItem(dataSourceItem, renderingConfiguration, renderedItem.ItemId);

                FillRenderedItemsForSnippets(rendered, element);
            }

            return renderedItem;
        }

        private RenderedItem GetRenderedElement(IRenderingConfiguration renderingConfiguration, Item item)
        {
            var pageContextItem = PageContext.Current.Item;
            PageContext.Current.Item = item;
            PageContext.Current.PageDefinition = null;

            var rootRendering = new Rendering
            {
                Item = item,
                DeviceId = new Guid(Constants.DefaultDeviceId),
                RenderingItemPath = Constants.DefaultDeviceId,
                RenderingType = Constants.SnippetRenderingType,
                Items = new[] { item },
                LayoutId = Constants.SnippetLayoutId
            };

            var options = new RenderOptions(
                rootRendering);
            var renderedElement = base.Render(
                item,
                renderingConfiguration,
                options);

            PageContext.Current.Item = pageContextItem;
            PageContext.Current.PageDefinition = null;
            return renderedElement;
        }
        
        private bool GetSnippetComponentCondition(Item item, RenderedPlaceholderElement element, Guid? parentItemId)
        {
            if (!(element is RenderedJsonRendering renderedJsonRendering) || string.IsNullOrEmpty(renderedJsonRendering.DataSource))
            {
                return false;
            }

            var datasourceId = GetIdFromDataSource(item, renderedJsonRendering);

            bool result = renderedJsonRendering.RenderingName == Constants.SnippetRenderingName &&
                            datasourceId != item.ID && (!parentItemId.HasValue || datasourceId != new ID(parentItemId.Value));

            return result;
        }

        private ID GetIdFromDataSource(Item item, RenderedJsonRendering renderedJsonRendering)
        {
            ID datasourceId;
            datasourceId = !ID.TryParse(renderedJsonRendering.DataSource, out var parsedDatasourceId)
                            ? item.Database.GetItem(renderedJsonRendering.DataSource)?.ID
                            : parsedDatasourceId;

            return datasourceId;
        }

        private bool ContainsRecursiveSnippet(
            Item item,
            List<RenderedPlaceholderElement> renderedItemElements,
            IRenderingConfiguration renderingConfiguration)
        {
            foreach (var renderedItemElement in renderedItemElements)
            {
                if (ContainsRecursiveRenderedRendering(item, renderedItemElement, renderingConfiguration, default, true))
                {
                    return true;
                }
            }

            return false;
        }

        private bool ContainsRecursiveRenderedRendering(
            Item item,
            RenderedPlaceholderElement renderedItemElement,
            IRenderingConfiguration renderingConfiguration,
            int nestingLevel = 0,
            bool itemRendered = false)
        {
            if (!(renderedItemElement is RenderedJsonRendering rendering) ||
                string.IsNullOrEmpty(rendering.DataSource))
            {
                return false;
            }

            var datasourceId = GetIdFromDataSource(item, rendering);

            if (datasourceId == item.ID && nestingLevel > 1 || nestingLevel > 15)
            {
                return true;
            }

            nestingLevel++;

            if (!string.IsNullOrEmpty(rendering.DataSource) &&
                rendering.RenderingName == Constants.SnippetRenderingName &&
                item.TemplateID == Constants.SnippedItemTemplateId &&
                !itemRendered)
            {
                if (CheckSnippetsContainsRecursiveElements(item, renderingConfiguration, nestingLevel, rendering))
                {
                    return true;
                }
            }

            if (rendering.Placeholders == null)
            {
                return false;
            }

            foreach (var renderingPlaceholder in rendering.Placeholders)
            {
                if (renderingPlaceholder.Elements.Any(
                    renderingPlaceholderElement => ContainsRecursiveRenderedRendering(
                        item,
                        renderingPlaceholderElement,
                        renderingConfiguration,
                        nestingLevel)))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CheckSnippetsContainsRecursiveElements(
            Item item,
            IRenderingConfiguration renderingConfiguration,
            int nestingLevel,
            RenderedJsonRendering rendering)
        {
            var datasourceItem = item.Database.GetItem(rendering.DataSource);
            var renderedRendering = GetRenderedElement(renderingConfiguration, datasourceItem);

            foreach (var renderedRenderingPlaceholder in renderedRendering.Placeholders)
            {
                if (renderedRenderingPlaceholder.Elements.Any(
                    renderingPlaceholderElement => ContainsRecursiveRenderedRendering(
                        item,
                        renderingPlaceholderElement,
                        renderingConfiguration,
                        nestingLevel)))
                {
                    return true;
                }
            }

            return false;
        }

        private void FillRenderedItemsForSnippets(RenderedItem rendered, RenderedJsonRendering element)
        {
            if (rendered == null || !rendered.Placeholders.First().Elements.Any())
            {
                return;
            }

            if (!rendered.Placeholders.First().Elements.Any())
            {
                return;
            }

            element.Placeholders.FirstOrDefault()?.Elements?.Clear();

            var renderedPlaceholderElements = rendered.Placeholders.FirstOrDefault()?.Elements;
            if (renderedPlaceholderElements == null)
            {
                return;
            }

            foreach (var renderedPlaceholderElement in renderedPlaceholderElements)
            {
                element.Placeholders.FirstOrDefault()?.Elements
                    .Add(renderedPlaceholderElement);
            }
        }
    }
}