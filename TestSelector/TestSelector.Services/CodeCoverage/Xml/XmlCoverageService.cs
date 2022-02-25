using System;
using System.Collections.Generic;
using System.Xml;
using TestSelector.Services.CodeCoverage.Model;

namespace TestSelector.Services.CodeCoverage.Xml
{
    public class XmlCoverageService : ICoverageService
    {
        public List<Model.CodeCoverage> GetCodeCoverage(ICoverageConfig config)
        {
            /* Supports the following xml schema
                <?xml version="1.0" encoding="ISO-8859-1"?>  
                <test identifier="...">  
                  <range source_id="..." start_line="..." end_line="..." />
                  <range source_id="..." start_line="..." end_line="..." />
                  <range source_id="..." start_line="..." end_line="..." />
                </test>  
                <test identifier="...">  
                  <range source_id="..." start_line="..." end_line="..." />
                  <range source_id="..." start_line="..." end_line="..." />
                  <range source_id="..." start_line="..." end_line="..." />
                </test>  
             */

            if (!(config is XmlCoverageConfig))
                throw new ArgumentException("Coverage config argument is supported");

            var xmlConfig = (XmlCoverageConfig)config;
            var doc = new XmlDocument();
            doc.Load(xmlConfig.Filepath);

            if (doc.DocumentElement == null) 
                throw new ArgumentException("XML file not found");
            
            var codeCoverages = new List<Model.CodeCoverage>();

            foreach (XmlNode testNode in doc.DocumentElement.ChildNodes)
            {
                var codeCoverage = ParseCodeCoverage(testNode);
                codeCoverages.Add(codeCoverage);
            }

            return codeCoverages;
        }

        private Model.CodeCoverage ParseCodeCoverage(XmlNode node)
        {
            string filepath = node.Attributes["source_id"].Value;
            var codeCoverage = new Model.CodeCoverage(filepath);

            foreach (XmlNode coverageNode in node.ChildNodes)
            {
                var codeRange = ParseCodeRange(coverageNode);
                codeCoverage.Ranges.Add(codeRange);
            }

            return codeCoverage;
        }

        private CodeRange ParseCodeRange(XmlNode node)
        {
            string filepath = node.Attributes["source_id"].Value;
            int from = int.Parse(node.Attributes["start_line"].Value);
            int to = int.Parse(node.Attributes["end_line"].Value);
            CodeRange codeCoverage = new CodeRange(filepath, from, to);

            return codeCoverage;
        }
    }
}