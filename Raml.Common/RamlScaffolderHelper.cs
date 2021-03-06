﻿using System;
using System.IO;
using System.Windows;
using Microsoft.VisualStudio.Shell;
using Raml.Tools.WebApiGenerator;

namespace Raml.Common
{
    public static class RamlScaffolderHelper
    {
        public static RamlData GetRamlData(string ramlSource, string targetNamespace)
        {
            try
            {
                var ramlInfo = RamlInfoService.GetRamlInfo(ramlSource);

                if (ramlInfo.HasErrors)
                {
                    MessageBox.Show(ramlInfo.ErrorMessage);
                    return null;
                }


				var model = new WebApiGeneratorService(ramlInfo.RamlDocument, targetNamespace).BuildModel();
				var filename = Path.GetFileName(ramlSource);
				if (string.IsNullOrWhiteSpace(filename))
					filename = "source.raml";

                return new RamlData ( model, ramlInfo.RamlContents, filename);
            }
            catch (Exception ex)
            {
                ActivityLog.LogError(VisualStudioAutomationHelper.RamlVsToolsActivityLogSource,
                    VisualStudioAutomationHelper.GetExceptionInfo(ex));
                var errorMessage = ex.Message;
                if (ex.InnerException != null)
                    errorMessage += " - " + ex.InnerException.Message;

                MessageBox.Show(errorMessage);
                return null;
            }
        }

    }
}