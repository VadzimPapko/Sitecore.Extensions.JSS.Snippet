using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JssSnippet
{
    static class Constants
    {
        internal const string SnippetRenderingName = "Snippet";
        internal const string SnippetRenderingType = "Layout";

        internal static readonly ID SnippedItemTemplateId = new ID("{E02C74AD-2EC4-4346-9D7D-B873C979D734}");
        internal const string DefaultDeviceId = "{FE5D7FDF-89C0-4D99-9AA3-B5FBD009C9F3}";
        internal static readonly Guid SnippetLayoutId = new Guid("{431CCB97-B48E-461F-94F1-CC7D4D4BC94D}");
    }
}